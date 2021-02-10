using Engin3D.Mesh;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engin3D.Lighting
{
    public class PhongLightingModel
    {
        public double FogIntencity { get; set; }
        public bool Fog { get; set; }

        public PhongLightingModel()
        {
            FogIntencity = 10;
            Fog = false;
        }
        public virtual Color CalculateColor(Vector3D position, Color color, Vector3D normal, Vector3D CameraPosition, List<LightModeler> lightModelers)
        {

            double distance = ((CameraPosition.X - position.X) / (FogIntencity));
            Vector3D lightVersor = (lightPosition - position);
            Vector3D RV = (Vector3D.Normalized(normal) * 2 * Vector3D.DotProduct(Vector3D.Normalized(normal), Vector3D.Normalized(lightVersor))) - Vector3D.Normalized(lightVersor);
            double mirroring = Math.Pow(Vector3D.DotProduct(RV, new Vector3D(0, 0, 1)), 1);
            double cosLight = Vector3D.DotProduct(Vector3D.Normalized(normal), Vector3D.Normalized(lightVersor));
            if (cosLight > 1) cosLight = 1;
            if (cosLight < 0) cosLight = 0;
            double R = ((double)color.R / 255.0) * (cosLight);
            double G = ((double)color.G / 255.0) * (cosLight);
            double B = ((double)color.B / 255.0) * (cosLight);
            return Color.FromArgb(
                Math.Max(0, Math.Min(255, (int)(255.0 * R + (Fog ? distance * 255 : 0)))),
                Math.Max(0, Math.Min(255, (int)(255.0 * G + (Fog ? distance * 255 : 0)))),
                Math.Max(0, Math.Min(255, (int)(255.0 * B + (Fog ? distance * 255 : 0))))
                );
        }
    }
}
