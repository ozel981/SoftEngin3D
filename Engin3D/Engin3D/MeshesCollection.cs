using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engin3D
{
    class MeshesCollection
    {
        public MeshData[] meshes { get; set; }
    }

    class MeshData
    {
        //public string  name { get; set; }
        public double[] position { get; set; }
        public double[] rotation { get; set; }
        //public double[] scaling { get; set; }
        public double[] positions { get; set; }
        public double[] normals { get; set; }
        public double[] uvs { get; set; }
        public int[] indices { get; set; }
        public MeshInfo[] subMeshes { get; set; }
    }

    class MeshInfo
    {
        public int verticesCount { get; set; }
    }

}
