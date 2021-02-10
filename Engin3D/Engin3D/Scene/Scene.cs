using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engin3D.Camera;
using Engin3D.Lighting;
using Engin3D.Mesh;

namespace Engin3D.Scene
{

    [Serializable]
    public class Scene
    {
        public int ActiveCameraIndex { get; set; }
        public CameraOrientation ActiveCamera { get => CamerasOrientation[ActiveCameraIndex]; }
        public List<CameraOrientation> CamerasOrientation { get; set; }
        public List<ReflectorOrientation> Reflectors { get; set; }
        public List<PointLightOrientation> PointLights { get; set; }
        public List<Mesh.Mesh> Meshes { get; set; }
        public Scene()
        {
            Meshes = new List<Mesh.Mesh>();
            CamerasOrientation = new List<CameraOrientation>();
            ActiveCameraIndex = 0;
        }

        public void AddMesh(Mesh.Mesh mesh)
        {
            Meshes.Add(mesh);
        }
    }

    public struct CameraOrientation 
    {
        public Vector3D Targer { get; set; }
        public Vector3D Position { get; set; }
        public int? StickPositionToMesh { get; set; }
        public int? StickTargetToMesh { get; set; }
        public CameraSettings CameraSettings { get; set; }
    }

    public struct ReflectorOrientation
    {
        public Reflector Reflector { get; set; }
        public int? StickPositionToMesh { get; set; }
        public int? StickTargetToMesh { get; set; }
    }

    public struct PointLightOrientation
{
        public PointLight PointLight { get; set; }
        public int? StickPositionToMesh { get; set; }
    }
}
