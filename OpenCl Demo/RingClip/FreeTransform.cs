using System;
using System.Collections.Generic;
using System.Drawing;

namespace RingClip
{
    public class FreeTransform
    {
        PointF[] vertex = new PointF[4];
        Vector AB, BC, CD, DA;
        Rectangle rect = new Rectangle();
        ColorPoint[] srcCB;
        int srcW = 0;
        int srcH = 0;

        public ColorPoint[] Bitmap
        {
            set
            {
                try
                {
                    srcCB = value;
                    srcH = 1;
                    srcW = value.Length;
                }
                catch
                {
                    srcW = 0; srcH = 0;
                }
            }
            get
            {
                return getTransformedBitmap();
            }
        }

        public Point ImageLocation
        {
            set { rect.Location = value; }
            get { return rect.Location; }
        }

        bool isBilinear = false;
        public bool IsBilinearInterpolation
        {
            set { isBilinear = value; }
            get { return isBilinear; }
        }

        public int ImageWidth
        {
            get { return rect.Width; }
        }

        public int ImageHeight
        {
            get { return rect.Height; }
        }

        public PointF VertexLeftTop
        {
            set { vertex[0] = value; setVertex(); }
            get { return vertex[0]; }
        }

        public PointF VertexTopRight
        {
            set { vertex[1] = value; setVertex(); }
            get { return vertex[1]; }
        }

        public PointF VertexRightBottom
        {
            set { vertex[2] = value; setVertex(); }
            get { return vertex[2]; }
        }

        public PointF VertexBottomLeft
        {
            set { vertex[3] = value; setVertex(); }
            get { return vertex[3]; }
        }

        public PointF[] FourCorners
        {
            set { vertex = value; setVertex(); }
            get { return vertex; }
        }

        private void setVertex()
        {
            float xmin = float.MaxValue;
            float ymin = float.MaxValue;
            float xmax = float.MinValue;
            float ymax = float.MinValue;

            for (int i = 0; i < 4; i++)
            {
                xmax = Math.Max(xmax, vertex[i].X);
                ymax = Math.Max(ymax, vertex[i].Y);
                xmin = Math.Min(xmin, vertex[i].X);
                ymin = Math.Min(ymin, vertex[i].Y);
            }

            rect = new Rectangle((int)xmin, (int)ymin, (int)(xmax - xmin), (int)(ymax - ymin));

            AB = new Vector(vertex[0], vertex[1]);
            BC = new Vector(vertex[1], vertex[2]);
            CD = new Vector(vertex[2], vertex[3]);
            DA = new Vector(vertex[3], vertex[0]);

            // get unit vector
            AB /= AB.Magnitude;
            BC /= BC.Magnitude;
            CD /= CD.Magnitude;
            DA /= DA.Magnitude;

            if (double.IsNaN(AB.Magnitude))
            {
                AB.X = 1;
                AB.Y = 1;
            }
            if (double.IsNaN(BC.Magnitude))
            {
                BC.X = 1;
                BC.Y = 1;
            }
            if (double.IsNaN(CD.Magnitude))
            {
                CD.X = 1;
                CD.Y = 1;
            }
            if (double.IsNaN(DA.Magnitude))
            {
                DA.X = 1;
                DA.Y = 1;
            }

        }

        private bool isOnPlaneABCD(PointF pt) //  including point on border
        {
            if (!Vector.IsCCW(pt, vertex[0], vertex[1]))
            {
                if (!Vector.IsCCW(pt, vertex[1], vertex[2]))
                {
                    if (!Vector.IsCCW(pt, vertex[2], vertex[3]))
                    {
                        if (!Vector.IsCCW(pt, vertex[3], vertex[0]))
                            return true;
                    }
                }
            }
            return false;
        }

        private ColorPoint[] getTransformedBitmap()
        {
            if (srcH == 0 || srcW == 0) return null;

            // Create a plain image, same size of source bitmap.
            ImageData destCB = new ImageData();
            destCB.A = new byte[rect.Width, rect.Height];
            destCB.B = new byte[rect.Width, rect.Height];
            destCB.G = new byte[rect.Width, rect.Height];
            destCB.R = new byte[rect.Width, rect.Height];


            PointF ptInPlane = new PointF();
            int x1, x2, y1, y2;
            double distTop, distRight, distBottom, distLeft;
            float dx1, dx2, dy1, dy2, dx1y1, dx1y2, dx2y1, dx2y2, nbyte;

            var result = new List<ColorPoint>();

            // Traversal all pixel at source btimap.
            for (int y = 0; y < rect.Height; y++)
            {
                for (int x = 0; x < rect.Width; x++)
                {
                    // Get pixel from plain image location(x, y)
                    Point srcPt = new Point(x, y);
                    srcPt.Offset(this.rect.Location);

                    // Check pixel is it in the new rectangle.
                    if (isOnPlaneABCD(srcPt))
                    {
                        // Get distance point to new rectangle.
                        distTop = Math.Abs((new Vector(vertex[0], srcPt)).CrossProduct(AB));
                        distRight = Math.Abs((new Vector(vertex[1], srcPt)).CrossProduct(BC));
                        distBottom = Math.Abs((new Vector(vertex[2], srcPt)).CrossProduct(CD));
                        distLeft = Math.Abs((new Vector(vertex[3], srcPt)).CrossProduct(DA));

                        // Get pixel map pixel at source bitmap.
                        ptInPlane.X = (float)(srcW * (distLeft / (distLeft + distRight)));
                        ptInPlane.Y = (float)(srcH * (distTop / (distTop + distBottom)));

                        x1 = (int)ptInPlane.X;
                        y1 = (int)ptInPlane.Y;

                        // If map pixel real in source btimap.
                        if (x1 >= 0 && x1 < srcW && y1 >= 0 && y1 < srcH)
                        {
                            result.Add(new ColorPoint(x + rect.Location.X, y + rect.Location.Y, srcCB[x1].Color));
                        }
                    }
                }
            }
            return result.ToArray();
        }


        public IEnumerable<ColorPoint> GetTransformedBitmap(RectangleF[] rects, Point centerPoint)
        {
            if (srcH == 0 || srcW == 0)
            {
                yield break;
            }

            var ptInPlane = new PointF();

            foreach (var childRect in rects)
            {
                // Traversal all pixel at source btimap.
                var location = new Point((int)childRect.Location.X, (int)childRect.Location.Y);
                for (int y = 0; y < childRect.Height; y++)
                {
                    for (int x = 0; x < childRect.Width; x++)
                    {
                        // Get pixel from plain image location(x, y)
                        var srcPt = new Point(x, y);
                        srcPt.Offset(location);

                        // Get distance point to new rectangle.
                        double distTop = Math.Abs((new Vector(vertex[0], srcPt)).CrossProduct(AB));
                        double distRight = Math.Abs((new Vector(vertex[1], srcPt)).CrossProduct(BC));
                        double distBottom = Math.Abs((new Vector(vertex[2], srcPt)).CrossProduct(CD));
                        double distLeft = Math.Abs((new Vector(vertex[3], srcPt)).CrossProduct(DA));

                        if (double.IsNaN(distTop)) distTop = 0;
                        if (double.IsNaN(distRight)) distRight = 0;
                        if (double.IsNaN(distBottom)) distBottom = 0;
                        if (double.IsNaN(distLeft)) distLeft = 0;

                        // Get pixel map pixel at source bitmap.
                        ptInPlane.X = (float)(srcW * (distLeft / (distLeft + distRight)));
                        ptInPlane.Y = (float)(srcH * (distTop / (distTop + distBottom)));

                        int x1 = (int)ptInPlane.X;
                        int y1 = (int)ptInPlane.Y;

                        // If map pixel real in source bitmap.
                        if (x1 >= 0 && x1 < srcW && y1 >= 0 && y1 <= srcH)
                        {
                            var newPoint = new Point(x + location.X, y + location.Y);

                            newPoint.Offset(centerPoint);
                            yield return new ColorPoint(newPoint, srcCB[x1].Color);
                        }
                    }
                }
            }
        }
    }
}