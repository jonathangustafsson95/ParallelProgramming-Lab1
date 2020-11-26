using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sorting
{
    public class TopNQuickSort<T> : ITopNSort<T>
    {
        public string Name { get { return "TopN-QuickSort"; } }
        public static int CONC_LIMIT = Environment.ProcessorCount * 2;
        public volatile int _invokeCalls = 0;

        public T[] TopNSort(T[] inputOutput, int n)
        {
            TopNSort(inputOutput, 0, inputOutput.Length - 1, Comparer<T>.Default);
            return inputOutput.Take<T>(n).ToArray();
        }

        public T[] TopNSort(T[] inputOutput, int start, int end, IComparer<T> comparer)
        {
            int threshold = 9; // nuffra för att bestämma när listan att sortera börjar bli så liten att
                               //insertionsort är effektivare. 9 ska tydligen vara ett optimalt tal.
                               //Console.WriteLine("Conc_Limit = {0}", CONC_LIMIT);
                               //Console.WriteLine("Start index: " + start + "\nEnd index: " + end);
                               //Console.WriteLine("Thread Id: {0}", Thread.CurrentThread.ManagedThreadId);

            if (end - start <= threshold)
            {
                InsertionSort(inputOutput, start, end + 1, comparer);
            }
            else
            {
                int pivotPos = Partition(inputOutput, start, end, comparer);
                if (_invokeCalls <= CONC_LIMIT)
                {
                    Interlocked.Increment(ref _invokeCalls);
                    Parallel.Invoke(
                        () => TopNSort(inputOutput, start, pivotPos - 1, comparer),
                        () => TopNSort(inputOutput, pivotPos + 1, end, comparer));
                    Interlocked.Decrement(ref _invokeCalls);
                }
                else
                {
                    TopNSort(inputOutput, start, pivotPos - 1, comparer);
                    TopNSort(inputOutput, pivotPos + 1, end, comparer);
                }
            }
            return inputOutput;
        }



        private void InsertionSort(T[] inputOutput, int start, int end, IComparer<T> comparer)
        {
            {
                for (int x = start + 1; x < end; x++)
                {
                    var val = inputOutput[x];
                    int j = x - 1;
                    while (j >= 0 && comparer.Compare(val, inputOutput[j]) < 0)
                    {
                        inputOutput[j + 1] = inputOutput[j];
                        j--;
                    }
                    inputOutput[j + 1] = val;
                }
            }
        }

        private int Partition(T[] inputOutput, int start, int end, IComparer<T> comparer)
        {

            var pivot = inputOutput[end];
            int i = (start - 1); // Index of start element

            for (int j = start; j < end; j++)
            {
                if (comparer.Compare(inputOutput[j], pivot) < 0)
                {
                    i++;
                    var tmp = inputOutput[i];
                    inputOutput[i] = inputOutput[j];
                    inputOutput[j] = tmp;
                }
            }

            var tmp2 = inputOutput[i + 1];
            inputOutput[i + 1] = inputOutput[end];
            inputOutput[end] = tmp2;

            return i + 1;
        }

        public T[] TopNSort(T[] inputOutput, int n, IComparer<T> comparer)
        {
            throw new NotImplementedException();
        }
    }
}