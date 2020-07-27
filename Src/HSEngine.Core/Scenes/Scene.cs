using HSEngine.Core.Entities;
using HSEngine.Core.Utility;
using HSEngine.Rendering;
using System;
using System.Collections.Generic;

namespace HSEngine.Core.Scenes
{
    public class Scene
    {
        private Camera mainCamera;
        private DirectionalLight sceneLight;

        private readonly List<Entity> entities;
        private readonly List<IRenderable> renderables;

        public Scene()
        {
            this.mainCamera = new Camera();
            this.sceneLight = new DirectionalLight(20, -20, -20);

            this.entities = new List<Entity>();
            this.renderables = new List<IRenderable>();
        }

        public void Initialize()
        {
            foreach (var entity in this.entities)
            {
                entity.Initialize();
            }
        }

        public void Update()
        {
            var context = new SceneContext
            {
                MainCamera = mainCamera
            };

            foreach (var entity in this.entities)
            {
                entity.Update(context);
            }
            this.mainCamera.Update(context);
        }

        public void Draw(Renderer renderer)
        {
            renderer.StartDrawing();
            foreach (var renderable in this.renderables)
            {
                renderable.Draw(renderer, this.GenerateRenderingContext());
            }
            renderer.FinishDrawing();
        }

        public void AddEntity(Entity entity)
        {
            this.entities.Add(entity);
            if (entity is IRenderable renderable)
            {
                renderables.Add(renderable);
            }
        }

        private SceneRenderContext GenerateRenderingContext()
        {
            return new SceneRenderContext
            {
                ViewMatrix = Maths.CreateViewMatrix(this.mainCamera),
                LightDirection = this.sceneLight.Direction,
                LightColor = this.sceneLight.ColorVector
            };
        }
    }
}
