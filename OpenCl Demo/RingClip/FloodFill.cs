using System;
using System.Collections.Generic;
using System.Drawing;

namespace RingClip
{
    public static class FloodFill
    {

        public static void FillScanLine(Point location, Func<int, int, bool> checkPixel, Action<int, int> setPixel)
        {
            var points = new Stack<Point>();
            points.Push(location);

            while (points.Count > 0)
            {
                Point p = points.Pop();
                var y1 = p.Y;
                var x = p.X;

                while (y1 >= 0 && checkPixel(x, y1)) y1--;
                y1++;
                bool spanRight;
                bool spanLeft = spanRight = false;

                while (checkPixel(x, y1))
                {

                    setPixel(x, y1);
                    if (!spanLeft && checkPixel(x - 1, y1))
                    {
                        points.Push(new Point(x - 1, y1));
                        spanLeft = true;
                    }
                    else if (spanLeft && checkPixel(x - 1, y1))
                    {
                        spanLeft = false;
                    }
                    if (!spanRight && checkPixel(x + 1, y1))
                    {
                        points.Push(new Point(x + 1, y1));
                        spanRight = true;
                    }
                    else if (spanRight && checkPixel(x + 1, y1))
                    {
                        spanRight = false;
                    }
                    y1++;
                }
            }

        }

    }
}
