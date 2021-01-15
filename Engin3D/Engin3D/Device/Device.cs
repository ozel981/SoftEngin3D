using Engin3D.Mesh;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

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
            // This part works but threads may conflict with each other TODO: fix it
            /*Parallel.ForEach(scene.Meshes, new ParallelOptions { MaxDegreeOfParallelism = 8 }, (mesh) =>
             {
                 List<(double x, double y, double z)> points = camera.Project(mesh);
                 foreach (Face face in mesh.Faces)
                 {
                     screen.FillTriangle(points[face.A], points[face.B], points[face.C], Color.Gray);
                     screen.DrawLine(points[face.A], points[face.B], Color.Black);
                     screen.DrawLine(points[face.B], points[face.C], Color.Black);
                     screen.DrawLine(points[face.C], points[face.A], Color.Black);
                 }
             });*/
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
            }
        }
    }
}
