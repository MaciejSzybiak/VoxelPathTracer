namespace VoxelPathTracer;

public class Floor
{
    public float Height { get; }
    public Material Material { get; }

    public Floor(float height, Material material)
    {
        Height = height;
        Material = material;
    }
}