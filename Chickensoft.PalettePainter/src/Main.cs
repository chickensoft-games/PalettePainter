namespace Chickensoft.PalettePainter;

using System.Reflection;
using System.Threading.Tasks;
using CliFx;
public static class PalettePainter
{
  public static async Task<int> Main(string[] args)
  {
    var version = Assembly
      .GetEntryAssembly()!
      .GetCustomAttribute<AssemblyInformationalVersionAttribute>()!
      .InformationalVersion;

    return await new CliApplicationBuilder()
      .SetExecutableName("palettepainter")
      .SetTitle("Palette Painter")
      .SetVersion(version)
      .SetDescription(
        """
        Create a palette of colors with the specified number of ramps. You may
        customize hue shifts and saturation levels to create a unique palette.
        """
      )
      .AddCommandsFromThisAssembly()
      .Build().RunAsync(args);
  }
}
