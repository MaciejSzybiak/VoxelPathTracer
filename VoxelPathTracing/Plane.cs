using System.Numerics;

namespace VoxelPathTracing;

public class Plane
{
    public Vector3 Center { get; }
    public  Vector3 Normal { get; }
    private readonly Material _material;

    public Plane(Vector3 center, Vector3 normal, Material material)
    {
        Center = center;
        Normal = normal;
        _material = material;
    }

    public bool Intersects(Ray ray, out Hit hit)
    {
        var denominator = Vector3.Dot(Normal, ray.Direction);

        if (MathF.Abs(denominator) > 0.0001f)
        {
            var diff = Center - ray.Origin;
            var t = Vector3.Dot(diff, Normal) / denominator;

            if (t > 0.0001f)
            {
                hit = new Hit(Normal, ray.Origin + ray.Direction * t, _material, t);
                return true;
            }
        }

        hit = default;
        return false;
    }
}