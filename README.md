# 🎨 PalettePainter

[![Chickensoft Badge][chickensoft-badge]][chickensoft-website] [![Discord][discord-badge]][discord]

Command-line, general-purpose palette generator for use with pixel art, textures, or art software. Palettes are constructed based on the principles described in [Slynyrd][slynyrd]'s seminal blog on [pixel art color palettes][palettes].

---

<p align="center">
<img alt="Chickensoft.PalettePainter" src="Chickensoft.PalettePainter/icon.png" width="200">
</p>

## 📦 Installation

This is a .NET Framework 8 tool written in C#. Once you have .NET installed, you can install this tool globally:

```sh
dotnet tool install -g Chickensoft.PalettePainter

# Run the tool
palettepainter generate --help
```

## 🖼️ Quick Start

```sh
palettepainter generate palette.png --scale 12
```

![default palette](doc_assets/default.png)

> [!NOTE]
The `--scale` or `-x` parameter controls how large a pixel is. These images were generated with `--scale 12` so that each palette swatch is 12x12 pixels.

## Winter Colors

```sh
palettepainter generate palette.png --hue 180 --saturation 1 --brightness 1 --num-ramps 8 --hue-shift 0.5 --hue-spectrum 100 --desaturate 0.3 --scale 12
```

![default palette](doc_assets/winter.png)

## Wooden Colors

```sh
palettepainter generate palette.png --hue 20 --saturation 1 --brightness 1 --num-ramps 4 --hue-shift 0.1 --hue-spectrum 15 --desaturate 0.6 --scale 12
```

![default palette](doc_assets/wooden.png)

## Leafy Greens

```sh
palettepainter generate palette.png --hue 110 --saturation 1  --brightness 1 --num-ramps 6 --hue-shift 0.5 --hue-spectrum 60  --desaturate 0.6 --scale 12
```

![default palette](doc_assets/leafy_greens.png)

## Tiny Full Spectrum

```sh
palettepainter generate palette.png --hue 110 --saturation 1  --brightness 1 --num-ramps 4 --num-colors-per-ramp 12 -z 8 --hue-shift 0.5 --hue-spectrum 360  --desaturate 0.3 --scale 12
```

![default palette](doc_assets/tiny_full_spectrum.png)

...etc!

## 🖥️ Usage Details

PalettePainter is a command-line tool that exposes a number of variables to help you generate the type of general-purpose palette you're looking for. Each of these parameters is described in the help text `palettepainter generate --help`:

```plaintext
Palette Painter 1.0.0
  Create a palette of colors with the specified number of ramps. You may
customize hue shifts and saturation levels to create a unique palette.

USAGE
  palettepainter generate <output> [options]

DESCRIPTION
  Generate a palette of colors.

PARAMETERS
* output            Palette output image file path

OPTIONS
  -n|--num-ramps    Number of color ramps to generate Default: "16".
  -c|--num-colors-per-ramp  Number of colors per ramp Default: "9".
  -z|--num-colors-to-trim-for-desaturated-ramp  Number of colors to remove from the ends of the ramp when constructing the desaturated color ramp variant. Default: "2".
  -d|--desaturate   Percentage of saturation to keep when creating a desaturated variant of a color. Default: "0.30000001192092896".
  -x|--scale        Scale factor for the output image (how big a single pixel should be). Default: "1".
  -h|--hue          Starting hue of the middle color in the first ramp. Default: "0".
  -s|--saturation   Percentage of saturation to keep when creating a color. Colors follow the built-in artistic saturation function. This value determines how much of the saturation function to use. Values greater than 1.0 will over-apply the result and can potentially result in color data loss. Default: "1".
  -b|--brightness   Percentage of brightness to keep when creating a color. Colors follow the built-in artistic brightness function. This value determines how much of the brightness function to use. Values greater than 1.0 will over-apply the result and can potentially result in color data loss. Default: "1".
  -m|--hue-shift    How much of the hue spectrum (as a fraction between 0 and 1) should  be covered by a single color ramp. Default: "0.5".
  -u|--hue-spectrum  The amount of the hue spectrum to cover in the palette (0-360). Default: "360".
  -h|--help         Shows help text.
```

---

🐣 Package generated from a 🐤 Chickensoft Template — <https://chickensoft.games>

[chickensoft-badge]: https://chickensoft.games/img/badges/chickensoft_badge.svg
[chickensoft-website]: https://chickensoft.games
[discord-badge]: https://chickensoft.games/img/badges/discord_badge.svg
[discord]: https://discord.gg/gSjaPgMmYW

[slynyrd]: https://www.slynyrd.com/pixelblog-catalogue
[palettes]: https://www.slynyrd.com/blog/2018/1/10/pixelblog-1-color-palettes
