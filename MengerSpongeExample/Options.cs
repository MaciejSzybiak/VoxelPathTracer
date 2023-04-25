using CommandLine;

namespace MengerSpongeExample;

// ReSharper disable once ClassNeverInstantiated.Global
internal class Options
{
    [Option('i', "iterations", Default = 3, HelpText = "Number of iterations in range [1, 6]")]
    public int Iterations { get; }
    
    [Option('s', "samples", Default = 30, HelpText = "Number of render samples")]
    public int Samples { get; }
    
    [Option('r', "resolution", Default = 500, HelpText = "Resolution of rendered image")]
    public int Resolution { get; }
    
    [Option('p', "path", Default = "image.png", HelpText = "Path where rendered image will be saved")]
    public string Path { get; }
    
    [Option('l', "light", Default = false, HelpText = "Add a light in the middle of the sponge")]
    public bool AddLight { get; }

    [Option('c', "color", Default = ColorValue.Cyan, HelpText = "White, Gray, Black, Cyan, Yellow, Red, Green, Blue")]
    public ColorValue ColorValue { get; }
    
    public Options(int iterations, int samples, int resolution, string path, bool addLight, ColorValue colorValue)
    {
        Iterations = iterations;
        Samples = samples;
        Resolution = resolution;
        Path = path;
        AddLight = addLight;
        ColorValue = colorValue;
    }

    public bool Validate(out string error)
    {
        if (Iterations is < 1 or > 6)
        {
            error = "Iterations should be in range [1, 6]";
            return false;
        }

        if (Samples < 1)
        {
            error = "At least 1 render sample is required";
            return false;
        }

        if (Resolution is < 16 or > 16384)
        {
            error = "Resolution should be in range [16, 16384]";
            return false;
        }

        error = "";
        return true;
    }
}