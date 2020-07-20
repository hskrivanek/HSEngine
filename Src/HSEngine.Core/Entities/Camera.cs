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
            Position = position;
            Pitch = pitch;
            Yaw = yaw;
            Roll = roll;
        }

        public Vector3 Position { get; private set; }
        public float Pitch { get; private set; }
        public float Yaw { get; private set; }
        public float Roll { get; private set; }

        public void Update(Veldrid.InputSnapshot input)
        {
            if (input.KeyEvents.Any(e => e.Key == Veldrid.Key.A))
            {
                this.Position -= 0.04f * Vector3.UnitX;
            }
            else if (input.KeyEvents.Any(e => e.Key == Veldrid.Key.D))
            {
                this.Position += 0.04f * Vector3.UnitX;
            }
        }
    }
}
