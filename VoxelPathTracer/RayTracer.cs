using System.Numerics;

namespace VoxelPathTracer;

internal class RayTracer
{
    private readonly World _world;
    private readonly Random _random;
    private readonly Vector3 _sunReflectionDir;
    private const float SunIntensity = 0.1f;

    public RayTracer(World world)
    {
        _world = world;
        _random = new Random();
        _sunReflectionDir = _world.Sun.Direction * -1;
    }

    public Vector3 Trace(Ray ray)
    {
        return TraceInternal(ray, _world.Reflections);
    }

    private Vector3 TraceInternal(Ray ray, int depth)
    {
        if (depth == 0)
        {
            return Vector3.Zero;
        }

        if (!IntersectsWorld(ray, out var hit))
        {
            return _world.BackgroundColor;
        }

        var bounce = hit.Material.GetBouncedRay(ray, hit, _random);
        if (!bounce.HasValue)
        {
            return Vector3.Zero;
        }

        var incomingLight = TraceInternal(bounce.Value, depth - 1) * (1 - SunIntensity);
        if (IsSunVisibleFromHit(hit, bounce.Value.Origin))
        {
            incomingLight += _world.Sun.Color * SunIntensity;
        }

        var light = hit.Material.Color * hit.Material.Emission +
                    hit.Material.Color * incomingLight;

        return light;
    }

    private bool IntersectsWorld(Ray ray, out Hit hit)
    {
        var distance = float.PositiveInfinity;
        hit = default;
        foreach (var volume in _world.Volumes)
        {
            if (volume.Intersects(ray, out var newHit) && newHit.Distance < distance)
            {
                distance = newHit.Distance;
                hit = newHit;
            }
        }

        return distance < float.PositiveInfinity;
    }

    private bool IsSunVisibleFromHit(Hit hit, Vector3 correctedHitPoint)
    {
        var dir = Vector3.Normalize(_sunReflectionDir + RandomUtil.GetRandomPointOnScaledSphere(_world.Sun.Softness, _random));
        if (Vector3.Dot(hit.Normal, dir) < 0.001f)
        {
            return false;
        }

        var ray = new Ray(correctedHitPoint, dir);

        return !IntersectsWorld(ray, out var _);
    }
}