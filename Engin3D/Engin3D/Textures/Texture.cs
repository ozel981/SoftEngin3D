using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engin3D.Textures
{
    public class Texture
    {
        private Bitmap bitmap;
        private int width;
        private int height;
        public Texture(string filename, int width, int height)
        {
            this.width = width;
            this.height = height;
            Load(filename);
        }

        private void Load(string filename)
        {
            bitmap = new Bitmap(new Bitmap(filename), new Size(width, height));
            
        }
        public Color Map(float tu, float tv)
        {
            if (bitmap == null)
            {
                return Color.White;
            }
            int u = Math.Abs((int)(tu * width) % width);
            int v = Math.Abs((int)(tv * height) % height);

            return bitmap.GetPixel(u,v);
        }
    }
}
