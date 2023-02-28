using System.Numerics;

namespace VoxelPathTracing;

public class Material
{
    public Vector3 Color { get; }
    public float Emission { get; }
    public float Mirror { get; }
    public float Roughness { get; }

    public Material(Vector3 color, float emission, float mirror = 0f, float roughness = 0f)
    {
        Color = color;
        Emission = emission;
        Mirror = mirror;
        Roughness = roughness;
    }
}