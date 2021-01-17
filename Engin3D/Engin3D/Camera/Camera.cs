
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
    class Camera
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
            Matrix<double> translation = CreateTranslationMatrix(mesh.Position);
            Matrix<double> rotation = CreateRotationMatrix(mesh.Rotation);
            Matrix<double> worldTransformation = translation * rotation;
            Matrix<double> transformation = projection * view * worldTransformation;
            foreach(Vertex vertex in mesh.Vertices)
            {
                points.Add(
                    new Vertex
                    {
                        Coordinates = Transform(vertex.Coordinates, transformation),
                        Normal = Transform(vertex.Normal, worldTransformation),
                        WorldCoordinates = Transform(vertex.Coordinates, worldTransformation),
                    });
            }
            return points;
        }

        private Vector3D Transform(Vector3D vector, Matrix<double> transformation)
        {
            MathNet.Numerics.LinearAlgebra.Vector<double> newVector = DenseVector.OfArray(new double[]
                { vector.Z, vector.X, vector.Y, 1 });
            newVector = transformation * newVector;
            newVector[0] /= newVector[3];
            newVector[1] /= newVector[3];
            newVector[2] /= newVector[3];
            newVector[3] /= newVector[3];
            return new Vector3D(newVector[0], newVector[1], newVector[2]);
        }
        private Matrix<double> CreateTranslationMatrix(Vector3D position)
        {
            Matrix<double> translationMatrix = DenseMatrix.OfArray(new double[,]{
                         {1,0,0,position.Z},
                         {0,1,0,position.X},
                         {0,0,1,position.Y},
                         {0,0,0,1}
                        });

            return translationMatrix;
        }
        private Matrix<double> CreateRotationMatrix(Vector3D rotation)
        {
            double x00 = Math.Cos(rotation.X) * Math.Cos(rotation.Y);
            double x01 = Math.Cos(rotation.X) * Math.Sin(rotation.Y) * Math.Sin(rotation.Z) - Math.Sin(rotation.X) * Math.Cos(rotation.Z);
            double x02 = Math.Cos(rotation.X) * Math.Sin(rotation.Y) * Math.Cos(rotation.Z) + Math.Sin(rotation.X) * Math.Sin(rotation.Z);
            double x10 = Math.Sin(rotation.X) * Math.Cos(rotation.Y);
            double x11 = Math.Sin(rotation.X) * Math.Sin(rotation.Y) * Math.Sin(rotation.Z) + Math.Cos(rotation.X) * Math.Cos(rotation.Z);
            double x12 = Math.Sin(rotation.X) * Math.Sin(rotation.Y) * Math.Cos(rotation.Z) - Math.Cos(rotation.X) * Math.Sin(rotation.Z);
            double x20 = -Math.Sin(rotation.Y);
            double x21 = Math.Cos(rotation.Y) * Math.Sin(rotation.Z);
            double x22 = Math.Cos(rotation.Y) * Math.Cos(rotation.Z);

            Matrix<double> rotationMatrix = DenseMatrix.OfArray(new double[,]{
                         {x00,x01,x02,0},
                         {x10,x11,x12,0},
                         {x20,x21,x22,0},
                         {0,0,0,1}
                        });

            return rotationMatrix;
        }
    }
} 
