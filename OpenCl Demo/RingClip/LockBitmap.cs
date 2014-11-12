using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RingClip
{
    public class LockBitmap
    {
        public Bitmap Source = null;
        public BitmapData BitmapData = null;

        public int Stride { get; private set; }
        public int PixelCount { get; private set; }
        public int DepthCount { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Rectangle Bound { get; set; }

        public LockBitmap(Bitmap source)
        {
            this.Source = source;
            this.Bound = new Rectangle(0, 0, source.Width, source.Height);
        }

        /// <summary>
        /// Lock bitmap data
        /// </summary>
        public void LockBits()
        {
            try
            {
                // Get width and height of bitmap
                Width = Source.Width;
                Height = Source.Height;

                // Create rectangle to lock
                var rect = new Rectangle(0, 0, Width, Height);


                DepthCount = Image.GetPixelFormatSize(Source.PixelFormat);
                PixelCount = DepthCount / 8;

                // Lock bitmap and return bitmap data
                BitmapData = Source.LockBits(rect, ImageLockMode.ReadWrite,
                                             Source.PixelFormat);
                Stride = BitmapData.Stride;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Unlock bitmap data
        /// </summary>
        public void UnlockBits()
        {
            try
            {
                // Unlock bitmap data
                Source.UnlockBits(BitmapData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Color GetPixel(int x, int y)
        {
            unsafe
            {
                Color clr = Color.Empty;

                // Get start index of the specified pixel
                var ptr = (byte*)(BitmapData.Scan0 + ((y * Stride) + (x * PixelCount)));

                if (DepthCount == 32) // For 32 bpp get Red, Green, Blue and Alpha
                {
                    byte b = *ptr;
                    byte g = *(ptr + 1);
                    byte r = *(ptr + 2);
                    byte a = *(ptr + 3);
                    clr = Color.FromArgb(a, r, g, b);
                }
                if (DepthCount == 24) // For 24 bpp get Red, Green and Blue
                {
                    byte b = *ptr;
                    byte g = *(ptr + 1);
                    byte r = *(ptr + 2);
                    clr = Color.FromArgb(r, g, b);
                }
                if (DepthCount == 8)
                // For 8 bpp get color value (Red, Green and Blue values are the same)
                {
                    byte c = *ptr;
                    clr = Color.FromArgb(c, c, c);
                }
                return clr;
            }
        }

        public void SetPixel(int x, int y, Color color)
        {
            unsafe
            {
                // Get start index of the specified pixel
                var ptr = (byte*)(BitmapData.Scan0 + ((y * Stride) + (x * PixelCount)));

                if (DepthCount == 32)
                {
                    *ptr = color.B;
                    *(ptr + 1) = color.G;
                    *(ptr + 2) = color.R;
                    *(ptr + 3) = color.A;
                }
                if (DepthCount == 24)
                {
                    *ptr = color.B;
                    *(ptr + 1) = color.G;
                    *(ptr + 2) = color.R;
                }
                if (DepthCount == 8)
                {
                    *ptr = color.B;
                }
            }
        }

        public void SetPixel(int x, int y, byte r, byte g, byte b, byte a)
        {
            unsafe
            {
                // Get start index of the specified pixel
                var ptr = (byte*)(BitmapData.Scan0 + ((y * Stride) + (x * PixelCount)));

                if (DepthCount == 32)
                {
                    *ptr = b;
                    *(ptr + 1) = g;
                    *(ptr + 2) = r;
                    *(ptr + 3) = a;
                }
                if (DepthCount == 24)
                {
                    *ptr = b;
                    *(ptr + 1) = g;
                    *(ptr + 2) = r;
                }
                if (DepthCount == 8)
                {
                    *ptr = b;
                }
            }
        }

        public Color this[int x, int y]
        {
            get { return GetPixel(x, y); }
            set { SetPixel(x, y, value); }
        }

        public void Clear(Color color)
        {
            for (int h = 0; h < this.Height; h++)
            {
                for (int w = 0; w < this.Width; w++)
                {
                    this[w, h] = color;
                }
            }
        }
    }
}
