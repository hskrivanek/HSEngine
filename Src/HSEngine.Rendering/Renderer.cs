using System;
using System.Numerics;
using Veldrid;
using Veldrid.ImageSharp;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace HSEngine.Rendering
{
    public class Renderer
    {
        private GraphicsDevice gd;
        private Sdl2Window window;

        private CommandList cl;

        private Matrix4x4 projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(1.5f, 1.7777777f, 0.1f, 1000);

        public Sdl2Window Window { get => window; private set => window = value; }

        public void Initialize(bool isDebug)
        {
            var windowCI = new WindowCreateInfo()
            {
                X = 100,
                Y = 100,
                WindowWidth = 960,
                WindowHeight = 540,
                WindowTitle = "Veldrid Tutorial",
                WindowInitialState = WindowState.Normal
            };

            var gdOptions = new GraphicsDeviceOptions(isDebug, PixelFormat.R16_UNorm, false, ResourceBindingModel.Improved, true, true);

            VeldridStartup.CreateWindowAndGraphicsDevice(windowCI, gdOptions, GraphicsBackend.Direct3D11, out window, out gd);

            cl = gd.ResourceFactory.CreateCommandList();
        }

        public TexturedMesh CreateMesh(RawModel model, ImageSharpTexture textureData, ShaderSet shaderSet)
        {
            (Shader vShader, Shader fShader) = shaderSet.CreateShaders(gd.ResourceFactory);
            return new TexturedMesh(model, textureData, vShader, fShader);
        }
        public void InitializeMesh(TexturedMesh mesh)
        {
            mesh.CreateDeviceResources(gd);
        }

        public void StartDrawing()
        {
            cl.Begin();
            cl.SetFramebuffer(gd.SwapchainFramebuffer);
            cl.ClearColorTarget(0, RgbaFloat.DarkRed);
            cl.ClearDepthStencil(1f);
        }

        public void FinishDrawing()
        {
            cl.End();
            gd.SubmitCommands(cl);
            gd.SwapBuffers();
        }

        public void Draw(TexturedMesh mesh, Matrix4x4 transformation, SceneRenderContext context)
        {
            this.Draw(mesh, transformation, context.ViewMatrix, context.LightDirection, context.LightColor);
        }

        public void Draw(TexturedMesh mesh, Matrix4x4 transformation, Matrix4x4 view, Vector3 lightDirection, Vector3 lightColor)
        {
            mesh.Draw(cl, transformation, projectionMatrix, view, lightDirection, lightColor);
        }

        public void DisposeGraphicsDevices()
        {
            cl.Dispose();
            gd.Dispose();
        }
    }
}
