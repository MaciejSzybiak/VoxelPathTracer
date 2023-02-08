using System.Numerics;

namespace VoxelPathTracing;

public class TestGridProvider : IGridProvider
{
    private const int GridSize = 11;
    private const float Cutout = 2f;
    
    public Grid Get()
    {
        var grid = new Grid(GridSize);

        for (var x = 0; x < GridSize; x++)
        {
            for (var y = 0; y < GridSize; y++)
            {
                for (var z = 0; z < GridSize; z++)
                {
                    if (x > GridSize / Cutout * 1.4f || y < GridSize / Cutout || z > GridSize / Cutout * 1.4f)
                    {
                        var color = new Vector3(0.8f, 0.8f, 0.8f);
                        grid[x, y, z] = new Material(color, 0f);
                    }
                }
            }
        }
        grid[(int) (GridSize / Cutout - 4), (int) (GridSize / Cutout + 1), (int) (GridSize / Cutout - 4)] = new Material(new Vector3(0.9f, 0.7f, 0.7f), 0f);
        grid[0, GridSize - 1, 7] = new Material(Vector3.One * 0.8f, 0f);

        return grid;
    }
}