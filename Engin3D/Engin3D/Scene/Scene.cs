using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engin3D.Mesh;

namespace Engin3D.Scene
{

    [Serializable]
    public class Scene
    {
        public int ActiveCameraIndex { get; set; }
        public CameraOrientation ActiveCamera { get => CamerasOrientation[ActiveCameraIndex]; }
        public List<CameraOrientation> CamerasOrientation { get; set; }
       // public List<Lighting.Light> Lights { get; set; }
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
        public Camera.CameraSettings CameraSettings { get; set; }
    }
}
