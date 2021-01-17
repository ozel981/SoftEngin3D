using Engin3D.Lighting;
using Engin3D.Mesh;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace Engin3D.Device
{
    static class Device
    {
        public static void Render(Scene.Scene scene, Camera.Camera camera, Screen.Screen screen,
            Shading shading)
        {
            screen.ClearZBuffor();
            // This part works but threads may conflict with each other TODO: fix it
            /*Parallel.ForEach(scene.Meshes, new ParallelOptions { MaxDegreeOfParallelism = 8 }, (mesh) =>
             {
                 RenderMesh(mesh, camera, screen,
                 shading);
             });*/
            foreach (Mesh.Mesh mesh in scene.Meshes)
            {
                RenderMesh(mesh, camera, screen,
                shading);
            }
            
        }
        private static void RenderMesh(Mesh.Mesh mesh, Camera.Camera camera, Screen.Screen screen,
                Shading shading)
        {
            List<Vertex> vertices = camera.Project(mesh);
            Shader shader;
            switch (shading)
            {
                case Shading.CONSTANT: 
                    shader = new ConstantShader(); 
                    break;
                case Shading.GOURAUD: 
                    shader =  new GouraudShader(vertices); 
                    break;
                case Shading.PHONG: 
                    shader = new PhongShader(); 
                    break;
                default: 
                    shader = new NoShader(); 
                    break;
            }
            shader.ShadeFaces(mesh.Faces, vertices, screen);
        }
    }
}
