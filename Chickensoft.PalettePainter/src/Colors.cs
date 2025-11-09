namespace Chickensoft.PalettePainter;

using System;
using SkiaSharp;

public static class Colors
{
  /// <summary>
  ///   Given an interval between 0 and 1, return the saturation level for
  ///   the color at that interval along the ramp (0-100).
  ///   This was determined from using polynomial regression based on the values
  ///   provided by slynyrd.
  ///   https://www.slynyrd.com/blog/2018/1/10/pixelblog-1-color-palettes
  /// </summary>
  /// <param name="interval">
  ///   Color position along the ramp as a value between 0
  ///   and 1.
  /// </param>
  /// <returns>Saturation value between 0 and 100.</returns>
  public static double SlynyrdSaturationFunction(double interval) =>
    Math.Clamp(20 + (169 * interval) + (257 * Math.Pow(interval, 2)) +
               (-1706 * Math.Pow(interval, 3)) +
               (5444 * Math.Pow(interval, 4)) +
               (-13860 * Math.Pow(interval, 5)) +
               (16731 * Math.Pow(interval, 6)) +
               (-7118 * Math.Pow(interval, 7)), 0, 100);

  /// <summary>
  ///   Given an interval between 0 and 1, return the brightness level for
  ///   the color at that interval along the ramp (0-100).
  ///   This was determined from using polynomial regression based on the values
  ///   provided by slynyrd.
  ///   https://www.slynyrd.com/blog/2018/1/10/pixelblog-1-color-palettes
  /// </summary>
  /// <param name="interval">
  ///   Color position along the ramp as a value between 0
  ///   and 1.
  /// </param>
  /// <returns>Saturation value between 0 and 100.</returns>
  public static double SlynyrdBrightnessFunction(double interval) =>
    Math.Clamp(15 + (110 * interval) + (347 * Math.Pow(interval, 2)) +
               (-1453 * Math.Pow(interval, 3)) +
               (2416 * Math.Pow(interval, 4)) +
               (-1888 * Math.Pow(interval, 5)) +
               (554 * Math.Pow(interval, 6)), 0, 100);

  /// <summary>
  ///   Converts HSV color values to RGB
  /// </summary>
  /// <param name="h">0 - 360</param>
  /// <param name="s">0 - 100</param>
  /// <param name="v">0 - 100</param>
  public static SKColor HsvToRgb(int h, int s, int v)
  {
    Span<int> rgb = stackalloc int[3];

    while (h < 0)
    {
      h += 360;
    }

    while (h >= 360)
    {
      h -= 360;
    }

    var baseColor = (h + 60) % 360 / 120;
    var shift = ((h + 60) % 360) - ((120 * baseColor) + 60);
    var secondaryColor = (baseColor + (shift >= 0 ? 1 : -1) + 3) % 3;

    // Setting Hue
    rgb[baseColor] = 255;
    rgb[secondaryColor] = (int)(Math.Abs(shift) / 60.0f * 255.0f);

    // Setting Saturation
    for (var i = 0; i < 3; i++)
    {
      rgb[i] += (int)((255 - rgb[i]) * ((100 - s) / 100.0f));
    }

    // Setting Value
    for (var i = 0; i < 3; i++)
    {
      rgb[i] -= (int)(rgb[i] * (100 - v) / 100.0f);
    }

    return new SKColor((byte)rgb[0], (byte)rgb[1], (byte)rgb[2]);
  }
}
