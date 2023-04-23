using System.Numerics;

namespace VoxelPathTracer;

public class RenderProgress
{
    public int Percentage { get; set; }
    public Vector3[,] Image { get; }

    public RenderProgress((int X, int Y) resolution)
    {
        Image = new Vector3[resolution.X, resolution.Y];
    }

    public void SetPixel(int x, int y, Vector3 color)
    {
        Image[x, y] = color;
    }
}