using Engin3D.Mesh;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engin3D.Lighting
{
    public interface IShader
    {
        void ShadeFaces(Face[] faces, List<Vertex> vertices, Screen.Screen screen);
    }

    public abstract class Shader : IShader
    {
        public Vector3D lightPos = new Vector3D(50, -50, 0);
        public void ShadeFaces(Face[] faces, List<Vertex> vertices,
            Screen.Screen screen)
        {
            Parallel.ForEach(faces, new ParallelOptions { MaxDegreeOfParallelism = 8 }, (face) =>
            { 
                if(true /*TODO: if face is visible*/)
                {
                    ShadeFace(face, vertices, screen);
                }
            });
        }

        protected abstract void ShadeFace(Face face, List<Vertex> vertices,
            Screen.Screen screen);

        protected virtual Color InterpolatedColor(Point point, (Point point, Color color) a, (Point point, Color color) b, (Point point, Color color) c)
        {
            double az = TriangleArea(point, b.point, c.point);
            double bz = TriangleArea(point, a.point, c.point);
            double cz = TriangleArea(point, a.point, b.point);
            double n = az + cz + bz;
            double R = (a.color.R * az + b.color.R * bz + c.color.R * cz) / n;
            double G = (a.color.G * az + b.color.G * bz + c.color.G * cz) / n;
            double B = (a.color.B * az + b.color.B * bz + c.color.B * cz) / n;
            return Color.FromArgb(
                Math.Max(0, Math.Min(255, (int)(R))),
                Math.Max(0, Math.Min(255, (int)(G))),
                Math.Max(0, Math.Min(255, (int)(B)))
                );
        }

        protected virtual Vector3D InterpolatedVector(Point point, (Point point, Vector3D normal) a, (Point point, Vector3D normal) b, (Point point, Vector3D normal) c)
        {
            double az = TriangleArea(point, b.point, c.point);
            double bz = TriangleArea(point, a.point, c.point);
            double cz = TriangleArea(point, a.point, b.point);
            double n = az + cz + bz;
            double X = (a.normal.X * az + b.normal.X * bz + c.normal.X * cz) / n;
            double Y = (a.normal.Y * az + b.normal.Y * bz + c.normal.Y * cz) / n;
            double Z = (a.normal.Z * az + b.normal.Z * bz + c.normal.Z * cz) / n;
            return new Vector3D(X,Y,Z);
        }

        protected virtual Color CalculateColor(Vector3D position, Vector3D lightPosition, Color color, Vector3D normal)
        {
            Vector3D lightVersor = (lightPosition - position);
            Vector3D RV = (Vector3D.Normalized(normal) * 2 * Vector3D.DotProduct(Vector3D.Normalized(normal), Vector3D.Normalized(lightVersor))) - Vector3D.Normalized(lightVersor);
            double mirroring = Math.Pow(Vector3D.DotProduct(RV, new Vector3D(0, 0, 1)), 1);
            double cosLight = Vector3D.DotProduct(Vector3D.Normalized(normal), Vector3D.Normalized(lightVersor));
            if (cosLight > 1) cosLight = 1;
            if (cosLight < 0) cosLight = 0;
            double R = ((double)color.R / 255.0) * (cosLight + 0);
            double G = ((double)color.G / 255.0) * (cosLight + 0);
            double B = ((double)color.B / 255.0) * (cosLight + 0);
            return Color.FromArgb(
                Math.Max(0, Math.Min(255, (int)(255.0 * R))),
                Math.Max(0, Math.Min(255, (int)(255.0 * G))),
                Math.Max(0, Math.Min(255, (int)(255.0 * B)))
                );
        }

        protected virtual double TriangleArea(Point A, Point B, Point C)
        {
            return Math.Abs((B.X - A.X) * (C.Y - A.Y) - (B.Y - A.Y) * (C.X - A.X)) / 2;
        }
    }

    public class NoShader : Shader
    {
        protected override void ShadeFace(Face face, List<Vertex> vertices, 
            Screen.Screen screen)
        {
            screen.FillTriangle(vertices[face.A], vertices[face.B], vertices[face.C], (p, a, b, c) => Color.LightGray);
        }


    }
    public class ConstantShader : Shader
    {
        protected override void ShadeFace(Face face, List<Vertex> vertices,
            Screen.Screen screen)
        {
            Vertex pointA = vertices[face.A];
            Vertex pointB = vertices[face.B];
            Vertex pointC = vertices[face.C];
            Vector3D worldCenterCoordinates = (pointA.WorldCoordinates +
                pointB.WorldCoordinates + pointC.WorldCoordinates) / 3;
            Vector3D centerNormal = (pointA.Normal + pointB.Normal +
                pointC.Normal) / 3;
            Color newColor = CalculateColor(worldCenterCoordinates, lightPos,
                Color.LightGray, centerNormal);
            screen.FillTriangle(pointA, pointB, pointC, ((p, a, b, c) => newColor));
        }
    }

    public class GouraudShader : Shader
    {
        protected List<Color> PointColors;
        public GouraudShader(List<Vertex> vertices)
        {
            PointColors = new List<Color>();
            foreach (Vertex vertex in vertices)
            {
                PointColors.Add(CalculateColor(vertex.WorldCoordinates,
                    lightPos, Color.LightGray, vertex.Normal));
            }
        }
        protected override void ShadeFace(Face face, List<Vertex> vertices,
            Screen.Screen screen)
        {
            screen.FillTriangle(vertices[face.A], vertices[face.B], vertices[face.C],
                (p, a, b, c) =>
                {
                    return InterpolatedColor(p, (a, PointColors[face.A]),
                        (b, PointColors[face.B]), (c, PointColors[face.C]));
                });
        }
    }

    public class PhongShader : Shader
    {
        protected override void ShadeFace(Face face, List<Vertex> vertices,
            Screen.Screen screen)
        {
            Vertex pointA = vertices[face.A];
            Vertex pointB = vertices[face.B];
            Vertex pointC = vertices[face.C];
            screen.FillTriangle(pointA, pointB, pointC,
                (p, a, b, c) =>
                {
                    Vector3D normal = InterpolatedVector(p, (a, pointA.Normal),
                        (b, pointB.Normal), (c, pointC.Normal));
                    Vector3D worldCoordinates = InterpolatedVector(p, (a, pointA.WorldCoordinates),
                        (b, pointB.WorldCoordinates), (c, pointC.WorldCoordinates));
                    return CalculateColor(worldCoordinates,
                    lightPos, Color.LightGray, normal);
                });
        }
    }
}
