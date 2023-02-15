using System.Diagnostics;
using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using VoxelPathTracing;

const int renderSamples = 2000;
const float gamma = 2.2f;
const float fov = 0.3f;
var resolution = (x: 3440, y: 1440);
var cameraOrigin = new Vector3(-9, 20, -10);
var gridOrigin = (x: 0, y: 0, z: 0);
var gridSize = (x: 6, y: 5, z: 6);

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

IGridProvider gridProvider = new ColumnGridProvider(gridOrigin, gridSize);
var grid = gridProvider.Get();
var world = new World(grid, Vector3.One * 0.7f, new Floor(0, new Material(Vector3.One * 0.7f, 0)));

var center = new Vector3(grid.Size.X / 2f + gridOrigin.x, grid.Size.Y / 2f + gridOrigin.y, grid.Size.Z / 2f + gridOrigin.z);
var target = center - Vector3.UnitY;
var camera = new PerspectiveCamera(fov, (float) resolution.x / resolution.y, 
    cameraOrigin, target, Vector3.UnitY);
var colorCorrection = new ColorCorrection(gamma, 1);
var renderer = new Renderer(world, camera, colorCorrection, renderSamples, resolution);
var stopwatch = new Stopwatch();

var progress = new Progress<RenderProgress>();
var image = new Vector3[1,1];
progress.ProgressChanged += (sender, renderProgress) => image = renderProgress.Image;

var cancellationTokenSource = new CancellationTokenSource();

stopwatch.Start();
await renderer.Render(progress, cancellationTokenSource.Token);
var timeSpan = stopwatch.Elapsed;
stopwatch.Stop();
Console.WriteLine($"Time: {timeSpan.Minutes}:{timeSpan.Seconds}.{timeSpan.Milliseconds}");

SaveImage(image);
