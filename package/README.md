# MapTeleport

A GM-style **Teleport here** on the map, for server admins.

- Open the large map and **right-click an empty spot**: a small card shows the spot's coordinates and a **Teleport here** button.
- You travel with Valheim's own long-distance teleport (a short loading pause, then you land on the ground).
- Right-clicking a **pin** still removes it, as in vanilla. The menu closes on Esc, a click elsewhere, or when the map closes.
- **Admins only:** the host, or a player on the server's `adminlist.txt`. Everyone else's map works as usual.
- Uses the game's own teleport, not the `goto` console command, so the world is not flagged as cheated.

Client-side; nothing is needed on the server. Works on its own: with Jatify UI installed the card has Jatify's look, without it Valheim's.

## Credits

- Code: written together with Claude (Anthropic's AI), which shows up as a co-author in the commit history.

Source: https://github.com/imdizmo/MapTeleport (MIT; the MIT licence covers MapTeleport's own code only)
