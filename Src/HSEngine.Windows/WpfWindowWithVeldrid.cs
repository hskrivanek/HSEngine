using HSEngine.Events;
using HSEngine.Logging;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using Veldrid;

namespace HSEngine.Windows
{
    public class WpfWindowWithVeldrid : Window
    {
        private System.Windows.Window mainWindow;
        private Win32HwndHost win32Host;

        private GraphicsDevice gd;
        private CommandList cl;

        public WpfWindowWithVeldrid(WindowProperties windowProperties)
        {
            this.Init(windowProperties);
        }

        public override int Width => (int)(win32Host.ActualWidth < 0 ? 0 : Math.Ceiling(win32Host.ActualWidth * GetDpiScale()));

        public override int Height => (int)(win32Host.ActualHeight < 0 ? 0 : Math.Ceiling(win32Host.ActualHeight * GetDpiScale()));


        public override bool IsVSyncEnabled => this.gd.SyncToVerticalBlank;

        public override void DisableVSync()
        {
            gd.SyncToVerticalBlank = false;
        }

        public override void EnableVSync()
        {
            gd.SyncToVerticalBlank = true;
        }

        public override void OnUpdateStart()
        {
            System.Windows.Forms.Application.DoEvents();

            cl.Begin();
            cl.SetFramebuffer(this.gd.SwapchainFramebuffer);
            cl.ClearColorTarget(0, RgbaFloat.CornflowerBlue);
        }

        public override void OnUpdateEnd()
        {
            cl.End();
            gd.SubmitCommands(cl);
            gd.SwapBuffers(this.gd.MainSwapchain);
        }

        public (GraphicsDevice, CommandList, Swapchain) TempGetGraphicsStuff()
        {
            return (this.gd, this.cl, this.gd.MainSwapchain);
        }

        private void Init(WindowProperties windowProperties)
        {
            CreateMainWindow(windowProperties);
            CreateWin32HostAndSetSizes();
            CreateGraphicsResources();

            win32Host.Resized += WindowHost_Resized;

            mainWindow.SizeChanged += MainWindow_SizeChanged;
            mainWindow.DpiChanged += MainWindow_DpiChanged;
            mainWindow.Closed += Window_Closed;

            mainWindow.KeyDown += Window_KeyDown;
            mainWindow.KeyUp += Window_KeyUp;

            mainWindow.MouseMove += Window_MouseMove;
            mainWindow.MouseWheel += Window_MouseWheel;
            mainWindow.MouseDown += Window_MouseDown;
            mainWindow.MouseUp += Window_MouseUp;
        }

        private void CreateMainWindow(WindowProperties windowProperties)
        {
            Log.CoreLogger.Info($"Creating window {windowProperties.Title} ({windowProperties.Width}, {windowProperties.Height}).");

            var dpi = (int)(typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null, null));
            var dpiScale = dpi / 96.0;

            this.mainWindow = new System.Windows.Window()
            {
                Title = windowProperties.Title,
                Width = windowProperties.Width / dpiScale,
                Height = (windowProperties.Height + SystemParameters.WindowCaptionHeight) / dpiScale 
            };
        }

        private void CreateWin32HostAndSetSizes()
        {
            Log.CoreLogger.Info($"Creating Win32 window host for rendering and setting actual window size.");

            this.win32Host = new Win32HwndHost();
            this.mainWindow.Content = this.win32Host;
            this.mainWindow.Show();
        }

        private void CreateGraphicsResources()
        {
            Log.CoreLogger.Info($"Creating Veldrid graphics rendering resources.");

            IntPtr hinstance = GetModuleHinstance();
            SwapchainDescription swapChainDescription = CreateSwapchainDescription(hinstance);

            gd = GraphicsDevice.CreateD3D11(
                new GraphicsDeviceOptions()
                {
                    SwapchainDepthFormat = Veldrid.PixelFormat.R16_UNorm,
                    HasMainSwapchain = true
                },
                swapChainDescription);

            Log.CoreLogger.Info($"Graphics backend type: {gd.BackendType}");
            cl = gd.ResourceFactory.CreateCommandList();
        }

        private SwapchainDescription CreateSwapchainDescription(IntPtr hinstance)
        {
            (uint width, uint height) = GetRenderAreaSizeInPixels();
            SwapchainSource source = SwapchainSource.CreateWin32(
                this.win32Host.Handle, hinstance);
            var swapChainDescription = new SwapchainDescription(source, width, height, null, false);
            return swapChainDescription;
        }

        private static IntPtr GetModuleHinstance()
        {
            var module = typeof(Win32HwndHost).Module;
            IntPtr hinstance = Marshal.GetHINSTANCE(module);
            return hinstance;
        }

        private (uint, uint) GetRenderAreaSizeInPixels()
        {
            double dpiScale = GetDpiScale();
            var width = (uint)(win32Host.ActualWidth < 0 ? 0 : Math.Ceiling(win32Host.ActualWidth * dpiScale));
            var height = (uint)(win32Host.ActualHeight < 0 ? 0 : Math.Ceiling(win32Host.ActualHeight * dpiScale));
            return (width, height);
        }

        private double GetDpiScale()
        {
            PresentationSource source = PresentationSource.FromVisual(this.win32Host);

            return source.CompositionTarget.TransformToDevice.M11;
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void MainWindow_DpiChanged(object sender, DpiChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void WindowHost_Resized(object sender, EventArgs e)
        {
            (uint width, uint height) = GetRenderAreaSizeInPixels();
            gd.MainSwapchain.Resize(width, height);
            EmitEngineEvent(
                new WindowResizeEventArgs(
                    (int)width,
                    (int)height));
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            EmitEngineEvent(new WindowCloseEventArgs());
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
            double dpiScale = GetDpiScale();

            var position = e.GetPosition(win32Host);
            EmitEngineEvent(new MouseMovedEventArgs((float)(position.X * dpiScale), (float)(position.Y * dpiScale)));
        }
    }
}
