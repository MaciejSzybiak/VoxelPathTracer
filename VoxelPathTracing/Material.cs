using System.Numerics;

namespace VoxelPathTracing;

public class Material
{
    public Vector3 Color { get; }
    public float Emission { get; }

    public Material(Vector3 color, float emission)
    {
        Color = color;
        Emission = emission;
    }
}