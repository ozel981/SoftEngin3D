using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engin3D.Screen
{
    public delegate void PutPixel(Point point);
    public interface LinePainter
    {
        void Paint(Point first, Point second, PutPixel DrawPoint);
    }
    public class LinePainterBresenhamAlgoritm : LinePainter
    {
        public void Paint(Point first, Point second, PutPixel DrawPoint)
        {
            if (Math.Abs(first.X - second.X) < 2 && Math.Abs(first.Y - second.Y) < 2)
            {
                //DrawPoint(first.X, first.Y); DrawPoint(second.X, second.Y);
            }
            int dx;
            int dy;
            int xf;
            int yf;
            int xb;
            int yb;
            int lxd = 1;
            int pxd = 1;
            int uyd = 1;
            int byd = 1;
            if (Math.Abs(first.X - second.X) < Math.Abs(first.Y - second.Y))
            {
                //vertical
                if (second.Y > first.Y)
                {
                    //standard
                    xf = first.Y; yf = first.X;
                    xb = second.Y; yb = second.X;
                }
                else
                {
                    //reverse
                    xf = second.Y; yf = second.X;
                    xb = first.Y; yb = first.X;
                }
            }
            else
            {
                //horizontal
                if (second.X > first.X)
                {
                    //standard
                    xf = first.X; yf = first.Y;
                    xb = second.X; yb = second.Y;
                }
                else
                {
                    //reverse
                    xf = second.X; yf = second.Y;
                    xb = first.X; yb = first.Y;
                }
            }
            dx = xb - xf;
            dy = yb - yf;
            int incrE = 0;
            int incrNE = 0;
            int d = 0;
            if (yb > yf)
            {
                incrE = 2 * dy;
                incrNE = 2 * (dy - dx);
                d = 2 * dy - dx;
            }
            else
            {
                incrE = -2 * dy;
                incrNE = 2 * (-dy - dx);
                d = -2 * dy - dx;
                uyd = -1;
                byd = -1;
            }
            while (xf < xb)
            {
                xf += lxd; xb -= pxd;
                if (d < 0) //Choose E and W
                    d += incrE;
                else //Choose NE and SW
                {
                    d += incrNE;
                    yf += uyd;
                    yb -= byd;
                }
                if (Math.Abs(first.X - second.X) < Math.Abs(first.Y - second.Y))
                {
                    DrawPoint(new Point(yf, xf));
                    DrawPoint(new Point(yb, xb));
                }
                else
                {

                    DrawPoint(new Point(xf, yf));
                    DrawPoint(new Point(xb, yb));
                }
            }

        }
    }
}
