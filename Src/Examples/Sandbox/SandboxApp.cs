using HSEngine;
using HSEngine.Events;
using HSEngine.ImGuiUtils;
using HSEngine.Logging;
using HSEngine.VeldridRendering;
using HSEngine.Windows;
using System;

namespace Sandbox
{
    class SandboxApp : Application
    {
        public SandboxApp(Func<Window> windowProvider) : base(windowProvider) { }

        [STAThread]
        static void Main()
        {
            static Window windowProvider() => new WpfWindowWithVeldrid(new WindowProperties() { Width = 1240, Height = 728, Title = "Sandbox" });
            var app = new SandboxApp(windowProvider);
            var window = (WpfWindowWithVeldrid)app.Window;
            var (gd, cl, _) = window.TempGetGraphicsStuff();
            app.PushOverlay(new ImGuiLayer(new VeldridImGuiRenderer(gd, cl)));
            app.PushLayer(new TestLayer());

            app.Run();

            Log.ClientLogger.Info("Exiting");
        }

        class TestLayer : Layer
        {
            public override void OnUpdate()
            {
                if (Input.KeyDown(KeyCode.Space))
                {
                    Log.ClientLogger.Debug("Space key down");
                }
                if (Input.Key(KeyCode.Space))
                {
                    Log.ClientLogger.Debug("Space key");
                }
                if (Input.KeyUp(KeyCode.Space))
                {
                    Log.ClientLogger.Debug("Space key up");
                }
            }
        }
    }
}
