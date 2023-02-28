using System.Numerics;

namespace VoxelPathTracing;

public class World
{
    public Grid Grid { get; }
    public Floor Floor { get; }
    public Vector3 BackgroundColor { get; }
    public Sun Sun { get; }
    public int Reflections { get; }

    public World(Grid grid, Vector3 backgroundColor, Floor floor, Sun sun, int reflections = 30)
    {
        Grid = grid;
        BackgroundColor = backgroundColor;
        Floor = floor;
        Sun = sun;
        Reflections = reflections;
    }
}