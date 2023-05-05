using System.Numerics;

namespace VoxelPathTracer;

public partial class Grid
{
    public (int X, int Y, int Z) Size { get; }
    public (int X, int Y, int Z) Origin { get; }
    private readonly Material?[,,] _voxels;
    
    public Vector3 Min { get; }
    public Vector3 Max { get; }
    
    public Grid((int X, int Y, int Z) size, (int X, int Y, int Z) origin)
    {
        Size = size;
        Origin = origin;
        Min = new Vector3(origin.X, origin.Y, origin.Z);
        Max = new Vector3(origin.X + size.X, origin.Y + size.Y, origin.Z + size.Z);
        _voxels = new Material?[size.X, size.Y, size.Z];
    }

    public Material? this[int x, int y, int z]
    {
        get => _voxels[x - Origin.X, y - Origin.Y, z - Origin.Z];
        set => _voxels[x, y, z] = value;
    }
}