﻿using HSEngine.Events;
using ImGuiNET;
using System;
using System.Numerics;

// TODO: Remove Veldrid from here => abstract renderer

namespace HSEngine.ImGuiUtils
{
    public class ImGuiLayer : Layer
    {
        private IntPtr context;
        private IImGuiRenderer renderer;

        public ImGuiLayer(IImGuiRenderer renderer, string debugName = "ImGuiLayer") : base(debugName)
        {
            this.renderer = renderer;
        }

        public override void OnAttach()
        {
            this.context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);
            var io = ImGui.GetIO();

            io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
            io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;

            SetKeyMappings();

            var fonts = ImGui.GetIO().Fonts;
            io.Fonts.AddFontDefault();

            io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out int bytesPerPixel);
            io.Fonts.SetTexID((IntPtr)1);
            io.Fonts.ClearTexData();

            ImGui.StyleColorsDark();

            var window = Application.GetCurrentApplication().Window;
            io.DeltaTime = 1f / 60f;
            io.DisplaySize = new Vector2(window.Width, window.Height);
            io.DisplayFramebufferScale = Vector2.One;

            this.renderer.InitializeRenderer(io, (IntPtr)1);

            ImGui.NewFrame();
        }

        public override void OnDetach()
        {
            ImGui.DestroyContext(this.context);
        }

        public override void OnEvent(EngineEventArgs eventArgs)
        {
            var io = ImGui.GetIO();
            switch (eventArgs)
            {
                case MouseButtonDownEventArgs e:
                    io.MouseDown[(int)e.MouseCode] = true;
                    break;
                case MouseButtonUpEventArgs e:
                    io.MouseDown[(int)e.MouseCode] = false;
                    break;
                case MouseMovedEventArgs e:
                    io.MousePos = new Vector2(e.X, e.Y);
                    break;
                case MouseScrolledEventArgs e:
                    io.MouseWheel += e.YOffset;
                    io.MouseWheelH += e.XOffset;
                    break;
                case KeyDownEventArgs e:
                    io.KeysDown[(int)e.KeyCode] = true;
                    e.Handled = true;
                    break;
                case KeyUpEventArgs e:
                    io.KeysDown[(int)e.KeyCode] = false;
                    e.Handled = true;
                    break;
                case WindowResizeEventArgs e:
                    io.DisplaySize = new Vector2(e.Width, e.Height);
                    io.DisplayFramebufferScale = Vector2.One;
                    break;
                default:
                    break;
            }
        }

        public override void OnUpdate()
        {
            ImGui.NewFrame();
            var show = true;
            ImGui.ShowDemoWindow(ref show);

            ImGui.Render();

            var drawData = ImGui.GetDrawData();
            this.renderer.RenderImDrawData(drawData);
        }
        
        private static void SetKeyMappings()
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.KeyMap[(int)ImGuiKey.Tab] = (int)KeyCode.Tab;
            io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)KeyCode.Left;
            io.KeyMap[(int)ImGuiKey.RightArrow] = (int)KeyCode.Right;
            io.KeyMap[(int)ImGuiKey.UpArrow] = (int)KeyCode.Up;
            io.KeyMap[(int)ImGuiKey.DownArrow] = (int)KeyCode.Down;
            io.KeyMap[(int)ImGuiKey.PageUp] = (int)KeyCode.PageUp;
            io.KeyMap[(int)ImGuiKey.PageDown] = (int)KeyCode.PageDown;
            io.KeyMap[(int)ImGuiKey.Home] = (int)KeyCode.Home;
            io.KeyMap[(int)ImGuiKey.End] = (int)KeyCode.End;
            io.KeyMap[(int)ImGuiKey.Delete] = (int)KeyCode.Delete;
            io.KeyMap[(int)ImGuiKey.Backspace] = (int)KeyCode.Back;
            io.KeyMap[(int)ImGuiKey.Enter] = (int)KeyCode.Enter;
            io.KeyMap[(int)ImGuiKey.Escape] = (int)KeyCode.Escape;
            io.KeyMap[(int)ImGuiKey.A] = (int)KeyCode.A;
            io.KeyMap[(int)ImGuiKey.C] = (int)KeyCode.C;
            io.KeyMap[(int)ImGuiKey.V] = (int)KeyCode.V;
            io.KeyMap[(int)ImGuiKey.X] = (int)KeyCode.X;
            io.KeyMap[(int)ImGuiKey.Y] = (int)KeyCode.Y;
            io.KeyMap[(int)ImGuiKey.Z] = (int)KeyCode.Z;
        }
    }
}
