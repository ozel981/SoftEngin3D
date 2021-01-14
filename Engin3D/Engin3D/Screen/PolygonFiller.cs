using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engin3D.Screen
{

    public delegate void FillRow(int rowNr, int fromX, int toX);
    public interface PolygonFiller
    {
       void Paint(List<(Point from, Point to)> edges, FillRow drawRow);
    }

    public class CrossPoint
    {
        public int MaxY { get; set; }
        public double X { get; set; }
        public double M { get; set; }
        public CrossPoint Next { get; set; }

        public CrossPoint((Point from, Point to) edge)
        {
            MaxY = Math.Max(edge.from.Y,edge.to.Y);
            X = edge.from.Y != MaxY ? edge.from.X : edge.to.X;
            double SX = edge.from.Y != MaxY ? edge.to.X : edge.from.X;
            double ds_x = SX - X;
            double ds_y = MaxY - Math.Min(edge.from.Y, edge.to.Y); ;
            if (ds_y == 0) M = 0;
            else M = ds_x / ds_y;
            Next = null;
        }
    }

    public class BucketSortScanLineFillAlgorithm : PolygonFiller
    {
        private Dictionary<int, CrossPoint> crossPoints;

        private void Sort(ref CrossPoint list)
        {
            if (list != null)
            {
                bool sorted = false;
                while (!sorted)
                {
                    sorted = true;
                    CrossPoint p = list.Next;
                    CrossPoint prev = list;
                    CrossPoint prevprev = null;
                    while (p != null && prev.X <= p.X)
                    {
                        prevprev = prev;
                        prev = p;
                        p = p.Next;
                    }
                    if (p != null)
                    {
                        sorted = false;
                        if (prevprev == null)
                        {
                            prev.Next = p.Next;
                            p.Next = prev;
                            list = p;
                        }
                        else
                        {
                            prevprev.Next = p;
                            while (p.Next != null && p.Next.X < prev.X)
                            {
                                p = p.Next;
                            }
                            if (p.Next == null)
                            {
                                p.Next = prev;
                                prev.Next = null;
                            }
                            else
                            {
                                prev.Next = p.Next;
                                p.Next = prev;
                            }

                        }

                    }
                }
            }
        }
        private void MergeAndSort(ref CrossPoint list, CrossPoint newNode)
        {
            Sort(ref list);
            CrossPoint iterNew = newNode;
            while (iterNew != null)
            {
                CrossPoint holder = iterNew.Next;
                CrossPoint iterList = list;
                if (iterList == null || iterList.X > iterNew.X)
                {
                    iterNew.Next = list;
                    list = iterNew;
                }
                else
                {
                    while (iterList.Next != null && iterList.Next.X < iterNew.X)
                    {
                        iterList = iterList.Next;
                    }
                    if (iterList.Next == null)
                    {
                        iterList.Next = iterNew;
                        iterNew.Next = null;
                    }
                    else
                    {
                        iterNew.Next = iterList.Next;
                        iterList.Next = iterNew;
                    }
                }
                iterNew = holder;
            }
        }

        public void Paint(List<(Point from, Point to)> edges, FillRow drawRow)
        {
            crossPoints = new Dictionary<int, CrossPoint>();
            int minHeight = int.MaxValue;
            int maxHeight = int.MinValue;
            foreach (var edge in edges)
            {
                if (edge.from.Y - edge.to.Y == 0) continue;
                if (edge.from.Y > maxHeight) maxHeight = edge.from.Y;
                if (edge.to.Y > maxHeight) maxHeight = edge.to.Y;
                if (edge.from.Y < minHeight) minHeight = edge.from.Y;
                if (edge.to.Y < minHeight) minHeight = edge.to.Y;
                int key = Math.Min(edge.from.Y,edge.to.Y);
                CrossPoint value = new CrossPoint(edge);
                if(crossPoints.ContainsKey(key))
                {
                    value.Next = crossPoints[key];
                    crossPoints[key] = value;
                }
                else
                {
                    crossPoints.Add(key, value);
                }
            }
            CrossPoint list = null;
            for (int i = minHeight; i <= maxHeight; i++)
            {
                if (crossPoints.ContainsKey(i)) MergeAndSort(ref list, crossPoints[i]);
                else Sort(ref list);
                CrossPoint p = list;
                CrossPoint prev = list;
                while (p != null)
                {
                    if (prev == p)
                    {
                        prev = p;
                        p = p.Next;
                    }
                    else
                    {
                        drawRow(i, (int)prev.X, (int)p.X);
                        prev = p = p.Next;
                    }
                }

                p = list;
                prev = list;
                while (p != null)
                {
                    p.X = (p.X + p.M);

                    //
                    if (p.MaxY == i + 1)
                    {
                        if (p == list)
                        {
                            list = p.Next;
                        }
                        else
                        {
                            prev.Next = p.Next;
                        }
                    }
                    prev = p;
                    p = p.Next;
                }
            }
        }
    }

}
