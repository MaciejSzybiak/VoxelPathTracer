using System.Diagnostics;
using System.Numerics;
using CommandLine;
using MengerSpongeExample;
using VoxelPathTracer;

Parser.Default.ParseArguments<Options>(args)
    .WithParsed(Execute)
    .WithNotParsed(PrintErrors);

void Execute(Options options)
{
    if (!options.Validate(out var error))
    {
        PrintErrorMessage(error);
        return;
    }

    var color = EnvironmentFactory.GetColor(options.ColorValue);
    var gridProvider = new MengerSpongeGridProvider(options.Iterations, options.AddLight, color);
    var world = EnvironmentFactory.GetWorld(gridProvider);
    var camera = EnvironmentFactory.GetPerspectiveCamera(options.Iterations);
    var colorCorrection = EnvironmentFactory.GetColorCorrection();
    var image = Render(world, camera, colorCorrection, options);
    SaveImage(image, options);
}

void PrintErrors(IEnumerable<Error> errors)
{
    foreach (var error in errors)
    {
        PrintErrorMessage(error.ToString());
    }
}

void PrintErrorMessage(string? message)
{
    Console.WriteLine($"ERROR: {message}");

}

Vector3[,] Render(World world, PerspectiveCamera camera, ColorCorrection colorCorrection, Options options)
{
    Console.WriteLine("Rendering...");
    
    var renderer = new Renderer(world, camera, colorCorrection, options.Samples, (options.Resolution, options.Resolution));
    var image = new Vector3[1,1];
    var progress = new Progress<RenderProgress>();
    progress.ProgressChanged += (_, renderProgress) => image = renderProgress.Image;

    ExecuteRender(renderer, progress);
    
    return image;
}

void ExecuteRender(Renderer renderer, IProgress<RenderProgress> renderProgress)
{
    var stopwatch = new Stopwatch();
    stopwatch.Start();
    renderer.Render(renderProgress).Wait();
    stopwatch.Stop();

    var timeSpan = stopwatch.Elapsed;
    Console.WriteLine($"Completed in {timeSpan.Hours}:{timeSpan.Minutes}:{timeSpan.Seconds}.{timeSpan.Milliseconds}");
}

void SaveImage(Vector3[,] render, Options options)
{
    var image = new Image<Rgb24>(render.GetLength(0), render.GetLength(1));

    for (var y = 0; y < render.GetLength(1); y++)
    {
        for (var x = 0; x < render.GetLength(0); x++)
        {
            var value = render[x, y];
            image[x, render.GetLength(1) - 1 - y] = 
                new Rgb24((byte) (value.X * 255), (byte) (value.Y * 255), (byte) (value.Z * 255));
        }
    }

    var path = options.Path;
    if (!path.EndsWith(".png"))
    {
        path += ".png";
    }

    Console.WriteLine($"Saving image to: {path}");
    image.Save(path);
}
