using System.Numerics;

namespace VoxelPathTracing;

public class Sun
{
    public Vector3 Direction { get; }
    public Vector3 Color { get; }
    public float Softness { get; }

    public Sun(Vector3 direction, Vector3 color, float softness = 0f)
    {
        Direction = direction;
        Color = color;
        Softness = softness;
    }
}