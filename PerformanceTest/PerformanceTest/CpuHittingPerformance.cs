using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace PerformanceTest
{
    public class CpuHittingPerformance
    {

        public static void Run(int testCount)
        {
            const int count = 4000;
            var arr = new int[count,count];
            var counter = new double[testCount];

            var watch = Stopwatch.StartNew();

            Console.WriteLine("按列遍历，数据量 1000 万");
//                        Console.WriteLine("按行遍历，数据量 1000 万");
            Console.ForegroundColor = ConsoleColor.Green;

            for (int index = 0; index < testCount; index++)
            {
                watch.Restart();

                for (int y = 0; y < count; y++)
                {
                    for (int x = 0; x < count; x++)
                    {
                        arr[x, y] = count;
                    }
                }

//                for (int x = 0; x < count; x++)
//                {
//                    for (int y = 0; y < count; y++)
//                    {
//                        arr[x, y] = count;
//                    }
//                }

                watch.Stop();
                counter[index] = watch.Elapsed.TotalMilliseconds;
                Console.WriteLine("{0}ms", watch.Elapsed.TotalMilliseconds);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("平均： {0}ms", counter.Average());
        }
    }
}
