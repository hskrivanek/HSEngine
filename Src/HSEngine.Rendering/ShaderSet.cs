using System.Text;
using Veldrid;
using Veldrid.SPIRV;

namespace HSEngine.Rendering
{
    public class ShaderSet
    {
        private readonly string vertexShader;
        private readonly string fragmentShader;

        public ShaderSet(string vertexShader, string fragmentShader)
        {
            this.vertexShader = vertexShader; 
            this.fragmentShader = fragmentShader;
        }

        public (Shader vertex, Shader fragment) CreateShaders(ResourceFactory factory)
        {
            var vertexDesc = new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(vertexShader), "main");
            var fragmentDesc = new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(fragmentShader), "main");

            var shaders = factory.CreateFromSpirv(vertexDesc, fragmentDesc);

            return (shaders[0], shaders[1]);
        }
    }
}
