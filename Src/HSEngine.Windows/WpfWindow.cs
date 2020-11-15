using HSEngine.Events;
using HSEngine.Logging;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using Veldrid;

namespace HSEngine.Windows
{
    public class WpfWindow : Window
    {
        private System.Windows.Window window;
        private GraphicsDevice gd;
        private Swapchain swapChain;
        private CommandList cl;

        public WpfWindow(WindowProperties windowProperties)
        {
            this.Width = windowProperties.Width;
            this.Height = windowProperties.Height;

            this.Init(windowProperties);
        }

        public override void DisableVSync()
        {
            throw new NotImplementedException();
        }

        public override void EnableVSync()
        {
            throw new NotImplementedException();
        }

        public override void OnUpdate()
        {
            System.Windows.Forms.Application.DoEvents();

            cl.Begin();
            cl.SetFramebuffer(swapChain.Framebuffer);
            cl.ClearColorTarget(0, RgbaFloat.CornflowerBlue);
            cl.End();
            gd.SubmitCommands(cl);
            gd.SwapBuffers(swapChain);
        }

        private void Init(WindowProperties windowProperties)
        {
            Log.CoreLogger.Info($"Creating window {windowProperties.Title} ({windowProperties.Width}, {windowProperties.Height})");

            this.window = new System.Windows.Window()
            {
                Title = windowProperties.Title,
                Width = windowProperties.Width,
                Height = windowProperties.Height
            };
            this.window.Show();

            // Will be needed for renderer binding
            var handle = new WindowInteropHelper(window).EnsureHandle();
            var module = Marshal.GetHINSTANCE(this.GetType().Module);

            gd = GraphicsDevice.CreateD3D11(new GraphicsDeviceOptions());
            SwapchainSource source = SwapchainSource.CreateWin32(
                handle, module);
            swapChain = gd.ResourceFactory.CreateSwapchain(
                new SwapchainDescription(source, (uint)Width, (uint)Height, null, false));
            cl = gd.ResourceFactory.CreateCommandList();

            window.Closed += Window_Closed;
            window.SizeChanged += Window_SizeChanged;

            window.KeyDown += Window_KeyDown;
            window.KeyUp += Window_KeyUp;

            window.MouseMove += Window_MouseMove;
            window.MouseWheel += Window_MouseWheel;
            window.MouseDown += Window_MouseDown;
            window.MouseUp += Window_MouseUp;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            EmitEngineEvent(new WindowCloseEventArgs());
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            EmitEngineEvent(
                new WindowResizeEventArgs(
                    this.WidthToPixels(e.NewSize.Width),
                    this.HeightToPixels(e.NewSize.Height)));
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            EmitEngineEvent(new KeyDownEventArgs(InputConverter.ConvertKeyToEngine(e.Key)));
        }



        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            EmitEngineEvent(new KeyUpEventArgs(InputConverter.ConvertKeyToEngine(e.Key)));
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            EmitEngineEvent(new MouseButtonDownEventArgs(InputConverter.ConvertMouseButtonToEngine(e.ChangedButton)));
        }

        private void Window_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            EmitEngineEvent(new MouseButtonUpEventArgs(InputConverter.ConvertMouseButtonToEngine(e.ChangedButton)));
        }

        private void Window_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            EmitEngineEvent(new MouseScrolledEventArgs(0, e.Delta));
        }

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var position = e.GetPosition(window);
            EmitEngineEvent(new MouseMovedEventArgs((float)position.X, (float)position.Y));
        }

        private int WidthToPixels(double wpfPoints)
            => (int)(wpfPoints * Screen.PrimaryScreen.WorkingArea.Width /
                        SystemParameters.WorkArea.Width);
        private int HeightToPixels(double wpfPoints)
            => (int)(wpfPoints * Screen.PrimaryScreen.WorkingArea.Height /
                        SystemParameters.WorkArea.Height);
    }
}
