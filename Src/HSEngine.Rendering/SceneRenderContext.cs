using System.Numerics;

namespace HSEngine.Rendering
{
    public class SceneRenderContext
    {
        public Matrix4x4 ViewMatrix { get; set; }
        public Vector3 LightDirection { get; set; }
        public Vector3 LightColor { get; set; }
    }
}