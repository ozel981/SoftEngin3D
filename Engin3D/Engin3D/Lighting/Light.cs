using Engin3D.Mesh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engin3D.Lighting
{

    public struct PointLight
    {
        public Vector3D Position { get; set; }
        public Vector3D Color { get; set; }
    }

    public struct Reflector
    {
        public Vector3D Position { get; set; }
        public Vector3D Target { get; set; }
        public Vector3D Color { get; set; }
    }
}
