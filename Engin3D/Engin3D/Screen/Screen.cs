using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Engin3D.Screen
{
    public class Screen
    {
        private Size size;
        private Int32[] Bits;
        private Device.DrawLineBresenhamAlgoritm LinePainter = new Device.DrawLineBresenhamAlgoritm();
        public Screen(ref PictureBox pictureBox)
        {
            size = pictureBox.Size;
            Bits = new Int32[pictureBox.Width * pictureBox.Height];
            GCHandle BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap bitmap = new Bitmap(pictureBox.Width, pictureBox.Height, pictureBox.Width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
            pictureBox.Image = bitmap;
        }

        public void PutPixel((double X, double Y) point, Color color)
        {
            Point scaledPoint = new Point
            {
                X = (int)(point.X * (size.Width / 2) + size.Width / 2),
                Y = (int)(-point.Y * (size.Height / 2) + size.Height / 2)               
            };

            if(scaledPoint.X >=0 && scaledPoint.Y >=0 && scaledPoint.X <= size.Width && scaledPoint.Y <= size.Height)
            {
                int index = scaledPoint.Y * size.Width + scaledPoint.X;
                if(index < Bits.Length)
                {
                    Bits[index] = color.ToArgb();
                    Bits[index+1] = color.ToArgb();
                    Bits[index-1] = color.ToArgb();
                }
            }
        }

        public void DrawLine((double X, double Y) begin, (double X, double Y) end, Color color)
        {
            Point scaledBegin = new Point
            {
                X = (int)(begin.X * (size.Width / 2) + size.Width / 2),
                Y = (int)(-begin.Y * (size.Height / 2) + size.Height / 2)
            };
            Point scaledEnd = new Point
            {
                X = (int)(end.X * (size.Width / 2) + size.Width / 2),
                Y = (int)(-end.Y * (size.Height / 2) + size.Height / 2)
            };
            LinePainter.Draw(scaledBegin, scaledEnd, (int x, int y) =>
              {
                  if (x >= 0 && y >= 0 && x <= size.Width && y <= size.Height)
                  {
                      int index = y * size.Width + x;
                      if (index < Bits.Length)
                      {
                          Bits[index] = color.ToArgb();
                          Bits[index + 1] = color.ToArgb();
                          Bits[index - 1] = color.ToArgb();
                      }
                  }
              });
        }
    }
}
