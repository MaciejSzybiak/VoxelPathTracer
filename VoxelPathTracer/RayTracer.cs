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

        var correctedHitPoint = hit.Point + hit.Normal * 0.001f;
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

    private bool GetReflectionDirection(Hit hit, Vector3 correctedHitPoint, Ray incomingRay, out Vector3 direction)
    {
        if (hit.Material.Mirror > 0f)
        {
            var isMetallicReflection = _random.NextSingle() < hit.Material.Mirror;

            if (isMetallicReflection)
            {
                var reflection = Vector3.Reflect(incomingRay.Direction, hit.Normal);
                if (hit.Material.Roughness > 0f)
                {
                    reflection += GetRandomPointOnScaledSphere(hit.Material.Roughness);
                }

                direction = Vector3.Normalize(reflection);
                return Vector3.Dot(direction, hit.Normal) > 0f;
            }
        }

        var point = GetRandomPointOnUnitSphere() + correctedHitPoint + hit.Normal;
        direction = Vector3.Normalize(point - correctedHitPoint);

        return true;
    }

    private bool IsSunVisibleFromHit(Hit hit, Vector3 correctedHitPoint)
    {
        var dir = Vector3.Normalize(_sunReflectionDir + GetRandomPointOnScaledSphere(_world.Sun.Softness));
        if (Vector3.Dot(hit.Normal, dir) < 0.001f)
        {
            return false;
        }

        var ray = new Ray(correctedHitPoint, dir);

        return !IntersectsWorld(ray, out var _);
    }

    private Vector3 GetRandomPointOnScaledSphere(float scale)
    {
        var scaleSquared = scale * scale;
        Vector3 point;
        do
        {
            point = new Vector3(RandomForUnitSphere() * scale, RandomForUnitSphere() * scale,
                RandomForUnitSphere() * scale);
        } while (point.LengthSquared() >= scaleSquared);

        return point;
    }

    private Vector3 GetRandomPointOnUnitSphere()
    {
        Vector3 point;
        do
        {
            point = new Vector3(RandomForUnitSphere(), RandomForUnitSphere(), RandomForUnitSphere());
        } while (point.LengthSquared() >= 1);

        return Vector3.Normalize(point);
    }

    private float RandomForUnitSphere()
    {
        return _random.NextSingle() * 2f - 1f;
    }
}