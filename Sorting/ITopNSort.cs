using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sorting
{
    public interface ITopNSort<T>
    {
        string Name { get; }
        T[] TopNSort(T[] inputOutput, int n);
        T[] TopNSort(T[] inputOutput, int n, IComparer<T> comparer);
    }
}
