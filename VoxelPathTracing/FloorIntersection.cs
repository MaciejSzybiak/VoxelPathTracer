using System.Numerics;

namespace VoxelPathTracing;

internal class FloorIntersection
{
    private readonly Floor _floor;
    private readonly Vector3 _normal = Vector3.UnitY;

    public FloorIntersection(Floor floor)
    {
        _floor = floor;
    }

    public bool Intersects(Ray ray, out Hit hit)
    {
        if (ray.Direction.Y >= -0.0001f)
        {
            hit = default;
            return false;
        }

        var t1 = -1 * (Vector3.Dot(ray.Origin, _normal) + _floor.Height);
        var t2 = Vector3.Dot(ray.Direction, _normal);

        var distance = t1 / t2;

        var point = ray.Origin + ray.Direction * distance;

        hit = new Hit(_normal, point, _floor.Material, distance);
        return true;
    }
}