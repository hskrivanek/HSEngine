using HSEngine.Core.Scenes;
using HSEngine.Core.Entities;
using HSEngine.RenderingOld;
using System;
using System.Threading;
using Veldrid;
using HSEngine.Core.InputSystem;

namespace HSEngine.Core
{
    public class Game
    {
        private readonly Renderer renderer;

        private Scene currentScene;

        public Game(Renderer renderer)
        {
            this.renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));

            this.currentScene = new Scene();
        }

        public void RunMainLoop()
        {
            var window = renderer.Window ?? throw new NullReferenceException("Window is null");

            while (window.Exists)
            {
                var input = window.PumpEvents();

                Input.UpdateFrameInput(input);

                Update();

                Draw();

                Thread.Sleep(15);
            }
        }

        private void Update()
        {
            currentScene.Update();
        }

        private void Draw()
        {
            this.renderer.StartDrawing();
            currentScene.Draw(this.renderer);
            this.renderer.FinishDrawing();
        }

        public void AddEntity(Entity entity)
        {
            this.currentScene.AddEntity(entity);
        }

        public void NewScene()
        {
            this.currentScene = new Scene();
        }
    }
}
