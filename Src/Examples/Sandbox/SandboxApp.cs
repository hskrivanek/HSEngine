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
            app.PushLayer(new ImGuiLayer(new VeldridImGuiRenderer(gd, cl)));

            app.Run();

            var e = new MouseButtonDownEventArgs(MouseCode.Button0);
            if ((e.EventCategory & EventCategory.Keyboard) != 0)
            {
                Log.ClientLogger.Trace(e.ToString());
            }

            Log.ClientLogger.Info("Exiting");
        }
    }
}
