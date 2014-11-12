using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace RingClip
{
    public static class Liner
    {
        public static int ipart(double x)
        {
            return (int)x;
        }

        public static int round(double x)
        {
            return ipart(x + 0.5);
        }

        public static double fpart(double x)
        {
            return x - Math.Round(x);
        }

        public static double rfpart(double x)
        {
            return 1 - fpart(x);
        }

        public static void swap<T>(ref T a, ref T b)
        {
            var t = a;
            a = b;
            b = t;
        }

        public static IEnumerable<Point> GetLinePoints(Point p1, Point p2)
        {
            return GetLinePoints(p1.X, p1.Y, p2.X, p2.Y);
        }

        public static IEnumerable<Point> GetLinePoints(int x1, int y1, int x2, int y2)
        {
            var result = new List<Point>();

            var steep = false;
            double dx = x2 - x1;
            double dy = y2 - y1;
            if (Math.Abs(dx) < Math.Abs(dy))
            {
                steep = true;
                swap(ref x1, ref y1);
                swap(ref x2, ref y2);
                swap(ref dx, ref dy);
            }
            if (x2 < x1)
            {

                swap(ref x1, ref x2);
                swap(ref y1, ref y2);
            }
            double gradient = dy / dx;

            // handle first endpoint
            var xend = round(x1);
            var yend = y1 + gradient * (xend - x1);
            var xpxl1 = xend;
            // this will be used in the main loop
            var ypxl1 = ipart(yend);
            if (steep)
            {
                result.Add(new Point(ypxl1, xpxl1));
                result.Add(new Point(ypxl1 + 1, xpxl1));
            }
            else
            {
                result.Add(new Point(xpxl1, ypxl1));
                result.Add(new Point(xpxl1, ypxl1 + 1));
            }
            double intery = yend + gradient;
            // first y-intersection for the main loop

            // handle second endpoint
            xend = round(x2);
            yend = y2 + gradient * (xend - x2);
            var xpxl2 = xend; // this will be used in the main loop
            var ypxl2 = ipart(yend);
            if (steep)
            {
                result.Add(new Point(ypxl2, xpxl2));
                result.Add(new Point(ypxl2 + 1, xpxl2));
            }
            else
            {
                result.Add(new Point(xpxl2, ypxl2));
                result.Add(new Point(xpxl2, ypxl2 + 1));
            }

            // main loop
            for (var x = xpxl1 + 1; x < xpxl2 - 1; x++)
            {
                if (steep)
                {
                    result.Add(new Point(ipart(intery), x));
                    result.Add(new Point(ipart(intery) + 1, x));
                }
                else
                {
                    result.Add(new Point(x, ipart(intery)));
                    result.Add(new Point(x, ipart(intery) + 1));
                }
                intery = intery + gradient;
            }

            return result;
        }

        public static IEnumerable<ColorPoint> GetLinePoints(Point p1, Point p2, LockBitmap surface, int orginY)
        {
            return GetLinePoints(p1.X, p1.Y, p2.X, p2.Y, surface, orginY);
        }

        public static IEnumerable<ColorPoint> GetLinePoints(int x1, int y1, int x2, int y2, LockBitmap surface, int orginY)
        {
            var result = new List<ColorPoint>();

            var steep = false;
            double dx = x2 - x1;
            double dy = y2 - y1;
            if (Math.Abs(dx) < Math.Abs(dy))
            {
                steep = true;
                swap(ref x1, ref y1);
                swap(ref x2, ref y2);
                swap(ref dx, ref dy);
            }
            if (x2 < x1)
            {

                swap(ref x1, ref x2);
                swap(ref y1, ref y2);
            }
            double gradient = dy / dx;

            // handle first endpoint
            var xend = round(x1);
            var yend = y1 + gradient * (xend - x1);
            var xpxl1 = xend;
            // this will be used in the main loop
            var ypxl1 = ipart(yend);
            if (steep)
            {
                result.Add(new ColorPoint(new Point(ypxl1, xpxl1), surface[0, orginY]));
                result.Add(new ColorPoint(new Point(ypxl1 + 1, xpxl1), surface[0, orginY]));
            }
            else
            {
                result.Add(new ColorPoint(new Point(xpxl1, ypxl1), surface[0, orginY]));
                result.Add(new ColorPoint(new Point(xpxl1, ypxl1 + 1), surface[0, orginY]));
            }
            double intery = yend + gradient;
            // first y-intersection for the main loop

            // handle second endpoint
            xend = round(x2);
            yend = y2 + gradient * (xend - x2);
            var xpxl2 = xend; // this will be used in the main loop
            var ypxl2 = ipart(yend);
            if (steep)
            {
                result.Add(new ColorPoint(new Point(ypxl2, xpxl2), surface[0, orginY]));
                result.Add(new ColorPoint(new Point(ypxl2 + 1, xpxl2), surface[0, orginY]));
            }
            else
            {
                result.Add(new ColorPoint(new Point(xpxl2, ypxl2), surface[0, orginY]));
                result.Add(new ColorPoint(new Point(xpxl2, ypxl2 + 1), surface[0, orginY]));
            }

            // main loop
            for (var x = xpxl1 + 1; x < xpxl2 - 1; x++)
            {
                if (steep)
                {
                    result.Add(new ColorPoint(new Point(ipart(intery), x), surface[x - x1, orginY]));
                    result.Add(new ColorPoint(new Point(ipart(intery) + 1, x), surface[x - x1, orginY]));
                }
                else
                {
                    result.Add(new ColorPoint(new Point(x, ipart(intery)), surface[x - x1, orginY]));
                    result.Add(new ColorPoint(new Point(x, ipart(intery) + 1), surface[x - x1, orginY]));
                }
                intery = intery + gradient;
            }

            return result;
        }
    }
}
