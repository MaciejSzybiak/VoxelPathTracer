using System.Numerics;

namespace VoxelPathTracing;

public class Material
{
    public Vector3 Color { get; }
    public float Emission { get; }
    public float Metallic { get; }
    public float Roughness { get; }

    public Material(Vector3 color, float emission, float metallic = 1f, float roughness = 0f)
    {
        Color = color;
        Emission = emission;
        Metallic = metallic;
        Roughness = roughness;
    }
}