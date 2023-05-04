using System.Numerics;

namespace VoxelPathTracer;

internal class RayTracer
{
    private readonly World _world;
    private readonly GridIntersection _gridIntersection;
    private readonly FloorIntersection _floorIntersection;
    private readonly Random _random;

    private readonly Vector3 _sunReflectionDir;
    private const float SunIntensity = 0.1f;

    public RayTracer(World world)
    {
        _world = world;
        _gridIntersection = new GridIntersection(_world.Grid);
        _floorIntersection = new FloorIntersection(_world.Floor);
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

        Vector3 correctedHitPoint;
        Vector3 direction;
        // if (_random.NextSingle() > hit.Material.Opacity)
        // {
        //     correctedHitPoint = hit.Point + hit.Normal * -0.001f;
        //     direction = GetRefractionDirection(hit, ray, correctedHitPoint);
        // }
        // else
        // {
            correctedHitPoint = hit.Point + hit.Normal * 0.001f;
            if (!GetReflectionDirection(hit, correctedHitPoint, ray, out direction))
            {
                return Vector3.Zero;
            }
        // }

        var incomingLight = TraceInternal(new Ray(correctedHitPoint, direction), depth - 1);
        if (IsSunVisibleFromHit(hit, correctedHitPoint))
        {
            incomingLight += _world.Sun.Color * SunIntensity;
        }

        if (hit.Material.Opacity < 1f)
        {
            correctedHitPoint = hit.Point + hit.Normal * -0.001f;
            direction = GetRefractionDirection(hit, ray, correctedHitPoint);
            var incomingRefraction = TraceInternal(new Ray(correctedHitPoint, direction), depth - 1);
            incomingLight = incomingLight * hit.Material.Opacity + incomingRefraction * (1f - hit.Material.Opacity);
        }

        var light = hit.Material.Color * hit.Material.Emission +
                    hit.Material.Color * incomingLight;

        return light;
    }

    private bool IntersectsWorld(Ray ray, out Hit hit)
    {
        if (!_floorIntersection.Intersects(ray, out hit))
        {
            if (!_gridIntersection.Intersects(ray, out hit))
            {
                return false;
            }
        }
        else
        {
            if (_gridIntersection.Intersects(ray, out var gridHit) && !(gridHit.Distance > hit.Distance))
            {
                hit = gridHit;
            }
        }

        return true;
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

    private Vector3 GetRefractionDirection(Hit hit, Ray incomingRay, Vector3 correctedHitPoint)
    {
        const float eta = 1f / 1.3f;

        var cosTheta = MathF.Min(Vector3.Dot(-incomingRay.Direction, hit.Normal), 1f);
        var perpendicular = eta * (incomingRay.Direction + cosTheta * hit.Normal);
        var parallel = -MathF.Sqrt(MathF.Abs(1f - perpendicular.LengthSquared())) * hit.Normal;
        return perpendicular + parallel;
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