using System.Numerics;

namespace VoxelPathTracing;

public struct Ray
{
    public Vector3 Origin { get; set; }
    public Vector3 Direction { get; }
    public Vector3 DirectionInverted { get; }

    public Ray(Vector3 origin, Vector3 direction)
    {
        Origin = origin;
        Direction = direction;
        DirectionInverted = new Vector3(1f / direction.X, 1f / direction.Y, 1f / direction.Z);
    }
}