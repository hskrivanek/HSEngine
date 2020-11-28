using HSEngine.Events;
using ImGuiNET;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

// TODO: Remove Veldrid from here => abstract renderer
using Veldrid;
using Veldrid.SPIRV;

namespace HSEngine.ImGuiUtils
{
    public class ImGuiLayer : Layer
    {
        private IntPtr context;

        // TODO: Veldrid stuff shouldn't be here, extract rendering to another assembly
        private GraphicsDevice gd;
        private CommandList cl;
        private DeviceBuffer vertexBuffer;
        private DeviceBuffer indexBuffer;
        private DeviceBuffer projMatrixBuffer;
        private Texture fontTexture;
        private TextureView fontTextureView;
        private Shader vertexShader;
        private Shader fragmentShader;
        private ResourceLayout layout;
        private ResourceLayout textureLayout;
        private Pipeline pipeline;
        private ResourceSet mainResourceSet;
        private ResourceSet fontTextureResourceSet;

        public ImGuiLayer(GraphicsDevice gd, CommandList cl, string debugName = "ImGuiLayer") : base(debugName)
        {
            this.gd = gd;
            this.cl = cl;
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
            
            VeldridInit();

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
            RenderImDrawData(drawData, this.gd, this.cl);
        }

        private void RenderImDrawData(ImDrawDataPtr drawData, GraphicsDevice gd, CommandList cl)
        {
            uint vertexOffsetInVertices = 0;
            uint indexOffsetInElements = 0;

            if (drawData.CmdListsCount == 0)
            {
                return;
            }

            uint totalVBSize = (uint)(drawData.TotalVtxCount * Unsafe.SizeOf<ImDrawVert>());
            if (totalVBSize > vertexBuffer.SizeInBytes)
            {
                gd.DisposeWhenIdle(vertexBuffer);
                vertexBuffer = gd.ResourceFactory.CreateBuffer(new BufferDescription((uint)(totalVBSize * 1.5f), BufferUsage.VertexBuffer | BufferUsage.Dynamic));
            }

            uint totalIBSize = (uint)(drawData.TotalIdxCount * sizeof(ushort));
            if (totalIBSize > indexBuffer.SizeInBytes)
            {
                gd.DisposeWhenIdle(indexBuffer);
                indexBuffer = gd.ResourceFactory.CreateBuffer(new BufferDescription((uint)(totalIBSize * 1.5f), BufferUsage.IndexBuffer | BufferUsage.Dynamic));
            }

            for (int i = 0; i < drawData.CmdListsCount; i++)
            {
                ImDrawListPtr cmdList = drawData.CmdListsRange[i];

                cl.UpdateBuffer(
                    vertexBuffer,
                    vertexOffsetInVertices * (uint)Unsafe.SizeOf<ImDrawVert>(),
                    cmdList.VtxBuffer.Data,
                    (uint)(cmdList.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>()));

                cl.UpdateBuffer(
                    indexBuffer,
                    indexOffsetInElements * sizeof(ushort),
                    cmdList.IdxBuffer.Data,
                    (uint)(cmdList.IdxBuffer.Size * sizeof(ushort)));

                vertexOffsetInVertices += (uint)cmdList.VtxBuffer.Size;
                indexOffsetInElements += (uint)cmdList.IdxBuffer.Size;
            }

            ImGuiIOPtr io = ImGui.GetIO();
            Matrix4x4 mvp = Matrix4x4.CreateOrthographicOffCenter(
                0f,
                io.DisplaySize.X,
                0.0f,
                io.DisplaySize.Y,
                -1.0f,
                1.0f);

            gd.UpdateBuffer(projMatrixBuffer, 0, ref mvp);

            cl.SetVertexBuffer(0, vertexBuffer);
            cl.SetIndexBuffer(indexBuffer, IndexFormat.UInt16);
            cl.SetPipeline(pipeline);
            cl.SetGraphicsResourceSet(0, mainResourceSet);

            drawData.ScaleClipRects(io.DisplayFramebufferScale);

            int vtx_offset = 0;
            int idx_offset = 0;
            for (int n = 0; n < drawData.CmdListsCount; n++)
            {
                ImDrawListPtr cmdList = drawData.CmdListsRange[n];
                for (int i = 0; i < cmdList.CmdBuffer.Size; i++)
                {
                    ImDrawCmdPtr pcmd = cmdList.CmdBuffer[i];
                    if (pcmd.UserCallback != IntPtr.Zero)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        if (pcmd.TextureId != IntPtr.Zero)
                        {
                            if (pcmd.TextureId == (IntPtr)1)
                            {
                                cl.SetGraphicsResourceSet(1, fontTextureResourceSet);
                            }
                            else
                            {
                                throw new NotImplementedException();
                                //cl.SetGraphicsResourceSet(1, GetImageResourceSet(pcmd.TextureId));
                            }
                        }

                        cl.SetScissorRect(
                            0,
                            (uint)pcmd.ClipRect.X,
                            (uint)pcmd.ClipRect.Y,
                            (uint)(pcmd.ClipRect.Z - pcmd.ClipRect.X),
                            (uint)(pcmd.ClipRect.W - pcmd.ClipRect.Y));

                        cl.DrawIndexed(pcmd.ElemCount, 1, (uint)idx_offset, vtx_offset, 0);
                    }

                    idx_offset += (int)pcmd.ElemCount;
                }
                vtx_offset += cmdList.VtxBuffer.Size;
            }
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

        private void VeldridInit()
        {
            ResourceFactory factory = gd.ResourceFactory;
            vertexBuffer = factory.CreateBuffer(new BufferDescription(10000, BufferUsage.VertexBuffer | BufferUsage.Dynamic));
            indexBuffer = factory.CreateBuffer(new BufferDescription(2000, BufferUsage.IndexBuffer | BufferUsage.Dynamic));
            projMatrixBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            RecreateFontDeviceTexture(gd);

            var vertexString = @"
#version 450

#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable

layout (location = 0) in vec2 vsin_position;
layout (location = 1) in vec2 vsin_texCoord;
layout (location = 2) in vec4 vsin_color;

layout (binding = 0) uniform Projection
{
    mat4 projection;
};

layout (location = 0) out vec4 vsout_color;
layout (location = 1) out vec2 vsout_texCoord;

out gl_PerVertex 
{
    vec4 gl_Position;
};

void main() 
{
    gl_Position = projection * vec4(vsin_position, 0, 1);
    vsout_color = vsin_color;
    vsout_texCoord = vsin_texCoord;
    gl_Position.y = -gl_Position.y;
}
";
            var fragString = @"
#version 450

#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable

layout(set = 1, binding = 0) uniform texture2D FontTexture;
layout(set = 0, binding = 1) uniform sampler FontSampler;

layout (location = 0) in vec4 color;
layout (location = 1) in vec2 texCoord;
layout (location = 0) out vec4 outputColor;

void main()
{
    outputColor = color * texture(sampler2D(FontTexture, FontSampler), texCoord);
}
";
            byte[] vertexShaderBytes = Encoding.UTF8.GetBytes(vertexString);
            byte[] fragmentShaderBytes = Encoding.UTF8.GetBytes(fragString);
            var shaders = factory.CreateFromSpirv(
                new ShaderDescription(ShaderStages.Vertex, vertexShaderBytes, "main"),
                new ShaderDescription(ShaderStages.Fragment, fragmentShaderBytes, "main"));
            vertexShader = shaders[0];
            fragmentShader = shaders[1];

            VertexLayoutDescription[] vertexLayouts = new VertexLayoutDescription[]
            {
                new VertexLayoutDescription(
                    new VertexElementDescription("vsin_position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                    new VertexElementDescription("vsin_texCoord", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                    new VertexElementDescription("vsin_color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Byte4_Norm))
            };

            layout = factory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("Projection", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("FontSampler", ResourceKind.Sampler, ShaderStages.Fragment)));
            textureLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("FontTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment)));

            GraphicsPipelineDescription pd = new GraphicsPipelineDescription(
                BlendStateDescription.SingleAlphaBlend,
                new DepthStencilStateDescription(false, false, ComparisonKind.Always),
                new RasterizerStateDescription(FaceCullMode.None, PolygonFillMode.Solid, FrontFace.Clockwise, false, true),
                PrimitiveTopology.TriangleList,
                new ShaderSetDescription(vertexLayouts, new[] { vertexShader, fragmentShader }),
                new ResourceLayout[] { layout, textureLayout },
                gd.SwapchainFramebuffer.OutputDescription);
            pipeline = factory.CreateGraphicsPipeline(pd);

            mainResourceSet = factory.CreateResourceSet(new ResourceSetDescription(layout,
                projMatrixBuffer,
                gd.PointSampler));

            fontTextureResourceSet = factory.CreateResourceSet(new ResourceSetDescription(textureLayout, fontTextureView));
        }

        public void RecreateFontDeviceTexture(GraphicsDevice gd)
        {
            ImGuiIOPtr io = ImGui.GetIO();
            // Build
            IntPtr pixels;
            int width, height, bytesPerPixel;
            io.Fonts.GetTexDataAsRGBA32(out pixels, out width, out height, out bytesPerPixel);
            // Store our identifier
            io.Fonts.SetTexID((IntPtr)1);

            fontTexture = gd.ResourceFactory.CreateTexture(TextureDescription.Texture2D(
                (uint)width,
                (uint)height,
                1,
                1,
                PixelFormat.R8_G8_B8_A8_UNorm,
                TextureUsage.Sampled));
            fontTexture.Name = "ImGui.NET Font Texture";
            gd.UpdateTexture(
                fontTexture,
                pixels,
                (uint)(bytesPerPixel * width * height),
                0,
                0,
                0,
                (uint)width,
                (uint)height,
                1,
                0,
                0);
            fontTextureView = gd.ResourceFactory.CreateTextureView(fontTexture);

            io.Fonts.ClearTexData();
        }
    }
}
