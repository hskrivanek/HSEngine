using HSEngine.Events;
using HSEngine.Logging;
using HSEngine.Windows.Input;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Veldrid;

namespace HSEngine.Windows
{
    public class WinFormsVeldridWindow : Window
    {
        private Form mainWindow;

        private GraphicsDevice gd;
        private CommandList cl;

        public WinFormsVeldridWindow(WindowProperties windowProperties)
        {
            this.Init(windowProperties);
        }

        public override bool IsVSyncEnabled => this.gd.SyncToVerticalBlank;

        public override int Width => this.mainWindow.ClientSize.Width;

        public override int Height => this.mainWindow.ClientSize.Height;

        public override void DisableVSync()
        {
            this.gd.SyncToVerticalBlank = false;
        }

        public override void EnableVSync()
        {
            this.gd.SyncToVerticalBlank = true;
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
            CreateWindow(windowProperties);
            CreateGraphicsDevices();

            this.mainWindow.Resize += WindowHost_Resized;

            this.mainWindow.Closed += Window_Closed;

            this.mainWindow.KeyDown += Window_KeyDown;
            this.mainWindow.KeyUp += Window_KeyUp;
            this.mainWindow.KeyPress += MainWindow_KeyPress;

            this.mainWindow.MouseMove += Window_MouseMove;
            this.mainWindow.MouseWheel += Window_MouseWheel;
            this.mainWindow.MouseDown += Window_MouseDown;
            this.mainWindow.MouseUp += Window_MouseUp;

            this.mainWindow.Show();
        }

        private void CreateWindow(WindowProperties windowProperties)
        {
            Log.CoreLogger.Info($"Creating window {windowProperties.Title} ({windowProperties.Width}, {windowProperties.Height}).");

            this.mainWindow = new Form
            {
                Text = windowProperties.Title,
                Width = windowProperties.Width,
                Height = windowProperties.Height
            };
        }

        private void CreateGraphicsDevices()
        {
            Log.CoreLogger.Info($"Creating Veldrid graphics rendering resources.");

            IntPtr hinstance = GetModuleHinstance();
            SwapchainDescription swapChainDescription = CreateSwapchainDescription(hinstance);

            this.gd = GraphicsDevice.CreateD3D11(
                new GraphicsDeviceOptions()
                {
                    SwapchainDepthFormat = Veldrid.PixelFormat.R16_UNorm,
                    HasMainSwapchain = true
                },
                swapChainDescription);

            Log.CoreLogger.Info($"Graphics backend type: {gd.BackendType}");
            this.cl = gd.ResourceFactory.CreateCommandList();
        }

        private static IntPtr GetModuleHinstance()
        {
            var module = typeof(Form).Module;
            IntPtr hinstance = Marshal.GetHINSTANCE(module);
            return hinstance;
        }

        private SwapchainDescription CreateSwapchainDescription(IntPtr hinstance)
        {
            SwapchainSource source = SwapchainSource.CreateWin32(
                this.mainWindow.Handle, hinstance);
            var swapChainDescription = new SwapchainDescription(
                source, 
                (uint)this.Width,
                (uint)this.Height,
                null,
                false);
            return swapChainDescription;
        }
        private void WindowHost_Resized(object sender, EventArgs e)
        {
            gd.MainSwapchain.Resize((uint)this.Width, (uint)this.Height);
            EmitEngineEvent(
                new WindowResizeEventArgs(
                    this.Width,
                    this.Height));
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            EmitEngineEvent(new WindowCloseEventArgs());
        }

        private void Window_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            var engineKey = WinFormsInputConverter.ConvertKeyToEngine(e.KeyCode);
            EmitEngineEvent(new KeyDownEventArgs(engineKey));
        }

        private void Window_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            EmitEngineEvent(new KeyUpEventArgs(WinFormsInputConverter.ConvertKeyToEngine(e.KeyCode)));
        }

        private void MainWindow_KeyPress(object sender, KeyPressEventArgs e)
        {
            EmitEngineEvent(new KeyTypedEventArgs(e.KeyChar));
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            EmitEngineEvent(new MouseMovedEventArgs((float)(e.X), (float)(e.Y)));
        }

        private void Window_MouseWheel(object sender, MouseEventArgs e)
        {
            EmitEngineEvent(new MouseScrolledEventArgs(0, (float)e.Delta / SystemInformation.MouseWheelScrollDelta));
        }

        private void Window_MouseDown(object sender, MouseEventArgs e)
        {
            EmitEngineEvent(new MouseButtonDownEventArgs(WinFormsInputConverter.ConvertMouseButtonToEngine(e.Button)));
        }

        private void Window_MouseUp(object sender, MouseEventArgs e)
        {
            EmitEngineEvent(new MouseButtonUpEventArgs(WinFormsInputConverter.ConvertMouseButtonToEngine(e.Button)));
        }
    }
}
