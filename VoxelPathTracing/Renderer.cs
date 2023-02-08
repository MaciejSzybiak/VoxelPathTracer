using System.Numerics;

namespace VoxelPathTracing;

public class Renderer
{
    private const int TileSize = 64;
    
    private readonly int _samples;
    private readonly PerspectiveCamera _perspectiveCamera;
    private readonly ColorCorrection _colorCorrection;

    private readonly Random _random = new();
    private readonly float _halfResolutionX;
    private readonly float _halfResolutionY;
    private readonly Vector3[,] _image;
    private readonly RayTracer _rayTracer;
    private readonly float _xHalfFraction;
    private readonly float _xFraction;
    private readonly float _yHalfFraction;
    private readonly float _yFraction;

    private readonly (int x, int y) _resolution;

    public Renderer(World world, PerspectiveCamera perspectiveCamera, ColorCorrection colorCorrection, int samples, (int x, int y) resolution)
    {
        _samples = samples;
        _colorCorrection = colorCorrection;
        _perspectiveCamera = perspectiveCamera;
        _rayTracer = new RayTracer(world);
        _resolution = resolution;
        _halfResolutionX = resolution.x / 2f;
        _halfResolutionY = resolution.y / 2f;

        _image = new Vector3[resolution.x, resolution.y];

        _xHalfFraction = 2f / _halfResolutionX;
        _xFraction = 1f / _halfResolutionX;
        _yHalfFraction = 2f / _halfResolutionY;
        _yFraction = 1f / _halfResolutionY;
    }

    public Vector3[,] Render()
    {
        RunThreads();

        for (var x = 0; x < _resolution.x; x++)
        {
            for (var y = 0; y < _resolution.y; y++)
            {
                _image[x, y] = _colorCorrection.Apply(_image[x, y] / _samples);
            }
        }

        return _image;
    }

    private void RunThreads()
    {
        var tasks = new List<Task>();
        
        for (var x = 0; x < _resolution.x; x += TileSize)
        {
            for (var y = 0; y < _resolution.y; y += TileSize)
            {
                var xEnd = Math.Min(_resolution.x, x + TileSize);
                var yEnd = Math.Min(_resolution.y, y + TileSize);

                var quad = new Quad(x, y, xEnd, yEnd);

                var task = Task.Run(() => RunWorkOnThread(quad));
                tasks.Add(task);
            }
        }

        Console.WriteLine($"Generated {tasks.Count} tasks");
        
        var completed = 0;
        var percentage = 0;

        while (completed < tasks.Count)
        {
            completed = tasks.Count(t => t.IsCompleted);

            var newPercentage = (int) (completed / (float) tasks.Count * 10);

            if (newPercentage != percentage)
            {
                percentage = newPercentage;
                Console.WriteLine($"Completed {percentage}0%");
            }
        }
    }

    private void RunWorkOnThread(object? quadObject)
    {
        var quad = (Quad) quadObject!;

        for (var i = 0; i < _samples; i++)
        {
            for (var x = quad.XStart; x < quad.XEnd; x++)
            {
                for (var y = quad.YStart; y < quad.YEnd; y++)
                {
                    // normalize coordinates
                    var dx = x / _halfResolutionX - 1;
                    var dy = y / _halfResolutionY - 1;
                        
                    // randomize
                    dx += _random.NextSingle() * _xFraction - _xHalfFraction;
                    dy += _random.NextSingle() * _yFraction - _yHalfFraction;
                        
                    var ray = _perspectiveCamera.GetRay(new Vector2(dx, dy));

                    _image[x, y] += _rayTracer.Trace(ray);
                }
            }
        }
    }

    private class Quad
    {
        public readonly int XStart;
        public readonly int YStart;
        public readonly int XEnd;
        public readonly int YEnd;

        public Quad(int xStart, int yStart, int xEnd, int yEnd)
        {
            XStart = xStart;
            YStart = yStart;
            XEnd = xEnd;
            YEnd = yEnd;
        }
    }
}