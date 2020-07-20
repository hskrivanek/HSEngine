using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace HSEngine.Rendering
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexData
    {
        public Vector3 Position;
        public Vector2 Uv;
        public Vector3 Normal;

        public VertexData(Vector3 position, Vector2 uv, Vector3 normal)
        {
            Position = position;
            Uv = uv;
            Normal = normal;
        }
        public const uint SizeInBytes = 32;

        public static bool operator ==(VertexData left, VertexData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VertexData left, VertexData right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            return obj is VertexData vertex &&
                   Position.Equals(vertex.Position) &&
                   Uv.Equals(vertex.Uv) &&
                   Normal.Equals(vertex.Normal);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Position, Uv, Normal);
        }
    }
}
