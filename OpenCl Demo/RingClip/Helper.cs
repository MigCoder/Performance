using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace RingClip
{
    public static class Helper
    {
        public static Point Add(this Point point, int x, int y)
        {
            point.X += x;
            point.Y += y;

            return point;
        }

        public static Point Add(this Point point, Point target)
        {
            point.X += target.X;
            point.Y += target.Y;

            return point;
        }

        public static PointF Add(this PointF point, Point target)
        {
            point.X += target.X;
            point.Y += target.Y;

            return point;
        }

        public static PointF Add(this PointF point, PointF target)
        {
            point.X += target.X;
            point.Y += target.Y;

            return point;
        }

        public static void Add(this IEnumerable<Point> points, int x, int y)
        {
            foreach (var point in points)
            {
                point.Add(x, y);
            }
        }

        public static void Add(this IEnumerable<Point> points, Point target)
        {
            foreach (var point in points)
            {
                point.Add(target);
            }
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> array, int size)
        {
            var count = array.Count();
            for (var i = 0; i < (float)count / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }
    }
}
