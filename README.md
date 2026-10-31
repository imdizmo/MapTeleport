# MapTeleport

[![Join the Discord](https://jatify.com/badges/discord.png)](https://discord.gg/Ru9t2KdfVK) [![Buy me a coffee](https://jatify.com/badges/coffee.png)](https://buymeacoffee.com/imdizmo)

A GM-style **Teleport here** on the map, for server admins.

- Open the large map and **right-click an empty spot**: a small card shows the spot's coordinates and a **Teleport here** button.
- You arrive at once, with no loading screen: you are held in place while the area loads, then set down on its ground, with no fall damage.
- Right-clicking a **pin** still removes it, as in vanilla. The menu closes on Esc, a click elsewhere, or when the map closes.
- **Admins only:** the host, or a player on the server's `adminlist.txt`. Everyone else's map works as usual.
- Uses the game's own teleport, not the `goto` console command, so the world is not flagged as cheated.

![Right-click on the map: the spot's coordinates and Teleport here](https://raw.githubusercontent.com/imdizmo/MapTeleport/main/screenshots/teleport.webp)

Client-side; nothing is needed on the server. Works on its own: with Jatify UI installed the card has Jatify's look, without it Valheim's.

## Credits

- Mod loader: [BepInEx](https://github.com/BepInEx/BepInEx).
- Code: written together with Claude (Anthropic's AI), which shows up as a co-author in the commit history.

MapTeleport is an unofficial, fan-made mod. It is not made, endorsed or supported by Iron Gate AB. Please report MapTeleport bugs [on GitHub](https://github.com/imdizmo/MapTeleport/issues). Valheim is a trademark of Iron Gate AB.

Source: https://github.com/imdizmo/MapTeleport (MIT; the MIT licence covers MapTeleport's own code only)
