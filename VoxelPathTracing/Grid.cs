using System.Numerics;

namespace VoxelPathTracing;

public class Grid
{
    public int Size { get; }
    private readonly Material?[,,] _voxels;
    
    public Grid(int size)
    {
        Size = size;
        _voxels = new Material?[size, size, size];
    }

    public Material? this[int x, int y, int z]
    {
        get => _voxels[x, y, z];
        set => _voxels[x, y, z] = value;
    }
}