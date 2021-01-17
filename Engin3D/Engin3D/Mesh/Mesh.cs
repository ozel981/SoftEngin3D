using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engin3D.Mesh
{
    public struct Vector3D
    {
        public double X  { get; set; }
        public double Y  { get; set; }
        public double Z  { get; set; }
        public Vector3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public void Normalize()
        {
            double length = Math.Sqrt(X * X + Y * Y + Z * Z);
            X /= length;
            Y /= length;
            Z /= length;
        }

        public static Vector3D operator*(Vector3D v, double a)
        {
            return new Vector3D(v.X * a, v.Y * a, v.Z * a);
        }

        public static Vector3D operator/(Vector3D v, double a)
        {
            return new Vector3D(v.X / a, v.Y / a, v.Z / a);
        }

        public static Vector3D operator+(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3D operator-(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static double DotProduct(Vector3D a, Vector3D b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        public static Vector3D Normalized(Vector3D a)
        {
            double length = Math.Sqrt(a.X * a.X + a.Y * a.Y + a.Z * a.Z);
            return new Vector3D(a.X / length, a.Y / length, a.Z / length);
        }
    }
    public struct Face
    {
        public int A;
        public int B;
        public int C;
    }
    public struct Vertex
    {
        public Vector3D Coordinates { get; set; }
        public Vector3D Normal { get; set; }
        public Vector3D WorldCoordinates { get; set; }
    }
    public class Mesh
    {
        public string Name { get; set; }
        public Vertex[] Vertices { get; set; }
        public Face[] Faces { get; set; }
        public Vector3D Position { get; set; }
        public Vector3D Rotation { get; set; }

        public Mesh(string name, Vertex[] vertices, Face[] faces)
        {
            Vertices = (Vertex[])vertices.Clone();
            Faces = (Face[])faces.Clone();
            Name = name;
            Position = new Vector3D(0, 0, 0);
            Rotation = new Vector3D(0, 0, 0);
        }
    }
}
