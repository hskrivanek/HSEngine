using HSEngine.Utility;
using System;
using System.Numerics;
using Veldrid;
using Veldrid.Utilities;

namespace HSEngine.Rendering
{
    public class RawModel
    {
        private readonly VertexData[] vertices;
        private readonly int[] indexes;

        public RawModel(VertexData[] vertices, int[] indexes)
        {
            this.vertices = vertices ?? throw new ArgumentNullException(nameof(vertices));
            this.indexes = indexes ?? throw new ArgumentNullException(nameof(indexes));
        }

        public RawModel(ObjModelData modelData)
        {
            int verticesCount = modelData.Vertices.Length;
            vertices = new VertexData[verticesCount];
            for (int i = 0; i < verticesCount; i++)
            {
                var uv = modelData.TextureCoords[i];
                uv = new Vector2(uv.X, 1 - uv.Y);
                vertices[i] = new VertexData(modelData.Vertices[i], uv, modelData.Normals[i]);
            }
            indexes = modelData.Indexes;
        }

        public DeviceBuffer CreateVertexBuffer(GraphicsDevice gd)
        {
            var factory = gd.ResourceFactory;
            var buffer = factory.CreateBuffer(new BufferDescription((uint)vertices.Length * VertexData.SizeInBytes, BufferUsage.VertexBuffer));
            gd.UpdateBuffer(buffer, 0, vertices);
            return buffer;
        }

        public DeviceBuffer CreateIndexBuffer(GraphicsDevice gd, out int indexCount)
        {
            var factory = gd.ResourceFactory;
            var buffer = factory.CreateBuffer(new BufferDescription((uint)indexes.Length * sizeof(int), BufferUsage.IndexBuffer));
            gd.UpdateBuffer(buffer, 0, indexes);
            indexCount = indexes.Length;
            return buffer;
        }
    }
}
