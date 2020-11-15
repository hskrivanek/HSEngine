using HSEngine.Events;
using HSEngine.Logging;

using System;

namespace HSEngine
{
    public abstract class Application
    {
        private readonly Window window;
        private readonly LayerStack layerStack = new LayerStack();

        private bool isRunning;

        protected Application(Func<Window> windowProvider)
        {
            Log.Init();
            Log.CoreLogger.Info("HSEngine application creating");

            this.window = windowProvider();
            isRunning = true;
            this.window.EventEmitted += Window_EventEmitted;
        }

        public virtual void Run()
        {
            Log.CoreLogger.Info("HSEngine application starting");
            while (isRunning)
            {
                UpdateLayers();
                this.window.OnUpdate();
            }
            Log.CoreLogger.Info("Application loop ended");
        }

        private void UpdateLayers()
        {
            foreach (var layer in this.layerStack.Layers)
            {
                layer.OnUpdate();
            }
        }

        private void Window_EventEmitted(object _, EngineEventArgs e)
        {
            Log.CoreLogger.Trace(e.ToString());
            switch(e)
            {
                case WindowCloseEventArgs _:
                    this.isRunning = false;
                    break;
                default:
                    PassEventToLayers(e);
                    break;
            }
        }

        private void PassEventToLayers(EngineEventArgs e)
        {
            var layers = this.layerStack.Layers;
            for (int i = layers.Count - 1; i >= 0; i--)
            {
                layers[i].OnEvent(e);
                if (e.Handled)
                {
                    break;
                }
            }
        }
    }
}
