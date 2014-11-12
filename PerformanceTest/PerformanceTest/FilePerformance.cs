using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace PerformanceTest
{
    public class FilePerformance
    {
        [DllImport("kernel32", SetLastError = true)]
        private static extern SafeFileHandle CreateFile(
            string FileName, // file name
            uint DesiredAccess, // access mode
            uint ShareMode, // share mode
            IntPtr SecurityAttributes, // Security Attr
            uint CreationDisposition, // how to create
            uint FlagsAndAttributes, // file attributes
            IntPtr hTemplate // template file  
            );

        private const uint FILE_FLAG_NO_BUFFERING = 0x20000000;

        private static long _fileSize = 1024 * 1024 * 1024;
        private static string _path = Environment.CurrentDirectory + @"\Test.dat";
        private static string _outpath = Environment.CurrentDirectory + @"\Copy.dat";
        
        public static void Run(int testCount)
        {
            if(!File.Exists(_path))
            {
                var datas = new byte[(int) _fileSize];
                var fs = File.Create(_path, (int)_fileSize);
                fs.Write(datas, 0, datas.Length);
                fs.Dispose();
            }

            var perCount = 65536 * 1024;
//            var perCount = 3 * 1024;
            var bytes = new byte[perCount];

            var counter = new double[testCount];

            var watch = Stopwatch.StartNew();

            Console.WriteLine("A:缓存大小不符合 CPU 缓存块");
            //            Console.WriteLine("B:缓存大小符合 CPU 缓存块");
            Console.ForegroundColor = ConsoleColor.Green;

            for (int index = 0; index < testCount; index++)
            {
                watch.Restart();

                var fileStream = new FileStream(_path, FileMode.Open, FileAccess.ReadWrite,
                    FileShare.None, perCount, FileOptions.SequentialScan);
                for (int ptr = 0; ptr < _fileSize; ptr += perCount)
                {
                    fileStream.Write(bytes, 0, perCount);
                    fileStream.Flush();
                }
                fileStream.Dispose();

                watch.Stop();
                counter[index] = watch.Elapsed.TotalMilliseconds;
                Console.WriteLine("{0}ms", watch.Elapsed.TotalMilliseconds);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("平均： {0}ms", counter.Average());

        }

        public static void RunUnBufferIo(int testCount)
        {
            if (!File.Exists(_path))
            {
                var datas = new byte[(int)_fileSize];
                var fs = File.Create(_path, (int)_fileSize);
                fs.Write(datas, 0, datas.Length);
                fs.Dispose();
            }

            var perCount = 4096*1024;
            var bytes = new byte[perCount];

            var handle = CreateFile(_path, (uint)FileAccess.ReadWrite, (uint)FileShare.None, IntPtr.Zero, (uint)FileMode.Open, FILE_FLAG_NO_BUFFERING,
                IntPtr.Zero);
            var fileStream = new FileStream(handle, FileAccess.ReadWrite, perCount);

            var counter = new double[testCount];

            var watch = Stopwatch.StartNew();

            Console.WriteLine("A:无缓冲 I/O");
            //            Console.WriteLine("B:缓存大小符合 CPU 缓存块");
            Console.ForegroundColor = ConsoleColor.Green;

            for (int index = 0; index < testCount; index++)
            {
                watch.Restart();
                for (int ptr = 0; ptr < _fileSize; ptr += perCount)
                {
                    fileStream.Write(bytes, 0, perCount);
                }

//                File.Copy(_path,_outpath,true);
//                AsyncUnbuffCopy.AsyncCopyFileUnbuffered(_path, _outpath, true, false, false, 32, true);

                watch.Stop();
                counter[index] = watch.Elapsed.TotalMilliseconds;
                Console.WriteLine("{0}ms", watch.Elapsed.TotalMilliseconds);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("平均： {0}ms", counter.Average());

            fileStream.Dispose();
        }

        public static void RunSetLength(int testCount)
        {
            var perCount = 64 * 1024;

            var bytes = new byte[perCount];

            var counter = new double[testCount];

            var fs = File.Create(_path, (int)_fileSize);
            fs.SetLength(_fileSize);

            var watch = Stopwatch.StartNew();
            Console.WriteLine("创建文件：调用 SetLength");
            Console.ForegroundColor = ConsoleColor.Green;
            for (int index = 0; index < testCount; index++)
            {
                watch.Restart();

                for (int ptr = 0; ptr < _fileSize; ptr += perCount)
                {
                    fs.Write(bytes, 0, perCount);
                }

                watch.Stop();
                counter[index] = watch.Elapsed.TotalMilliseconds;
                Console.WriteLine("{0}ms", watch.Elapsed.TotalMilliseconds);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("平均： {0}ms", counter.Average());

            fs.Close();
        }

        public static void RunUnSetLength(int testCount)
        {
            var perCount = 64 * 1024;

            var bytes = new byte[perCount];

            var counter = new double[testCount];

            var fs = File.Create(_path, (int)_fileSize);

            var watch = Stopwatch.StartNew();

            Console.WriteLine("创建文件：不调用 SetLength");
            Console.ForegroundColor = ConsoleColor.Green;

            for (int index = 0; index < testCount; index++)
            {
                watch.Restart();

                for (int ptr = 0; ptr < _fileSize; ptr += perCount)
                {
                    fs.Write(bytes, 0, perCount);
                }


                watch.Stop();
                counter[index] = watch.Elapsed.TotalMilliseconds;
                Console.WriteLine("{0}ms", watch.Elapsed.TotalMilliseconds);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("平均： {0}ms", counter.Average());

            fs.Close();
        }
    }
}
