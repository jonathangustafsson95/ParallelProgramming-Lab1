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
        public string Name { get { return "TopN-SelectionSort"; } }
        public static int CONC_LIMIT = Environment.ProcessorCount * 2;
        public volatile int _invokeCalls = 0;


        public T[] TopNSort(T[] inputOutput, int n)
        {
            List<T[]> subLists = new List<T[]>();
            Parallel.ForEach(Partitioner.Create(0, inputOutput.Length), range =>
            {
                var result = TopNSort(inputOutput.Skip(range.Item1).Take(range.Item2).ToArray(), n, Comparer<T>.Default);
                lock(subLists){
                    subLists.Add(result);
                }
            });
            
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

        //void MergeArrays(T[] arr1, T[] arr2, int n1, int n2, T[] arr3)
        //{
        //    int i = 0;
        //    int j = 0;
        //    int k = 0;

        //    // Traverse both array  
        //    while (i < n1 && j < n2)
        //    {
        //        // Check if current element of first  
        //        // array is smaller than current element  
        //        // of second array. If yes, store first  
        //        // array element and increment first array  
        //        // index. Otherwise do same with second array  
        //        if (arr1[i] < arr2[j])
        //            arr3[k++] = arr1[i++];
        //        else
        //            arr3[k++] = arr2[j++];
        //    }

        //    // Store remaining elements of first array  
        //    while (i < n1)
        //        arr3[k++] = arr1[i++];

        //    // Store remaining elements of second array  
        //    while (j < n2)
        //        arr3[k++] = arr2[j++];
        //}

        //// This function takes an array of arrays as an argument and 
        //// All arrays are assumed to be sorted. It merges them together 
        //// and prints the final sorted output. 
        //void mergeKArrays(int arr[][n], int i, int j, int output[])
        //{
        //    //if one array is in range 
        //    if (i == j)
        //    {
        //        for (int p = 0; p < n; p++)
        //            output[p] = arr[i][p];
        //        return;
        //    }

        //    //if only two arrays are left them merge them  
        //    if (j - i == 1)
        //    {
        //        mergeArrays(arr[i], arr[j], n, n, output);
        //        return;
        //    }

        //    //output arrays  
        //    int out1[n * (((i + j) / 2) - i + 1)], out2[n * (j - ((i + j) / 2))];

        //    //divide the array into halves 
        //    mergeKArrays(arr, i, (i + j) / 2, out1);
        //    mergeKArrays(arr, (i + j) / 2 + 1, j, out2);

        //    //merge the output array 
        //    mergeArrays(out1, out2, n * (((i + j) / 2) - i + 1), n * (j - ((i + j) / 2)), output);


        //    // https://www.geeksforgeeks.org/merge-k-sorted-arrays/
        //}
    }
}
