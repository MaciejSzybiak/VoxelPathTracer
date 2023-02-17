using System.Numerics;

namespace VoxelPathTracing;

public class RayTracer
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
        return TraceInternal(ray, 30);
    }

    private Vector3 TraceInternal(Ray ray, int depth)
    {
        Hit hit;
        
        if (!_floorIntersection.Intersects(ray, out var floorHit))
        {
            if (!_gridIntersection.Intersects(ray, out hit))
            {
                return _world.BackgroundColor;
            }
        }
        else
        {
            if (!_gridIntersection.Intersects(ray, out var gridHit) || gridHit.Distance > floorHit.Distance)
            {
                hit = floorHit;
            }
            else
            {
                hit = gridHit;
            }
        }
        
        if (depth == 0)
        {
            return Vector3.Zero;
        }

        var correctedHitPoint = hit.Point + hit.Normal * 0.001f;
        var point = GetRandomPointOnUnitSphere() + correctedHitPoint + hit.Normal;
        var direction = Vector3.Normalize(point - correctedHitPoint);

        var incomingLight = TraceInternal(new Ray(correctedHitPoint, direction), depth - 1);
        if (IsSunVisibleFromHit(hit))
        {
            incomingLight += _world.Sun.Color * SunIntensity;
        }

        var light = hit.Material.Color * hit.Material.Emission +
                    hit.Material.Color * incomingLight;

        return light;
    }

    private bool IsSunVisibleFromHit(Hit hit)
    {
        if (Vector3.Dot(hit.Normal, _sunReflectionDir) < 0.001f)
        {
            return false;
        }
        
        var correctedHitPoint = hit.Point + hit.Normal * 0.001f;
        var ray = new Ray(correctedHitPoint, _sunReflectionDir);

        return !_floorIntersection.Intersects(ray, out var _) && !_gridIntersection.Intersects(ray, out var _);
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