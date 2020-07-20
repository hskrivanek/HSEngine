using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;

namespace HSEngine.Utility
{
    public static class ObjParser
    {
        public static ObjModelData LoadModel(TextReader reader)
        {
            var model = new ObjModelData();

            var positions = new List<Vector3>();
            var textureCoords = new List<Vector2>();
            var normals = new List<Vector3>();
            var indexGroups = new List<(string, string, string)>();

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("o "))
                {
                    var name = line.Split(' ')[1];
                    model.Name = name;
                }

                if (line.StartsWith("v "))
                {
                    positions.Add(ParseVector3(line));
                }

                if (line.StartsWith("vt "))
                {
                    textureCoords.Add(ParseVector2(line));
                }

                if (line.StartsWith("vn "))
                {
                    normals.Add(ParseVector3(line));
                }

                if (line.StartsWith("f "))
                {
                    indexGroups.Add(ParseIndexes(line));
                }
            }

            reader.Close();

            var orderedPositions = new List<Vector3>();
            var orderedTextureCoords = new List<Vector2>();
            var orderedNormals = new List<Vector3>();
            var indexes = new List<int>();
            var existingVertices = new Dictionary<string, int>();

            foreach (var indexGroup in indexGroups)
            {
                ProcessVertexIndexes(positions, textureCoords, normals, 
                    orderedPositions, orderedTextureCoords, orderedNormals, 
                    indexes, indexGroup.Item1, existingVertices);
                ProcessVertexIndexes(positions, textureCoords, normals, 
                    orderedPositions, orderedTextureCoords, orderedNormals, 
                    indexes, indexGroup.Item2, existingVertices);
                ProcessVertexIndexes(positions, textureCoords, normals, 
                    orderedPositions, orderedTextureCoords, orderedNormals, 
                    indexes, indexGroup.Item3, existingVertices);
            }

            model.Vertices = orderedPositions.ToArray();
            model.TextureCoords = orderedTextureCoords.ToArray();
            model.Normals = orderedNormals.ToArray();
            model.Indexes = indexes.ToArray(); ;

            return model;
        }

        private static void ProcessVertexIndexes(
            List<Vector3> positions,
            List<Vector2> textureCoords, 
            List<Vector3> normals,
            List<Vector3> orderedPositions,
            List<Vector2> orderedTextureCoords, 
            List<Vector3> orderedNormals, 
            List<int> indexes, 
            string indexString,
            Dictionary<string, int> existingVertices)
        {
            if (!existingVertices.ContainsKey(indexString))
            {
                var indexValues = indexString.Split('/')
                                       .Select(i => int.Parse(i, CultureInfo.InvariantCulture) - 1)
                                       .ToArray();
                var currentIndex = orderedPositions.Count;
                indexes.Add(currentIndex);

                orderedPositions.Add(positions[indexValues[0]]);
                orderedTextureCoords.Add(textureCoords[indexValues[1]]);
                orderedNormals.Add(normals[indexValues[2]]);

                existingVertices.Add(indexString, currentIndex);
            }
            else
            {
                indexes.Add(existingVertices[indexString]);
            }
        }

        private static Vector3 ParseVector3(string line)
        {
            var elements = line.Split(' ');
            var x = ParseFloat(elements[1]);
            var y = ParseFloat(elements[2]);
            var z = ParseFloat(elements[3]);
            return new Vector3(x, y, z);
        }
        private static Vector2 ParseVector2(string line)
        {
            var elements = line.Split(' ');
            var u = ParseFloat(elements[1]);
            var v = ParseFloat(elements[2]);
            return new Vector2(u, v);
        }

        private static (string, string, string) ParseIndexes(string line)
        {
            var elements = line.Split(' ');
            if (elements.Length > 4)
            {
                throw new InvalidDataException("Mesh can contain only triangles.");
            }
            return (elements[1], elements[2], elements[3]);
        }

        private static float ParseFloat(string number)
        {
            return float.Parse(number, CultureInfo.InvariantCulture);
        }
    }
}
