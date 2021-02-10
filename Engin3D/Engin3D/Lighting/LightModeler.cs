using Engin3D.Mesh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engin3D.Lighting
{
    public interface LightModeler
    {
        double ModelLight();
    }

    public class PointLightModeler : LightModeler
    {
        public PointLight PointLight { get; set; }
        public double ModelLight()
        {
            throw new NotImplementedException();
        }
    }

    public class ReflectorModeler : LightModeler
    {
        public Reflector Reflector { get; set; }
        public double ModelLight()
        {
            throw new NotImplementedException();
        }
    }
}
