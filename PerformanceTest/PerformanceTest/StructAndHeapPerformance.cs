using System;
using System.Diagnostics;
using System.Linq;

namespace PerformanceTest
{
    public class StructAndHeapPerformance
    {
        const int count = 250000;

        public static void RunStack(int testCount)
        {
            Alloc_Stack(0);

            var counter = new double[testCount];

            var watch = Stopwatch.StartNew();

            Console.WriteLine("栈上分配 char 数组并遍历，数据量 25 万");
            Console.ForegroundColor = ConsoleColor.Green;

            for (int index = 0; index < testCount; index++)
            {
                watch.Restart();

                Alloc_Stack(count);

                watch.Stop();
                counter[index] = watch.Elapsed.TotalMilliseconds;
                Console.WriteLine("{0}ms", watch.Elapsed.TotalMilliseconds);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("平均： {0}ms", counter.Average());
        }

        public static void RunHeap(int testCount)
        {
            Alloc_Heap(0);

            var counter = new double[testCount];

            var watch = Stopwatch.StartNew();

            Console.WriteLine("堆上分配 char 数组并遍历，数据量 25 万");
            Console.ForegroundColor = ConsoleColor.Green;

            for (int index = 0; index < testCount; index++)
            {
                watch.Restart();

                Alloc_Heap(count);

                watch.Stop();
                counter[index] = watch.Elapsed.TotalMilliseconds;
                Console.WriteLine("{0}ms", watch.Elapsed.TotalMilliseconds);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("平均： {0}ms", counter.Average());
        }

        private unsafe static void Alloc_Stack(int size)
        {
            var arr = stackalloc char[size];
            for (int i = 0; i < size; i++)
            {
                arr[i] = '0';
            }
        }

        private static void Alloc_Heap(int size)
        {
            var arr = new char[size];
            for (int i = 0; i < size; i++)
            {
                arr[i] = '0';
            }
        }
    }
}
