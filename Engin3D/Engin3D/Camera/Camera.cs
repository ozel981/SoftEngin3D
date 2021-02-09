
using Engin3D.Device;
using Engin3D.Mesh;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Engin3D.Camera
{ 
    public class Camera
    {
        private MathNet.Numerics.LinearAlgebra.Vector<double> position { get; set; }
        private MathNet.Numerics.LinearAlgebra.Vector<double> target { get; set; }
        private Matrix<double> projection;
        private Matrix<double> view;
        public Vector3D Position 
        {
            get => new Vector3D((float)position[1], (float)position[2], (float)position[0]);
            set { position = DenseVector.OfArray(new double[] { value.Z, value.X, value.Y, }); CreateViewMatrix(); }
}
        public Vector3D Target
        {
            get => new Vector3D((float)target[1], (float)target[2], (float)target[0]);
            set { target = DenseVector.OfArray(new double[] { value.Z, value.X, value.Y, }); CreateViewMatrix(); }
        }
        public Camera(Vector3D position, Vector3D target, CameraSettings settings)
        {
            this.position = DenseVector.OfArray(new double[] { position.Z, position.X, position.Y, });
            this.target = DenseVector.OfArray(new double[] { target.Z, target.X, target.Y,  });
            CreateProjectionMatrix(settings);
            CreateViewMatrix();

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                { Console.Write($"{view[i,j]}, "); }
                Console.Write("\n");
            }
        }

        private void CreateProjectionMatrix(CameraSettings settings)
        {
            double e = 1 / Math.Tan((Math.PI * settings.FieldOfView / 180) / 2);
            double n = settings.NearVerge;
            double f = settings.FarVerge;

            projection = DenseMatrix.OfArray(new double[,]
            {
                {e,0,0,0},
                {0,e/settings.AspectRatio,0,0},
                {0,0,(-1) * ((f + n) / (f - n)),(-1) * ((2 * f * n) / (f - n))},
                {0,0,-1,0}
            });
        }

        private void CreateViewMatrix()
        {
            MathNet.Numerics.LinearAlgebra.Vector<double> UpVector = DenseVector.OfArray(new double[] { 0, 0, 1 });
            MathNet.Numerics.LinearAlgebra.Vector<double> zAxis = (position - target).Normalize(1);
            MathNet.Numerics.LinearAlgebra.Vector<double> xAxis = Cross3(UpVector, zAxis).Normalize(1);
            MathNet.Numerics.LinearAlgebra.Vector<double> yAxis = Cross3(zAxis, xAxis);
            view = DenseMatrix.OfArray(new double[,]
            {
                {xAxis[0],yAxis[0],zAxis[0],0},
                {xAxis[1],yAxis[1],zAxis[1],0},
                {xAxis[2],yAxis[2],zAxis[2],0},
                {-xAxis.DotProduct(position), -yAxis.DotProduct(position), -zAxis.DotProduct(position), 1}
            });
            view = view.Transpose();
        }

        private MathNet.Numerics.LinearAlgebra.Vector<double> Cross3(
            MathNet.Numerics.LinearAlgebra.Vector<double> v1, 
            MathNet.Numerics.LinearAlgebra.Vector<double> v2)
        {
            double x = v1[1] * v2[2] - v1[2] * v2[1];
            double y = v1[0] * v2[2] + v1[2] * v2[0];
            double z = v1[0] * v2[1] - v1[1] * v2[0];
            return DenseVector.OfArray(new double[] { x, y, z });
        }
        
        public List<Vertex> Project(Mesh.Mesh mesh)
        {
            List<Vertex> points = new List<Vertex>();
            Matrix<double> transformation = projection * view;
            Vector3DTransformator transformator = new Vector3DTransformator();
            Matrix<double> worldTransformation = transformator.CreateTranslationMatrix(mesh.Position) * transformator.CreateRotationMatrix(mesh.Rotation);
            transformation = transformation * worldTransformation;
            foreach (Vertex vertex in mesh.Vertices)
            {
                points.Add(
                    new Vertex
                    {
                        Coordinates = transformator.Transform(vertex.Coordinates, transformation),
                        Normal = transformator.Transform(vertex.Normal, worldTransformation),
                        WorldCoordinates = transformator.Transform(vertex.Coordinates, worldTransformation),
                    });
            }
            return points;
        }
    }
} 
