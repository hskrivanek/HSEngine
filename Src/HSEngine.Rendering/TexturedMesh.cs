using System.Numerics;
using Veldrid;
using Veldrid.ImageSharp;
using Veldrid.Utilities;

namespace HSEngine.Rendering
{
    public class TexturedMesh
    {
        private readonly RawModel model;
        private readonly ImageSharpTexture textureData;
        private readonly Shader vertexShader;
        private readonly Shader fragmentShader;

        private DeviceBuffer vertexBuffer;
        private DeviceBuffer indexBuffer;
        private int indexCount;

        private DeviceBuffer transformationBuffer;
        private DeviceBuffer projectionBuffer;
        private DeviceBuffer viewBuffer;

        private Texture texture;
        private DeviceBuffer lightDirectionBuffer;
        private DeviceBuffer lightColorBuffer;
        private DeviceBuffer shineDamperBuffer;
        private DeviceBuffer reflectivityBuffer;

        private Pipeline pipeline;
        private ResourceSet transformationResourceSet;
        private ResourceSet textureResourceSet;
        private readonly DisposeCollector disposeCollector = new DisposeCollector();

        public TexturedMesh(RawModel model, ImageSharpTexture textureData, Shader vertexShader, Shader fragmentShader)
        {
            this.model = model;
            this.textureData = textureData;
            this.vertexShader = vertexShader;
            this.fragmentShader = fragmentShader;
        }

        public void CreateDeviceResources(GraphicsDevice gd)
        {
            var factory = new DisposeCollectorResourceFactory(gd.ResourceFactory, disposeCollector);
            vertexBuffer = model.CreateVertexBuffer(gd);
            indexBuffer = model.CreateIndexBuffer(gd, out indexCount);

            transformationBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
            projectionBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
            viewBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));

            texture = textureData.CreateDeviceTexture(gd, factory);
            lightDirectionBuffer = factory.CreateBuffer(new BufferDescription(16, BufferUsage.UniformBuffer));
            lightColorBuffer = factory.CreateBuffer(new BufferDescription(16, BufferUsage.UniformBuffer));
            shineDamperBuffer = factory.CreateBuffer(new BufferDescription(16, BufferUsage.UniformBuffer));
            reflectivityBuffer = factory.CreateBuffer(new BufferDescription(16, BufferUsage.UniformBuffer));

            var vertexLayoutDesc = new VertexLayoutDescription[]
            {
                new VertexLayoutDescription(
                    new VertexElementDescription("Position", VertexElementFormat.Float3, VertexElementSemantic.TextureCoordinate),
                    new VertexElementDescription("TexCoord", VertexElementFormat.Float2, VertexElementSemantic.TextureCoordinate),
                    new VertexElementDescription("Normal", VertexElementFormat.Float3, VertexElementSemantic.TextureCoordinate)
                    )
            };

            var transformationResourceLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription[]
                    {
                        new ResourceLayoutElementDescription("TransformationBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                        new ResourceLayoutElementDescription("ProjectionBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                        new ResourceLayoutElementDescription("ViewBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex)
                    }
                    ));

            var textureResourceLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription[]
                    {
                        new ResourceLayoutElementDescription("Texture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                        new ResourceLayoutElementDescription("Sampler", ResourceKind.Sampler, ShaderStages.Fragment),
                        new ResourceLayoutElementDescription("LightDirectionBuffer", ResourceKind.UniformBuffer, ShaderStages.Fragment),
                        new ResourceLayoutElementDescription("LightColorBuffer", ResourceKind.UniformBuffer, ShaderStages.Fragment),
                        new ResourceLayoutElementDescription("ShineDamperBuffer", ResourceKind.UniformBuffer, ShaderStages.Fragment),
                        new ResourceLayoutElementDescription("ReflectivityBuffer", ResourceKind.UniformBuffer, ShaderStages.Fragment)
                    }
                ));

            transformationResourceSet = factory.CreateResourceSet(
                new ResourceSetDescription(
                    transformationResourceLayout,
                    transformationBuffer,
                    projectionBuffer,
                    viewBuffer));

            textureResourceSet = factory.CreateResourceSet(
                new ResourceSetDescription(
                    textureResourceLayout,
                    texture,
                    gd.Aniso4xSampler,
                    lightDirectionBuffer,
                    lightColorBuffer,
                    shineDamperBuffer,
                    reflectivityBuffer));

            var pipelineDesc = new GraphicsPipelineDescription
            {
                BlendState = BlendStateDescription.SingleOverrideBlend,
                DepthStencilState = new DepthStencilStateDescription(
                    depthTestEnabled: true,
                    depthWriteEnabled: true,
                    comparisonKind: ComparisonKind.LessEqual
                    ),
                RasterizerState = new RasterizerStateDescription(
                    cullMode: FaceCullMode.Back,
                    fillMode: PolygonFillMode.Solid,
                    frontFace: FrontFace.CounterClockwise,
                    depthClipEnabled: true,
                    scissorTestEnabled: false),
                PrimitiveTopology = PrimitiveTopology.TriangleList,
                ResourceLayouts = new ResourceLayout[] { transformationResourceLayout, textureResourceLayout },
                ShaderSet = new ShaderSetDescription(
                    vertexLayouts: vertexLayoutDesc,
                    shaders: new[] { vertexShader, fragmentShader }),
                Outputs = gd.SwapchainFramebuffer.OutputDescription
            };

            pipeline = factory.CreateGraphicsPipeline(pipelineDesc);
        }

        public void Draw(CommandList cl, Matrix4x4 transformation, Matrix4x4 projection, Matrix4x4 view, Vector3 lightDirection, Vector3 lightColor)
        {
            cl.SetVertexBuffer(0, vertexBuffer);
            cl.SetIndexBuffer(indexBuffer, IndexFormat.UInt32);
            cl.UpdateBuffer(transformationBuffer, 0, transformation);
            cl.UpdateBuffer(projectionBuffer, 0, projection);
            cl.UpdateBuffer(viewBuffer, 0, view);

            cl.UpdateBuffer(lightDirectionBuffer, 0, lightDirection);
            cl.UpdateBuffer(lightColorBuffer, 0, lightColor);

            cl.UpdateBuffer(shineDamperBuffer, 0, 10.0f);
            cl.UpdateBuffer(reflectivityBuffer, 0, 1.0f);

            cl.SetPipeline(pipeline);
            cl.SetGraphicsResourceSet(0, transformationResourceSet);
            cl.SetGraphicsResourceSet(1, textureResourceSet);
            cl.DrawIndexed((uint)indexCount);
        }

        public void DisposeDeviceResources()
        {
            disposeCollector.DisposeAll();
        }
    }
}
