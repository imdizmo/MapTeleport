using System;
using BepInEx.Bootstrap;
using TMPro;
using UnityEngine;

namespace Jatify.MapTeleport
{
    // Jatify UI's look when it is installed (its public Jatify.UI.Look, found at runtime), Valheim's otherwise. Nothing here
    // compiles against Jatify, so this mod loads without it and follows Jatify's hot reloads. Same file in each Jatify mod.
    internal static class JatifyLook
    {
        private const string Guid = "jatify.ui";
        private static Type _look; private static System.Reflection.Assembly _from;

        private static Type Look()
        {
            if (!Chainloader.PluginInfos.TryGetValue(Guid, out var info) || info.Instance == null) return null;
            var asm = info.Instance.GetType().Assembly;
            if (asm != _from) { _from = asm; _look = asm.GetType("Jatify.UI.Look"); }
            return _look;
        }

        private static object Call(string method, object arg)
        {
            var t = Look(); if (t == null) return null;
            try { return t.GetMethod(method)?.Invoke(null, new[] { arg }); } catch { return null; }
        }

        internal static bool On => Look() != null;
        internal static Color Color(string token, Color fallback) => Call("Color", token) is Color c && c.a > 0f ? c : fallback;
        internal static TMP_FontAsset Font(string kind, TMP_FontAsset fallback) => Call("Font", kind) as TMP_FontAsset ?? fallback;
        internal static Sprite Rounded(int radius) => Call("Rounded", radius) as Sprite;
        internal static Sprite Ring(int radius) => Call("Ring", radius) as Sprite;
        internal static Sprite Icon(string name) => Call("Icon", name) as Sprite;
    }
}
