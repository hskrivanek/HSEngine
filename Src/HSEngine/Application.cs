using HSEngine.Events;
using HSEngine.Logging;

using System;

namespace HSEngine
{
    public abstract class Application
    {
        private const string multipleApplicationsErrorMessage = "Only one instance of HSEngine application can be running in single program.";
        private static Application currentApplication;

        private readonly Window window;
        private readonly LayerStack layerStack = new LayerStack();

        private bool isRunning;

        protected Application(Func<Window> windowProvider)
        {
            Log.Init();
            Log.CoreLogger.Info("Creating HSEngine application.");

            if (currentApplication != null)
            {
                Log.CoreLogger.Fatal(multipleApplicationsErrorMessage);
                throw new ApplicationException(multipleApplicationsErrorMessage);
            }
            currentApplication = this;

            this.window = windowProvider();
            isRunning = true;
            this.window.EventEmitted += Window_EventEmitted;
        }

        public Window Window => this.window;

        public virtual void Run()
        {
            Log.CoreLogger.Info("HSEngine application starting.");
            RunMainLoop();
            Log.CoreLogger.Info("Application loop ended");
        }

        private void RunMainLoop()
        {
            Log.CoreLogger.Info("Running main engine loop.");

            while (isRunning)
            {
                this.window.OnUpdateStart();
                UpdateLayers();
                this.window.OnUpdateEnd();
            }
        }

        public void PushLayer(Layer layer)
        {
            this.layerStack.PushLayer(layer);
            layer.OnAttach();
        }

        public void PushOverlay(Layer layer)
        {
            this.layerStack.PushOverlay(layer);
            layer.OnAttach();
        }

        public static Application GetCurrentApplication() => currentApplication;

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

            if (e.EventCategory == EventCategory.Input)
            {
                Input.UpdateState(e);
            }

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
