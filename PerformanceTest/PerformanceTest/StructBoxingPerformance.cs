using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace PerformanceTest
{
    public class StructBoxingPerformance
    {
        public static void RunBoxing(int testCount)
        {
            const int count = 10000000;
            var arr = new TestStruct[count];
            var equal = new TestStruct() { A = 1, B = 2 };
            var counter = new double[testCount];

            var watch = Stopwatch.StartNew();

            Console.WriteLine("装箱调用方法，数据量 1000 万");
            Console.ForegroundColor = ConsoleColor.Green;

            for (int index = 0; index < testCount; index++)
            {
                watch.Restart();

                foreach (var @struct in arr)
                {
                    @struct.Equals(equal);
                }

                watch.Stop();
                counter[index] = watch.Elapsed.TotalMilliseconds;
                Console.WriteLine("{0}ms", watch.Elapsed.TotalMilliseconds);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("平均： {0}ms", counter.Average());
        }

        public static void RunUnBoxing(int testCount)
        {
            const int count = 10000000;
            var arr = new TestStruct[count];
            var equal = new TestStruct() { A = 1, B = 2 };
            var counter = new double[testCount];

            var watch = Stopwatch.StartNew();
            Console.WriteLine("重载避免装箱调用方法，数据量 1000 万");
            Console.ForegroundColor = ConsoleColor.Green;

            for (int index = 0; index < testCount; index++)
            {
                watch.Restart();

                foreach (var @struct in arr)
                {
                    // 默认不重写 Equals 方法会导致两次装箱，一次是 @struct 本身，一次是将 equal 保证成 object 方法参数
                    @struct.Equals(equal);
                }

                watch.Stop();
                counter[index] = watch.Elapsed.TotalMilliseconds;
                Console.WriteLine("{0}ms", watch.Elapsed.TotalMilliseconds);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("平均： {0}ms", counter.Average());
        }
    }

    [StructLayout(LayoutKind.Auto)]
    public struct TestStruct
    {
        public int A;
        public int B;
        public int C;
        public int D;
        public int E;
        public int F;
        public int G;
        public int H;
        public int I;
        public int J;
        public int K;
        public int L;
        public int M;
        public int N;
        public int O;

        
//        public override bool Equals(object obj)
//        {
//            if (!(obj is TestStruct)) return false;
//
//            var point = (TestStruct)obj;
//            return A == point.A && B == point.B && C == point.C && D == point.D && E == point.E && F == point.F &&
//                   G == point.G;
//        }

        public bool Equals(TestStruct point)
        {
            return A == point.A && B == point.B && C == point.C && D == point.D && E == point.E && F == point.F &&
                   G == point.G;
        }
    }
}
