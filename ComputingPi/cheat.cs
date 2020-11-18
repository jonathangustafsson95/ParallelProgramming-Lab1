using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EstimatePerformanceDemo
{
    class Program
    {
        const int NumberOfSteps = 500_000_000;
        static void Main(string[] args)
        {
            Console.WriteLine("Function               | Elapsed Time     | Estimated Pi");
            Console.WriteLine("-----------------------------------------------------------------");

            Time(SerialLinqPi, nameof(SerialLinqPi));
            Time(ParallelLinqPi, nameof(ParallelLinqPi));
            Time(SerialPi, nameof(SerialPi));
            Time(ParallelPi, nameof(ParallelPi));
            Time(ParallelPiUseInterlocked, nameof(ParallelPiUseInterlocked));
            Time(ParallelPartitionerPi, nameof(ParallelPartitionerPi));
            Time(ParallelPartitionerPiUseInterlocked, nameof(ParallelPartitionerPiUseInterlocked));
            Console.ReadLine();
        }

        /// <summary>
        /// Time the execution of the function, and output the time used and the result of the function.
        /// </summary>
        /// <param name="estimatePi">The commission function for calculating the Pi</param>
        /// <param name="function">method name</param>
        static void Time(Func<double> estimatePi, string function)
        {
            var sw = Stopwatch.StartNew();
            var pi = estimatePi();
            Console.WriteLine($"{function.PadRight(36)} | {sw.Elapsed} | {pi}");
        }

        /// <summary>
        /// Estimates the value of PI using a LINQ-based implementation.
        /// </summary>
        /// <returns></returns>
        static double SerialLinqPi()
        {
            double step = 1.0 / (double)NumberOfSteps;
            return (from i in Enumerable.Range(0, NumberOfSteps)
                    let x = (i + 0.5) * step
                    select 4.0 / (1.0 + x * x)).Sum() * step;
        }

        /// <summary>
        /// Use LINQ-based implementation to estimate the value of PI.
        /// </summary>
        /// <returns></returns>
        static double ParallelLinqPi()
        {
            double step = 1.0 / (double)NumberOfSteps;
            return (from i in ParallelEnumerable.Range(0, NumberOfSteps)
                    let x = (i + 0.5) * step
                    select 4.0 / (1.0 + x * x)).Sum() * step;
        }

        /// <summary>
        /// Use [Sequence for Loop] to estimate the value of PI.
        /// </summary>
        /// <returns></returns>
        static double SerialPi()
        {
            double sum = 0.0;
            double step = 1.0 / (double)NumberOfSteps;
            for (int i = 0; i < NumberOfSteps; i++)
            {
                double x = (i + 0.5) * step;
                sum += 4.0 / (1.0 + x * x);
            }
            return step * sum;
        }

        /// <summary>
        /// Use concurrency [Parallel.For] to estimate the value of PI. [Lock] is used by default
        /// </summary>
        /// <returns></returns>
        static double ParallelPi()
        {
            double sum = 0.0;
            double step = 1.0 / (double)NumberOfSteps;
            object monitor = new object();
            Parallel.For(0, NumberOfSteps, () => 0.0, (i, state, local) =>
            {
                double x = (i + 0.5) * step;
                return local + 4.0 / (1.0 + x * x);
            }, local => { lock (monitor) sum += local; });
            return step * sum;
        }

        /// <summary>
        /// Use concurrency [Parallel.For] to estimate the value of PI. [Use Interlocked is better than lock performance]
        /// </summary>
        /// <returns></returns>
        static double ParallelPiUseInterlocked()
        {
            double sum = 0.0;
            double step = 1.0 / (double)NumberOfSteps;
            int monitor = 0;
            Parallel.For(0, NumberOfSteps, () => 0.0, (i, state, local) =>
            {
                double x = (i + 0.5) * step;
                return local + 4.0 / (1.0 + x * x);
            }, local =>
            {
                while (Interlocked.Exchange(ref monitor, 1) != 0)
                { }
                sum += local;
                Interlocked.Exchange(ref monitor, 0);
            });
            return step * sum;
        }

        /// <summary>
        /// Use concurrency [Parallel.ForEach] to estimate the value of PI. [Use partition interval]
        /// </summary>
        /// <returns></returns>
        static double ParallelPartitionerPi()
        {
            double sum = 0.0;
            double step = 1.0 / (double)NumberOfSteps;
            object monitor = new object();
            Parallel.ForEach(Partitioner.Create(0, NumberOfSteps), () => 0.0, (range, state, local) =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    double x = (i + 0.5) * step;
                    local += 4.0 / (1.0 + x * x);
                }
                return local;
            }, local => { lock (monitor) sum += local; });
            return step * sum;
        }

        /// <summary>
        /// Use concurrency [Parallel.ForEach] to estimate the value of PI. [Use partition interval] [Use Interlocked]
        /// </summary>
        /// <returns></returns>
        static double ParallelPartitionerPiUseInterlocked()
        {
            double sum = 0.0;
            double step = 1.0 / (double)NumberOfSteps;
            int monitor = 0;
            Parallel.ForEach(Partitioner.Create(0, NumberOfSteps), () => 0.0, (range, state, local) =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    double x = (i + 0.5) * step;
                    local += 4.0 / (1.0 + x * x);
                }
                return local;
            }, local =>
            {
                while (Interlocked.Increment(ref monitor) != 1)
                { }
                sum += local;
                Interlocked.Decrement(ref monitor);
            });
            return step * sum;
        }
    }
}
