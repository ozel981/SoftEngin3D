using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engin3D.Camera
{
    public struct CameraSettings
    {
        public double FieldOfView { get; set; }
        public double AspectRatio { get; set; }
        public double NearVerge { get; set; }
        public double FarVerge { get; set; }
    }
}
