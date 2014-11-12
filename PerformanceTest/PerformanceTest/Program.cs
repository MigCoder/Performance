using System;

namespace PerformanceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            CpuHittingPerformance.Run(5);
//            StructBoxingPerformance.RunUnBoxing(5);
//            FilePerformance.Run(5);
            //            FilePerformance.RunUnBufferIo(5);
//            FilePerformance.RunSetLength(5);
            //            FilePerformance.RunUnSetLength(5);
//            StructAndHeapPerformance.RunStack(5);
//            StructAndHeapPerformance.RunHeap(5);
            Console.ReadKey();
        }
    }
}
