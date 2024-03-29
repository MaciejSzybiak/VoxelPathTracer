﻿using System.Numerics;

namespace VoxelPathTracer;

internal readonly struct Hit
{
    public readonly Vector3 Normal;
    public readonly Vector3 Point;
    public readonly Material Material;
    public readonly float Distance;

    public Hit(Vector3 normal, Vector3 point, Material material, float distance)
    {
        Normal = normal;
        Point = point;
        Material = material;
        Distance = distance;
    }
}