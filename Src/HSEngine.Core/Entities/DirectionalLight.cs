using System.Drawing;
using System.Numerics;

namespace HSEngine.Core.Entities
{
    public class DirectionalLight: Entity
    {
        public DirectionalLight(float x, float y, float z) : this(new Vector3(x, y, z)) { }

        public DirectionalLight(Vector3 direction) : this(direction, Color.White) { }

        public DirectionalLight(Vector3 direction, Color color)
        {
            Direction = direction;
            Color = color;
        }

        public Vector3 Direction { get; set; }
        public Color Color { get; set; }
        public Vector3 ColorVector => new Vector3(Color.R, Color.G, Color.B) / 255f;
    }
}
