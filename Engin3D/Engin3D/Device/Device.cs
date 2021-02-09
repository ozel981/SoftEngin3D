using Engin3D.Lighting;
using Engin3D.Mesh;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace Engin3D.Device
{
    static class Device
    {
        public static void Render(Scene.Scene scene, Screen.Screen screen,
            Shading shading)
        {
            screen.ClearZBuffor();
            Vector3DTransformator transformator = new Vector3DTransformator();
            Vector3D CameraPosition = scene.ActiveCamera.Position;
            if (scene.ActiveCamera.StickPositionToMesh != null)
            {
                int meshIndex = scene.ActiveCamera.StickPositionToMesh.GetValueOrDefault();
                CameraPosition = transformator.Transform(scene.ActiveCamera.Position,scene.Meshes[meshIndex].Position, scene.Meshes[meshIndex].Rotation);
                CameraPosition = new Vector3D(CameraPosition.Y, CameraPosition.Z, CameraPosition.X);
            }
            Vector3D CameraTarget = scene.ActiveCamera.Targer;
            if (scene.ActiveCamera.StickTargetToMesh != null)
            {
                int meshIndex = scene.ActiveCamera.StickTargetToMesh.GetValueOrDefault();
                CameraTarget = transformator.Transform(scene.ActiveCamera.Targer, scene.Meshes[meshIndex].Position, scene.Meshes[meshIndex].Rotation);
                CameraTarget = new Vector3D(CameraTarget.Y, CameraTarget.Z, CameraTarget.X);
            }
            Camera.Camera camera = new Camera.Camera(CameraPosition,CameraTarget,scene.ActiveCamera.CameraSettings);
            // This part works but threads may conflict with each other TODO: fix it
            Parallel.ForEach(scene.Meshes, new ParallelOptions { MaxDegreeOfParallelism = 4 }, (mesh) =>
             {
                RenderMesh(mesh, camera, screen,
                shading);
             });         
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
