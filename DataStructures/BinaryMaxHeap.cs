using System;
using System.Collections.Generic;
using DataStructures.Common;
using DataStructures.Lists;

namespace DataStructures.Heaps
{
    public class BinaryMaxHeap<T> : IMaxHeap<T> where T : IComparable<T>
    {
        private readonly Comparer<T> _heapComparer = Comparer<T>.Default;


        /// <summary>
        ///     CONSTRUCTORS
        /// </summary>
        public BinaryMaxHeap() : this(0, null)
        {
        }

        public BinaryMaxHeap(Comparer<T> comparer) : this(0, comparer)
        {
        }

        public BinaryMaxHeap(int capacity, Comparer<T> comparer)
        {
            _collection = new ArrayList<T>(capacity);
            _heapComparer = comparer ?? Comparer<T>.Default;
        }

        /// <summary>
        ///     Instance Variables.
        ///     _collection: The list of elements. Implemented as an array-based list with auto-resizing.
        /// </summary>
        private ArrayList<T> _collection { get; set; }

        /// <summary>
        ///     Gets or sets the at the specified index.
        /// </summary>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index > Count || Count == 0)
                {
                    throw new IndexOutOfRangeException();
                }

                return _collection[index];
            }
            set
            {
                if (index < 0 || index >= Count)
                {
                    throw new IndexOutOfRangeException();
                }

                _collection[index] = value;

                if (_heapComparer.Compare(_collection[index], _collection[0]) >= 0) // greater than or equal to max
                {
                    _collection.Swap(0, index);
                    _buildMaxHeap();
                }
            }
        }


        /// <summary>
        ///     Returns the number of elements in heap
        /// </summary>
        public int Count
        {
            get { return _collection.Count; }
        }

        /// <summary>
        ///     Checks whether this heap is empty
        /// </summary>
        public bool IsEmpty
        {
            get { return _collection.Count == 0; }
        }

        /// <summary>
        ///     Heapifies the specified newCollection. Overrides the current heap.
        /// </summary>
        public void Initialize(IList<T> newCollection)
        {
            if (newCollection.Count > 0)
            {
                // Reset and reserve the size of the newCollection
                _collection = new ArrayList<T>(newCollection.Count);

                // Copy the elements from the newCollection to the inner collection
                for (var i = 0; i < newCollection.Count; ++i)
                {
                    _collection.InsertAt(newCollection[i], i);
                }

                // Build the heap
                _buildMaxHeap();
            }
        }

        /// <summary>
        ///     Adding a new key to the heap.
        /// </summary>
        public void Add(T heapKey)
        {
            if (IsEmpty)
            {
                _collection.Add(heapKey);
            }
            else
            {
                _collection.Add(heapKey);
                _buildMaxHeap();
            }
        }

        /// <summary>
        ///     Find the maximum node of a max heap.
        /// </summary>
        public T Peek()
        {
            if (IsEmpty)
            {
                throw new Exception("Heap is empty.");
            }

            return _collection.First;
        }

        /// <summary>
        ///     Removes the node of minimum value from a min heap.
        /// </summary>
        public void RemoveMax()
        {
            if (IsEmpty)
            {
                throw new Exception("Heap is empty.");
            }

            var max = 0;
            var last = _collection.Count - 1;
            _collection.Swap(max, last);

            _collection.RemoveAt(last);
            last--;

            _maxHeapify(0, last);
        }

        /// <summary>
        ///     Returns the node of maximum value from a max heap after removing it from the heap.
        /// </summary>
        public T ExtractMax()
        {
            var max = Peek();
            RemoveMax();
            return max;
        }

        /// <summary>
        ///     Clear this heap.
        /// </summary>
        public void Clear()
        {
            if (IsEmpty)
            {
                throw new Exception("Heap is empty.");
            }

            _collection.Clear();
        }

        /// <summary>
        ///     Rebuilds the heap.
        /// </summary>
        public void RebuildHeap()
        {
            _buildMaxHeap();
        }

        /// <summary>
        ///     Returns an array version of this heap.
        /// </summary>
        public T[] ToArray()
        {
            return _collection.ToArray();
        }

        /// <summary>
        ///     Returns a list version of this heap.
        /// </summary>
        public List<T> ToList()
        {
            return _collection.ToList();
        }

        /// <summary>
        ///     Returns a new min heap that contains all elements of this heap.
        /// </summary>
        public IMinHeap<T> ToMinHeap()
        {
            var newMinHeap = new BinaryMinHeap<T>(Count, _heapComparer);
            newMinHeap.Initialize(_collection.ToArray());
            return newMinHeap;
        }

        /// <summary>
        ///     Private Method. Builds a max heap from the inner array-list _collection.
        /// </summary>
        private void _buildMaxHeap()
        {
            var lastIndex = _collection.Count - 1;
            var lastNodeWithChildren = lastIndex/2;

            for (var node = lastNodeWithChildren; node >= 0; node--)
            {
                _maxHeapify(node, lastIndex);
            }
        }

        /// <summary>
        ///     Private Method. Used in Building a Max Heap.
        /// </summary>
        private void _maxHeapify(int nodeIndex, int lastIndex)
        {
            // assume that the subtrees left(node) and right(node) are max-heaps
            var left = nodeIndex*2 + 1;
            var right = left + 1;
            var largest = nodeIndex;

            // If collection[left] > collection[nodeIndex]
            if (left <= lastIndex && _heapComparer.Compare(_collection[left], _collection[nodeIndex]) > 0)
                largest = left;

            // If collection[right] > collection[largest]
            if (right <= lastIndex && _heapComparer.Compare(_collection[right], _collection[largest]) > 0)
                largest = right;

            // Swap and heapify
            if (largest != nodeIndex)
            {
                _collection.Swap(nodeIndex, largest);
                _maxHeapify(largest, lastIndex);
            }
        }

        /// <summary>
        ///     Union two heaps together, returns a new min-heap of both heaps' elements,
        ///     ... and then destroys the original ones.
        /// </summary>
        public BinaryMaxHeap<T> Union(ref BinaryMaxHeap<T> firstMaxHeap, ref BinaryMaxHeap<T> secondMaxHeap)
        {
            if (firstMaxHeap == null || secondMaxHeap == null)
                throw new ArgumentNullException("Null heaps are not allowed.");

            // Create a new heap with reserved size.
            var size = firstMaxHeap.Count + secondMaxHeap.Count;
            var newHeap = new BinaryMaxHeap<T>(size, Comparer<T>.Default);

            // Insert into the new heap.
            while (firstMaxHeap.IsEmpty == false)
                newHeap.Add(firstMaxHeap.ExtractMax());

            while (secondMaxHeap.IsEmpty == false)
                newHeap.Add(secondMaxHeap.ExtractMax());

            // Destroy the two heaps.
            firstMaxHeap = secondMaxHeap = null;

            return newHeap;
        }
    }

    public class BinaryMinHeap<T> : IMinHeap<T> where T : IComparable<T>
    {
        private readonly Comparer<T> _heapComparer = Comparer<T>.Default;


        /// <summary>
        ///     CONSTRUCTORS
        /// </summary>
        public BinaryMinHeap() : this(0, null)
        {
        }

        public BinaryMinHeap(Comparer<T> comparer) : this(0, comparer)
        {
        }

        public BinaryMinHeap(int capacity, Comparer<T> comparer)
        {
            _collection = new ArrayList<T>(capacity);
            _heapComparer = comparer ?? Comparer<T>.Default;
        }

        /// <summary>
        ///     Instance Variables.
        ///     _collection: The list of elements. Implemented as an array-based list with auto-resizing.
        /// </summary>
        private ArrayList<T> _collection { get; set; }

        /// <summary>
        ///     Gets or sets the at the specified index.
        /// </summary>
        /// <param name="index">Index.</param>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index > Count || Count == 0)
                {
                    throw new IndexOutOfRangeException();
                }

                return _collection[index];
            }
            set
            {
                if (index < 0 || index >= Count)
                {
                    throw new IndexOutOfRangeException();
                }

                _collection[index] = value;

                if (_heapComparer.Compare(_collection[index], _collection[0]) <= 0) // less than or equal to min
                {
                    _collection.Swap(0, index);
                    _buildMinHeap();
                }
            }
        }


        /// <summary>
        ///     Returns the number of elements in heap
        /// </summary>
        public int Count
        {
            get { return _collection.Count; }
        }

        /// <summary>
        ///     Checks whether this heap is empty
        /// </summary>
        public bool IsEmpty
        {
            get { return _collection.Count == 0; }
        }

        /// <summary>
        ///     Heapifies the specified newCollection. Overrides the current heap.
        /// </summary>
        /// <param name="newCollection">New collection.</param>
        public void Initialize(IList<T> newCollection)
        {
            if (newCollection.Count > 0)
            {
                // Reset and reserve the size of the newCollection
                _collection = new ArrayList<T>(newCollection.Count);

                // Copy the elements from the newCollection to the inner collection
                for (var i = 0; i < newCollection.Count; ++i)
                {
                    _collection.InsertAt(newCollection[i], i);
                }

                // Build the heap
                _buildMinHeap();
            }
        }

        /// <summary>
        ///     Adding a new key to the heap.
        /// </summary>
        /// <param name="heapKey">Heap key.</param>
        public void Add(T heapKey)
        {
            if (IsEmpty)
            {
                _collection.Add(heapKey);
            }
            else
            {
                _collection.Add(heapKey);
                _buildMinHeap();
            }
        }

        /// <summary>
        ///     Find the minimum node of a min heap.
        /// </summary>
        /// <returns>The minimum.</returns>
        public T Peek()
        {
            if (IsEmpty)
            {
                throw new Exception("Heap is empty.");
            }

            return _collection.First;
        }

        /// <summary>
        ///     Removes the node of minimum value from a min heap.
        /// </summary>
        public void RemoveMin()
        {
            if (IsEmpty)
            {
                throw new Exception("Heap is empty.");
            }

            var min = 0;
            var last = _collection.Count - 1;
            _collection.Swap(min, last);

            _collection.RemoveAt(last);
            last--;

            _minHeapify(0, last);
        }

        /// <summary>
        ///     Returns the node of minimum value from a min heap after removing it from the heap.
        /// </summary>
        /// <returns>The min.</returns>
        public T ExtractMin()
        {
            var min = Peek();
            RemoveMin();
            return min;
        }

        /// <summary>
        ///     Clear this heap.
        /// </summary>
        public void Clear()
        {
            if (IsEmpty)
            {
                throw new Exception("Heap is empty.");
            }

            _collection.Clear();
        }

        /// <summary>
        ///     Rebuilds the heap.
        /// </summary>
        public void RebuildHeap()
        {
            _buildMinHeap();
        }

        /// <summary>
        ///     Returns an array version of this heap.
        /// </summary>
        public T[] ToArray()
        {
            return _collection.ToArray();
        }

        /// <summary>
        ///     Returns a list version of this heap.
        /// </summary>
        public List<T> ToList()
        {
            return _collection.ToList();
        }

        /// <summary>
        ///     Returns a new max heap that contains all elements of this heap.
        /// </summary>
        public IMaxHeap<T> ToMaxHeap()
        {
            var newMaxHeap = new BinaryMaxHeap<T>(Count, _heapComparer);
            newMaxHeap.Initialize(_collection.ToArray());
            return newMaxHeap;
        }


        /// <summary>
        ///     Builds a min heap from the inner array-list _collection.
        /// </summary>
        private void _buildMinHeap()
        {
            var lastIndex = _collection.Count - 1;
            var lastNodeWithChildren = lastIndex/2;

            for (var node = lastNodeWithChildren; node >= 0; node--)
            {
                _minHeapify(node, lastIndex);
            }
        }

        /// <summary>
        ///     Private Method. Used in Building a Min Heap.
        /// </summary>
        /// <typeparam name="T">Type of Heap elements</typeparam>
        /// <param name="nodeIndex">The node index to heapify at.</param>
        /// <param name="lastIndex">The last index of collection to stop at.</param>
        private void _minHeapify(int nodeIndex, int lastIndex)
        {
            // assume that the subtrees left(node) and right(node) are max-heaps
            var left = nodeIndex*2 + 1;
            var right = left + 1;
            var smallest = nodeIndex;

            // If collection[left] < collection[nodeIndex]
            if (left <= lastIndex && _heapComparer.Compare(_collection[left], _collection[nodeIndex]) < 0)
                smallest = left;

            // If collection[right] < collection[smallest]
            if (right <= lastIndex && _heapComparer.Compare(_collection[right], _collection[smallest]) < 0)
                smallest = right;

            // Swap and heapify
            if (smallest != nodeIndex)
            {
                _collection.Swap(nodeIndex, smallest);
                _minHeapify(smallest, lastIndex);
            }
        }

        /// <summary>
        ///     Union two heaps together, returns a new min-heap of both heaps' elements,
        ///     ... and then destroys the original ones.
        /// </summary>
        public BinaryMinHeap<T> Union(ref BinaryMinHeap<T> firstMinHeap, ref BinaryMinHeap<T> secondMinHeap)
        {
            if (firstMinHeap == null || secondMinHeap == null)
                throw new ArgumentNullException("Null heaps are not allowed.");

            // Create a new heap with reserved size.
            var size = firstMinHeap.Count + secondMinHeap.Count;
            var newHeap = new BinaryMinHeap<T>(size, Comparer<T>.Default);

            // Insert into the new heap.
            while (firstMinHeap.IsEmpty == false)
                newHeap.Add(firstMinHeap.ExtractMin());

            while (secondMinHeap.IsEmpty == false)
                newHeap.Add(secondMinHeap.ExtractMin());

            // Destroy the two heaps.
            firstMinHeap = secondMinHeap = null;

            return newHeap;
        }
    }

    public class BinomialMinHeap<T> : IMinHeap<T> where T : IComparable<T>
    {
        private const int _defaultCapacity = 8;


        /// <summary>
        ///     CONSTRUCTORS
        /// </summary>
        public BinomialMinHeap()
            : this(8)
        {
            // Empty constructor
        }

        public BinomialMinHeap(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException();

            capacity = capacity < _defaultCapacity ? _defaultCapacity : capacity;

            _size = 0;
            _forest = new ArrayList<BinomialNode<T>>(capacity);
        }


        /// <summary>
        ///     INSTANCE VARIABLES
        /// </summary>
        private int _size { get; set; }

        private ArrayList<BinomialNode<T>> _forest { get; set; }


        /************************************************************************************************/
        /** PUBLIC API FUNCTIONS                                                                        */


        /// <summary>
        ///     Returns count of elements in heap.
        /// </summary>
        public int Count
        {
            get { return _size; }
        }

        /// <summary>
        ///     Checks if heap is empty
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty
        {
            get { return _size == 0; }
        }

        /// <summary>
        ///     Initializes this heap with a collection of elements.
        /// </summary>
        public void Initialize(IList<T> newCollection)
        {
            if (newCollection == null)
                throw new ArgumentNullException();

            if (newCollection.Count > ArrayList<T>.MAXIMUM_ARRAY_LENGTH_x64)
                throw new OverflowException();

            _forest = new ArrayList<BinomialNode<T>>(newCollection.Count + 1);

            for (var i = 0; i < newCollection.Count; ++i)
                Add(newCollection[i]);
        }

        /// <summary>
        ///     Inserts a new item to heap.
        /// </summary>
        public void Add(T heapKey)
        {
            var tempHeap = new BinomialMinHeap<T>();
            tempHeap._forest.Add(new BinomialNode<T>(heapKey));
            tempHeap._size = 1;

            // Merge this with tempHeap
            Merge(tempHeap);

            // Increase the _size
            ++_size;
        }

        /// <summary>
        ///     Return the min element.
        /// </summary>
        public T Peek()
        {
            if (IsEmpty)
                throw new Exception("Heap is empty.");

            var minIndex = _findMinIndex();
            var minValue = _forest[minIndex].Value;

            return minValue;
        }

        /// <summary>
        ///     Remove the min element from heap.
        /// </summary>
        public void RemoveMin()
        {
            if (IsEmpty)
                throw new Exception("Heap is empty.");

            _removeAtIndex(_findMinIndex());
        }

        /// <summary>
        ///     Return the min element and then remove it from heap.
        /// </summary>
        public T ExtractMin()
        {
            if (IsEmpty)
                throw new Exception("Heap is empty.");

            // Get the min-node index
            var minIndex = _findMinIndex();
            var minValue = _forest[minIndex].Value;

            // Remove min from heap
            _removeAtIndex(minIndex);

            return minValue;
        }

        /// <summary>
        ///     Returns an array copy of heap.
        /// </summary>
        public T[] ToArray()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Rebuilds the heap.
        /// </summary>
        public void RebuildHeap()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Returns a list copy of heap.
        /// </summary>
        public List<T> ToList()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Returns a binomial max-heap copy of this instance.
        /// </summary>
        public IMaxHeap<T> ToMaxHeap()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Clear this instance.
        /// </summary>
        public void Clear()
        {
            _size = 0;
            _forest.Clear();
        }


        /************************************************************************************************/
        /** PRIVATE HELPER FUNCTIONS                                                                    */


        /// <summary>
        ///     Removes root of tree at he specified index.
        /// </summary>
        private void _removeAtIndex(int minIndex)
        {
            // Get the deletedTree
            // The min-root lies at _forest[minIndex]
            var deletedTreeRoot = _forest[minIndex].Child;

            // Exit if there was no children under old-min-root
            if (deletedTreeRoot == null)
                return;

            // CONSTRUCT H'' (double-prime)
            var deletedForest = new BinomialMinHeap<T>();
            deletedForest._forest.Resize(minIndex + 1);
            deletedForest._size = (1 << minIndex) - 1;

            for (var i = minIndex - 1; i >= 0; --i)
            {
                deletedForest._forest[i] = deletedTreeRoot;
                deletedTreeRoot = deletedTreeRoot.Sibling;
                deletedForest._forest[i].Sibling = null;
            }

            // CONSTRUCT H' (single-prime)
            _forest[minIndex] = null;
            _size = deletedForest._size + 1;

            Merge(deletedForest);

            // Decrease the size
            --_size;
        }

        /// <summary>
        ///     Returns index of the tree with the minimum root's value.
        /// </summary>
        private int _findMinIndex()
        {
            int i, minIndex;

            // Loop until you reach a slot in the _forest that is not null.
            // The final value of "i" will be pointing to the non-null _forest slot.
            for (i = 0; i < _forest.Count && _forest[i] == null; ++i) ;

            // Loop over the trees in forest, and return the index of the slot that has the tree with the min-valued root
            for (minIndex = i; i < _forest.Count; ++i)
                if (_forest[i] != null && _forest[i].Value.IsLessThan(_forest[minIndex].Value))
                    minIndex = i;

            return minIndex;
        }

        /// <summary>
        ///     Combines two trees and returns the new tree root node.
        /// </summary>
        private BinomialNode<T> _combineTrees(BinomialNode<T> firstTreeRoot, BinomialNode<T> secondTreeRoot)
        {
            if (firstTreeRoot == null || secondTreeRoot == null)
                throw new ArgumentNullException("Either one of the nodes or both are null.");

            if (secondTreeRoot.Value.IsLessThan(firstTreeRoot.Value))
                return _combineTrees(secondTreeRoot, firstTreeRoot);

            secondTreeRoot.Sibling = firstTreeRoot.Child;
            firstTreeRoot.Child = secondTreeRoot;
            secondTreeRoot.Parent = firstTreeRoot;

            return firstTreeRoot;
        }

        /// <summary>
        ///     Clones a tree, given it's root node.
        /// </summary>
        private BinomialNode<T> _cloneTree(BinomialNode<T> treeRoot)
        {
            if (treeRoot == null)
                return null;
            return new BinomialNode<T>
            {
                Value = treeRoot.Value,
                Child = _cloneTree(treeRoot.Child),
                Sibling = _cloneTree(treeRoot.Sibling)
            };
        }

        /// <summary>
        ///     Merges the elements of another heap with this heap.
        /// </summary>
        public void Merge(BinomialMinHeap<T> otherHeap)
        {
            // Avoid aliasing problems
            if (this == otherHeap)
                return;

            // Avoid null or empty cases
            if (otherHeap == null || otherHeap.IsEmpty)
                return;

            BinomialNode<T> carryNode = null;
            _size = _size + otherHeap._size;

            // One capacity-change step
            if (_size > _forest.Count)
            {
                var newSize = Math.Max(_forest.Count, otherHeap._forest.Count) + 1;
                _forest.Resize(newSize);
            }

            for (int i = 0, j = 1; j <= _size; i++, j *= 2)
            {
                var treeRoot1 = _forest.IsEmpty ? null : _forest[i];
                var treeRoot2 = i < otherHeap._forest.Count ? otherHeap._forest[i] : null;

                var whichCase = treeRoot1 == null ? 0 : 1;
                whichCase += treeRoot2 == null ? 0 : 2;
                whichCase += carryNode == null ? 0 : 4;

                switch (whichCase)
                {
                    /*** SINGLE CASES ***/
                    case 0: /* No trees */
                    case 1: /* Only this */
                        break;
                    case 2: /* Only otherHeap */
                        _forest[i] = treeRoot2;
                        otherHeap._forest[i] = null;
                        break;
                    case 4: /* Only carryNode */
                        _forest[i] = carryNode;
                        carryNode = null;
                        break;

                    /*** BINARY CASES ***/
                    case 3: /* this and otherHeap */
                        carryNode = _combineTrees(treeRoot1, treeRoot2);
                        _forest[i] = otherHeap._forest[i] = null;
                        break;
                    case 5: /* this and carryNode */
                        carryNode = _combineTrees(treeRoot1, carryNode);
                        _forest[i] = null;
                        break;
                    case 6: /* otherHeap and carryNode */
                        carryNode = _combineTrees(treeRoot2, carryNode);
                        otherHeap._forest[i] = null;
                        break;
                    case 7: /* all the nodes */
                        _forest[i] = carryNode;
                        carryNode = _combineTrees(treeRoot1, treeRoot2);
                        otherHeap._forest[i] = null;
                        break;
                } //end-switch
            } //end-for

            // Clear otherHeap
            otherHeap.Clear();
        }

        /// <summary>
        ///     The Heap Node class.
        /// </summary>
        private class BinomialNode<T> where T : IComparable<T>
        {
            // Constructors
            public BinomialNode() : this(default(T), null, null, null)
            {
            }

            public BinomialNode(T value) : this(value, null, null, null)
            {
            }

            public BinomialNode(T value, BinomialNode<T> parent, BinomialNode<T> sibling, BinomialNode<T> child)
            {
                Value = value;
                Parent = parent;
                Sibling = sibling;
                Child = child;
            }

            public T Value { get; set; }
            public BinomialNode<T> Parent { get; set; }
            public BinomialNode<T> Sibling { get; set; } // Right-Sibling
            public BinomialNode<T> Child { get; set; } // Left-Child

            // Helper boolean flags
            public bool HasSiblings
            {
                get { return Sibling != null; }
            }

            public bool HasChildren
            {
                get { return Child != null; }
            }
        }
    }

    public interface IMaxHeap<T> where T : IComparable<T>
    {
        /// <summary>
        ///     Returns the number of elements in heap
        /// </summary>
        int Count { get; }

        /// <summary>
        ///     Checks whether this heap is empty
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        ///     Heapifies the specified newCollection. Overrides the current heap.
        /// </summary>
        /// <param name="newCollection">New collection.</param>
        void Initialize(IList<T> newCollection);

        /// <summary>
        ///     Adding a new key to the heap.
        /// </summary>
        /// <param name="heapKey">Heap key.</param>
        void Add(T heapKey);

        /// <summary>
        ///     Find the maximum node of a max heap.
        /// </summary>
        /// <returns>The maximum.</returns>
        T Peek();

        /// <summary>
        ///     Removes the node of maximum value from a max heap.
        /// </summary>
        void RemoveMax();

        /// <summary>
        ///     Returns the node of maximum value from a max heap after removing it from the heap.
        /// </summary>
        /// <returns>The max.</returns>
        T ExtractMax();

        /// <summary>
        ///     Clear this heap.
        /// </summary>
        void Clear();

        /// <summary>
        ///     Rebuilds the heap.
        /// </summary>
        void RebuildHeap();

        /// <summary>
        ///     Returns an array version of this heap.
        /// </summary>
        /// <returns>The array.</returns>
        T[] ToArray();

        /// <summary>
        ///     Returns a list version of this heap.
        /// </summary>
        /// <returns>The list.</returns>
        List<T> ToList();

        /// <summary>
        ///     Returns a new max heap that contains all elements of this heap.
        /// </summary>
        /// <returns>The max heap.</returns>
        IMinHeap<T> ToMinHeap();
    }

    public interface IMinHeap<T> where T : IComparable<T>
    {
        /// <summary>
        ///     Returns the number of elements in heap
        /// </summary>
        int Count { get; }

        /// <summary>
        ///     Checks whether this heap is empty
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        ///     Heapifies the specified newCollection. Overrides the current heap.
        /// </summary>
        /// <param name="newCollection">New collection.</param>
        void Initialize(IList<T> newCollection);

        /// <summary>
        ///     Adding a new key to the heap.
        /// </summary>
        /// <param name="heapKey">Heap key.</param>
        void Add(T heapKey);

        /// <summary>
        ///     Find the minimum node of a min heap.
        /// </summary>
        /// <returns>The minimum.</returns>
        T Peek();

        /// <summary>
        ///     Removes the node of minimum value from a min heap.
        /// </summary>
        void RemoveMin();

        /// <summary>
        ///     Returns the node of minimum value from a min heap after removing it from the heap.
        /// </summary>
        /// <returns>The min.</returns>
        T ExtractMin();

        /// <summary>
        ///     Clear this heap.
        /// </summary>
        void Clear();

        /// <summary>
        ///     Rebuilds the heap.
        /// </summary>
        void RebuildHeap();

        /// <summary>
        ///     Returns an array version of this heap.
        /// </summary>
        /// <returns>The array.</returns>
        T[] ToArray();

        /// <summary>
        ///     Returns a list version of this heap.
        /// </summary>
        /// <returns>The list.</returns>
        List<T> ToList();

        /// <summary>
        ///     Returns a new min heap that contains all elements of this heap.
        /// </summary>
        /// <returns>The min heap.</returns>
        IMaxHeap<T> ToMaxHeap();
    }

    /// <summary>
    ///     Implements the Keyed Priority Queue Data Structure.
    ///     All nodes have: a Key, a Value, a Priority
    ///     <typeparam name="K">Node's Key type</typeparam>
    ///     <typeparam name="V">Node's Value type</typeparam>
    ///     <typeparam name="P">Node's Priority type</typeparam>
    /// </summary>
    public class PriorityQueue<K, V, P> where P : IComparable<P>
    {
        /// <summary>
        ///     CONSTRUCTOR
        /// </summary>
        public PriorityQueue() : this(0, null)
        {
        }

        /// <summary>
        ///     CONSTRUCTOR
        /// </summary>
        /// <param name="capacity">Capacity of priority queue.</param>
        public PriorityQueue(int capacity) : this(capacity, null)
        {
        }

        /// <summary>
        ///     CONSTRUCTOR
        /// </summary>
        /// <param name="capacity">Capacity of priority queue.</param>
        /// <param name="priorityComparer">The node's priority comparer.</param>
        public PriorityQueue(int capacity, Comparer<PriorityQueueNode<K, V, P>> priorityComparer)
        {
            if (capacity >= 0)
            {
                if (priorityComparer == null)
                {
                    _priorityComparer = Comparer<PriorityQueueNode<K, V, P>>.Default;
                }
                else
                {
                    _priorityComparer = priorityComparer;
                }

                _heap = new BinaryMaxHeap<PriorityQueueNode<K, V, P>>(capacity, _priorityComparer);
                _keysMap = new Dictionary<K, int>();
            }
            else
            {
                throw new ArgumentOutOfRangeException("Please provide a valid capacity.");
            }
        }

        /// <summary>
        ///     Instance variables
        /// </summary>
        private BinaryMaxHeap<PriorityQueueNode<K, V, P>> _heap { get; }

        private Comparer<PriorityQueueNode<K, V, P>> _priorityComparer { get; }
        private Dictionary<K, int> _keysMap { get; }


        /// <summary>
        ///     Returns the count of elements in the queue.
        /// </summary>
        public int Count
        {
            get { return _heap.Count; }
        }


        /// <summary>
        ///     Checks if the queue is empty
        ///     <returns>True if queue is empty; false otherwise.</returns>
        /// </summary>
        public bool IsEmpty
        {
            get { return _heap.IsEmpty; }
        }


        /// <summary>
        ///     Returns an array of keys
        /// </summary>
        public K[] Keys
        {
            get
            {
                var keysArray = new K[_keysMap.Count];
                _keysMap.Keys.CopyTo(keysArray, 0);
                return keysArray;
            }
        }


        /// <summary>
        ///     Returns the highest priority element.
        /// </summary>
        /// <returns>The at highest priority.</returns>
        public V PeekAtHighestPriority()
        {
            if (_heap.IsEmpty)
            {
                throw new ArgumentOutOfRangeException("Queue is empty.");
            }

            return _heap.Peek().Value;
        }


        /// <summary>
        ///     Enqueue the specified key and value without priority.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public void Enqueue(K key, V value)
        {
            Enqueue(key, value, default(P));
        }


        /// <summary>
        ///     Enqueue the specified key, value and priority.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        /// <param name="priority">Priority.</param>
        public void Enqueue(K key, V value, P priority)
        {
            if (!_keysMap.ContainsKey(key))
            {
                _keysMap.Add(key, 1);
            }
            else
            {
                _keysMap[key] += 1;
            }

            var newNode = new PriorityQueueNode<K, V, P>(key, value, priority);
            _heap.Add(newNode);
        }


        /// <summary>
        ///     Dequeue this instance.
        /// </summary>
        public V Dequeue()
        {
            if (_heap.IsEmpty)
            {
                throw new ArgumentOutOfRangeException("Queue is empty.");
            }

            var highest = _heap.Peek();

            // Decrement the key's counter
            _keysMap[highest.Key] = _keysMap[highest.Key] - 1;
            if (_keysMap[highest.Key] == 0)
            {
                _keysMap.Remove(highest.Key);
            }

            _heap.RemoveMax();
            return highest.Value;
        }


        /// <summary>
        ///     Sets the priority.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="newPriority">New priority.</param>
        public void SetPriority(K key, P newPriority)
        {
            // Handle boundaries errors
            if (_heap.IsEmpty)
            {
                throw new ArgumentOutOfRangeException("Queue is empty.");
            }
            if (!_keysMap.ContainsKey(key))
            {
                throw new KeyNotFoundException();
            }

            var keyComparer = Comparer<K>.Default;

            for (var i = 0; i < _heap.Count; ++i)
            {
                if (keyComparer.Compare(_heap[i].Key, key) == 0)
                {
                    _heap[i].Priority = newPriority;
                    break;
                }
            }
        }


        /// <summary>
        ///     Clear this priority queue.
        /// </summary>
        public void Clear()
        {
            _heap.Clear();
        }

        //        new Predicate<PriorityQueueNode<K, V, P>> (
        //    Predicate<PriorityQueueNode<K, V, P>> match = 
        //
        //    var keyComparer = Comparer<K>.Default;
        //
        //    }
        //        throw new ArgumentOutOfRangeException ("Queue is empty.");
        //    {
        //    if (_heap.IsEmpty)
        //{
        //public void Remove(K key)
        ///// <param name="key">Key.</param>
        ///// </summary>
        ///// Removes the node that has the specified key.

        ///// <summary>
        //            item => keyComparer.Compare(item.Key, key) == 0);
        //
        //    _heap.RemoveAll (match);
        //}


        ///// <summary>
        ///// Removes the node that has the specified key and value.
        ///// </summary>
        ///// <param name="key">Key.</param>
        ///// <param name="value">Value.</param>
        //public void Remove(K key, V value)
        //{
        //    if (_heap.IsEmpty)
        //    {
        //        throw new ArgumentOutOfRangeException ("Queue is empty.");
        //    }
        //
        //    var keyComparer = Comparer<K>.Default;
        //    var valueComparer = Comparer<V>.Default;
        //
        //    Predicate<PriorityQueueNode<K, V, P>> match = 
        //        new Predicate<PriorityQueueNode<K, V, P>> (
        //            item => 
        //            keyComparer.Compare(item.Key, key) == 0 && 
        //            valueComparer.Compare(item.Value, value) == 0);
        //
        //    _heap.RemoveAll (match);
        //}
    }


    /// <summary>
    ///     The Priority-queue node.
    /// </summary>
    /// <typeparam name="K">Node's Key type</typeparam>
    /// <typeparam name="V">Node's Value type</typeparam>
    /// <typeparam name="P">Node's Priority type</typeparam>
    public class PriorityQueueNode<K, V, P> : IComparable<PriorityQueueNode<K, V, P>> where P : IComparable<P>
    {
        public PriorityQueueNode() : this(default(K), default(V), default(P))
        {
        }

        public PriorityQueueNode(K key, V value, P priority)
        {
            Key = key;
            Value = value;
            Priority = priority;
        }

        public K Key { get; set; }
        public V Value { get; set; }
        public P Priority { get; set; }

        public int CompareTo(PriorityQueueNode<K, V, P> other)
        {
            if (other == null)
                return -1;

            return Priority.CompareTo(other.Priority);
        }
    } //end-of-node-class


    /// <summary>
    ///     Keyed Priority-queue node comparer.
    /// </summary>
    public class PriorityQueueNodeComparer<K, V, P> : IComparer<PriorityQueueNode<K, V, P>> where P : IComparable<P>
    {
        public int Compare(PriorityQueueNode<K, V, P> first, PriorityQueueNode<K, V, P> second)
        {
            return first.Priority.CompareTo(second.Priority);
        }
    } //end-of-comparer-class

    /// <summary>
    ///     Implements the Priority Queue Data Structure.
    ///     <typeparam name="TKey">Node's Value type</typeparam>
    ///     <typeparam name="TPriority">Node's Priority type</typeparam>
    /// </summary>
    public class MinPriorityQueue<TKey, TPriority>
        where TKey : IComparable<TKey>
        where TPriority : IComparable<TPriority>
    {
        /// <summary>
        ///     CONSTRUCTOR
        /// </summary>
        public MinPriorityQueue() : this(0, null)
        {
        }

        public MinPriorityQueue(uint capacity) : this(capacity, null)
        {
        }

        public MinPriorityQueue(uint capacity, Comparer<PriorityQueueNode<TKey, TPriority>> priorityComparer)
        {
            // Make sure the TPriority is elegible for a priority
            if (!_validPriorityType())
                throw new NotSupportedException("The priority type is not supported.");

            // Initialize comparer
            if (priorityComparer == null)
                _priorityComparer = Comparer<PriorityQueueNode<TKey, TPriority>>.Default;
            else
                _priorityComparer = priorityComparer;

            // Initialize.
            _keys = new Dictionary<TKey, long>();
            _heap = new BinaryMinHeap<PriorityQueueNode<TKey, TPriority>>((int) capacity, _priorityComparer);
        }

        /// <summary>
        ///     Instance variables
        /// </summary>
        // A dictionary of keys and number of copies in the heap.
        private Dictionary<TKey, long> _keys { get; }

        // The internal heap.
        private BinaryMinHeap<PriorityQueueNode<TKey, TPriority>> _heap { get; }

        // The priorities value comparer.
        private Comparer<PriorityQueueNode<TKey, TPriority>> _priorityComparer { get; }


        /// <summary>
        ///     Returns the count of elements in the queue.
        /// </summary>
        public int Count
        {
            get { return _heap.Count; }
        }

        /// <summary>
        ///     Checks if the queue is empty
        /// </summary>
        public bool IsEmpty
        {
            get { return _heap.IsEmpty; }
        }

        /// <summary>
        ///     Get the default max priority, if set, raises an exception if not set.
        ///     Also sets the default max priority.
        /// </summary>
        public TPriority DefaultMaxPriority
        {
            get
            {
                object maxValue = default(TPriority);
                var typeCode = Type.GetTypeCode(typeof (TPriority));
                switch (typeCode)
                {
                    case TypeCode.Byte:
                        maxValue = byte.MaxValue;
                        break;
                    case TypeCode.Char:
                        maxValue = char.MaxValue;
                        break;
                    case TypeCode.DateTime:
                        maxValue = DateTime.MaxValue;
                        break;
                    case TypeCode.Decimal:
                        maxValue = decimal.MaxValue;
                        break;
                    case TypeCode.Double:
                        maxValue = decimal.MaxValue;
                        break;
                    case TypeCode.Int16:
                        maxValue = short.MaxValue;
                        break;
                    case TypeCode.Int32:
                        maxValue = int.MaxValue;
                        break;
                    case TypeCode.Int64:
                        maxValue = long.MaxValue;
                        break;
                    case TypeCode.SByte:
                        maxValue = sbyte.MaxValue;
                        break;
                    case TypeCode.Single:
                        maxValue = float.MaxValue;
                        break;
                    case TypeCode.UInt16:
                        maxValue = ushort.MaxValue;
                        break;
                    case TypeCode.UInt32:
                        maxValue = uint.MaxValue;
                        break;
                    case TypeCode.UInt64:
                        maxValue = ulong.MaxValue;
                        break;
                }

                return (TPriority) maxValue;
            }
        }


        /// <summary>
        ///     Validates the Type of TPriority. Returns true if acceptable, false otherwise.
        /// </summary>
        /// <returns></returns>
        private bool _validPriorityType()
        {
            var isValid = false;
            var typeCode = Type.GetTypeCode(typeof (TPriority));

            switch (typeCode)
            {
                //case TypeCode.DateTime:
                case TypeCode.Byte:
                case TypeCode.Char:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    isValid = true;
                    break;
                default:
                    isValid = false;
                    break;
            }

            return isValid;
        }

        /// <summary>
        ///     Returns the highest priority element.
        /// </summary>
        /// <returns>The at highest priority.</returns>
        public TKey PeekAtMinPriority()
        {
            if (_heap.IsEmpty)
            {
                throw new ArgumentOutOfRangeException("Queue is empty.");
            }

            return _heap.Peek().Key;
        }

        /// <summary>
        ///     Checks for the existence of a key in the queue
        /// </summary>
        public bool Contains(TKey key)
        {
            return _keys.ContainsKey(key);
        }

        /// <summary>
        ///     Enqueue the specified key, with the default-max-priority value.
        /// </summary>
        public void Enqueue(TKey key)
        {
            Enqueue(key, DefaultMaxPriority);
        }

        /// <summary>
        ///     Enqueue the specified key, value and priority.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="priority">Priority.</param>
        public void Enqueue(TKey key, TPriority priority)
        {
            var newNode = new PriorityQueueNode<TKey, TPriority>(key, priority);
            _heap.Add(newNode);

            if (_keys.ContainsKey(key))
                _keys[key] += 1;
            else
                _keys.Add(key, 1);
        }

        /// <summary>
        ///     Dequeue this instance.
        /// </summary>
        public TKey DequeueMin()
        {
            if (_heap.IsEmpty)
                throw new ArgumentOutOfRangeException("Queue is empty.");

            var key = _heap.ExtractMin().Key;

            // Decrease the key count.
            _keys[key] = _keys[key] - 1;

            // Remove key if its count is zero
            if (_keys[key] == 0)
                _keys.Remove(key);

            return key;
        }

        /// <summary>
        ///     Sets the priority.
        /// </summary>
        public void UpdatePriority(TKey key, TPriority newPriority)
        {
            // Handle boundaries errors
            if (_heap.IsEmpty)
                throw new ArgumentOutOfRangeException("Queue is empty.");
            if (!_keys.ContainsKey(key))
                throw new KeyNotFoundException();

            int i;
            for (i = 0; i < _heap.Count; ++i)
                if (_heap[i].Key.IsEqualTo(key))
                    break;

            _heap[i].Priority = newPriority;
        }

        /// <summary>
        ///     Clear this priority queue.
        /// </summary>
        public void Clear()
        {
            _heap.Clear();
            _keys.Clear();
        }
    }


    /// <summary>
    ///     The Priority-queue node.
    /// </summary>
    /// <typeparam name="K">Node's Key type</typeparam>
    /// <typeparam name="TKey">Node's Value type</typeparam>
    public class PriorityQueueNode<TKey, TPriority> : IComparable<PriorityQueueNode<TKey, TPriority>>
        where TKey : IComparable<TKey>
        where TPriority : IComparable<TPriority>
    {
        public PriorityQueueNode() : this(default(TKey), default(TPriority))
        {
        }

        public PriorityQueueNode(TKey value, TPriority priority)
        {
            Key = value;
            Priority = priority;
        }

        public TKey Key { get; set; }
        public TPriority Priority { get; set; }

        public int CompareTo(PriorityQueueNode<TKey, TPriority> other)
        {
            if (other == null)
                return -1;

            return Priority.CompareTo(other.Priority);
        }
    } //end-of-node-class
}