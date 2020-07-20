using HSEngine.Core;
using HSEngine.Core.Components;
using HSEngine.Core.Entities;
using HSEngine.Rendering;
using HSEngine.Utility;
using System.IO;
using System.Numerics;
using Veldrid.ImageSharp;

namespace HSEngineTest
{
    class Program
    {
        static void Main()
        {
            var renderer = new Renderer();
            var game = new Game(renderer);

            var vertShaderString = File.ReadAllText("Assets/Shaders/basic.vert");
            var fragShaderString = File.ReadAllText("Assets/Shaders/basic.frag");

            RawModel model;

            using (var reader = new StreamReader("Assets/Models/dragon.obj"))
            {
                model = new RawModel(ObjParser.LoadModel(reader));
            }

            var textureData = new ImageSharpTexture("Assets/Textures/white.png");
            var shaderSet = new ShaderSet(vertShaderString, fragShaderString);
            renderer.Initialize(true);

            var mesh = renderer.CreateMesh(model, textureData, shaderSet);
            var entity = new RenderableEntity(new Transform(new Vector3(0,-5,-10), new Vector3(), 1), mesh);
            entity.InitializeMesh(renderer);

            game.AddEntity(entity);

            game.RunMainLoop();
            renderer.DisposeGraphicsDevices();
        }
    }
}
