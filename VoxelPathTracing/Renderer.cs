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

        _xHalfFraction = 2f / _halfResolutionX;
        _xFraction = 1f / _halfResolutionX;
        _yHalfFraction = 2f / _halfResolutionY;
        _yFraction = 1f / _halfResolutionY;
    }

    public async Task Render(IProgress<RenderProgress> progress, CancellationToken cancellationToken)
    {
        var quads = new List<Quad>();
        var renderProgress = new RenderProgress(_resolution);
        var taskCount = 0;
        var currentTaskCount = 0;
        
        for (var x = 0; x < _resolution.x; x += TileSize)
        {
            for (var y = 0; y < _resolution.y; y += TileSize)
            {
                var xEnd = Math.Min(_resolution.x, x + TileSize);
                var yEnd = Math.Min(_resolution.y, y + TileSize);

                var quad = new Quad(x, y, xEnd, yEnd);
                quads.Add(quad);

                taskCount++;
            }
        }

        try
        {
            await Parallel.ForEachAsync(quads, cancellationToken, async (quad, token) =>
            {
                var renderedQuad = await RunWorkOnQuad(quad);

                currentTaskCount++;
                var lastPercentage = renderProgress.Percentage;
                renderProgress.Percentage = currentTaskCount * 100 / taskCount;

                for (var x = quad.XStart; x < quad.XEnd; x++)
                {
                    for (var y = quad.YStart; y < quad.YEnd; y++)
                    {
                        renderProgress.SetPixel(x, y, _colorCorrection.Apply(renderedQuad.Image[x - quad.XStart, y - quad.YStart] / _samples));
                    }
                }

                if (lastPercentage != renderProgress.Percentage)
                {
                    Console.WriteLine($"Rendered {renderProgress.Percentage}%");
                }

                progress.Report(renderProgress);
            });
        }
        catch (TaskCanceledException)
        {
        }
    }

    private async Task<RenderedQuad> RunWorkOnQuad(object? quadObject)
    {
        var quad = (Quad) quadObject!;
        var renderedQuad = new RenderedQuad(quad);

        await Task.Run(() =>
        {
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

                        renderedQuad.Image[x - quad.XStart, y - quad.YStart] += _rayTracer.Trace(ray);
                    }
                }
            }
        });
        
        return renderedQuad;
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

    private class RenderedQuad
    {
        public readonly Vector3[,] Image;

        public RenderedQuad(Quad quad)
        {
            Image = new Vector3[quad.XEnd - quad.XStart, quad.YEnd - quad.YStart];
        }
    }
}