using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace RingClip
{
    public partial class Form1 : Form
    {
        [DllImport("gdi32.dll", EntryPoint = "BitBlt")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BitBlt(
            [In()] System.IntPtr hdc, int x, int y, int cx, int cy,
            [In()] System.IntPtr hdcSrc, int x1, int y1, uint rop);

        [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
        public static extern System.IntPtr SelectObject(
            [In()] System.IntPtr hdc,
            [In()] System.IntPtr h);

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject(
            [In()] System.IntPtr ho);

        public Form1()
        {
            InitializeComponent();
        }

        private Dictionary<double, double> _tableX = new Dictionary<double, double>(360);
        private Dictionary<double, double> _tableY = new Dictionary<double, double>(360);

        private void Form1_Load(object sender, EventArgs e)
        {

            for (double angle = 0; angle <= 360;)
            {
                _tableX[angle] = Math.Cos((angle/180*Math.PI));
                _tableY[angle] = Math.Sin((angle/180*Math.PI));
                angle += 0.01f;
                angle = Math.Round(angle, 2);
            }

            for (double angle = -1; angle >= -360;)
            {
                _tableX[angle] = Math.Cos((angle/180*Math.PI));
                _tableY[angle] = Math.Sin((angle/180*Math.PI));
                angle -= 0.01f;
                angle = Math.Round(angle, 2);
            }

            this.Paint += Form1_Paint;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            this.Invalidate();
            base.OnMouseMove(e);
        }


        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            var pen = new Pen(Color.Red);
            var pen1 = new Pen(Color.Black);
            var brush = new SolidBrush(Color.Black);

            var fillPath = new GraphicsPath();
            const int count = 2;
            var angle = 0;
            const int spileAngle = 360/count;

            var point = PointToClient(Cursor.Position);

            if (point.X <= 0 || point.X > this.Width) return;

            var fillW = point.X/2;
            const int fillH = 100;
            const int sourceW = 72;
            const int sourceH = 72;
            var centerPoint = new Point(Width/2, Height/2);

            //            var outEllipse = new Rectangle(centerPoint.X, centerPoint.Y, point.X, point.Y);
            //            var insideEllipse = new Rectangle(centerPoint.X + 100, centerPoint.Y + 100, point.X - fillW, point.Y - fillH);
            //
            //                        e.Graphics.DrawEllipse(pen1, outEllipse);
            //                        e.Graphics.DrawEllipse(pen1, insideEllipse);
            var stopWatch = Stopwatch.StartNew();
            var resultPoints = new List<ColorPoint>();
            for (int index = 0; index <= count; index++)
            {
                var childPoints = GetPoints(angle, point.X, centerPoint, sourceW, sourceH, 100, 100, e.Graphics);
                resultPoints.AddRange(childPoints);

                angle += spileAngle;

            }

            var sourceBmp = new Bitmap(this.Width, this.Height, PixelFormat.Format24bppRgb);
            var unit = GraphicsUnit.Pixel;
            var sourceBound = sourceBmp.GetBounds(ref unit);

            if (fillW == 0) return;

            var sourceData = sourceBmp.LockBits(new Rectangle(new Point(0, 0), sourceBmp.Size), ImageLockMode.ReadWrite,
                PixelFormat.Format24bppRgb);

            unsafe
            {
                foreach (var p in resultPoints)
                {
                    if (sourceBound.Contains(p.Point))
                    {
                        var sourcePtr = (byte*) (sourceData.Scan0 + (p.Y*sourceData.Stride) + (p.X*3));

                        *sourcePtr = p.Color.B;
                        *(sourcePtr + 1) = p.Color.G;
                        *(sourcePtr + 2) = p.Color.R;
                    }
                }
            }

            sourceBmp.UnlockBits(sourceData);

            e.Graphics.DrawImage(sourceBmp, new Point(0, 0));

            stopWatch.Stop();
            Console.WriteLine(stopWatch.ElapsedMilliseconds);

        }

        private readonly Bitmap _surface = new Bitmap(Environment.CurrentDirectory + @"\Test.jpg");

        private IEnumerable<ColorPoint> GetPoints(double angle, int rMax, Point centerPoint, int fillW, int fillH,
            int originalW, int orginalH, Graphics graphics = null)
        {
            var rMin = rMax - fillW;
            var pen = new Pen(Color.Red);

            var realAngle = angle%360;

            var startAngle = realAngle;
            var endAngle = (realAngle + fillH)%360;
            var midAngle = (startAngle + Math.Abs((realAngle + fillH) - startAngle)/2)%360;

            var surfaceLock = new LockBitmap(_surface);
            surfaceLock.LockBits();

            var unit = GraphicsUnit.Pixel;
            var surfaceBound = _surface.GetBounds(ref unit);

            var points = new List<ColorPoint>();
            for (int h = 0; h < fillH; h++)
            {
                var cosAngle = _tableX[realAngle];
                var sinAngle = _tableY[realAngle];

                var startX = (int)(rMax * cosAngle);
                var startY = (int)(rMax * sinAngle);
                var endX = (int)(rMin * cosAngle) ;
                var endY = (int) (rMin*sinAngle);

                var distX = endX - startX;
                var distY = endY - startY;

                var step = (distX != 0 && distY != 0) ? distX/distY : 0;

                distX = Math.Abs(distX);

                var error = 0f;
                var x = startX;
                var y = startY;

//                graphics.DrawLine(pen, startX, startY, endX, endY);

                for (double i = 0.01; i < 0.99;)
                {
                    var newMidStartX = (int)(rMax * _tableX[Math.Round(realAngle + i, 2)]);
                    var newMidStartY = (int)(rMax * _tableY[Math.Round(realAngle + i, 2)]);
                    var newMidEndX = (int)(rMin * _tableX[Math.Round(realAngle + i, 2)]);
                    var newMidEndY = (int)(rMin * _tableY[Math.Round(realAngle + i, 2)]);
                    points.AddRange(GetLinePoints(newMidStartX, newMidStartY, newMidEndX, newMidEndY, surfaceLock));

                    i = Math.Round(i + 0.1, 2);
                }
                points.AddRange(GetLinePoints(startX, startY, endX, endY, surfaceLock));

                realAngle = angle % 360;
                angle++;
            }

//                for (int w = 0; w < fillW; w++)
//                {
//                    var radius = rMax - w;
//                    var newX = (int)(radius * cosAngle);
//                    var newY = (int)(radius * sinAngle);
//
//                    var oldPoint = new Point(w, h);
//
//                    if (surfaceBound.Contains(oldPoint))
//                    {
//                        var color = surfaceLock[oldPoint.X, oldPoint.Y];
//                        var newPoint = new Point(newX + centerPoint.X, newY + centerPoint.Y);
//
//                        points.Add(new ColorPoint(newPoint, color));
//
//                        //Interpolation
//
//                        for (double i = 0.01; i < 0.99; )
//                        {
//                            var newMidX = (int)(radius * _tableX[Math.Round(realAngle + i, 2)]);
//                            var newMidY = (int)(radius * _tableY[Math.Round(realAngle + i, 2)]);
//                            var newMidPoint = new Point(newMidX + centerPoint.X, newMidY + centerPoint.Y);
//                            points.Add(new ColorPoint(newMidPoint, color));
//
//                            i = Math.Round(i + 0.2, 2);
//                        }
//                    }
//
//                }

            surfaceLock.UnlockBits();

            return points;
        }

        public static IEnumerable<ColorPoint> GetLinePoints(int x0, int y0, int x1, int y1,LockBitmap surface)
        {
            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep) { Swap<int>(ref x0, ref y0); Swap<int>(ref x1, ref y1); }
            if (x0 > x1) { Swap<int>(ref x0, ref x1); Swap<int>(ref y0, ref y1); }
            int dX = (x1 - x0), dY = Math.Abs(y1 - y0), err = (dX / 2), ystep = (y0 < y1 ? 1 : -1), y = y0;

            var result = new List<ColorPoint>();
            for (int x = x0; x <= x1; ++x)
            {
                if (steep)
                {
                    var point = new Point(y, x);
                    if (surface.Bound.Contains(point))
                        result.Add(new ColorPoint(point, surface[y, y0]));
                }
                else
                {
                    var point = new Point(x, y);
                    if (surface.Bound.Contains(point))
                        result.Add(new ColorPoint(x, y, surface[x, y0]));
                }
                err = err - dY;
                if (err < 0) { y += ystep; err += dX; }
            }

            return result;
        }

        public static void Swap<T>(ref int a, ref int b)
        {
            var t = a;
            a = b;
            b = t;
        }
    }
}
