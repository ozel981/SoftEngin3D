using Engin3D.Mesh;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace Engin3D.Device
{
    static class Device
    {
        public static void Render(Scene.Scene scene, Camera.Camera camera, Screen.Screen screen)
        {
            screen.ClearZBuffor();
            // This part works but threads may conflict with each other TODO: fix it
            /*Parallel.ForEach(scene.Meshes, new ParallelOptions { MaxDegreeOfParallelism = 8 }, (mesh) =>
             {
                List<Vertex> points = camera.Project(mesh);
                foreach (Face face in mesh.Faces)
                {
                    screen.FillTriangle(points[face.A], points[face.B], points[face.C], Color.Gray);
                    screen.DrawLine(points[face.A].Coordinates, points[face.B].Coordinates, Color.Black);
                    screen.DrawLine(points[face.B].Coordinates, points[face.C].Coordinates, Color.Black);
                    screen.DrawLine(points[face.C].Coordinates, points[face.A].Coordinates, Color.Black);
                }
             });*/
            foreach (Mesh.Mesh mesh in scene.Meshes)
            {
                List<Vertex> points = camera.Project(mesh);
                foreach (Face face in mesh.Faces)
                {
                    Vertex a = points[face.A];
                    Vertex b = points[face.B];
                    Vertex c = points[face.C];
                    screen.FillTriangle(a, b, c, Color.LightGray);
                    /*screen.DrawLine(points[face.A].Coordinates, points[face.B].Coordinates, Color.Black);
                    screen.DrawLine(points[face.B].Coordinates, points[face.C].Coordinates, Color.Black);
                    screen.DrawLine(points[face.C].Coordinates, points[face.A].Coordinates, Color.Black);*/
                }
            }
        }
    }
}
