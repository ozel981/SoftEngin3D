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
using Engin3D.Mesh;

namespace Engin3D.Device
{
    class Device
    {
        public Device()
        {

        }

        public static void Render(Scene.Scene scene, Camera.Camera camera, Screen.Screen screen)
        {
            screen.ClearZBuffor();
            foreach (Mesh.Mesh mesh in scene.Meshes)
            {
                List<(double x, double y, double z)> points = camera.Project(mesh);
                foreach (Face face in mesh.Faces)
                {
                    screen.FillTriangle(points[face.A], points[face.B], points[face.C], Color.Gray);
                    screen.DrawLine(points[face.A], points[face.B], Color.Black);
                    screen.DrawLine(points[face.B], points[face.C], Color.Black);
                    screen.DrawLine(points[face.C], points[face.A], Color.Black);
                }

                screen.FillTriangle(points[mesh.Faces[0].A], points[mesh.Faces[0].B], points[mesh.Faces[0].C], Color.Gray);
            }
        }
    }
}
