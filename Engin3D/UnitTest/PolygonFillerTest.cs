using Engin3D.Screen;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Drawing;

namespace UnitTest
{
    [TestClass]
    public class PolygonFillerTest
    {
        [TestMethod]
        public void PaintTriangleTest()
        {
            PolygonFiller polygonFiller = new BucketSortScanLineFillAlgorithm();
            Point[] points = new Point[] { new Point(10, 10), new Point(10, 20), new Point(20, 20)};
            List<(Point from, Point to)> edges = new List<(Point from, Point to)>
            {
                (points[0],points[1]),
                (points[1],points[2]),
                (points[2],points[0])
            };
            int row = 10;
            int to = 10;
            polygonFiller.Paint(edges, (int rowNr, int fromX, int toX) =>
            {
                Assert.AreEqual(rowNr, row);
                Assert.AreEqual(fromX, 10);
                Assert.AreEqual(toX, to);
                row++;
                to++;
            });
        }

        [TestMethod]
        public void PaintSquareTest()
        {
            PolygonFiller polygonFiller = new BucketSortScanLineFillAlgorithm();
            Point[] points = new Point[] { new Point(10, 10), new Point(10, 20), new Point(20, 20), new Point(20, 10) };
            List<(Point from, Point to)> edges = new List<(Point from, Point to)>
            {
                (points[0],points[1]),
                (points[1],points[2]),
                (points[2],points[3]),
                (points[3],points[0])
            };
            int row = 10;
            polygonFiller.Paint(edges, (int rowNr, int fromX, int toX) =>
             {
                 Assert.AreEqual(rowNr, row);
                 Assert.AreEqual(fromX, 10);
                 Assert.AreEqual(toX, 20);
                 row++;
             });
        }

        [TestMethod]
        public void PaintPentagonTest()
        {
            PolygonFiller polygonFiller = new BucketSortScanLineFillAlgorithm();
            Point[] points = new Point[] { new Point(10, 10), new Point(0, 20), new Point(0, 40), new Point(20, 40), new Point(20, 20) };
            List<(Point from, Point to)> edges = new List<(Point from, Point to)>
            {
                (points[0],points[1]),
                (points[1],points[2]),
                (points[2],points[3]),
                (points[3],points[4]),
                (points[4],points[0])
            };
            int row = 10;
            int from = 10;
            int to = 10;
            polygonFiller.Paint(edges, (int rowNr, int fromX, int toX) =>
            {
                Assert.AreEqual(rowNr, row);
                Assert.AreEqual(fromX, from);
                Assert.AreEqual(toX, to);
                row++;
                if (to < 20) to++;
                if (from > 0) from--;
            });
        }
    }
}
