using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Cloo;
using Cloo.Bindings;

namespace RingClip
{
    public class OpenClInvoke
    {
        private static ComputePlatform _platform;
        private static ComputeDevice _device;
        private static ComputeContext _context;
        private static ComputeCommandQueue _commandQueue;
        private static string _sourceCode;
        private static ComputeKernel _compute;
        private static ComputeProgram _program;

        private static Bitmap _outputBmp;
        private static LockBitmap _srcLock;
        private static LockBitmap _outputLock;
        private static ComputeImage2D _climg;
        private static ComputeImage2D _climg2;
        private static byte[] _img;
        private static IntPtr _resultPtr;

        private static readonly ComputeImageFormat ComputeImageFormat = new ComputeImageFormat(ComputeImageChannelOrder.Rgba,
            ComputeImageChannelType.UnsignedInt8);

        public static void Init(ComputePlatform platform, ComputeDevice device)
        {
            _sourceCode = File.ReadAllText(Environment.CurrentDirectory + @"\OpenCLFunctions.cl");

            _platform = platform;

            _device = device;

            var properties = new ComputeContextPropertyList(_platform);

            _context = new ComputeContext(new[] { _device }, properties, null, IntPtr.Zero);
            _program = new ComputeProgram(_context, _sourceCode);

            try
            {
                _program.Build(null, null, null, IntPtr.Zero);
            }
            catch (Exception)
            {
                var log = _program.GetBuildLog(_device);
                Debugger.Break();
            }

            _compute = _program.CreateKernel("ComputeImage");

            // Create the command queue. This is used to control kernel execution and manage read/write/copy operations.
            _commandQueue = new ComputeCommandQueue(_context, _context.Devices[0], ComputeCommandQueueFlags.None);

        }

        public static void SetImageData(Bitmap srcImg, Bitmap outputImg)
        {
            _outputBmp = outputImg;
            _srcLock = new LockBitmap(srcImg);
            _outputLock = new LockBitmap(_outputBmp);

            _srcLock.LockBits();
            _outputLock.LockBits();

            var imgW = _srcLock.Width;
            var imgH = _srcLock.Height;
            var resultImgW = _outputLock.Width;
            var resultImgH = _outputLock.Height;

            var srcByteCount = imgW * imgH * 4;
            var resultByteCount = resultImgW * resultImgH * 4;

            _img = new byte[srcByteCount];

            _resultPtr = Marshal.AllocHGlobal(resultByteCount);

            #region Copy Image Data

            var rowSize = imgW * 4;
            for (int y = 0; y < imgH; y++)
            {
                for (int x = 0; x < imgW; x++)
                {
                    var index = x * 4 + y * rowSize;
                    var color = _srcLock[x, y];
                    _img[index] = color.B;
                    _img[index + 1] = color.G;
                    _img[index + 2] = color.R;
                    _img[index + 3] = color.A;
                }
            }

            #endregion

            unsafe
            {
                fixed (byte* imgPtr = _img)
                {
                    _climg = new ComputeImage2D(_context,
                        ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer,
                        ComputeImageFormat, imgW, imgH, 0, (IntPtr)imgPtr);
                }
            }

            CL10.RetainMemObject(_climg.Handle);

            _srcLock.UnlockBits();
            _outputLock.UnlockBits();
        }

        public static void Dispose()
        {
        }

        public static Image ComputeImage(
            ClPoint center,
            int radiusW,
            int radiusH,
            int fillW,
            int fillH,
            int count,
            float[] offsets,
            float[] offsetRadians,
            float accuracy)
        {
            var resultImgW = _outputBmp.Width;
            var resultImgH = _outputBmp.Height;

            unsafe
            {
                _climg2 = new ComputeImage2D(_context,
                    ComputeMemoryFlags.WriteOnly, ComputeImageFormat, resultImgW, resultImgH, 0, IntPtr.Zero);

                ComputeBuffer<float> arcPoints;
                ComputeBuffer<float> arcRadians;
                fixed (float* ptr = offsets)
                {
                    arcPoints = new ComputeBuffer<float>(_context,
                        ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.UseHostPointer, offsets.Length, (IntPtr)ptr);
                }

                fixed (float* ptr = offsetRadians)
                {
                    arcRadians = new ComputeBuffer<float>(_context,
                        ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.UseHostPointer, offsetRadians.Length, (IntPtr)ptr);
                }

                _compute.SetValueArgument(0, center.X);
                _compute.SetValueArgument(1, center.Y);
                _compute.SetValueArgument(2, radiusW);
                _compute.SetValueArgument(3, radiusH);
                _compute.SetValueArgument(4, fillW);
                _compute.SetValueArgument(5, fillH);
                _compute.SetValueArgument(6, count);
                _compute.SetMemoryArgument(7, _climg);
                _compute.SetMemoryArgument(8, _climg2);
                _compute.SetMemoryArgument(9, arcPoints);
                _compute.SetMemoryArgument(10, arcRadians);
                _compute.SetValueArgument(11, accuracy);

                count = count % 2 == 0 ? count / 2 : count;
                _commandQueue.Execute(_compute, null, new long[] { count }, null, null);

                _commandQueue.Read(_climg2, true,
                    new SysIntX3(0, 0, 0),
                    new SysIntX3(resultImgW, resultImgH, 1),
                    0, 0, _resultPtr, null);

                _commandQueue.Flush();
                arcPoints.Dispose();
                arcRadians.Dispose();
                _climg2.Dispose();
            }

//            GCHandle pinnedOutputArray = GCHandle.Alloc(_img2, GCHandleType.Pinned);
//            IntPtr outputBmpPointer = pinnedOutputArray.AddrOfPinnedObject();
            //Create a new bitmap with processed data and save it to a file.
            var outputBitmap = new Bitmap(resultImgW, resultImgH,
                  _outputLock.Stride, PixelFormat.Format32bppPArgb, _resultPtr);
            
//            pinnedOutputArray.Free();

            return outputBitmap;
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct ClPoint
    {
        [MarshalAs(UnmanagedType.R4), FieldOffset(0)]
        public float X;
        [MarshalAs(UnmanagedType.R4), FieldOffset(4)]
        public float Y;

        public ClPoint(float x, float y)
        {
            X = x;
            Y = y;
        }
        public static implicit operator Point(ClPoint d)
        {
            return new Point((int)d.X, (int)d.Y);
        }
    };
}
