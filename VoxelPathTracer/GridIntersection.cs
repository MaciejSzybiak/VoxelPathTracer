using System.Numerics;

namespace VoxelPathTracer;

internal class GridIntersection
{
    private const float RayCropEpsilon = 0.01f;
    
    private readonly Grid _grid;

    public GridIntersection(Grid grid)
    {
        _grid = grid;
    }

    public bool Intersects(Ray ray, out Hit hit)
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
        var xMin = _grid.Origin.X;
        var yMin = _grid.Origin.Y;
        var zMin = _grid.Origin.Z;
        var xMax = _grid.Size.X + _grid.Origin.X;
        var yMax = _grid.Size.Y + _grid.Origin.Y;
        var zMax = _grid.Size.Z + _grid.Origin.Z;

        var refracts = false;
        Material? lastVoxel = null;
        if (ray.Origin.X >= xMin && ray.Origin.X < xMax
              && ray.Origin.Y >= yMin && ray.Origin.Y < yMax
              && ray.Origin.Z >= zMin && ray.Origin.Z < zMax)
        {
            var voxel = _grid[xPosition, yPosition, zPosition];
            if (voxel is not null)
            {
                refracts = true;
                lastVoxel = voxel;
            }
        }

        while (rayLength < maxLength)
        {
            if (xPosition >= xMin && xPosition < xMax
                && yPosition >= yMin && yPosition < yMax
                && zPosition >= zMin && zPosition < zMax)
            {
                var voxel = _grid[xPosition, yPosition, zPosition];
                if (refracts)
                {
                    if (voxel is null)
                    {
                        hit = new Hit(hitNormal * -1, ray.Origin + ray.Direction * rayLength, lastVoxel, rayLength); // fixme
                        return true;
                    }
                }
                else if (voxel is not null)
                {
                    hit = new Hit(hitNormal, ray.Origin + ray.Direction * rayLength, voxel, rayLength);
                    return true;
                }

                lastVoxel = voxel;
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

        var v1 = (_grid.Min - ray.Origin) * ray.DirectionInverted;
        var v2 = (_grid.Max - ray.Origin) * ray.DirectionInverted;

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