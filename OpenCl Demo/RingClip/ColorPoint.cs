using System.Diagnostics;
using System.Drawing;

namespace RingClip
{
    public class ColorPoint
    {
        public Point Point { get; set; }
        public Color Color { get; set; }

        public int X { get { return Point.X; } }
        public int Y { get { return Point.Y; } }

        public ColorPoint(int x, int y, Color color)
        {
            Point = new Point(x, y);
            Color = color;
        }

        public ColorPoint(Point point, Color color)
            : this(point.X, point.Y, color)
        {
        }

        public override string ToString()
        {
            return Point + "" + Color;
        }
    }
}
