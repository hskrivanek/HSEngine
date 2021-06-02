using HSEngine;
using HSEngine.Events;
using HSEngine.ImGuiUtils;
using HSEngine.Logging;
using HSEngine.VeldridRendering;
using HSEngine.Windows;
using ImGuiNET;
using System;

namespace Sandbox
{
    class SandboxApp : Application
    {
        public SandboxApp(Func<Window> windowProvider) : base(windowProvider) { }

        [STAThread]
        static void Main()
        {
            static Window windowProvider() => new WinFormsVeldridWindow(new WindowProperties() { Width = 1240, Height = 728, Title = "Sandbox" });
            var app = new SandboxApp(windowProvider);
            var window = (WinFormsVeldridWindow)app.Window;
            var (gd, cl, _) = window.TempGetGraphicsStuff();
            var imGuiFrameBuffer = new ImGuiFrameBuffer();
            app.PushOverlay(new ImGuiLayer(new VeldridImGuiRenderer(gd, cl), imGuiFrameBuffer));
            app.PushLayer(new TestLayer(imGuiFrameBuffer));

            app.Run();

            Log.ClientLogger.Info("Exiting");
        }

        class TestLayer : Layer
        {
            private readonly ImGuiFrameBuffer imGuiFrameBuffer;

            public TestLayer(ImGuiFrameBuffer imGuiFrameBuffer)
            {
                this.imGuiFrameBuffer = imGuiFrameBuffer;
            }

            public override void OnUpdate()
            {
                static void drawTest()
                {
                    ImGui.Begin("TEST WINDOW");
                    ImGui.Text("test");
                    ImGui.End();
                }

                this.imGuiFrameBuffer.EnqueueDemoWindowDrawing();
                this.imGuiFrameBuffer.EnqueueCustomDrawingAction(drawTest);

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
