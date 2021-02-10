﻿using Engin3D.Lighting;
using Engin3D.Mesh;
using Engin3D.Scene;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace Engin3D.Device
{
    public struct ProjectedMesh
    {
        public Color Color { get; set; }
        public List<Vertex> Vertices { get; set; }
        public Face[] Faces { get; set; }
    }
    static class Device
    {
        public static Camera.Camera CreateCamera(Scene.Scene scene)
        {
            Vector3DTransformator transformator = new Vector3DTransformator();
            Vector3D CameraPosition = scene.ActiveCamera.Position;
            if (scene.ActiveCamera.StickPositionToMesh != null)
            {
                int meshIndex = scene.ActiveCamera.StickPositionToMesh.GetValueOrDefault();
                CameraPosition = transformator.Transform(scene.ActiveCamera.Position, scene.Meshes[meshIndex].Position, scene.Meshes[meshIndex].Rotation);
                CameraPosition = new Vector3D(CameraPosition.Y, CameraPosition.Z, CameraPosition.X);
            }
            Vector3D CameraTarget = scene.ActiveCamera.Targer;
            if (scene.ActiveCamera.StickTargetToMesh != null)
            {
                int meshIndex = scene.ActiveCamera.StickTargetToMesh.GetValueOrDefault();
                CameraTarget = transformator.Transform(scene.ActiveCamera.Targer, scene.Meshes[meshIndex].Position, scene.Meshes[meshIndex].Rotation);
                CameraTarget = new Vector3D(CameraTarget.Y, CameraTarget.Z, CameraTarget.X);
            }
            return new Camera.Camera(CameraPosition, CameraTarget, scene.ActiveCamera.CameraSettings);
        }

        public static Vector3D TransformVector(Vector3D vector, Vector3D position, Vector3D rotation)
        {
            Vector3D Vector = vector;
            Vector3DTransformator transformator = new Vector3DTransformator();
            Vector = transformator.Transform(vector, position, rotation);
            return new Vector3D(Vector.Y, Vector.Z, Vector.X);
        }

        public static void Render(Scene.Scene scene, Screen.Screen screen,
            Shading shading, PhongLightingModel phongLightingModel)
        {
            screen.ClearZBuffor();
            Camera.Camera camera = CreateCamera(scene);
            List<LightModeler> lightModelers = new List<LightModeler>();
            foreach(ReflectorOrientation reflector in scene.Reflectors)
            {
                Reflector newReflector = new Reflector
                {
                    Color = reflector.Reflector.Color,
                    Position = reflector.Reflector.Position,
                    Target = reflector.Reflector.Target,
                };
                if(reflector.StickPositionToMesh != null)
                {
                    int meshIndex = reflector.StickPositionToMesh.GetValueOrDefault();
                    newReflector.Position = TransformVector(reflector.Reflector.Position, scene.Meshes[meshIndex].Position, scene.Meshes[meshIndex].Rotation);
                }
                if (reflector.StickTargetToMesh != null)
                {
                    int meshIndex = reflector.StickTargetToMesh.GetValueOrDefault();
                    newReflector.Target = TransformVector(reflector.Reflector.Target, scene.Meshes[meshIndex].Position, scene.Meshes[meshIndex].Rotation);
                }
                lightModelers.Add(new ReflectorModeler { Reflector = newReflector });
            }
            foreach (PointLightOrientation pointLight in scene.PointLights)
            {
                PointLight newPointLight = new PointLight
                {
                    Color = pointLight.PointLight.Color,
                    Position = pointLight.PointLight.Position
                };
                if (pointLight.StickPositionToMesh != null)
                {
                    int meshIndex = pointLight.StickPositionToMesh.GetValueOrDefault();
                    newPointLight.Position = TransformVector(pointLight.PointLight.Position, scene.Meshes[meshIndex].Position, scene.Meshes[meshIndex].Rotation);
                }
                lightModelers.Add(new PointLightModeler { PointLight = newPointLight });
            }
            Parallel.ForEach(scene.Meshes, new ParallelOptions { MaxDegreeOfParallelism = 4 }, (mesh) =>
             {
                RenderMesh(mesh, camera, screen,
                shading, phongLightingModel, lightModelers);
             });         
        }
        private static void RenderMesh(Mesh.Mesh mesh, Camera.Camera camera, Screen.Screen screen,
                Shading shading, PhongLightingModel phongLightingModel, List<LightModeler> lightModelers)
        {
            List<Vertex> vertices = camera.Project(mesh);
            ProjectedMesh projectedMesh = new ProjectedMesh
            {
                Color = mesh.Color,
                Vertices = vertices,
                Faces = mesh.Faces
            };
            Shader shader;
            switch (shading)
            {
                case Shading.CONSTANT: 
                    shader = new ConstantShader(phongLightingModel, lightModelers); 
                    break;
                case Shading.GOURAUD: 
                    shader =  new GouraudShader(phongLightingModel, lightModelers, projectedMesh, camera.Position); 
                    break;
                case Shading.PHONG: 
                    shader = new PhongShader(phongLightingModel, lightModelers); 
                    break;
                default: 
                    shader = new NoShader(phongLightingModel, lightModelers); 
                    break;
            }
            shader.ShadeFaces(projectedMesh, screen, camera.Position);
        }
    }
}
