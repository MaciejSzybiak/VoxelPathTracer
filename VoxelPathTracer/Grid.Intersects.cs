using System.Numerics;

namespace VoxelPathTracer;

public partial class Grid : Volume
{
    private const float RayCropEpsilon = 0.01f;
    
    internal override bool Intersects(Ray ray, out Hit hit)
    {
        if (!TryCropRayToBounds(ref ray, out var maxLength))
        {
            hit = default;
            return false;
        }
        
        var dx = ray.Direction.X;
        var dy = ray.Direction.Y;
        var dz = ray.Direction.Z;

        var xPosition = (int) Math.Floor(ray.Origin.X);
        var yPosition = (int) Math.Floor(ray.Origin.Y);
        var zPosition = (int) Math.Floor(ray.Origin.Z);

        int xStep, yStep, zStep;
        Vector3 xNormal, yNormal, zNormal;
        float xRayLength, yRayLength, zRayLength;

        var xRayStep = MathF.Sqrt(1 + (dy / dx) * (dy / dx) + (dz / dx) * (dz / dx));
        var yRayStep = MathF.Sqrt(1 + (dx / dy) * (dx / dy) + (dz / dy) * (dz / dy));
        var zRayStep = MathF.Sqrt(1 + (dx / dz) * (dx / dz) + (dy / dz) * (dy / dz));

        Vector3 hitNormal;

        // first step
        if (dx < 0)
        {
            xStep = -1;
            xNormal = new Vector3(1, 0, 0);
            xRayLength = (ray.Origin.X - xPosition) * xRayStep;
        }
        else
        {
            xStep = 1;
            xNormal = new Vector3(-1, 0, 0);
            xRayLength = (xPosition + 1 - ray.Origin.X) * xRayStep;
        }

        if (dy < 0)
        {
            yStep = -1;
            yNormal = new Vector3(0, 1, 0);
            yRayLength = (ray.Origin.Y - yPosition) * yRayStep;
        }
        else
        {
            yStep = 1;
            yNormal = new Vector3(0, -1, 0);
            yRayLength = (yPosition + 1 - ray.Origin.Y) * yRayStep;
        }
        
        if (dz < 0)
        {
            zStep = -1;
            zNormal = new Vector3(0, 0, 1);
            zRayLength = (ray.Origin.Z - zPosition) * zRayStep;
        }
        else
        {
            zStep = 1;
            zNormal = new Vector3(0, 0, -1);
            zRayLength = (zPosition + 1 - ray.Origin.Z) * zRayStep;
        }

        if (xRayLength < yRayLength)
        {
            hitNormal = xRayLength < zRayLength ? xNormal : zNormal;
        }
        else
        {
            hitNormal = yRayLength < zRayLength ? yNormal : zNormal;
        }
        
        var rayLength = 0f;
        var xMin = Origin.X;
        var yMin = Origin.Y;
        var zMin = Origin.Z;
        var xMax = Size.X + Origin.X;
        var yMax = Size.Y + Origin.Y;
        var zMax = Size.Z + Origin.Z;

        while (rayLength < maxLength)
        {
            if (xPosition >= xMin && xPosition < xMax
                && yPosition >= yMin && yPosition < yMax
                && zPosition >= zMin && zPosition < zMax)
            {
                var voxel = this[xPosition, yPosition, zPosition];
                if (voxel is not null)
                {
                    hit = new Hit(hitNormal, ray.Origin + ray.Direction * rayLength, voxel, rayLength);
                    return true;
                }
            }

            if (xRayLength < yRayLength)
            {
                if (xRayLength < zRayLength)
                {
                    xPosition += xStep;
                    rayLength = xRayLength;
                    xRayLength += xRayStep;
                    hitNormal = xNormal;
                }
                else
                {
                    zPosition += zStep;
                    rayLength = zRayLength;
                    zRayLength += zRayStep;
                    hitNormal = zNormal;
                }
            }
            else
            {
                if (yRayLength < zRayLength)
                {
                    yPosition += yStep;
                    rayLength = yRayLength;
                    yRayLength += yRayStep;
                    hitNormal = yNormal;
                }
                else
                {
                    zPosition += zStep;
                    rayLength = zRayLength;
                    zRayLength += zRayStep;
                    hitNormal = zNormal;
                }
            }
        }
        
        hit = new Hit();
        return false;
    }
    
    private bool TryCropRayToBounds(ref Ray ray, out float maxLength)
    {
        var tMin = float.NegativeInfinity;
        var tMax = float.PositiveInfinity;

        var v1 = (Min - ray.Origin) * ray.DirectionInverted;
        var v2 = (Max - ray.Origin) * ray.DirectionInverted;

        for (var i = 0; i < 3; i++)
        {
            tMin = MathF.Max(tMin, MathF.Min(v1[i], v2[i]));
            tMax = MathF.Min(tMax, MathF.Max(v1[i], v2[i]));
        }

        if (tMin >= 0)
        {
            ray.Origin += ray.Direction * (tMin - RayCropEpsilon);
        }

        maxLength = tMax - tMin + RayCropEpsilon;
        return tMax >= tMin;
    }
}