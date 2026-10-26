#!/bin/sh
# Build the plugin and the Thunderstore zip: dist/Jatify-MapTeleport-<version>.zip
set -e
R="$(cd "$(dirname "$0")/.." && pwd)"
V=$(python3 -c "import json;print(json.load(open('$R/package/manifest.json'))['version_number'])")
dotnet build "$R/MapTeleport.csproj" -c Release -v minimal -o "$R/build" -p:ValheimDir="${VALHEIM:-/mnt/games/SteamLibrary/steamapps/common/Valheim}" "$@" >/dev/null
S=$(mktemp -d); trap 'rm -rf "$S"' EXIT
mkdir -p "$S/plugins" "$R/dist"
cp "$R/package/manifest.json" "$R/package/icon.png" "$R/package/README.md" "$R/package/CHANGELOG.md" "$S/"
cp "$R/build/MapTeleport.dll" "$S/plugins/"
cp "$R/LICENSE" "$S/"
rm -f "$R/dist/Jatify-MapTeleport-$V.zip"
(cd "$S" && zip -qr -X "$R/dist/Jatify-MapTeleport-$V.zip" .)
echo "$R/dist/Jatify-MapTeleport-$V.zip"
