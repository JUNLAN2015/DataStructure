using System;
using System.Collections.Generic;
using System.Linq;
using Algorithms.Common;
using DataStructures.Lists;

namespace Algorithms.Sorting
{
    public static class BinarySearchTreeSorter
    {
        public static void UnbalancedBSTSort<T>(this List<T> collection) where T : IComparable<T>
        {
            if (collection.Count == 0)
                return;

            var treeRoot = new Node<T> {Value = collection[0]};

            // Get a handle on root.
            for (var i = 1; i < collection.Count; ++i)
            {
                var currentNode = treeRoot;
                var newNode = new Node<T> {Value = collection[i]};

                while (true)
                {
                    // Go left
                    if (newNode.Value.IsLessThan(currentNode.Value))
                    {
                        if (currentNode.Left == null)
                        {
                            newNode.Parent = currentNode;
                            currentNode.Left = newNode;
                            break;
                        }

                        currentNode = currentNode.Left;
                    }
                    // Go right
                    else
                    {
                        if (currentNode.Right == null)
                        {
                            newNode.Parent = currentNode;
                            currentNode.Right = newNode;
                            break;
                        }

                        currentNode = currentNode.Right;
                    }
                } //end-while
            } //end-for

            // Reference handle to root again.
            collection.Clear();
            var treeRootReference = treeRoot;
            _inOrderTravelAndAdd(treeRootReference, ref collection);

            treeRootReference = treeRoot = null;
        }

        private static void _inOrderTravelAndAdd<T>(Node<T> currentNode, ref List<T> collection)
            where T : IComparable<T>
        {
            if (currentNode == null)
                return;

            _inOrderTravelAndAdd(currentNode.Left, ref collection);
            collection.Add(currentNode.Value);
            _inOrderTravelAndAdd(currentNode.Right, ref collection);
        }


        private class Node<T> : IComparable<Node<T>> where T : IComparable<T>
        {
            public Node()
            {
                Value = default(T);
                Parent = null;
                Left = null;
                Right = null;
            }

            public T Value { get; set; }
            public Node<T> Parent { get; set; }
            public Node<T> Left { get; set; }
            public Node<T> Right { get; set; }

            public int CompareTo(Node<T> other)
            {
                if (other == null) return -1;
                return Value.CompareTo(other.Value);
            }
        }
    }

    public static class BubbleSorter
    {
        public static void BubbleSort<T>(this IList<T> collection, Comparer<T> comparer = null)
        {
            comparer = comparer ?? Comparer<T>.Default;
            collection.BubbleSortAscending(comparer);
        }

        /// <summary>
        ///     Public API: Sorts ascending
        /// </summary>
        public static void BubbleSortAscending<T>(this IList<T> collection, Comparer<T> comparer)
        {
            for (var i = 0; i < collection.Count; i++)
            {
                for (var index = 0; index < collection.Count - 1; index++)
                {
                    if (comparer.Compare(collection[index], collection[index + 1]) > 0)
                    {
                        collection.Swap(index, index + 1);
                    }
                }
            }
        }

        /// <summary>
        ///     Public API: Sorts descending
        /// </summary>
        public static void BubbleSortDescending<T>(this IList<T> collection, Comparer<T> comparer)
        {
            for (var i = 0; i < collection.Count - 1; i++)
            {
                for (var index = 1; index < collection.Count - i; index++)
                {
                    if (comparer.Compare(collection[index], collection[index - 1]) > 0)
                    {
                        collection.Swap(index - 1, index);
                    }
                }
            }
        }
    }

    public static class BucketSorter
    {
        public static void BucketSort(this IList<int> collection)
        {
            collection.BucketSortAscending();
        }

        /// <summary>
        ///     Public API: Sorts ascending
        /// </summary>
        public static void BucketSortAscending(this IList<int> collection)
        {
            var maxValue = collection.Max();
            var minValue = collection.Min();

            var bucket = new List<int>[maxValue - minValue + 1];

            for (var i = 0; i < bucket.Length; i++)
            {
                bucket[i] = new List<int>();
            }

            foreach (var i in collection)
            {
                bucket[i - minValue].Add(i);
            }

            var k = 0;
            foreach (var i in bucket)
            {
                if (i.Count > 0)
                {
                    foreach (var j in i)
                    {
                        collection[k] = j;
                        k++;
                    }
                }
            }
        }

        /// <summary>
        ///     Public API: Sorts descending
        /// </summary>
        public static void BucketSortDescending(this IList<int> collection)
        {
            var maxValue = collection[0];
            var minValue = collection[0];
            for (var i = 1; i < collection.Count; i++)
            {
                if (collection[i] > maxValue)
                    maxValue = collection[i];

                if (collection[i] < minValue)
                    minValue = collection[i];
            }
            var bucket = new List<int>[maxValue - minValue + 1];

            for (var i = 0; i < bucket.Length; i++)
            {
                bucket[i] = new List<int>();
            }

            foreach (var i in collection)
            {
                bucket[i - minValue].Add(i);
            }

            var k = collection.Count - 1;
            foreach (var i in bucket)
            {
                if (i.Count > 0)
                {
                    foreach (var j in i)
                    {
                        collection[k] = j;
                        k--;
                    }
                }
            }
        }
    }

    public static class CombSorter
    {
        public static void CombSort<T>(this IList<T> collection, Comparer<T> comparer = null)
        {
            comparer = comparer ?? Comparer<T>.Default;
            collection.ShellSortAscending(comparer);
        }

        /// <summary>
        ///     Public API: Sorts ascending
        /// </summary>
        public static void CombSortAscending<T>(this IList<T> collection, Comparer<T> comparer)
        {
            double gap = collection.Count;
            var swaps = true;
            while (gap > 1 || swaps)
            {
                gap /= 1.247330950103979;
                if (gap < 1)
                {
                    gap = 1;
                }
                var i = 0;
                swaps = false;
                while (i + gap < collection.Count)
                {
                    var igap = i + (int) gap;
                    if (comparer.Compare(collection[i], collection[igap]) > 0)
                    {
                        collection.Swap(i, igap);
                        swaps = true;
                    }
                    i++;
                }
            }
        }

        /// <summary>
        ///     Public API: Sorts descending
        /// </summary>
        public static void CombSortDescending<T>(this IList<T> collection, Comparer<T> comparer)
        {
            double gap = collection.Count;
            var swaps = true;
            while (gap > 1 || swaps)
            {
                gap /= 1.247330950103979;
                if (gap < 1)
                {
                    gap = 1;
                }
                var i = 0;
                swaps = false;
                while (i + gap < collection.Count)
                {
                    var igap = i + (int) gap;
                    if (comparer.Compare(collection[i], collection[igap]) < 0)
                    {
                        collection.Swap(i, igap);
                        swaps = true;
                    }
                    i++;
                }
            }
        }
    }

    public static class CountingSorter
    {
        public static void CountingSort(this IList<int> collection)
        {
            if (collection == null || collection.Count == 0)
                return;

            // Get the maximum number in array.
            var maxK = 0;
            var index = 0;
            while (true)
            {
                if (index >= collection.Count)
                    break;

                maxK = Math.Max(maxK, collection[index] + 1);
                index++;
            }

            // The array of keys, used to sort the original array.
            var keys = new int[maxK];
            keys.Populate(0); // populate it with zeros

            // Assign the keys
            for (var i = 0; i < collection.Count; ++i)
            {
                keys[collection[i]] += 1;
            }

            // Reset index.
            index = 0;

            // Sort the elements
            for (var j = 0; j < keys.Length; ++j)
            {
                var val = keys[j];

                if (val > 0)
                {
                    while (val-- > 0)
                    {
                        collection[index] = j;
                        index++;
                    }
                }
            }
        }
    }

    public static class CycleSorter
    {
        public static void CycleSort<T>(this IList<T> collection, Comparer<T> comparer = null)
        {
            comparer = comparer ?? Comparer<T>.Default;
            collection.CycleSortAscending(comparer);
        }

        public static void CycleSortDescending<T>(this IList<T> collection, Comparer<T> comparer)
        {
            for (var cycleStart = 0; cycleStart < collection.Count; cycleStart++)
            {
                var item = collection[cycleStart];
                var position = cycleStart;

                do
                {
                    var to = 0;
                    for (var i = 0; i < collection.Count; i++)
                    {
                        if (i != cycleStart && comparer.Compare(collection[i], item) > 0)
                        {
                            to++;
                        }
                    }
                    if (position != to)
                    {
                        while (position != to && comparer.Compare(item, collection[to]) == 0)
                        {
                            to++;
                        }

                        var temp = collection[to];
                        collection[to] = item;
                        item = temp;
                        position = to;
                    }
                } while (position != cycleStart);
            }
        }

        public static void CycleSortAscending<T>(this IList<T> collection, Comparer<T> comparer)
        {
            for (var cycleStart = 0; cycleStart < collection.Count; cycleStart++)
            {
                var item = collection[cycleStart];
                var position = cycleStart;

                do
                {
                    var to = 0;
                    for (var i = 0; i < collection.Count; i++)
                    {
                        if (i != cycleStart && comparer.Compare(collection[i], item) < 0)
                        {
                            to++;
                        }
                    }
                    if (position != to)
                    {
                        while (position != to && comparer.Compare(item, collection[to]) == 0)
                        {
                            to++;
                        }

                        var temp = collection[to];
                        collection[to] = item;
                        item = temp;
                        position = to;
                    }
                } while (position != cycleStart);
            }
        }
    }

    public static class GnomeSorter
    {
        public static void GnomeSort<T>(this IList<T> collection, Comparer<T> comparer = null)
        {
            comparer = comparer ?? Comparer<T>.Default;
            collection.GnomeSortAscending(comparer);
        }

        /// <summary>
        ///     Public API: Sorts ascending
        /// </summary>
        public static void GnomeSortAscending<T>(this IList<T> collection, Comparer<T> comparer)
        {
            var pos = 1;
            while (pos < collection.Count)
            {
                if (comparer.Compare(collection[pos], collection[pos - 1]) >= 0)
                {
                    pos++;
                }
                else
                {
                    collection.Swap(pos, pos - 1);
                    if (pos > 1)
                    {
                        pos--;
                    }
                }
            }
        }

        /// <summary>
        ///     Public API: Sorts descending
        /// </summary>
        public static void GnomeSortDescending<T>(this IList<T> collection, Comparer<T> comparer)
        {
            var pos = 1;
            while (pos < collection.Count)
            {
                if (comparer.Compare(collection[pos], collection[pos - 1]) <= 0)
                {
                    pos++;
                }
                else
                {
                    collection.Swap(pos, pos - 1);
                    if (pos > 1)
                    {
                        pos--;
                    }
                }
            }
        }
    }

    public static class HeapSorter
    {
        /// <summary>
        ///     Public API: Default functionality.
        ///     Sorts in ascending order. Uses Max-Heaps.
        /// </summary>
        public static void HeapSort<T>(this IList<T> collection, Comparer<T> comparer = null)
        {
            collection.HeapSortAscending(comparer);
        }

        /// <summary>
        ///     Public API: Sorts ascending
        ///     Uses Max-Heaps
        /// </summary>
        public static void HeapSortAscending<T>(this IList<T> collection, Comparer<T> comparer = null)
        {
            // Handle the comparer's default null value
            comparer = comparer ?? Comparer<T>.Default;

            var lastIndex = collection.Count - 1;
            collection.BuildMaxHeap(0, lastIndex, comparer);

            while (lastIndex >= 0)
            {
                collection.Swap(0, lastIndex);
                lastIndex--;
                collection.MaxHeapify(0, lastIndex, comparer);
            }
        }

        /// <summary>
        ///     Public API: Sorts ascending
        ///     Uses Min-Heaps
        /// </summary>
        public static void HeapSortDescending<T>(this IList<T> collection, Comparer<T> comparer = null)
        {
            // Handle the comparer's default null value
            comparer = comparer ?? Comparer<T>.Default;

            var lastIndex = collection.Count - 1;
            collection.BuildMinHeap(0, lastIndex, comparer);

            while (lastIndex >= 0)
            {
                collection.Swap(0, lastIndex);
                lastIndex--;
                collection.MinHeapify(0, lastIndex, comparer);
            }
        }


        /****************************************************************************/


        /// <summary>
        ///     Private Max-Heap Builder.
        ///     Builds a max heap from an IList<T> collection.
        /// </summary>
        private static void BuildMaxHeap<T>(this IList<T> collection, int firstIndex, int lastIndex,
            Comparer<T> comparer)
        {
            var lastNodeWithChildren = lastIndex/2;

            for (var node = lastNodeWithChildren; node >= 0; --node)
            {
                collection.MaxHeapify(node, lastIndex, comparer);
            }
        }

        /// <summary>
        ///     Private Max-Heapifier. Used in BuildMaxHeap.
        ///     Heapfies the elements between two indexes (inclusive), maintaining the maximum at the top.
        /// </summary>
        private static void MaxHeapify<T>(this IList<T> collection, int nodeIndex, int lastIndex, Comparer<T> comparer)
        {
            // assume left(i) and right(i) are max-heaps
            var left = nodeIndex*2 + 1;
            var right = left + 1;
            var largest = nodeIndex;

            // If collection[left] > collection[nodeIndex]
            if (left <= lastIndex && comparer.Compare(collection[left], collection[nodeIndex]) > 0)
                largest = left;

            // If collection[right] > collection[largest]
            if (right <= lastIndex && comparer.Compare(collection[right], collection[largest]) > 0)
                largest = right;

            // Swap and heapify
            if (largest != nodeIndex)
            {
                collection.Swap(nodeIndex, largest);
                collection.MaxHeapify(largest, lastIndex, comparer);
            }
        }

        /// <summary>
        ///     Private Min-Heap Builder.
        ///     Builds a min heap from an IList<T> collection.
        /// </summary>
        private static void BuildMinHeap<T>(this IList<T> collection, int firstIndex, int lastIndex,
            Comparer<T> comparer)
        {
            var lastNodeWithChildren = lastIndex/2;

            for (var node = lastNodeWithChildren; node >= 0; --node)
            {
                collection.MinHeapify(node, lastIndex, comparer);
            }
        }

        /// <summary>
        ///     Private Min-Heapifier. Used in BuildMinHeap.
        ///     Heapfies the elements between two indexes (inclusive), maintaining the minimum at the top.
        /// </summary>
        private static void MinHeapify<T>(this IList<T> collection, int nodeIndex, int lastIndex, Comparer<T> comparer)
        {
            // assume left(i) and right(i) are max-heaps
            var left = nodeIndex*2 + 1;
            var right = left + 1;
            var smallest = nodeIndex;

            // If collection[left] > collection[nodeIndex]
            if (left <= lastIndex && comparer.Compare(collection[left], collection[nodeIndex]) < 0)
                smallest = left;

            // If collection[right] > collection[largest]
            if (right <= lastIndex && comparer.Compare(collection[right], collection[smallest]) < 0)
                smallest = right;

            // Swap and heapify
            if (smallest != nodeIndex)
            {
                collection.Swap(nodeIndex, smallest);
                collection.MinHeapify(smallest, lastIndex, comparer);
            }
        }
    }

    public static class InsertionSorter
    {
        public static void InsertionSort<T>(this IList<T> list, Comparer<T> comparer = null)
        {
            comparer = comparer ?? Comparer<T>.Default;
            int i, j;
            for (i = 1; i < list.Count; i++)
            {
                var value = list[i];
                j = i - 1;

                while ((j >= 0) && (comparer.Compare(list[j], value) > 0))
                {
                    list[j + 1] = list[j];
                    j--;
                }

                list[j + 1] = value;
            }
        }

        public static void InsertionSort<T>(this ArrayList<T> list, Comparer<T> comparer = null)
        {
            comparer = comparer ?? Comparer<T>.Default;

            for (var i = 1; i < list.Count; i++)
            {
                for (var j = i; j > 0; j--)
                {
                    if (comparer.Compare(list[j], list[j - 1]) < 0)
                    {
                        var temp = list[j - 1];
                        list[j - 1] = list[j];
                        list[j] = temp;
                    }
                }
            }
        }
    }

    public static class LSDRadixSorter
    {
        /// <summary>
        ///     Extension method for sorting strings.
        /// </summary>
        public static string LSDRadixSort(this string source)
        {
            if (string.IsNullOrEmpty(source) || source.Length <= 1)
                return source;

            // LSD Radix Sort the character arrapy representation of the string
            var charArray = source.ToCharArray();
            charArray.LSDRadixSort();

            return new string(charArray);
        }


        /// <summary>
        ///     Extension method for sorting character arrays, in place.
        /// </summary>
        public static void LSDRadixSort(this char[] source)
        {
            if (source == null || source.Length <= 1)
                return;

            // extend ASCII alphabet size
            var asciiSize = 256;

            var length = source.Length;
            var auxiliary = new char[length];

            // compute frequency counts
            var count = new int[asciiSize + 1];

            for (var i = 0; i < length; i++)
                count[source[i] + 1]++;

            // compute cumulates
            for (var r = 0; r < asciiSize; r++)
                count[r + 1] += count[r];

            // move data
            for (var i = 0; i < length; i++)
                auxiliary[count[source[i]]++] = source[i];

            // copy back
            for (var i = 0; i < length; i++)
                source[i] = auxiliary[i];
        }


        /// <summary>
        ///     Extension method for sorting collections of strings of the same width, in place.
        /// </summary>
        public static void LSDRadixSort(this IList<string> collection, int stringFixedWidth)
        {
            // Validate input
            if (collection == null || collection.Count <= 1)
                return;
            for (var i = 0; i < collection.Count; ++i)
                if (collection[i] == null || collection[i].Length != stringFixedWidth)
                    throw new ApplicationException("Not all strings have the same width");

            // extend ASCII alphabet size
            var asciiSize = 256;

            var stringsCount = collection.Count;
            var auxiliary = new string[stringsCount];

            for (var d = stringFixedWidth - 1; d >= 0; d--)
            {
                // compute frequency counts
                var count = new int[asciiSize + 1];

                for (var i = 0; i < stringsCount; i++)
                    count[collection[i][d] + 1]++;

                // compute cumulates
                for (var r = 0; r < asciiSize; r++)
                    count[r + 1] += count[r];

                // move data
                for (var i = 0; i < stringsCount; i++)
                    auxiliary[count[collection[i][d]]++] = collection[i];

                // copy back
                for (var i = 0; i < stringsCount; i++)
                    collection[i] = auxiliary[i];
            }
        }
    }

    public static class MergeSorter
    {
        //
        // Public merge-sort API
        public static List<T> MergeSort<T>(this List<T> collection, Comparer<T> comparer = null)
        {
            comparer = comparer ?? Comparer<T>.Default;

            return InternalMergeSort(collection, 0, collection.Count - 1, comparer);
        }


        //
        // Private static method
        // Implements the recursive merge-sort algorithm
        private static List<T> InternalMergeSort<T>(List<T> collection, int startIndex, int endIndex,
            Comparer<T> comparer)
        {
            if (collection.Count < 2)
            {
                return collection;
            }
            if (collection.Count == 2)
            {
                if (comparer.Compare(collection[endIndex], collection[startIndex]) < 0)
                {
                    collection.Swap(endIndex, startIndex);
                }

                return collection;
            }
            var midIndex = collection.Count/2;

            var leftCollection = collection.GetRange(startIndex, midIndex);
            var rightCollection = collection.GetRange(midIndex, endIndex - midIndex + 1);

            leftCollection = InternalMergeSort(leftCollection, 0, leftCollection.Count - 1, comparer);
            rightCollection = InternalMergeSort(rightCollection, 0, rightCollection.Count - 1, comparer);

            return InternalMerge(leftCollection, rightCollection, comparer);
        }


        //
        // Private static method
        // Implements the merge function inside the merge-sort
        private static List<T> InternalMerge<T>(List<T> leftCollection, List<T> rightCollection, Comparer<T> comparer)
        {
            var left = 0;
            var right = 0;
            int index;
            var length = leftCollection.Count + rightCollection.Count;

            var result = new List<T>(length);

            for (index = 0; index < length; ++index)
            {
                if (right < rightCollection.Count && comparer.Compare(rightCollection[right], leftCollection[left]) <= 0)
                    // rightElement <= leftElement
                {
                    //resultArray.Add(rightCollection[right]);
                    result.Insert(index, rightCollection[right]);
                    right++;
                }
                else
                {
                    //result.Add(leftCollection[left]);
                    result.Insert(index, leftCollection[left]);
                    left++;

                    if (left == leftCollection.Count)
                        break;
                }
            }

            //
            // Either one might have elements left
            var rIndex = index + 1;
            var lIndex = index + 1;

            while (right < rightCollection.Count)
            {
                result.Insert(rIndex, rightCollection[right]);
                rIndex++;
                right++;
            }

            while (left < leftCollection.Count)
            {
                result.Insert(lIndex, leftCollection[left]);
                lIndex++;
                left++;
            }

            return result;
        }
    }

    public static class OddEvenSorter
    {
        public static void OddEvenSort<T>(this IList<T> collection, Comparer<T> comparer = null)
        {
            comparer = comparer ?? Comparer<T>.Default;
            collection.OddEvenSortAscending(comparer);
        }

        /// <summary>
        ///     Public API: Sorts ascending
        /// </summary>
        public static void OddEvenSortAscending<T>(this IList<T> collection, Comparer<T> comparer)
        {
            var sorted = false;
            while (!sorted)
            {
                sorted = true;
                for (var i = 1; i < collection.Count - 1; i += 2)
                {
                    if (comparer.Compare(collection[i], collection[i + 1]) > 0)
                    {
                        collection.Swap(i, i + 1);
                        sorted = false;
                    }
                }

                for (var i = 0; i < collection.Count - 1; i += 2)
                {
                    if (comparer.Compare(collection[i], collection[i + 1]) > 0)
                    {
                        collection.Swap(i, i + 1);
                        sorted = false;
                    }
                }
            }
        }

        /// <summary>
        ///     Public API: Sorts descending
        /// </summary>
        public static void OddEvenSortDescending<T>(this IList<T> collection, Comparer<T> comparer)
        {
            var sorted = false;
            while (!sorted)
            {
                sorted = true;
                for (var i = 1; i < collection.Count - 1; i += 2)
                {
                    if (comparer.Compare(collection[i], collection[i + 1]) < 0)
                    {
                        collection.Swap(i, i + 1);
                        sorted = false;
                    }
                }

                for (var i = 0; i < collection.Count - 1; i += 2)
                {
                    if (comparer.Compare(collection[i], collection[i + 1]) < 0)
                    {
                        collection.Swap(i, i + 1);
                        sorted = false;
                    }
                }
            }
        }
    }

    public static class PigeonHoleSorter
    {
        public static void PigeonHoleSort(this IList<int> collection)
        {
            collection.PigeonHoleSortAscending();
        }

        public static void PigeonHoleSortAscending(this IList<int> collection)
        {
            var min = collection.Min();
            var max = collection.Max();
            var size = max - min + 1;
            var holes = new int[size];
            foreach (var x in collection)
            {
                holes[x - min]++;
            }

            var i = 0;
            for (var count = 0; count < size; count++)
            {
                while (holes[count]-- > 0)
                {
                    collection[i] = count + min;
                    i++;
                }
            }
        }

        public static void PigeonHoleSortDescending(this IList<int> collection)
        {
            var min = collection.Min();
            var max = collection.Max();
            var size = max - min + 1;
            var holes = new int[size];
            foreach (var x in collection)
            {
                holes[x - min]++;
            }

            var i = 0;
            for (var count = size - 1; count >= 0; count--)
            {
                while (holes[count]-- > 0)
                {
                    collection[i] = count + min;
                    i++;
                }
            }
        }
    }

    public static class QuickSorter
    {
        //
        // The public APIs for the quick sort algorithm.
        public static void QuickSort<T>(this IList<T> collection, Comparer<T> comparer = null)
        {
            var startIndex = 0;
            var endIndex = collection.Count - 1;

            //
            // If the comparer is Null, then initialize it using a default typed comparer
            comparer = comparer ?? Comparer<T>.Default;

            collection.InternalQuickSort(startIndex, endIndex, comparer);
        }


        //
        // Private static method
        // The recursive quick sort algorithm
        private static void InternalQuickSort<T>(this IList<T> collection, int leftmostIndex, int rightmostIndex,
            Comparer<T> comparer)
        {
            //
            // Recursive call check
            if (leftmostIndex < rightmostIndex)
            {
                var wallIndex = collection.InternalPartition(leftmostIndex, rightmostIndex, comparer);
                collection.InternalQuickSort(leftmostIndex, wallIndex - 1, comparer);
                collection.InternalQuickSort(wallIndex + 1, rightmostIndex, comparer);
            }
        }


        //
        // Private static method
        // The partition function, used in the quick sort algorithm
        private static int InternalPartition<T>(this IList<T> collection, int leftmostIndex, int rightmostIndex,
            Comparer<T> comparer)
        {
            int wallIndex, pivotIndex;

            // Choose the pivot
            pivotIndex = rightmostIndex;
            var pivotValue = collection[pivotIndex];

            // Compare remaining array elements against pivotValue
            wallIndex = leftmostIndex;

            // Loop until pivot: exclusive!
            for (var i = leftmostIndex; i <= rightmostIndex - 1; i++)
            {
                // check if collection[i] <= pivotValue
                if (comparer.Compare(collection[i], pivotValue) <= 0)
                {
                    collection.Swap(i, wallIndex);
                    wallIndex++;
                }
            }

            collection.Swap(wallIndex, pivotIndex);

            return wallIndex;
        }
    }

    public static class SelectionSorter
    {
        public static void SelectionSort<T>(this IList<T> collection, Comparer<T> comparer = null)
        {
            comparer = comparer ?? Comparer<T>.Default;
            collection.SelectionSortAscending(comparer);
        }

        /// <summary>
        ///     Public API: Sorts ascending
        /// </summary>
        public static void SelectionSortAscending<T>(this IList<T> collection, Comparer<T> comparer)
        {
            int i;
            for (i = 0; i < collection.Count; i++)
            {
                var min = i;
                for (var j = i + 1; j < collection.Count; j++)
                {
                    if (comparer.Compare(collection[j], collection[min]) < 0)
                        min = j;
                }
                collection.Swap(i, min);
            }
        }

        /// <summary>
        ///     Public API: Sorts ascending
        /// </summary>
        public static void SelectionSortDescending<T>(this IList<T> collection, Comparer<T> comparer)
        {
            int i;
            for (i = collection.Count - 1; i > 0; i--)
            {
                var max = i;
                for (var j = 0; j <= i; j++)
                {
                    if (comparer.Compare(collection[j], collection[max]) < 0)
                        max = j;
                }
                collection.Swap(i, max);
            }
        }
    }

    public static class ShellSorter
    {
        public static void ShellSort<T>(this IList<T> collection, Comparer<T> comparer = null)
        {
            comparer = comparer ?? Comparer<T>.Default;
            collection.ShellSortAscending(comparer);
        }

        /// <summary>
        ///     Public API: Sorts ascending
        /// </summary>
        public static void ShellSortAscending<T>(this IList<T> collection, Comparer<T> comparer)
        {
            var flag = true;
            var d = collection.Count;
            while (flag || (d > 1))
            {
                flag = false;
                d = (d + 1)/2;
                for (var i = 0; i < collection.Count - d; i++)
                {
                    if (comparer.Compare(collection[i + d], collection[i]) < 0)
                    {
                        collection.Swap(i + d, i);
                        flag = true;
                    }
                }
            }
        }

        /// <summary>
        ///     Public API: Sorts descending
        /// </summary>
        public static void ShellSortDescending<T>(this IList<T> collection, Comparer<T> comparer)
        {
            var flag = true;
            var d = collection.Count;
            while (flag || (d > 1))
            {
                flag = false;
                d = (d + 1)/2;
                for (var i = 0; i < collection.Count - d; i++)
                {
                    if (comparer.Compare(collection[i + d], collection[i]) > 0)
                    {
                        collection.Swap(i + d, i);
                        flag = true;
                    }
                }
            }
        }
    }
}