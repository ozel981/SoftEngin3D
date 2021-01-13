using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engin3D.Mesh
{
    public struct Face
    {
        public int A;
        public int B;
        public int C;
    }
    public class Mesh
    {
        public string Name { get; set; }
        public Vector3[] Vertices { get; set; }
        public Face[] Faces { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }

        public Mesh(string name, Vector3[] vertices, Face[] faces)
        {
            Vertices = (Vector3[])vertices.Clone();
            Faces = (Face[])faces.Clone();
            Name = name;
        }
    }
}
