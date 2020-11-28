using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace HSEngine.Windows
{
    internal class Win32HwndHost : HwndHost
    {
        internal const int
          WS_CHILD = 0x40000000,
          WS_VISIBLE = 0x10000000,
          LBS_NOTIFY = 0x00000001,
          HOST_ID = 0x00000002,
          LISTBOX_ID = 0x00000001,
          WS_VSCROLL = 0x00200000,
          WS_BORDER = 0x00800000;

        private IntPtr hwnd;

        public bool HwndInitialized { get; private set; }

        public event EventHandler Resized;

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            this.hwnd = IntPtr.Zero;

            this.hwnd = CreateWindowEx(0, "static", "",
                                      WS_CHILD | WS_VISIBLE,
                                      0, 0,
                                      (int)Width, (int)Height,
                                      hwndParent.Handle,
                                      (IntPtr)HOST_ID,
                                      IntPtr.Zero,
                                      0);

            return new HandleRef(this, this.hwnd);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            DestroyWindow(hwnd.Handle);
            this.hwnd = IntPtr.Zero;
        }

        protected override void OnRenderSizeChanged(System.Windows.SizeChangedInfo sizeInfo)
        {
            UpdateWindowPos();

            base.OnRenderSizeChanged(sizeInfo);
            Resized?.Invoke(this, EventArgs.Empty);
        }

        [DllImport("user32.dll", EntryPoint = "CreateWindowEx", CharSet = CharSet.Unicode)]
        internal static extern IntPtr CreateWindowEx(int dwExStyle,
                                                     string lpszClassName,
                                                     string lpszWindowName,
                                                     int style,
                                                     int x, int y,
                                                     int width, int height,
                                                     IntPtr hwndParent,
                                                     IntPtr hMenu,
                                                     IntPtr hInst,
                                                     object pvParam);

        [DllImport("user32.dll", EntryPoint = "DestroyWindow", CharSet = CharSet.Auto)]
        public static extern bool DestroyWindow(IntPtr hwnd);
    }
}
