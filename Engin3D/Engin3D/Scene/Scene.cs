using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engin3D.Mesh;

namespace Engin3D.Scene
{
    class Scene
    {
        public List<Mesh.Mesh> Meshes { get; private set; }
        public Scene()
        {
            Meshes = new List<Mesh.Mesh>();
        }

        public void AddMesh(Mesh.Mesh mesh)
        {
            Meshes.Add(mesh);
        }
    }
}
