using System.Numerics;

namespace VoxelPathTracing;

public class Grid
{
    public (int X, int Y, int Z) Size { get; }
    public (int X, int Y, int Z) Origin { get; }
    private readonly Material?[,,] _voxels;
    
    public Grid((int X, int Y, int Z) size, (int X, int Y, int Z) origin)
    {
        Size = size;
        Origin = origin;
        _voxels = new Material?[size.X, size.Y, size.Z];
    }

    public Material? this[int x, int y, int z]
    {
        get => _voxels[x - Origin.X, y - Origin.Y, z - Origin.Z];
        set => _voxels[x, y, z] = value;
    }
}