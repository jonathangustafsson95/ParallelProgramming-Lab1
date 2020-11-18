using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sorting
{
    public interface ISort<T>
    {
        string Name { get; }
        void Sort(T[] inputOutput);
        void Sort(T[] inputOutput, IComparer<T> comparer);
    }
}
