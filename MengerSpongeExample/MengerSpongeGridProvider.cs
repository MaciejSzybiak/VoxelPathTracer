using System.Numerics;
using VoxelPathTracer;

namespace MengerSpongeExample;

public class MengerSpongeGridProvider : IGridProvider
{
    private readonly int _iterations;
    private readonly bool _addLight;
    private readonly Vector3 _color;
    private int Size => (int) Math.Pow(3, _iterations);
    private int Origin => -Size / 2;

    public MengerSpongeGridProvider(int iterations, bool addLight, Vector3 color)
    {
        _iterations = iterations;
        _addLight = addLight;
        _color = color;
    }

    public Grid Get()
    {
        var grid = new Grid((Size, Size, Size), (Origin, 0, Origin));

        Fill(grid);
        CarveRecursively(grid, _iterations, new Cube((0, 0, 0), (Size, Size, Size)));

        if (_addLight)
        {
            AddLight(grid);
        }

        return grid;
    }

    private void Fill(Grid grid)
    {
        var material = new Material(_color, 0f, 0.1f, 0.8f, 0f);
        for (var x = 0; x < Size; x++)
        for (var y = 0; y < Size; y++)
        for (var z = 0; z < Size; z++)
        {
            grid[x, y, z] = material;
        }
    }

    private void CarveRecursively(Grid grid, int iterationsLeft, Cube cube)
    {
        cube.GetEmptyChildren().ForEach(c => CarveCube(grid, c));

        if (iterationsLeft == 1) return;

        cube.GetFullChildren().ForEach(c => CarveRecursively(grid, iterationsLeft - 1, c));
    }

    private void CarveCube(Grid grid, Cube cube)
    {
        for (var x = cube.Start.x; x < cube.End.x; x++)
        for (var y = cube.Start.y; y < cube.End.y; y++)
        for (var z = cube.Start.z; z < cube.End.z; z++)
        {
            grid[x, y, z] = null;
        }
    }

    private void AddLight(Grid grid)
    {
        var factor = Size / 12;
        var start = Math.Max(factor * 4, 1);
        var end = Size - start;
        for (var x = start; x < end; x++)
        for (var y = start; y < end; y++)
        for (var z = start; z < end; z++)
        {
            grid[x, y, z] = new Material(Vector3.One, 1.5f);
        }
    }

    private class Cube
    {
        public (int x, int y, int z) Start { get; }
        public (int x, int y, int z) End { get; }

        private readonly (int x, int y, int z) _third;
        private readonly (int x, int y, int z) _twoThird;

        public Cube((int x, int y, int z) start, (int x, int y, int z) end)
        {
            Start = start;
            End = end;
            var xFraction = (end.x - start.x) / 3;
            var yFraction = (end.x - start.x) / 3;
            var zFraction = (end.x - start.x) / 3;
            _third = (start.x + xFraction, start.y + yFraction, start.z + zFraction);
            _twoThird = (start.x + xFraction * 2, start.y + yFraction * 2, start.z + zFraction * 2);
        }

        public List<Cube> GetFullChildren()
        {
            return new List<Cube>
            {
                // center
                new(_twoThird, End),

                // Start.x side
                new(Start, _third),
                new((Start.x, Start.y, _third.z), (_third.x, _third.y, _twoThird.z)),
                new((Start.x, Start.y, _twoThird.z), (_third.x, _third.y, End.z)),
                new((Start.x, _twoThird.y, _third.z), (_third.x, End.y, _twoThird.z)),
                new((Start.x, _twoThird.y, _twoThird.z), (_third.x, End.y, End.z)),
                new((Start.x, _twoThird.y, Start.z), (_third.x, End.y, _third.z)),
                new((Start.x, _third.y, Start.z), (_third.x, _twoThird.y, _third.z)),
                new((Start.x, _third.y, _twoThird.z), (_third.x, _twoThird.y, End.z)),

                // End.x side
                new((_twoThird.x, Start.y, Start.z), (End.x, _third.y, _third.z)),
                new((_twoThird.x, Start.y, _third.z), (End.x, _third.y, _twoThird.z)),
                new((_twoThird.x, Start.y, _twoThird.z), (End.x, _third.y, End.z)),
                new((_twoThird.x, _twoThird.y, _third.z), (End.x, End.y, _twoThird.z)),
                new((_twoThird.x, _twoThird.y, _twoThird.z), (End.x, End.y, End.z)),
                new((_twoThird.x, _twoThird.y, Start.z), (End.x, End.y, _third.z)),
                new((_twoThird.x, _third.y, Start.z), (End.x, _twoThird.y, _third.z)),
                new((_twoThird.x, _third.y, _twoThird.z), (End.x, _twoThird.y, End.z)),

                // middle
                new((_third.x, Start.y, Start.z), (_twoThird.x, _third.y, _third.z)),
                new((_third.x, _twoThird.y, Start.z), (_twoThird.x, End.y, _third.z)),
                new((_third.x, _twoThird.y, _twoThird.z), (_twoThird.x, End.y, End.z)),
                new((_third.x, Start.y, _twoThird.z), (_twoThird.x, _third.y, End.z)),
            };
        }

        public List<Cube> GetEmptyChildren()
        {
            return new List<Cube>
            {
                new(_third, _twoThird),

                new((Start.x, _third.y, _third.z), (_third.x, _twoThird.y, _twoThird.z)),
                new((_third.x, Start.y, _third.z), (_twoThird.x, _third.y, _twoThird.z)),
                new((_third.x, _third.y, Start.z), (_twoThird.x, _twoThird.y, _third.z)),

                new((_twoThird.x, _third.y, _third.z), (End.x, _twoThird.y, _twoThird.z)),
                new((_third.x, _twoThird.y, _third.z), (_twoThird.x, End.y, _twoThird.z)),
                new((_third.x, _third.y, _twoThird.z), (_twoThird.x, _twoThird.y, End.z))
            };
        }
    }
}