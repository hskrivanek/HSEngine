using HSEngine.Core.Components;
using System.Linq;
using System.Numerics;

namespace HSEngine.Core.Entities
{
    public class Camera: Entity
    {
        public Camera() : this(new Vector3(), 0, 0, 0)
        {
        }

        public Camera(Vector3 position, float pitch, float yaw, float roll)
        {
            this.Transform = new Transform(position, new Vector3(pitch, yaw, roll), 1);
        }

        public Vector3 Position => this.Transform.Position;
        public float Pitch => this.Transform.Rotation.X;
        public float Yaw => this.Transform.Rotation.Y;
        public float Roll => this.Transform.Rotation.Z;
    }
}
