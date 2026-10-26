#!/usr/bin/env python3
"""Thunderstore icon (256x256): a map pin with a teleport swirl on a dark tile with a brass edge. Own art."""
import sys, math
from PIL import Image, ImageDraw, ImageFilter
S = 4; W = 256 * S
img = Image.new("RGBA", (W, W), (0, 0, 0, 0)); d = ImageDraw.Draw(img)
d.rounded_rectangle([8*S, 8*S, 248*S, 248*S], radius=36*S, fill=(24, 22, 20, 255), outline=(201, 160, 86, 255), width=6*S)
# parchment map
d.rounded_rectangle([36*S, 52*S, 220*S, 204*S], radius=10*S, fill=(214, 194, 150, 255), outline=(120, 92, 52, 255), width=3*S)
for i, (a, b) in enumerate([((60, 110), (110, 90)), ((110, 90), (150, 120)), ((150, 120), (196, 100))]):
    d.line([(a[0]*S, a[1]*S), (b[0]*S, b[1]*S)], fill=(120, 92, 52, 255), width=3*S)
d.line([(60*S, 170*S), (196*S, 170*S)], fill=(90, 120, 150, 255), width=6*S)
# glow + swirl
glow = Image.new("RGBA", (W, W), (0, 0, 0, 0)); g = ImageDraw.Draw(glow)
g.ellipse([78*S, 70*S, 178*S, 170*S], fill=(110, 220, 210, 120)); glow = glow.filter(ImageFilter.GaussianBlur(18*S))
img = Image.alpha_composite(img, glow); d = ImageDraw.Draw(img)
for k in range(3):
    r = (14 + k * 12) * S
    d.arc([128*S - r, 120*S - r, 128*S + r, 120*S + r], 200 + k * 40, 470 + k * 40, fill=(160, 240, 230, 255), width=4*S)
# pin
d.ellipse([104*S, 40*S, 152*S, 88*S], fill=(200, 60, 48, 255), outline=(80, 20, 14, 255), width=3*S)
d.polygon([(110*S, 76*S), (146*S, 76*S), (128*S, 118*S)], fill=(200, 60, 48, 255), outline=(80, 20, 14, 255))
d.ellipse([120*S, 54*S, 136*S, 70*S], fill=(250, 230, 200, 255))
img.resize((256, 256), Image.LANCZOS).save(sys.argv[1])
