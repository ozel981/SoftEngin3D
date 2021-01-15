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
    public struct Point3
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public static implicit operator Point3((double x, double y, double z) point)
        {
            return new Point3 { X = point.x, Y = point.y, Z = point.z };
        }

        public static implicit operator (double x, double y, double z)(Point3 point)
        {
            return (point.X, point.Y, point.Z);
        }
    }
    public class Screen
    {
        private Size size;
        private Int32[] Bits;
        private double[] ZBuffor;
        private LinePainter LinePainter = new LinePainterBresenhamAlgoritm();
        private PolygonFiller PolygonFiller = new BucketSortScanLineFillAlgorithm();
        public Screen(ref PictureBox pictureBox)
        {
            size = pictureBox.Size;
            Bits = new Int32[pictureBox.Width * pictureBox.Height];
            ZBuffor = new double[Bits.Length];
            for(int i=0;i<Bits.Length;i++)
            {
                ZBuffor[i] = double.MaxValue;
            }
            GCHandle BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap bitmap = new Bitmap(pictureBox.Width, pictureBox.Height, pictureBox.Width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
            pictureBox.Image = bitmap;
        }

        public void PutPixel(Point3 point, Color color)
        {
            Point scaledPoint = TransformPoint(point);
            SetPixel(scaledPoint, 0,  color);
        }

        public void DrawLine(Point3 begin, Point3 end, Color color)
        {
            Point scaledBegin = TransformPoint(begin);
            Point scaledEnd = TransformPoint(end);
            LinePainter.Paint(scaledBegin, scaledEnd, (Point point) =>
              {
                  double bz = Distance(point, scaledEnd);
                  double ez = Distance(point, scaledBegin);
                  double z = (begin.Z * bz + end.Z * ez) / (bz + ez);
                  SetPixel(point, z - 0.001, color);
              });
        }

        private double Distance(Point x, Point y)
        {
            return Math.Sqrt((x.X - y.X) * (x.X - y.X) + (x.Y - y.Y) * (x.Y - y.Y));
        }

        public void ClearZBuffor()
        {
            for (int i = 0; i < Bits.Length; i++)
            {
                ZBuffor[i] = double.MaxValue;
            }
        }

        public void FillTriangle(Point3 a, Point3 b, Point3 c, Color color)
        {
            
            Point[] vertices = new Point[] { TransformPoint(a), TransformPoint(b), TransformPoint(c) };
            List<(Point from, Point to)> edges = new List<(Point from, Point to)>
            {
                (vertices[0],vertices[1]),
                (vertices[1],vertices[2]),
                (vertices[2],vertices[0])
            };
            PolygonFiller.Paint(edges, (int rowNr, int fromX, int toX) =>
             {
                 for(int i= fromX; i<= toX; i++)
                 {
                     Point point = new Point(i, rowNr);
                     double z = InterpolatedZ(point, (vertices[0], a.Z), (vertices[1], b.Z), (vertices[2], c.Z));
                     SetPixel(point, z , color);
                 }
             });
        }

        private double InterpolatedZ(Point point, (Point point, double z) a, (Point point, double z) b, (Point point, double z) c)
        {
            double az = TriangleArea(point, b.point, c.point);
            double bz = TriangleArea(point, a.point, c.point);
            double cz = TriangleArea(point, a.point, b.point);
            double z = a.z * az + b.z * bz + c.z * cz;
            double n = az + bz + cz;
            return z / n;
        }
        private double TriangleArea(Point A, Point B, Point C)
        {
            return Math.Abs((B.X - A.X) * (C.Y - A.Y) - (B.Y - A.Y) * (C.X - A.X)) / 2;
        }

        private void SetPixel(Point point, double z, Color color)
        {
            if (point.X >= 0 && point.Y >= 0 && point.X <= size.Width && point.Y <= size.Height)
            {
                int index = point.Y * size.Width + point.X;
                if (index < Bits.Length)
                {
                    if(z < ZBuffor[index])
                    {
                        Bits[index] = color.ToArgb();
                        ZBuffor[index] = z;
                    } 
                }
            }
        }

        private Point TransformPoint(Point3 point)
        {
            return new Point
            {
                X = (int)(point.X * (size.Width / 2) + size.Width / 2),
                Y = (int)(-point.Y * (size.Height / 2) + size.Height / 2)
            };
        }
    }
}
