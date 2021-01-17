using Engin3D.Device;
using Engin3D.Mesh;
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
    public delegate Color ShadeColor(Point actualPoint, Point a, Point b, Point c);
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

        public void PutPixel(Vector3D point, Color color)
        {
            Point scaledPoint = TransformPoint(point);
            SetPixel(scaledPoint, 0,  color);
        }

        public void DrawLine(Vector3D begin, Vector3D end, Color color)
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

        public void FillTriangle(Vertex a, Vertex b, Vertex c, ShadeColor shadeColor)
        {           
            Point[] vertices = new Point[] { TransformPoint(a.Coordinates), TransformPoint(b.Coordinates), TransformPoint(c.Coordinates) };
            List<(Point from, Point to)> edges = new List<(Point from, Point to)>
            {
                (vertices[0],vertices[1]),
                (vertices[1],vertices[2]),
                (vertices[2],vertices[0])
            };
            //Vector3D lightPos = new Vector3D(10, 0, 100);
            /*Color aColor = CalculateColor(a.WorldCoordinates, lightPos, color, a.Normal);
            Color bColor = CalculateColor(b.WorldCoordinates, lightPos, color, b.Normal);
            Color cColor = CalculateColor(c.WorldCoordinates, lightPos, color, c.Normal);*/
            (new BucketSortScanLineFillAlgorithm()).Paint(edges, (int rowNr, int fromX, int toX) =>
             {
                 for(int i= fromX; i<= toX; i++)
                 {
                     Point point = new Point(i, rowNr);
                     double z = InterpolatedZ(point, (vertices[0], a.Coordinates.Z), 
                         (vertices[1], b.Coordinates.Z), (vertices[2], c.Coordinates.Z));
                     Color newColor = shadeColor(point, vertices[0], vertices[1], vertices[2]);
                     /*InterpolatedColor(point, (vertices[0], aColor),
                         (vertices[1], bColor), (vertices[2], cColor));*/
                     SetPixel(point, z , newColor);
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

        private Point TransformPoint(Vector3D point)
        {
            return new Point
            {
                X = (int)(point.X * (size.Width / 2) + size.Width / 2),
                Y = (int)(-point.Y * (size.Height / 2) + size.Height / 2)
            };
        }
    }
}
