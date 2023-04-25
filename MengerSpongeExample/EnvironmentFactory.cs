using System.Numerics;
using VoxelPathTracer;

namespace MengerSpongeExample;

internal static class EnvironmentFactory
{
    private static readonly Dictionary<ColorValue, Vector3> Colors = new()
    {
        {ColorValue.White, Vector3.One},
        {ColorValue.Gray, Vector3.One * 0.8f},
        {ColorValue.Black, Vector3.Zero},
        {ColorValue.Cyan, new Vector3(0.8f, 1f, 1f)},
        {ColorValue.Yellow, new Vector3(1f, 1f, 0.8f)},
        {ColorValue.Red, new Vector3(1f, 0.8f, 0.8f)},
        {ColorValue.Green, new Vector3(0.8f, 1f, 0.8f)},
        {ColorValue.Blue, new Vector3(0.8f, 0.8f, 1f)}
    };
    
    public static PerspectiveCamera GetPerspectiveCamera(int iterations)
    {
        var distance = (float) Math.Pow(3, iterations) * 2;
        return new PerspectiveCamera(0.4f, 1f, new Vector3(-distance, distance, -distance), 
            new Vector3(0, distance / 4, 0), Vector3.UnitY);
    }

    public static ColorCorrection GetColorCorrection()
    {
        return new ColorCorrection(2.2f, 1f);
    }

    public static World GetWorld(IGridProvider gridProvider)
    {
        var sun = new Sun(Vector3.Normalize(new Vector3(1f, -1f, -0.2f)), Vector3.One, 0.08f);
        var floor = new Floor(0, new Material(Vector3.One * 0.8f, 0, 0.5f, 0.35f));
        return new World(gridProvider.Get(), Vector3.One * 0.8f, floor, sun);
    }

    public static Vector3 GetColor(ColorValue colorValue)
    {
        return Colors[colorValue];
    }
}