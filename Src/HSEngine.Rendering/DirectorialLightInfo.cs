using System.Numerics;
using System.Runtime.InteropServices;

namespace HSEngine.Rendering
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DirectorialLightInfo
    {
        public Vector3 Direction;
        private readonly float padding;
        public Vector4 Color;
    }
}
