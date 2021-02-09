using Engin3D.Mesh;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engin3D.Device
{
    class Vector3DTransformator
    {
        public Vector3D Transform(Vector3D vector, Vector3D positionVector, Vector3D rotationVector, Matrix<double> transformation = null)
        {
            Matrix<double> translation = CreateTranslationMatrix(positionVector);
            Matrix<double> rotation = CreateRotationMatrix(rotationVector);
            if(transformation != null)
            {
                return Transform(vector, transformation * translation * rotation);
            }
            else
            {
                return Transform(vector, translation * rotation);
            }
        }
        public Vector3D Transform(Vector3D vector, Matrix<double> transformation)
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

        public Matrix<double> CreateTranslationMatrix(Vector3D position)
        {
            Matrix<double> translationMatrix = DenseMatrix.OfArray(new double[,]{
                         {1,0,0,position.Z},
                         {0,1,0,position.X},
                         {0,0,1,position.Y},
                         {0,0,0,1}
                        });

            return translationMatrix;
        }
        public Matrix<double> CreateRotationMatrix(Vector3D rotation)
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
