using BepInEx;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Jatify.MapTeleport
{
    // GM-style "Teleport here": right-click an empty spot on the large map and a small card offers to take you there.
    // Right-clicking a pin still removes it, as in vanilla. Only for the host or a server admin (adminlist.txt); for
    // everyone else the map behaves as before. Works on its own: with Jatify UI the card has Jatify's look, without it
    // Valheim's (JatifyLook).
    [BepInPlugin(Guid, "MapTeleport", "1.1.0")]
    [BepInDependency("jatify.ui", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Guid = "jatify.mapteleport";

        private void Awake()
        {
            Log.Bind(Logger);
            new Harmony(Guid).PatchAll(typeof(Plugin).Assembly);
        }

        private void Update() => Menu.Tick();
        private void OnDestroy() => Menu.Destroy();

        internal static bool CanAdmin
        {
            get
            {
                if (ZNet.instance == null) return false;
                if (ZNet.instance.LocalPlayerIsAdminOrHost()) return true;
                var me = UserInfo.GetLocalUser().UserId;
                var list = ZNet.instance.GetAdminList();
                return list != null && (list.Contains(me.ToString()) || list.Contains(me.m_userID.ToString()));
            }
        }
    }

    // Vanilla's right-click handler on the large map: on empty map (no pin under the cursor) open our card.
    [HarmonyPatch(typeof(Minimap), "RemovePinUnderPointer")]
    internal static class RightClick
    {
        private static void Prefix(Minimap __instance)
        {
            var t = Traverse.Create(__instance);
            if (t.Method("GetClosestPinToCursor").GetValue<Minimap.PinData>() != null) return;   // vanilla removes it
            if (!Plugin.CanAdmin || Player.m_localPlayer == null) return;
            var world = t.Method("ScreenToWorldPoint", new[] { typeof(Vector3) }).GetValue<Vector3>((Vector3)ZInput.pointerPosition);
            Menu.Open(__instance, world);
        }
    }

    internal static class Menu
    {
        private const float W = 260f, H = 48f;
        private static RectTransform _root;
        private static Vector3 _target;
        private static Minimap _map;

        internal static bool IsOpen => _root != null && _root.gameObject.activeSelf;

        internal static void Open(Minimap map, Vector3 world)
        {
            Destroy(); _map = map; _target = world;
            Build(map, world);   // built each time: the look follows Jatify being there (or reloaded) without bookkeeping
            // next to the cursor, kept on screen
            var parent = (RectTransform)_root.parent;
            var canvas = parent.GetComponentInParent<Canvas>(); var cam = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, ZInput.pointerPosition, cam, out var local);
            var r = parent.rect;
            local.x = Mathf.Clamp(local.x + 8f, r.xMin + 4f, r.xMax - W - 4f);
            local.y = Mathf.Clamp(local.y - 8f, r.yMin + H + 4f, r.yMax - 4f);
            _root.anchoredPosition = local;
        }

        internal static void Close() { if (_root != null) _root.gameObject.SetActive(false); }
        internal static void Destroy() { if (_root != null) Object.Destroy(_root.gameObject); _root = null; }

        // design (world map): a glass card, the spot in mono on the left, a "Teleport here" button on the right
        private static void Build(Minimap map, Vector3 world)
        {
            bool jatify = JatifyLook.On;
            var vanillaFont = map.m_biomeNameLarge != null ? map.m_biomeNameLarge.font : TMP_Settings.defaultFontAsset;
            Color glass = JatifyLook.Color("glass", new Color(0.09f, 0.09f, 0.1f, 0.94f)), line = JatifyLook.Color("glassLine", new Color(1f, 1f, 1f, 0.15f));
            Color button = JatifyLook.Color("raised2", new Color(0.2f, 0.2f, 0.22f, 1f)), dim = JatifyLook.Color("dim", new Color(0.75f, 0.75f, 0.78f));

            var go = new GameObject("MapTeleportCard", typeof(RectTransform)); _root = (RectTransform)go.transform;
            _root.SetParent(map.m_largeRoot.transform, false);
            _root.anchorMin = _root.anchorMax = new Vector2(0.5f, 0.5f); _root.pivot = new Vector2(0f, 1f); _root.sizeDelta = new Vector2(W, H);
            var cv = go.AddComponent<Canvas>(); cv.overrideSorting = true; cv.sortingOrder = 32500;   // above map overlays (Jatify's are near 32000)
            go.AddComponent<GraphicRaycaster>();
            Box(_root, glass, line, 10, jatify);

            var xy = Text(_root, $"x {world.x:0} · z {world.z:0}", JatifyLook.Font("mono", vanillaFont), 12f, dim);
            Place(xy.rectTransform, 12f, 0f, 120f, H); xy.alignment = TextAlignmentOptions.MidlineLeft;

            // the button: Valheim's portal picture and the label, its width from the label
            var label = Text(null, "Teleport here", JatifyLook.Font("medium", vanillaFont), 12.5f, Color.white);
            float bw = 8f + 18f + 6f + label.GetPreferredValues().x + 12f;
            var b = new GameObject("Teleport", typeof(RectTransform)); var br = (RectTransform)b.transform; br.SetParent(_root, false);
            Place(br, W - 8f - bw, 8f, bw, 32f);
            var face = Box(br, button, line, 8, jatify);
            var btn = b.AddComponent<Button>(); btn.targetGraphic = face;
            var cb = btn.colors; cb.highlightedColor = new Color(1.25f, 1.25f, 1.25f); cb.pressedColor = new Color(0.85f, 0.85f, 0.85f); cb.selectedColor = Color.white; btn.colors = cb;
            btn.onClick.AddListener(Teleport);
            var portal = ZNetScene.instance != null ? ZNetScene.instance.GetPrefab("portal_wood")?.GetComponent<Piece>()?.m_icon : null;
            if (portal != null) { var ic = Img(br, portal, Color.white); ic.preserveAspect = true; Place(ic.rectTransform, 8f, 7f, 18f, 18f); }
            label.transform.SetParent(br, false); Place(label.rectTransform, 32f, 0f, bw - 32f, 32f); label.alignment = TextAlignmentOptions.MidlineLeft;
        }

        // a rounded fill and its outline with Jatify, a plain fill without
        private static Image Box(RectTransform parent, Color fill, Color line, int radius, bool jatify)
        {
            var ring = jatify ? JatifyLook.Ring(radius) : null;
            if (ring != null) { var o = Img(parent, ring, line); o.type = Image.Type.Sliced; o.rectTransform.anchorMin = Vector2.zero; o.rectTransform.anchorMax = Vector2.one; o.rectTransform.offsetMin = o.rectTransform.offsetMax = Vector2.zero; }
            var f = Img(parent, jatify ? JatifyLook.Rounded(radius) : null, fill); f.type = Image.Type.Sliced; f.raycastTarget = true;
            f.rectTransform.anchorMin = Vector2.zero; f.rectTransform.anchorMax = Vector2.one; f.rectTransform.offsetMin = f.rectTransform.offsetMax = Vector2.zero;
            if (ring != null) f.transform.SetAsFirstSibling();
            return f;
        }

        private static Image Img(Transform parent, Sprite s, Color c)
        {
            var go = new GameObject("Image", typeof(RectTransform)); go.transform.SetParent(parent, false);
            var i = go.AddComponent<Image>(); i.sprite = s; i.color = c; i.raycastTarget = false; return i;
        }

        private static TextMeshProUGUI Text(Transform parent, string s, TMP_FontAsset font, float size, Color c)
        {
            var go = new GameObject("Text", typeof(RectTransform)); if (parent != null) go.transform.SetParent(parent, false);
            var t = go.AddComponent<TextMeshProUGUI>(); t.font = font; t.fontSize = size; t.color = c; t.text = s; t.raycastTarget = false;
            t.textWrappingMode = TextWrappingModes.NoWrap; t.overflowMode = TextOverflowModes.Overflow; return t;
        }

        // top-left placement inside the parent, in pixels
        private static void Place(RectTransform r, float x, float y, float w, float h)
        {
            r.anchorMin = r.anchorMax = r.pivot = new Vector2(0f, 1f); r.anchoredPosition = new Vector2(x, -y); r.sizeDelta = new Vector2(w, h);
        }

        private static void Teleport()
        {
            Close();
            var p = Player.m_localPlayer;
            if (p == null || !Plugin.CanAdmin) return;
            // Start well above the ground; Valheim's distant teleport waits for the area and drops you onto the floor.
            float ground = WorldGenerator.instance != null ? WorldGenerator.instance.GetHeight(_target.x, _target.z) : _target.y;
            var dest = new Vector3(_target.x, Mathf.Max(ground, ZoneSystem.instance != null ? ZoneSystem.instance.m_waterLevel : 30f) + 2f, _target.z);
            Log.Info($"teleport to x={dest.x:0} z={dest.z:0}");
            Minimap.instance?.SetMapMode(Minimap.MapMode.Small);
            p.TeleportTo(dest, p.transform.rotation, distantTeleport: true);
        }

        // Closes on Esc, on the map closing, or on a click anywhere outside the card.
        internal static void Tick()
        {
            if (!IsOpen) return;
            if (_map == null || Minimap.instance == null || !Minimap.IsOpen() || Input.GetKeyDown(KeyCode.Escape)) { Close(); return; }
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                var canvas = _root.GetComponentInParent<Canvas>();
                var cam = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;
                if (!RectTransformUtility.RectangleContainsScreenPoint(_root, ZInput.pointerPosition, cam)) Close();
            }
        }
    }
}
