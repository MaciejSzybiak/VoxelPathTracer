using System.Numerics;

namespace VoxelPathTracing;

public class World
{
    public Grid Grid { get; }
    public Floor Floor { get; }
    public Vector3 BackgroundColor { get; }

    public World(Grid grid, Vector3 backgroundColor, Floor floor)
    {
        Grid = grid;
        BackgroundColor = backgroundColor;
        Floor = floor;
    }
}