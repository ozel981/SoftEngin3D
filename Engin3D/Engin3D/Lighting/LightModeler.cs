using Engin3D.Mesh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engin3D.Lighting
{
    public interface ILightModeler
    {
        Vector3D ModelLight(Vector3D position, Vector3D normal, Vector3D CameraPosition);
    }

    public class PointLightModeler : ILightModeler
    {
        public PointLight PointLight { get; set; }
        public Vector3D ModelLight(Vector3D position, Vector3D normal, Vector3D CameraPosition)
        {
            Vector3D lightVersor = (PointLight.Position - position);
            //Vector3D V = CameraPosition - position;
            //Vector3D R = (Vector3D.Normalized(normal) * ( 2 * Vector3D.DotProduct(Vector3D.Normalized(normal), Vector3D.Normalized(lightVersor)))) - Vector3D.Normalized(lightVersor);
            double cosLight = Vector3D.DotProduct(Vector3D.Normalized(normal), Vector3D.Normalized(lightVersor));
            //double mirroring = Math.Pow(Vector3D.DotProduct(V, R), 1);
            double result = Math.Min(1, Math.Max(0, cosLight));
            return new Vector3D
            {
                X = (PointLight.Color.X / 255) * result,
                Y = (PointLight.Color.Y / 255) * result,
                Z = (PointLight.Color.Z / 255) * result,
            };
        }
    }

    public class ReflectorModeler : ILightModeler
    {
        public Reflector Reflector { get; set; }
        public Vector3D ModelLight(Vector3D position, Vector3D normal, Vector3D CameraPosition)
        {
            Vector3D lightVersor = Reflector.Position - position;
            //Vector3D V = CameraPosition - position;
            //Vector3D R = (Vector3D.Normalized(normal) * (2 * Vector3D.DotProduct(Vector3D.Normalized(normal), Vector3D.Normalized(lightVersor)))) - Vector3D.Normalized(lightVersor);
            double cosLight = Vector3D.DotProduct(Vector3D.Normalized(normal), Vector3D.Normalized(lightVersor));
            //double mirroring = Math.Pow(Vector3D.DotProduct(R, V), 1);
            double result = cosLight * Math.Pow(Vector3D.DotProduct(lightVersor,Reflector.Target - Reflector.Position), 9);
            result = Math.Min(1, Math.Max(0, result));
            return new Vector3D
            {
                X = (Reflector.Color.X / 255) * result,
                Y = (Reflector.Color.Y / 255) * result,
                Z = (Reflector.Color.Z / 255) * result,
            };
        }
    }
}
