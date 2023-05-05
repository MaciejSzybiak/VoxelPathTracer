namespace VoxelPathTracer;

public abstract class Volume
{
    internal abstract bool Intersects(Ray ray, out Hit hit);
}