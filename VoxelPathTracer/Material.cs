using System.Numerics;

namespace VoxelPathTracer;

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

    internal virtual Ray? GetBouncedRay(Ray incomingRay, Hit hit, Random random)
    {
        Vector3 direction;
        var correctedHitPoint = hit.Point + hit.Normal * 0.001f;
        if (hit.Material.Mirror > 0f)
        {
            var isMetallicReflection = random.NextSingle() < hit.Material.Mirror;

            if (isMetallicReflection)
            {
                var reflection = Vector3.Reflect(incomingRay.Direction, hit.Normal);
                if (Roughness > 0f)
                {
                    reflection += GetRandomPointOnScaledSphere(Roughness, random);
                }

                direction = Vector3.Normalize(reflection);
                if (Vector3.Dot(direction, hit.Normal) <= 0f)
                {
                    return null;
                }
                return new Ray(correctedHitPoint, direction);
            }
        }

        var point = GetRandomPointOnUnitSphere(random) + correctedHitPoint + hit.Normal;
        direction = Vector3.Normalize(point - correctedHitPoint);
        return new Ray(correctedHitPoint, direction);
    }
    
    private Vector3 GetRandomPointOnScaledSphere(float scale, Random random)
    {
        var scaleSquared = scale * scale;
        Vector3 point;
        do
        {
            point = new Vector3(RandomForUnitSphere(random) * scale, RandomForUnitSphere(random) * scale,
                RandomForUnitSphere(random) * scale);
        } while (point.LengthSquared() >= scaleSquared);

        return point;
    }

    private Vector3 GetRandomPointOnUnitSphere(Random random)
    {
        Vector3 point;
        do
        {
            point = new Vector3(RandomForUnitSphere(random), RandomForUnitSphere(random), RandomForUnitSphere(random));
        } while (point.LengthSquared() >= 1);

        return Vector3.Normalize(point);
    }

    private float RandomForUnitSphere(Random random)
    {
        return random.NextSingle() * 2f - 1f;
    }
}