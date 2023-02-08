using System.Diagnostics;
using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using VoxelPathTracing;

const int renderSamples = 1000;
const float gamma = 2.2f;
const float fov = 0.45f;
var resolution = (x: 1000, y: 1000);
var cameraOrigin = new Vector3(-6, 15, -10);

void SaveImage(Vector3[,] render)
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

Console.WriteLine($"SIMD: {Vector.IsHardwareAccelerated}");

IGridProvider gridProvider = new ColumnGridProvider();
var grid = gridProvider.Get();
var world = new World(grid, Vector3.One, new Floor(0, new Material(Vector3.One * 0.7f, 0)));

var camera = new PerspectiveCamera(fov, (float) resolution.x / resolution.y, 
    cameraOrigin, Vector3.One * (grid.Size / 2f) - Vector3.UnitY * 1.7f, Vector3.UnitY);
var colorCorrection = new ColorCorrection(gamma, 1);
var renderer = new Renderer(world, camera, colorCorrection, renderSamples, resolution);
var stopwatch = new Stopwatch();

stopwatch.Start();
var render = renderer.Render();
stopwatch.Stop();
var timeSpan = stopwatch.Elapsed;
Console.WriteLine($"Time: {timeSpan.Minutes}:{timeSpan.Seconds}.{timeSpan.Milliseconds}");

SaveImage(render);
