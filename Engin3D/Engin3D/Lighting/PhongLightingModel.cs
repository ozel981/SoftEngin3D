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
        public virtual Color CalculateColor(Vector3D position, Color color, Vector3D normal, Vector3D CameraPosition, List<ILightModeler> lightModelers)
        {
            double distance = ((CameraPosition.Z - position.Z) / (FogIntencity));
            double R = 0;
            double G = 0;
            double B = 0;
            foreach(ILightModeler lightModeler in lightModelers)
            {
                R += ((double)color.R / 255.0) * lightModeler.ModelLight(position, normal, CameraPosition);
                G += ((double)color.G / 255.0) * lightModeler.ModelLight(position, normal, CameraPosition);
                B += ((double)color.B / 255.0) * lightModeler.ModelLight(position, normal, CameraPosition);
            }
            return Color.FromArgb(
                Math.Max(0, Math.Min(255, (int)(255.0 * R + (Fog ? distance * 255 : 0)))),
                Math.Max(0, Math.Min(255, (int)(255.0 * G + (Fog ? distance * 255 : 0)))),
                Math.Max(0, Math.Min(255, (int)(255.0 * B + (Fog ? distance * 255 : 0))))
                );
        }
    }
}
