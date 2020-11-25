using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sorting
{
    public class TopNPrioQueue<T> : ITopNSort<T>
    {
        public string Name { get { return "TopN-PrioQueue"; } }

        public T[] TopNSort(T[] inputOutput, int n)
        {
            return TopNSort(inputOutput, n, Comparer<T>.Default);
        }

        public T[] TopNSort(T[] inputOutput, int n, IComparer<T> comparer)
        {
            ConcurrentQueue<int> prioQueue = new ConcurrentQueue<int>();

        }
    }
}
