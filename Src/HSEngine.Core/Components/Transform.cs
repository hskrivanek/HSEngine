using System.Numerics;

namespace HSEngine.Core.Components
{
    public class Transform
    {
        public Transform() : this(new Vector3()) { }
        public Transform(float x, float y, float z) : this(new Vector3(x, y, z)) { }

        public Transform(Vector3 position) : this(position, new Vector3(), 1)
        {
            Position = position;
        }

        public Transform(Vector3 position, Vector3 rotation, float scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }

        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public float Scale { get; set; }

        public void Translate(Vector3 delta)
        {
            this.Position += delta;
        }

        public void Rotate(Vector3 delta)
        {
            this.Rotation += delta;
        }
    }
}
