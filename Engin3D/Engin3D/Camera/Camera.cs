
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
        private Vector<double> position { get; set; }
        private Vector<double> target { get; set; }
        private Matrix<double> projection;
        private Matrix<double> view;
        public Vector3 Position 
        {
            get => new Vector3((float)position[1], (float)position[2], (float)position[0]);
            set { position = DenseVector.OfArray(new double[] { value.Z, value.X, value.Y, }); CreateViewMatrix(); }
}
        public Vector3 Target
        {
            get => new Vector3((float)target[1], (float)target[2], (float)target[0]);
            set { target = DenseVector.OfArray(new double[] { value.Z, value.X, value.Y, }); CreateViewMatrix(); }
        }
        public Camera(Vector3 position, Vector3 target, CameraSettings settings)
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
            Vector<double> UpVector = DenseVector.OfArray(new double[] { 0, 0, 1 });
            Vector<double> zAxis = (position - target).Normalize(1);
            Vector<double> xAxis = Cross3(UpVector, zAxis).Normalize(1);
            Vector<double> yAxis = Cross3(zAxis, xAxis);
            view = DenseMatrix.OfArray(new double[,]
            {
                {xAxis[0],yAxis[0],zAxis[0],0},
                {xAxis[1],yAxis[1],zAxis[1],0},
                {xAxis[2],yAxis[2],zAxis[2],0},
                {-xAxis.DotProduct(position), -yAxis.DotProduct(position), -zAxis.DotProduct(position), 1}
            });
            view = view.Transpose();
        }

        private Vector<double> Cross3(Vector<double> v1, Vector<double> v2)
        {
            double x = v1[1] * v2[2] - v1[2] * v2[1];
            double y = v1[0] * v2[2] + v1[2] * v2[0];
            double z = v1[0] * v2[1] - v1[1] * v2[0];
            return DenseVector.OfArray(new double[] { x, y, z });
        }

        public (double x, double y) Project(Vector3 point, Matrix<double> transformation)
        {
            Vector<double> newPoint = DenseVector.OfArray(new double[] {point.Z,point.X,point.Y,1});
            newPoint = projection * view * transformation * newPoint;
            newPoint[0] /= newPoint[3];
            newPoint[1] /= newPoint[3];
            newPoint[2] /= newPoint[3];
            newPoint[3] /= newPoint[3];
            return (newPoint[0], newPoint[1]);
        }
    }
} 
