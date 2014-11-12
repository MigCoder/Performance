using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace RingClip
{
    public partial class CircleImage : Form
    {
        private bool _init;
        private OpenClSetting _setting = new OpenClSetting();

        public CircleImage()
        {
            InitializeComponent();

            Config();

            this.Load += CircleImage_Load;
            this.KeyDown += Setting;
            Application.ApplicationExit += Application_ApplicationExit;
        }

        void Config()
        {
            _setting.ShowDialog();

            OpenClInvoke.Init(_setting.Platform, _setting.Device);

            _count = _setting.Count;
            _fillW = _setting.FillW;
            _fillH = _setting.FillH;
            _useGdi = _setting.UseGdi;
            this.Size = _setting.WindowSize;
            if (_init) CircleImage_SizeChanged(null, null);

            _init = true;
        }

        void Setting(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Config();
            }
        }

        void CircleImage_SizeChanged(object sender, EventArgs e)
        {
            OpenClInvoke.Dispose();
            var resultBmp = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppRgb);
            OpenClInvoke.SetImageData(_surface, resultBmp);
        }

        void Application_ApplicationExit(object sender, EventArgs e)
        {
            OpenClInvoke.Dispose();
        }

        void CircleImage_Load(object sender, EventArgs e)
        {
            this.Left = this.Top = 0;
            var resultBmp = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppRgb);
            if (_useGdi)
            {
                GdiInvoke.SetImageData(_surface, resultBmp);
            }
            else
            {
                OpenClInvoke.SetImageData(_surface, resultBmp);
            }

            this.SizeChanged += CircleImage_SizeChanged;
            this.Paint += CircleImage_Paint;
        }

        private int _count;
        private int _fillW;
        private int _fillH;
        private bool _useGdi;

        private void CircleImage_Paint(object sender, PaintEventArgs e)
        {

            var stopWatch = Stopwatch.StartNew();

            var point = PointToClient(Cursor.Position);

            if (point.X <= 0 || point.X > this.Width) return;
            if (point.Y <= 0 || point.Y > this.Height) return;

            var radiusW = point.X/2;
            var radiusH = point.Y/2;

            var centerPoint = new ClPoint(radiusW, radiusH);

            const double quanlity = 0.5;
            var scaleRadian = Math.Atan(1.0);
            var equalRadian = Math.Sin(scaleRadian);
            var y = equalRadian*radiusW;
            var x = equalRadian*radiusW;
            var accuracy = (float) (Math.Atan2(y + quanlity, x) - scaleRadian);

            var arcLength = GetArcLength(centerPoint, radiusW, radiusH, 0.0, 0.0, accuracy);

            var offsetCount = _count + 1;
            var offsets = new float[offsetCount];
            var equalLength = arcLength/_count;
            for (int index = 0; index < offsetCount; index++)
            {
                offsets[index] = (float) equalLength*index;
            }

            var arcPoints = GetArcEqualPoint(centerPoint, radiusW, radiusH, 0.0, 0.0, accuracy, offsets);
            Image bmp;
            if (_useGdi)
            {
                bmp = GdiInvoke.ComputeImage(centerPoint, radiusW, radiusH, _fillW, _fillH, _count, offsets, arcPoints,
                    accuracy);
            }
            else
            {
                bmp = OpenClInvoke.ComputeImage(centerPoint, radiusW, radiusH, _fillW, _fillH, _count, offsets,
                    arcPoints, accuracy);
            }

            this.BackgroundImage = bmp;

            stopWatch.Stop();
            this.Text = string.Format("{0}ms", stopWatch.ElapsedMilliseconds);
        }

        private float[] GetArcEqualPoint(Point centerPoint, double majorRadius, double minorRadius, double startAngle, double endAngle, double accuracy, float[] offsets)
        {
            int index = 0;
            double startRadian = (startAngle * Math.PI) / 180.0;
            double endRadian = (endAngle * Math.PI) / 180.0;
            double betweenRadian = (endRadian - startRadian) + ((startAngle != endAngle) ? 0.0 : (Math.PI * 2));
            double axisX = majorRadius * Math.Cos(startRadian);
            double axisY = minorRadius * Math.Sin(startRadian);
            axisX += centerPoint.X;
            axisY = centerPoint.Y - axisY;

            double arcLength = 0.0;
            var realEndRadian = startRadian + betweenRadian;

            var result = new float[offsets.Length];
            for (double currentRadian = startRadian; currentRadian <= realEndRadian; currentRadian += accuracy)
            {
                var currentOffset = offsets[index];

                double nextAxisX = majorRadius * Math.Cos(currentRadian);
                double nextAxisY = minorRadius * Math.Sin(currentRadian);
                nextAxisX += centerPoint.X;
                nextAxisY = -nextAxisY + centerPoint.Y;
                arcLength += Math.Sqrt((Math.Abs(nextAxisX - axisX) * Math.Abs(nextAxisX - axisX)) + (Math.Abs(nextAxisY - axisY) * Math.Abs(nextAxisY - axisY)));
                axisX = nextAxisX;
                axisY = nextAxisY;

                if (currentOffset - arcLength < 0 && index < (result.Length - 1))
                {
                    result[index] = (float)currentRadian;
                    index++;
                }
            }

            return result;
        }

        private double GetArcLength(Point centerPoint, double majorRadius, double minorRadius, double startAngle, double endAngle, double accuracy)
        {
            double startRadian = (startAngle * Math.PI) / 180.0;
            double endRadian = (endAngle * Math.PI) / 180.0;
            double betweenRadian = (endRadian - startRadian) + ((startAngle != endAngle) ? 0.0 : (Math.PI * 2));
            double axisX = majorRadius * Math.Cos(startRadian);
            double axisY = minorRadius * Math.Sin(startRadian);
            axisX += centerPoint.X;
            axisY = centerPoint.Y - axisY;

            double arcLength = 0.0;
            var realEndRadian = startRadian + betweenRadian;

            for (double currentRadian = startRadian; currentRadian <= realEndRadian; currentRadian += accuracy)
            {
                double nextAxisX = majorRadius * Math.Cos(currentRadian);
                double nextAxisY = minorRadius * Math.Sin(currentRadian);
                nextAxisX += centerPoint.X;
                nextAxisY = -nextAxisY + centerPoint.Y;
                arcLength += Math.Sqrt((Math.Abs(nextAxisX - axisX) * Math.Abs(nextAxisX - axisX)) + (Math.Abs(nextAxisY - axisY) * Math.Abs(nextAxisY - axisY)));
                axisX = nextAxisX;
                axisY = nextAxisY;
            }

            return arcLength;
        }


        private readonly Bitmap _surface = new Bitmap(Environment.CurrentDirectory + @"\Test.jpg");

        private void CircleImage_MouseMove(object sender, MouseEventArgs e)
        {
            this.Invalidate();
        }
    }
}
