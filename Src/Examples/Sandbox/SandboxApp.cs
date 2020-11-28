﻿using HSEngine;
using HSEngine.Events;
using HSEngine.ImGuiUtils;
using HSEngine.Logging;
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
            var (gd, cl, _) = ((WpfWindowWithVeldrid)app.Window).TempGetGraphicsStuff();
            app.PushLayer(new ImGuiLayer(gd, cl));

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
