using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engin3D.Scene;
using Engin3D.Screen;
using Engin3D.Camera;
using System.Drawing;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Windows.Forms;

namespace Engin3D.Device
{
    class Device
    {
        DrawLineBresenhamAlgoritm lineDrower = new DrawLineBresenhamAlgoritm();
        public Device()
        {

        }

        public static void Render(Scene.Scene scene, Camera.Camera camera, Screen.Screen screen)
        {

            
            foreach (Mesh.Mesh mesh in scene.Meshes)
            {
                Matrix<double> transform = DenseMatrix.OfArray(new double[,]{
                         {1,0,0,mesh.Position.Z},
                         {0,1,0,mesh.Position.X},
                         {0,0,1,mesh.Position.Y},
                         {0,0,0,1}
                        });
                double x00 = Math.Cos(mesh.Rotation.X) * Math.Cos(mesh.Rotation.Y);
                double x01 = Math.Cos(mesh.Rotation.X) * Math.Sin(mesh.Rotation.Y) * Math.Sin(mesh.Rotation.Z) - Math.Sin(mesh.Rotation.X) * Math.Cos(mesh.Rotation.Z);
                double x02 = Math.Cos(mesh.Rotation.X) * Math.Sin(mesh.Rotation.Y) * Math.Cos(mesh.Rotation.Z) + Math.Sin(mesh.Rotation.X) * Math.Sin(mesh.Rotation.Z);
                double x10 = Math.Sin(mesh.Rotation.X) * Math.Cos(mesh.Rotation.Y);
                double x11 = Math.Sin(mesh.Rotation.X) * Math.Sin(mesh.Rotation.Y) * Math.Sin(mesh.Rotation.Z) + Math.Cos(mesh.Rotation.X) * Math.Cos(mesh.Rotation.Z);
                double x12 = Math.Sin(mesh.Rotation.X) * Math.Sin(mesh.Rotation.Y) * Math.Cos(mesh.Rotation.Z) - Math.Cos(mesh.Rotation.X) * Math.Sin(mesh.Rotation.Z);
                double x20 = -Math.Sin(mesh.Rotation.Y);
                double x21 = Math.Cos(mesh.Rotation.Y) *Math.Sin(mesh.Rotation.Z);
                double x22 = Math.Cos(mesh.Rotation.Y) *Math.Cos(mesh.Rotation.Z);
                Matrix<double> rotation = DenseMatrix.OfArray(new double[,]{
                         {x00,x01,x02,0},
                         {x10,x11,x12,0},
                         {x20,x21,x22,0},
                         {0,0,0,1}
                        });
                (double X, double Y)[] points = new (double X, double Y)[mesh.Vertices.Length];
                int index = 0;
                foreach (Vector3 vertice in mesh.Vertices)
                {               
                    points[index++] = camera.Project(vertice, transform * rotation);
                    // First, we project the 3D coordinates into the 2D space
                    //var point = Project(vertice, transformMatrix);
                    // Then we can draw on screen
                    //screen.PutPixel(points[index++], Color.Black);
                }
                for(int i=1;i<index;i++)
                {
                    screen.DrawLine(points[i], points[i - 1], Color.Black);
                }
            }
        }
    }
}
