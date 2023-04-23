using System.Numerics;

namespace VoxelPathTracer;

public class ColorCorrection
{
    public float Gamma { get; }
    public float Exposure { get; }

    public ColorCorrection(float gamma, float exposure)
    {
        Gamma = gamma;
        Exposure = exposure;
    }

    public Vector3 Apply(Vector3 color)
    {
        return new Vector3(
            MathF.Pow(Exposure * MathF.Min(1f, color.X), Gamma),
            MathF.Pow(Exposure * MathF.Min(1f, color.Y), Gamma),
            MathF.Pow(Exposure * MathF.Min(1f, color.Z), Gamma)
        );
    }
}