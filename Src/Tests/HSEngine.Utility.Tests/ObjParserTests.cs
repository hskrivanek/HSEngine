using NUnit.Framework;
using System.IO;
using System.Numerics;

namespace HSEngine.Utility.Tests
{
    [TestFixture]
    public class ObjParserTests
    {
        [Test]
        public void ModelNameIsLoaded()
        {
            var model = ObjParser.LoadModel(new StringReader(quadData));

            Assert.AreEqual("Quad_Plane", model.Name);
        }

        [Test]
        public void Quad_VerticesAreLoaded()
        {
            var expectedVertices = new Vector3[]
            {
                new Vector3(1, 0, 1),
                new Vector3(-1, 0, -1),
                new Vector3(-1, 0, 1),
                new Vector3(1, 0, -1)
            };

            var model = ObjParser.LoadModel(new StringReader(quadData));

            CollectionAssert.AreEqual(expectedVertices, model.Vertices);
        }

        [Test]
        public void Quad_TextureCoordsAreLoadedAndOrdered()
        {
            var expectedTextureCoords = new Vector2[]
            {
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(0, 0),
                new Vector2(1, 1)
            };

            var model = ObjParser.LoadModel(new StringReader(quadData));

            CollectionAssert.AreEqual(expectedTextureCoords, model.TextureCoords);
        }

        [Test]
        public void Quad_NormalsAreLoadedAndOrdered()
        {
            var expectedNormals = new Vector3[]
            {
                new Vector3(0,1,0),
                new Vector3(0,1,0),
                new Vector3(0,1,0),
                new Vector3(0,1,0)
            };

            var model = ObjParser.LoadModel(new StringReader(quadData));

            CollectionAssert.AreEqual(expectedNormals, model.Normals);
        }

        [Test]
        public void Quad_IndexesAreLoaded()
        {
            var expectedIndexes = new int[]
            {
                0, 1, 2,
                0, 3, 1
            };

            var model = ObjParser.LoadModel(new StringReader(quadData));

            CollectionAssert.AreEqual(expectedIndexes, model.Indexes);
        }

        [Test]
        public void Cube_NormalsInATriangleAreTheSame()
        {
            var model = ObjParser.LoadModel(new StringReader(cubeData));

            for (int i = 0; i < model.Indexes.Length; i+=3)
            {
                var normals = model.Normals;
                var indexes = model.Indexes;
                Assert.AreEqual(normals[indexes[i]], normals[indexes[i + 1]], "First and second normal in a triangle should be the same.");
                Assert.AreEqual(normals[indexes[i]], normals[indexes[i + 2]], "First and second normal in a triangle should be the same.");
            }
        }

        private const string quadData = @"# Blender v2.81 (sub 16) OBJ File: ''
# www.blender.org
mtllib quad.mtl
o Quad_Plane
v -1.000000 0.000000 1.000000
v 1.000000 0.000000 1.000000
v -1.000000 0.000000 -1.000000
v 1.000000 0.000000 -1.000000
vt 1.000000 0.000000
vt 0.000000 1.000000
vt 0.000000 0.000000
vt 1.000000 1.000000
vn 0.0000 1.0000 0.0000
usemtl None
s off
f 2/1/1 3/2/1 1/3/1
f 2/1/1 4/4/1 3/2/1
";

        private const string cubeData = @"# Blender v2.81 (sub 16) OBJ File: ''
# www.blender.org
mtllib cube.mtl
o Cube
v 1.000000 1.000000 -1.000000
v 1.000000 -1.000000 -1.000000
v 1.000000 1.000000 1.000000
v 1.000000 -1.000000 1.000000
v -1.000000 1.000000 -1.000000
v -1.000000 -1.000000 -1.000000
v -1.000000 1.000000 1.000000
v -1.000000 -1.000000 1.000000
v 1.000000 -1.000000 -1.000000
v -1.000000 -1.000000 -1.000000
v -1.000000 -1.000000 1.000000
v -1.000000 1.000000 1.000000
v -1.000000 1.000000 -1.000000
v 1.000000 -1.000000 1.000000
vt 0.500000 0.500000
vt 0.749957 0.250043
vt 0.749957 0.500000
vt 0.999913 0.000087
vt 0.999913 0.250043
vt 0.500000 0.250043
vt 0.250043 0.500000
vt 0.250043 0.250043
vt 0.000087 0.500000
vt 0.999913 0.500000
vt 0.749957 0.749957
vt 0.999913 0.749956
vt 0.749957 0.000087
vt 0.000087 0.250043
vn 0.0000 1.0000 0.0000
vn 0.0000 0.0000 1.0000
vn -1.0000 0.0000 0.0000
vn 0.0000 -1.0000 0.0000
vn 1.0000 0.0000 0.0000
vn 0.0000 0.0000 -1.0000
usemtl Material
s off
f 5/1/1 3/2/1 1/3/1
f 3/2/2 8/4/2 14/5/2
f 12/6/3 10/7/3 11/8/3
f 9/9/4 11/8/4 10/7/4
f 1/3/5 14/5/5 2/10/5
f 13/11/6 2/10/6 6/12/6
f 5/1/1 12/6/1 3/2/1
f 3/2/2 7/13/2 8/4/2
f 12/6/3 5/1/3 10/7/3
f 9/9/4 4/14/4 11/8/4
f 1/3/5 3/2/5 14/5/5
f 13/11/6 1/3/6 2/10/6
";
    }
}