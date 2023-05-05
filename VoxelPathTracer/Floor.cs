using System.Numerics;

namespace VoxelPathTracer;

public class Floor : Volume
{
    public float Height { get; }
    public Material Material { get; }
    private readonly Vector3 _normal = Vector3.UnitY;

    public Floor(float height, Material material)
    {
        Height = height;
        Material = material;
    }

    internal override bool Intersects(Ray ray, out Hit hit)
    {
        if (ray.Direction.Y >= -0.0001f)
        {
            hit = default;
            return false;
        }

        var t1 = -1 * (Vector3.Dot(ray.Origin, _normal) + Height);
        var t2 = Vector3.Dot(ray.Direction, _normal);

        var distance = t1 / t2;

        var point = ray.Origin + ray.Direction * distance;

        hit = new Hit(_normal, point, Material, distance);
        return true;
    }
}