using System.Numerics;

namespace VoxelPathTracer;

public class ColumnGridProvider : IGridProvider
{
    private readonly (int X, int Y, int Z) _size;
    private const float MaxColor = 0.85f;
    private const float MinColor = 0.55f;
    private readonly Random _random = new(999);

    private readonly (int X, int Y, int Z) _origin;

    public ColumnGridProvider((int X, int Y, int Z) origin, (int X, int Y, int Z) size)
    {
        _origin = origin;
        _size = size;
    }

    public Grid Get()
    {
        var grid = new Grid(_size, _origin);

        for (var x = 0; x < _size.X; x++)
        {
            for (var z = 0; z < _size.Z; z++)
            {
                var height = _random.Next(0, _size.Y - 1);
                Vector3 color;
                float emission;
                if (_random.Next() % 5 == 0)
                {
                    color = GetEmissiveColor();
                    emission = 0.5f;
                }
                else
                {
                    color = GetRandomVoxelColor();
                    emission = 0f;
                }

                for (var y = 0; y < _size.Y; y++)
                {
                    if (y <= height)
                    {
                        grid[x, y, z] = new Material(color, emission);
                    }
                }
            }
        }

        return grid;
    }

    private Vector3 GetRandomVoxelColor()
    {
        var rnd = GetRandomColorComponent();
        return new Vector3(0.9f, rnd, rnd);
    }

    private Vector3 GetEmissiveColor()
    {
        return new Vector3(1f, 1f, 0.95f);
    }

    private float GetRandomColorComponent()
    {
        return _random.NextSingle() * (MaxColor - MinColor) + MinColor;
    }
}