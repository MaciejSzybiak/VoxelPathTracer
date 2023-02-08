using System.Numerics;

namespace VoxelPathTracing;

public class PerspectiveCamera
{
    private readonly Vector3 _origin;

    private readonly Vector3 _forward;
    private readonly Vector3 _up;
    private readonly Vector3 _right;
    private readonly float _height;
    private readonly float _width;

    public PerspectiveCamera(float fov, float aspectRatio, Vector3 origin, Vector3 target, Vector3 upHint)
    {
        _origin = origin;

        _forward = Vector3.Normalize(target - _origin);
        _right = Vector3.Normalize(Vector3.Cross(_forward, upHint));
        _up = Vector3.Cross(_right, _forward);

        _height = MathF.Tan(fov);
        _width = _height * aspectRatio;
    }

    public Ray GetRay(Vector2 point)
    {
        var direction =
            _forward
            + point.X * _width * _right
            + point.Y * _height * _up;

        return new Ray(_origin, Vector3.Normalize(direction));
    }
}