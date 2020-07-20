using HSEngine.Core.Components;
using HSEngine.Core.Entities;
using System.Numerics;

namespace HSEngine.Core.Utility
{
    public static class Maths
    {
        internal static Matrix4x4 CreateTransformationMatrix(Vector3 translation, float rotationX, float rotationY, float rotationZ, float scale)
        {
            return Matrix4x4.CreateScale(scale)
                 * Matrix4x4.CreateRotationX(rotationX)
                 * Matrix4x4.CreateRotationY(rotationY)
                 * Matrix4x4.CreateRotationZ(rotationZ)
                 * Matrix4x4.CreateTranslation(translation);
        }

        internal static Matrix4x4 CreateTransformationMatrix(Transform transform)
        {
            return CreateTransformationMatrix(transform.Position, transform.Rotation.X, transform.Rotation.Y, transform.Rotation.Z, transform.Scale);
        }

        internal static Matrix4x4 CreateViewMatrix(Camera camera)
        {
            return Matrix4x4.CreateFromYawPitchRoll(camera.Yaw, camera.Pitch, camera.Roll)
                 * Matrix4x4.CreateTranslation(-camera.Position);
        }
    }
}
