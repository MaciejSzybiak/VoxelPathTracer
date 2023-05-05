using System.Diagnostics;
using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using VoxelPathTracer;

namespace BasicExample;

internal static class Program
{
    private const int RenderSamples = 1000;
    private const float Gamma = 2.2f;
    private const float Fov = 0.3f;
    private static readonly Vector3 CameraOrigin = new(-9, 20, -10);
    private static readonly (int x, int y, int z) GridOrigin = (x: 0, y: 0, z: 0);
    private static readonly (int x, int y, int z) GridSize = (x: 6, y: 5, z: 6);
    private static readonly (int x, int y) Resolution = (x: 500, y: 500);
    
    public static void Main()
    {
        var world = GetWorld();
        var camera = GetCamera();
        var colorCorrection = new ColorCorrection(Gamma, 1);

        var image = Render(world, camera, colorCorrection);

        SaveImage(image);
    }

    private static Vector3[,] Render(World world, PerspectiveCamera camera, ColorCorrection colorCorrection)
    {
        var renderer = new Renderer(world, camera, colorCorrection, RenderSamples, Resolution);

        var image = new Vector3[1,1];
        var progress = new Progress<RenderProgress>();
        progress.ProgressChanged += (_, renderProgress) => image = renderProgress.Image;

        ExecuteRender(renderer, progress);

        return image;
    }

    private static void ExecuteRender(Renderer renderer, IProgress<RenderProgress> progress)
    {
        var stopwatch = new Stopwatch();
        
        stopwatch.Start();
        renderer.Render(progress).Wait();
        stopwatch.Stop();
        
        var timeSpan = stopwatch.Elapsed;
        Console.WriteLine($"Time: {timeSpan.Hours}:{timeSpan.Minutes}:{timeSpan.Seconds}.{timeSpan.Milliseconds}");
    }

    private static World GetWorld()
    {
        IGridProvider gridProvider = new RandomGridProvider(GridOrigin, GridSize);
        var grid = gridProvider.Get();
        var sun = new Sun(Vector3.Normalize(new Vector3(1f, -1f, -0.5f)), Vector3.One, 0.03f);
        var floor = new Floor(0, new Material(Vector3.One * 0.65f, 0, 0.5f, 0.35f));
        return new World(grid, Vector3.One * 0.8f, floor, sun);
    }

    private static PerspectiveCamera GetCamera()
    {
        var center = new Vector3(GridSize.x / 2f + GridOrigin.x, GridSize.y / 2f + GridOrigin.y, 
            GridSize.z / 2f + GridOrigin.z);
        var target = center - Vector3.UnitY;
        return new PerspectiveCamera(Fov, (float) Resolution.x / Resolution.y, 
            CameraOrigin, target, Vector3.UnitY);
    }
    
    private static void SaveImage(Vector3[,] render)
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

        image.Save("image.png");
    }
}