using System.Numerics;

namespace VoxelPathTracer;

internal static class RandomUtil
{
    internal static Vector3 GetRandomPointOnScaledSphere(float scale, Random random)
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

    internal static Vector3 GetRandomPointOnUnitSphere(Random random)
    {
        Vector3 point;
        do
        {
            point = new Vector3(RandomForUnitSphere(random), RandomForUnitSphere(random), RandomForUnitSphere(random));
        } while (point.LengthSquared() >= 1);

        return Vector3.Normalize(point);
    }

    internal static float RandomForUnitSphere(Random random)
    {
        return random.NextSingle() * 2f - 1f;
    }
}