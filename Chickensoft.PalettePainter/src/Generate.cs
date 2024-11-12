namespace Chickensoft.PalettePainter;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using SkiaSharp;

[Command("generate", Description = "Generate a palette of colors.")]
public class Generate : ICommand
{
  private const int NUM_RAMPS_DEFAULT = 16;
  private const int NUM_COLORS_PER_RAMP_DEFAULT = 9;
  private const int NUM_COLORS_TO_TRIM_FOR_DESATURATED_RAMP_DEFAULT = 2;
  private const double DESATURATE_DEFAULT = 0.3f;
  private const int SCALE_DEFAULT = 1;
  private const int HUE_DEFAULT = 0;
  private const double HUE_SHIFT_DEFAULT = 0.5f;
  private const double SATURATION_DEFAULT = 1f;
  private const double BRIGHTNESS_DEFAULT = 1f;
  private const int HUE_SPECTRUM_DEFAULT = 360;

  [CommandParameter(0, Name = "output",
    Description = "Palette output image file path", IsRequired = true)]
  public required string Output { get; init; }

  [CommandOption("num-ramps", 'n',
    Description = "Number of color ramps to generate", IsRequired = false)]
  public int NumRamps { get; init; } = NUM_RAMPS_DEFAULT;

  [CommandOption("num-colors-per-ramp", 'c',
    Description = "Number of colors per ramp", IsRequired = false)]
  public int NumColorsPerRamp { get; init; } = NUM_COLORS_PER_RAMP_DEFAULT;

  [CommandOption("num-colors-to-trim-for-desaturated-ramp", 'z',
    Description = "Number of colors to remove from the ends of the ramp when " +
                  "constructing the desaturated color ramp variant.",
    IsRequired = false)]
  public int NumColorsToTrimForDesaturatedRamp { get; init; } =
    NUM_COLORS_TO_TRIM_FOR_DESATURATED_RAMP_DEFAULT;

  [CommandOption("desaturate", 'd',
    Description = "Percentage of saturation to keep when creating a " +
                  "desaturated variant of a color.")]
  public double Desaturate { get; init; } = DESATURATE_DEFAULT;

  [CommandOption("scale", 'x',
    Description = "Scale factor for the output image (how big a single pixel " +
                  "should be).", IsRequired = false)]
  public int Scale { get; init; } = SCALE_DEFAULT;

  [CommandOption("hue", 'h',
    Description = "Starting hue of the middle color in the first ramp.")]
  public int Hue { get; init; } = HUE_DEFAULT;

  [CommandOption("saturation", 's',
    Description = "Percentage of saturation to keep when creating a color. " +
                  "Colors follow the built-in artistic saturation function. " +
                  "This value determines how much of the saturation function " +
                  "to use. Values greater than 1.0 will over-apply the " +
                  "result and can potentially result in color data loss.")]
  public double Saturation { get; init; } = SATURATION_DEFAULT;

  [CommandOption("brightness", 'b',
    Description = "Percentage of brightness to keep when creating a color. " +
                  "Colors follow the built-in artistic brightness function. " +
                  "This value determines how much of the brightness function " +
                  "to use. Values greater than 1.0 will over-apply the " +
                  "result and can potentially result in color data loss.")]
  public double Brightness { get; init; } = BRIGHTNESS_DEFAULT;

  [CommandOption("hue-shift", 'm',
    Description =
      "How much of the hue spectrum (as a fraction between 0 and 1) should " +
      " be covered by a single color ramp.")]
  public double HueShift { get; init; } = HUE_SHIFT_DEFAULT;

  [CommandOption("hue-spectrum", 'u',
    Description = "The amount of the hue spectrum to cover in the palette " +
                  "(0-360).")]
  public int HueSpectrum { get; init; } = HUE_SPECTRUM_DEFAULT;

  // Computed
  public double TotalChangeInHuePerRamp => 360 * HueShift;

  public double ChangeInHuePerColor =>
    TotalChangeInHuePerRamp / NumColorsPerRamp;

  public int NumDesaturatedColors =>
    NumColorsPerRamp - NumColorsToTrimForDesaturatedRamp;

  public ValueTask ExecuteAsync(IConsole console)
  {
    // a palette is a list of ramps
    List<List<SKColor>> palette = [];

    var cwd = Environment.CurrentDirectory;
    var outputFile = Path.Combine(cwd, Output);

    console.Output.WriteLine("Creating a palette...");

    for (var rampIndex = 0; rampIndex < NumRamps; rampIndex++)
    {
      var hue = (int)(Hue + ((double)rampIndex / NumRamps * HueSpectrum));

      var colors = GetRamp(
        NumColorsPerRamp,
        ChangeInHuePerColor,
        hue,
        Saturation,
        Brightness,
        0d,
        1d
      );

      // compute the start progress and end progress such that the desaturated
      // color ramp trims the lightest and darkest colors from the ramp since
      // those colors would be nearly equivalent to their counterparts in
      // the saturated ramp.
      var desaturatedStartP =
        NumColorsToTrimForDesaturatedRamp / 2d / NumColorsPerRamp;
      var desaturatedEndP = 1 - desaturatedStartP;

      var desaturatedColors = GetRamp(
        NumDesaturatedColors,
        ChangeInHuePerColor,
        hue,
        Desaturate,
        Brightness,
        desaturatedStartP,
        desaturatedEndP
      ).Reverse<SKColor>();

      var ramp = colors.Concat(desaturatedColors).ToList();

      palette.Add(ramp);
    }

    var image = Render(palette);
    SaveImage(outputFile, image);

    console.Output.WriteLine($"Palette saved: {outputFile}");

    return new();
  }

  public static List<SKColor> GetRamp(
    int numColors,
    double changeInHuePerColor,
    int hue,
    double saturation,
    double brightness,
    double startP, /* start percentage */
    double endP /* end percentage */
  )
  {
    var colors = new SKColor[numColors];

    var middleIndex = numColors / 2;

    var changeInHuePerColorRemapped = changeInHuePerColor * (endP - startP);

    // create colors (from darkest to lightest)
    for (var colorIndex = 0; colorIndex < numColors; colorIndex++)
    {
      var p = (double)colorIndex / numColors;
      var remappedP = startP + (p * (endP - startP));
      var distanceFromMiddle = middleIndex - colorIndex;

      var h = hue - (distanceFromMiddle * changeInHuePerColorRemapped);
      var s = Colors.SlynyrdSaturationFunction(remappedP) * saturation;
      var b = Colors.SlynyrdBrightnessFunction(remappedP) * brightness;

      colors[colorIndex] = Colors.HsvToRgb((int)h, (int)s, (int)b);
    }

    return [.. colors];
  }

  public SKImage Render(List<List<SKColor>> palette)
  {
    var surface = SKSurface.Create(
      new SKImageInfo(
        (Scale * NumColorsPerRamp) +
        (Scale * (NumColorsPerRamp - NumColorsToTrimForDesaturatedRamp)),
        Scale * NumRamps
      )
    );

    var canvas = surface.Canvas;

    canvas.Clear(SKColors.Black);

    using var paint = new SKPaint();

    paint.IsAntialias = false;
    paint.Style = SKPaintStyle.Fill;

    // draw swatches from the palette
    for (var rampIndex = 0; rampIndex < NumRamps; rampIndex++)
    {
      var ramp = palette[rampIndex];

      for (var colorIndex = 0; colorIndex < ramp.Count; colorIndex++)
      {
        var color = ramp[colorIndex];

        paint.Color = new SKColor(color.Red, color.Green, color.Blue);

        var x = colorIndex;
        var y = rampIndex;

        canvas.DrawRect(
          x * Scale, y * Scale, Scale, Scale, paint
        );
      }
    }

    return surface.Snapshot();
  }

  public void SaveImage(string fullyQualifiedPath, SKImage image)
  {
    using var data = image.Encode(SKEncodedImageFormat.Png, 100);

    using var stream = File.OpenWrite(fullyQualifiedPath);

    data.SaveTo(stream);

    stream.Flush();
    stream.Close();
  }
}
