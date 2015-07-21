using System;
using System.Linq;
using Nlpcn.Net.Commons.Lang.Tire.Domain;

namespace Nlpcn.Net.Commons.Lang.Util
{
    public class AnsjArrays
    {
        private static readonly int InsertionsortThreshold = 7;
        /// <summary>
        /// 二分法查找.摘抄了jdk的东西..只不过把他的自动装箱功能给去掉了
        /// </summary>
        /// <param name="branches"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static int BinarySearch(IWoodInterface[] branches, char c)
        {
            var high = branches.Length - 1;
            if (branches.Length < 1)
            {
                return high;
            }
            var low = 0;
            while (low <= high)
            {
                var mid = (low + high) >> 1;
                var cmp = branches[mid].CompareTo(c);

                if (cmp < 0)
                    low = mid + 1;
                else if (cmp > 0)
                    high = mid - 1;
                else
                    return mid; // key found
            }
            return -(low + 1); // key not found.
        }

        public static void Main(string[] args)
        {
            int[] chars = {1, 2, 3, 4, 5, 6, 8, 7};
            Array.Resize(ref chars, 100);
            Console.WriteLine(chars.Length);
            for (var i = 0; i < chars.Length; i++)
            {
                Console.WriteLine(chars[i]);
            }
        }

        public static void Sort(IWoodInterface[] a)
        {
            var aux = new IWoodInterface[a.Length];
            Array.Copy(a, aux, a.Length);
            MergeSort(aux, a, 0, a.Length, 0);
        }

        public static void Sort(IWoodInterface[] a, int fromIndex, int toIndex)
        {
            RangeCheck(a.Length, fromIndex, toIndex);
            var aux = CopyOfRange(a, fromIndex, toIndex);
            MergeSort(aux, a, fromIndex, toIndex, -fromIndex);
        }

        private static void RangeCheck(int arrayLen, int fromIndex, int toIndex)
        {
            if (fromIndex > toIndex)
                throw new Exception("fromIndex(" + fromIndex + ") > toIndex(" + toIndex + ")");
            if (fromIndex < 0)
                throw new IndexOutOfRangeException();
            if (toIndex > arrayLen)
                throw new IndexOutOfRangeException();
        }

        private static void MergeSort(IWoodInterface[] src, IWoodInterface[] dest, int low, int high, int off)
        {
            var length = high - low;

            // Insertion sort on smallest arrays
            if (length < InsertionsortThreshold)
            {
                for (var i = low; i < high; i++)
                    for (var j = i; j > low && (dest[j - 1]).CompareTo(dest[j].GetChar()) > 0; j--)
                        Swap(dest, j, j - 1);
                return;
            }

            // Recursively sort halves of dest into src
            var destLow = low;
            var destHigh = high;
            low += off;
            high += off;
            var mid = (low + high) >> 1;
            MergeSort(dest, src, low, mid, -off);
            MergeSort(dest, src, mid, high, -off);

            // If list is already sorted, just copy from src to dest. This is an
            // optimization that results in faster sorts for nearly ordered lists.
            if (src[mid - 1].CompareTo(src[mid].GetChar()) <= 0)
            {
                Array.Copy(src, low, dest, destLow, length);
                return;
            }

            // Merge sorted halves (now in src) into dest
            for (int i = destLow, p = low, q = mid; i < destHigh; i++)
            {
                if (q >= high || p < mid && src[p].CompareTo(src[q].GetChar()) <= 0)
                    dest[i] = src[p++];
                else
                    dest[i] = src[q++];
            }
        }

        /**
         * Swaps x[a] with x[b].
         */

        private static void Swap(IWoodInterface[] x, int a, int b)
        {
            var t = x[a];
            x[a] = x[b];
            x[b] = t;
        }

        public static T[] CopyOfRange<T>(T[] original, int from, int to)
        {
            return original.Skip(from).Take(to - from + 1).ToArray();
        }
    }
}