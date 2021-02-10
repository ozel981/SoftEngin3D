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
        void ShadeFaces(Device.ProjectedMesh projectedMesh, Screen.Screen screen, Vector3D CemeraPosition);
    }

    public abstract class Shader : IShader
    {
        protected PhongLightingModel PhongLightingModel;
        protected List<LightModeler> LightModelers;
        public Vector3D lightPos = new Vector3D(50, -50, 0);
        public Shader(PhongLightingModel phongLightingModel, List<LightModeler> lightModelers)
        {
            PhongLightingModel = phongLightingModel;
            LightModelers = lightModelers;
        }
        public void ShadeFaces(Device.ProjectedMesh projectedMesh, Screen.Screen screen, Vector3D CemeraPosition)
        {
            Parallel.ForEach(projectedMesh.Faces, new ParallelOptions { MaxDegreeOfParallelism = 8 }, (face) =>
            {
                ShadeFace(face, projectedMesh.Vertices, screen, new Vector3D(CemeraPosition.Z, CemeraPosition.X, CemeraPosition.Y), 
                    projectedMesh.Color);
            });
        }

        protected abstract void ShadeFace(Face face, List<Vertex> vertices,
            Screen.Screen screen, Vector3D CemeraPosition, Color color);

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

        

        protected virtual double TriangleArea(Point A, Point B, Point C)
        {
            return Math.Abs((B.X - A.X) * (C.Y - A.Y) - (B.Y - A.Y) * (C.X - A.X)) / 2;
        }
    }

    public class NoShader : Shader
    {
        public NoShader(PhongLightingModel phongLightingModel, List<LightModeler> lightModelers) : base(phongLightingModel, lightModelers) { }
        protected override void ShadeFace(Face face, List<Vertex> vertices, 
            Screen.Screen screen, Vector3D CemeraPosition, Color color)
        {
            screen.FillTriangle(vertices[face.A], vertices[face.B], vertices[face.C], (p, a, b, c) => color);
        }


    }
    public class ConstantShader : Shader
    {
        public ConstantShader(PhongLightingModel phongLightingModel, List<LightModeler> lightModelers) : base(phongLightingModel, lightModelers) { }
        protected override void ShadeFace(Face face, List<Vertex> vertices,
            Screen.Screen screen, Vector3D CemeraPosition, Color color)
        {
            Vertex pointA = vertices[face.A];
            Vertex pointB = vertices[face.B];
            Vertex pointC = vertices[face.C];
            Vector3D worldCenterCoordinates = (pointA.WorldCoordinates +
                pointB.WorldCoordinates + pointC.WorldCoordinates) / 3;
            Vector3D centerNormal = (pointA.Normal + pointB.Normal +
                pointC.Normal) / 3;
            Color newColor = PhongLightingModel.CalculateColor(worldCenterCoordinates,
                color, centerNormal, CemeraPosition, LightModelers);
            screen.FillTriangle(pointA, pointB, pointC, ((p, a, b, c) => newColor));
        }
    }

    public class GouraudShader : Shader
    {
        protected List<Color> PointColors;
        public GouraudShader(PhongLightingModel phongLightingModel, List<LightModeler> lightModelers, Device.ProjectedMesh projectedMesh, Vector3D CameraPosition)
            : base(phongLightingModel, lightModelers) 
        {
            PointColors = new List<Color>();
            foreach (Vertex vertex in projectedMesh.Vertices)
            {
                PointColors.Add(PhongLightingModel.CalculateColor(vertex.WorldCoordinates,
                    projectedMesh.Color, vertex.Normal, CameraPosition, LightModelers));
            }
        }
        protected override void ShadeFace(Face face, List<Vertex> vertices,
            Screen.Screen screen, Vector3D CemeraPosition, Color color)
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
        public PhongShader(PhongLightingModel phongLightingModel, List<LightModeler> lightModelers) : base(phongLightingModel, lightModelers) { }
        protected override void ShadeFace(Face face, List<Vertex> vertices,
            Screen.Screen screen, Vector3D CemeraPosition, Color color)
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
                    return PhongLightingModel.CalculateColor(worldCoordinates,
                     color, normal, CemeraPosition, LightModelers);
                });
        }
    }
}
