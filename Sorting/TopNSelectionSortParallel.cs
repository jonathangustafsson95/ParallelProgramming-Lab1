using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sorting
{
    public class TopNSelectionSortParallel<T> : ITopNSort<T>
    {
        public string Name { get { return "TopN-SelectionSortParallel"; } }
        public static int CONC_LIMIT = Environment.ProcessorCount * 2;

        public T[] TopNSort(T[] inputOutput, int n)
        {
            List<T[]> subIO = new List<T[]>();
            object monitor = new object();

            Parallel.ForEach(Partitioner.Create(0, inputOutput.Length), range =>
            {
                T[] result = TopNSort(inputOutput.Take(range.Item2).Skip(range.Item1).ToArray(), n, Comparer<T>.Default);
                lock(monitor){
                    subIO.Add(result);
                }
            });

            return MergeKArrays(subIO, n);
        }
        public T[] TopNSort(T[] inputOutput, int n, IComparer<T> comparer)
        {  
            int m = inputOutput.Length;
            for (int i = 0; i < n - 1; i++)
            {
                int min = i;
                for (int j = i + 1; j < m; j++)
                {
                    if (comparer.Compare(inputOutput[j], inputOutput[min]) < 0)
                    {
                        T tmp = inputOutput[j];
                        inputOutput[j] = inputOutput[min];
                        inputOutput[min] = tmp;
                    }
                }
            }
            return inputOutput;
        }

        T[] MergeArrays(T[] arr1, T[] arr2, int n, IComparer<T> comparer)
        {
            int i = 0;
            int j = 0;
            int k = 0;

            if (arr1.Length + arr2.Length < n)
                n = arr1.Length + arr2.Length;

            T[] arr3 = new T[n];

            while (i < n && i < arr1.Length && j < n && j < arr2.Length && k < n) {
                if (comparer.Compare(arr1[i], arr2[j]) < 0) {
                    arr3[k++] = arr1[i++];
                }
                else {
                    arr3[k++] = arr2[j++];
                }
            }

            if (i == arr1.Length)
                while (j < n && k < n)
                    arr3[k++] = arr2[j++];
            else if (j == arr2.Length)
                while (i < n && k < n)
                    arr3[k++] = arr1[i++];

            return arr3;
        }

        T[] MergeKArrays(List<T[]> arr, int n)
        {
            int i = 0;
            T[] output = arr[i++];

            while (i < arr.Count)
            {
                output = MergeArrays(output, arr[i++], n, Comparer<T>.Default);
            }
            
            return output;
        }
    }
}
