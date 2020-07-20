using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace HSEngine.Utility
{
    public struct ObjModelData
    {
        public string Name;
        public Vector3[] Vertices;
        public Vector2[] TextureCoords;
        public Vector3[] Normals;
        public int[] Indexes;
    }
}
