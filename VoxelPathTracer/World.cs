using System.Collections.Immutable;
using System.Numerics;

namespace VoxelPathTracer;

public class World
{
    public ImmutableList<Volume> Volumes { get; }
    public Vector3 BackgroundColor { get; }
    public Sun Sun { get; }
    public int Reflections { get; }

    public World(Grid grid, Vector3 backgroundColor, Floor floor, Sun sun, int reflections = 30)
    {
        Volumes = new List<Volume> {grid, floor}.ToImmutableList();
        BackgroundColor = backgroundColor;
        Sun = sun;
        Reflections = reflections;
    }

    public World(List<Volume> volumes, Vector3 backgroundColor, Sun sun, int reflections = 30)
    {
        Volumes = volumes.ToImmutableList();
        BackgroundColor = backgroundColor;
        Sun = sun;
        Reflections = reflections;
    }
}