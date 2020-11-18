using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ComputingPi
{
    public class SerialPi : IComputePi
    {
        public string Name {
            get {
                return nameof(SerialPi);
            }
        }

        /// <summary>Estimates the value of PI using a for loop.</summary>
        public double ComputePi(int numberOfSteps)
        {
            double sum = 0.0;
            double step = 1.0 / (double)numberOfSteps;
            for (int i = 0; i < numberOfSteps; i++)
            {
                double x = (i + 0.5) * step;
                sum += 4.0 / (1.0 + x * x);
            }
            return step * sum;
        }
    }

    public class PFor : IComputePi
    {
        public string Name
        {
            get
            {
                return nameof(PFor);
            }
        }

        /// <summary>Estimates the value of PI using a for loop.</summary>
        public double ComputePi(int numberOfSteps)
        {
            double sum = 0.0;
            double step = 1.0 / (double)numberOfSteps;
            object l = new object();
            Parallel.For(0, numberOfSteps, i =>
            {
                double x = (i + 0.5) * step;
                lock (l)
                {
                    sum += 4.0 / (1.0 + x * x);             
                }
            });
            return step * sum;
        }
    }

    public class PForLocalSum : IComputePi
    {
        public string Name
        {
            get
            {
                return nameof(PForLocalSum);
            }
        }

        /// <summary>Estimates the value of PI using a for loop.</summary> ÖHHHH!!!ÖH! ÖH! öööö!
        public double ComputePi(int numberOfSteps) 
        {
            double sum = 0.0;
            double step = 1.0 / (double)numberOfSteps;
            object l = new object();

            Parallel.For(0, numberOfSteps, () => 0.0, (i, state, local_sum) =>
            {
                double x = (i + 0.5) * step;
                return local_sum + 4.0 / (1.0 + x * x);
            }, local_sum => 
            { 
                lock (l) sum += local_sum; 
            });
            return step * sum;
        }
    }

    public class PForLocalSumInterLock : IComputePi
    {
        public string Name
        {
            get
            {
                return nameof(PForLocalSumInterLock);
            }
        }

        /// <summary>Estimates the value of PI using a for loop.</summary>
        public double ComputePi(int numberOfSteps)
        {
            double sum = 0.0;
            double step = 1.0 / (double)numberOfSteps;
            int monitor = 0;

            Parallel.For(0, numberOfSteps, () => 0.0, (i, state, local_sum) =>
            {
                double x = (i + 0.5) * step;
                return local_sum + 4.0 / (1.0 + x * x);
            },

            local_sum =>
            {
                while (Interlocked.Exchange(ref monitor, 1) != 0)
                { }
                sum += local_sum;
                Interlocked.Exchange(ref monitor, 0);
            });
            return step * sum;
        }
    }
    public class PForPartitionedRange : IComputePi
    {
        public string Name
        {
            get
            {
                return nameof(PForPartitionedRange);
            }
        }

        /// <summary>Estimates the value of PI using a for loop.</summary>
        public double ComputePi(int numberOfSteps)
        {
            double sum = 0.0;
            double step = 1.0 / (double)numberOfSteps;
            object monitor = new object();
            var rangePartitioner = Partitioner.Create(0, numberOfSteps);
            
            Parallel.ForEach(rangePartitioner, () => 0.0, (range, state, local) =>
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
    }
    public class ParallelPLINQ : IComputePi
    {
        public string Name
        {
            get
            {
                return nameof(ParallelPLINQ);
            }
        }

        /// <summary>Estimates the value of PI using a for loop.</summary>
        public double ComputePi(int numberOfSteps)
        {
            double step = 1.0 / (double)numberOfSteps;
            return (from i in ParallelEnumerable.Range(0, numberOfSteps) // går att göra sequentiellt
                    let x = (i + 0.5) * step
                    select 4.0 / (1.0 + x * x)).Sum() * step;
        }
    }
}