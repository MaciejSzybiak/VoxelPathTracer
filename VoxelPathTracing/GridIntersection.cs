using System.Numerics;

namespace VoxelPathTracing;

public class GridIntersection
{
    private readonly Grid _grid;

    public GridIntersection(Grid grid)
    {
        _grid = grid;
    }

    public bool Intersects(Ray ray, out Hit hit)
    {
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
            if (xRayLength < zRayLength)
            {
                hitNormal = xNormal;
            }
            else
            {
                hitNormal = zNormal;
            }
        }
        else
        {
            if (yRayLength < zRayLength)
            {
                hitNormal = yNormal;
            }
            else
            {
                hitNormal = zNormal;
            }
        }
        
        var rayLength = 0f;

        while (rayLength < 50)
        {
            if (xPosition >= 0 && xPosition < _grid.Size
               && yPosition >= 0 && yPosition < _grid.Size
               && zPosition >= 0 && zPosition < _grid.Size)
            {
                var voxel = _grid[xPosition, yPosition, zPosition];
                if (voxel is not null)
                {
                    hit = new Hit(hitNormal, ray.Origin + ray.Direction * rayLength, voxel, rayLength);
                    return true;
                }
            }
            // else if (yPosition < 0 && ray.Direction.Y < 0)
            // {
            //     hit = new Hit(Vector3.UnitY, ray.Origin + ray.Direction * rayLength, (int.MaxValue, 0, 0), rayLength);
            //     return true;
            // }

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
}