using HSEngine.Core.Components;
using HSEngine.Core.Utility;
using HSEngine.Rendering;
using System;

namespace HSEngine.Core.Entities
{
    public class RenderableEntity : Entity, IRenderable
    {
        public RenderableEntity(Transform transform, TexturedMesh mesh)
        {
            this.Transform = transform;
            this.Mesh = mesh;
        }

        public TexturedMesh Mesh { get; private set; }

        public void InitializeMesh(Renderer renderer)
        {
            renderer.InitializeMesh(Mesh);
        }

        public void Draw(Renderer renderer, SceneRenderContext context)
        {
            renderer.Draw(this.Mesh, Maths.CreateTransformationMatrix(this.Transform), context);
        }
    }
}
