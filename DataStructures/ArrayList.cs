using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataStructures.Common;

namespace DataStructures.Lists
{
    public class ArrayList<T> : IEnumerable<T>
    {
        // The C# Maximum Array Length (before encountering overflow)
        // Reference: http://referencesource.microsoft.com/#mscorlib/system/array.cs,2d2b551eabe74985
        public const int MAXIMUM_ARRAY_LENGTH_x64 = 0X7FEFFFFF; //x64
        public const int MAXIMUM_ARRAY_LENGTH_x86 = 0x8000000; //x86

        // The default capacity to resize to, when a minimum is lower than 5.
        private const int _defaultCapacity = 8;

        // This is used as a default empty list initialization.
        private readonly T[] _emptyArray = new T[0];

        // The internal array of elements.
        // NOT A PROPERTY.
        private T[] _collection;

        /// <summary>
        ///     Instance variables.
        /// </summary>

        // This sets the default maximum array length to refer to MAXIMUM_ARRAY_LENGTH_x64
        // Set the flag IsMaximumCapacityReached to false
        private bool DefaultMaxCapacityIsX64 = true;

        private bool IsMaximumCapacityReached;


        /// <summary>
        ///     CONSTRUCTORS
        /// </summary>
        public ArrayList() : this(0)
        {
        }

        public ArrayList(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (capacity == 0)
            {
                _collection = _emptyArray;
            }
            else
            {
                _collection = new T[capacity];
            }

            // Zerofiy the _size;
            _size = 0;
        }

        // This keeps track of the number of elements added to the array.
        // Serves as an index of last item + 1.
        private int _size { get; set; }


        /// <summary>
        ///     Gets the the number of elements in list.
        /// </summary>
        /// <value>Int.</value>
        public int Count
        {
            get { return _size; }
        }


        /// <summary>
        ///     Returns the capacity of list, which is the total number of slots.
        /// </summary>
        public int Capacity
        {
            get { return _collection.Length; }
        }


        /// <summary>
        ///     Determines whether this list is empty.
        /// </summary>
        /// <returns><c>true</c> if list is empty; otherwise, <c>false</c>.</returns>
        public bool IsEmpty
        {
            get { return Count == 0; }
        }


        /// <summary>
        ///     Gets the first element in the list.
        /// </summary>
        /// <value>The first.</value>
        public T First
        {
            get
            {
                if (Count == 0)
                {
                    throw new IndexOutOfRangeException("List is empty.");
                }
                return _collection[0];
            }
        }


        /// <summary>
        ///     Gets the last element in the list.
        /// </summary>
        /// <value>The last.</value>
        public T Last
        {
            get
            {
                if (IsEmpty)
                {
                    throw new IndexOutOfRangeException("List is empty.");
                }
                return _collection[Count - 1];
            }
        }


        /// <summary>
        ///     Gets or sets the item at the specified index.
        ///     example: var a = list[0];
        ///     example: list[0] = 1;
        /// </summary>
        /// <param name="index">Index.</param>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _size)
                {
                    throw new IndexOutOfRangeException();
                }

                return _collection[index];
            }

            set
            {
                if (index < 0 || index >= _size)
                {
                    throw new IndexOutOfRangeException();
                }

                _collection[index] = value;
            }
        }


        /********************************************************************************/


        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < Count; i++)
            {
                yield return _collection[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        /// <summary>
        ///     Ensures the capacity.
        /// </summary>
        /// <param name="minCapacity">Minimum capacity.</param>
        private void _ensureCapacity(int minCapacity)
        {
            // If the length of the inner collection is less than the minCapacity
            // ... and if the maximum capacity wasn't reached yet, 
            // ... then maximize the inner collection.
            if (_collection.Length < minCapacity && IsMaximumCapacityReached == false)
            {
                var newCapacity = _collection.Length == 0 ? _defaultCapacity : _collection.Length*2;

                // Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
                // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
                var maxCapacity = DefaultMaxCapacityIsX64 ? MAXIMUM_ARRAY_LENGTH_x64 : MAXIMUM_ARRAY_LENGTH_x86;

                if (newCapacity < minCapacity)
                    newCapacity = minCapacity;

                if (newCapacity >= maxCapacity)
                {
                    newCapacity = maxCapacity - 1;
                    IsMaximumCapacityReached = true;
                }

                _resizeCapacity(newCapacity);
            }
        }


        /// <summary>
        ///     Resizes the collection to a new maximum number of capacity.
        /// </summary>
        /// <param name="newCapacity">New capacity.</param>
        private void _resizeCapacity(int newCapacity)
        {
            if (newCapacity != _collection.Length && newCapacity > _size)
            {
                try
                {
                    Array.Resize(ref _collection, newCapacity);
                }
                catch (OutOfMemoryException)
                {
                    if (DefaultMaxCapacityIsX64)
                    {
                        DefaultMaxCapacityIsX64 = false;
                        _ensureCapacity(newCapacity);
                    }

                    throw;
                }
            }
        }


        /// <summary>
        ///     Add the specified dataItem to list.
        /// </summary>
        /// <param name="dataItem">Data item.</param>
        public void Add(T dataItem)
        {
            if (_size == _collection.Length)
            {
                _ensureCapacity(_size + 1);
            }

            _collection[_size++] = dataItem;
        }


        /// <summary>
        ///     Adds an enumerable collection of items to list.
        /// </summary>
        /// <param name="elements"></param>
        public void AddRange(IEnumerable<T> elements)
        {
            if (elements == null)
                throw new ArgumentNullException();

            // make sure the size won't overflow by adding the range
            if ((uint) _size + elements.Count() > MAXIMUM_ARRAY_LENGTH_x64)
                throw new OverflowException();

            // grow the internal collection once to avoid doing multiple redundant grows
            if (elements.Any())
            {
                _ensureCapacity(_size + elements.Count());

                foreach (var element in elements)
                    Add(element);
            }
        }


        /// <summary>
        ///     Adds an element to list repeatedly for a specified count.
        /// </summary>
        public void AddRepeatedly(T value, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException();

            if ((uint) _size + count > MAXIMUM_ARRAY_LENGTH_x64)
                throw new OverflowException();

            // grow the internal collection once to avoid doing multiple redundant grows
            if (count > 0)
            {
                _ensureCapacity(_size + count);

                for (var i = 0; i < count; i++)
                    Add(value);
            }
        }


        /// <summary>
        ///     Inserts a new element at an index. Doesn't override the cell at index.
        /// </summary>
        /// <param name="dataItem">Data item to insert.</param>
        /// <param name="index">Index of insertion.</param>
        public void InsertAt(T dataItem, int index)
        {
            if (index < 0 || index > _size)
            {
                throw new IndexOutOfRangeException("Please provide a valid index.");
            }

            // If the inner array is full and there are no extra spaces, 
            // ... then maximize it's capacity to a minimum of _size + 1.
            if (_size == _collection.Length)
            {
                _ensureCapacity(_size + 1);
            }

            // If the index is not "at the end", then copy the elements of the array
            // ... between the specified index and the last index to the new range (index + 1, _size);
            // The cell at "index" will become available.
            if (index < _size)
            {
                Array.Copy(_collection, index, _collection, index + 1, _size - index);
            }

            // Write the dataItem to the available cell.
            _collection[index] = dataItem;

            // Increase the size.
            _size++;
        }


        /// <summary>
        ///     Removes the specified dataItem from list.
        /// </summary>
        /// <returns>>True if removed successfully, false otherwise.</returns>
        /// <param name="dataItem">Data item.</param>
        public bool Remove(T dataItem)
        {
            var index = IndexOf(dataItem);

            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }


        /// <summary>
        ///     Removes the list element at the specified index.
        /// </summary>
        /// <param name="index">Index of element.</param>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _size)
            {
                throw new IndexOutOfRangeException("Please pass a valid index.");
            }

            // Decrease the size by 1, to avoid doing Array.Copy if the element is to be removed from the tail of list. 
            _size--;

            // If the index is still less than size, perform an Array.Copy to override the cell at index.
            // This operation is O(N), where N = size - index.
            if (index < _size)
            {
                Array.Copy(_collection, index + 1, _collection, index, _size - index);
            }

            // Reset the writable cell to the default value of type T.
            _collection[_size] = default(T);
        }


        /// <summary>
        ///     Clear this instance.
        /// </summary>
        public void Clear()
        {
            if (_size > 0)
            {
                _size = 0;
                Array.Clear(_collection, 0, _size);
                _collection = _emptyArray;
            }
        }


        /// <summary>
        ///     Resize the List to a new size.
        /// </summary>
        public void Resize(int newSize)
        {
            Resize(newSize, default(T));
        }


        /// <summary>
        ///     Resize the list to a new size.
        /// </summary>
        public void Resize(int newSize, T defaultValue)
        {
            var currentSize = Count;

            if (newSize < currentSize)
            {
                _ensureCapacity(newSize);
            }
            else if (newSize > currentSize)
            {
                // Optimisation step.
                // This is just to avoid multiple automatic capacity changes.
                if (newSize > _collection.Length)
                    _ensureCapacity(newSize + 1);

                AddRange(Enumerable.Repeat(defaultValue, newSize - currentSize));
            }
        }


        /// <summary>
        ///     Reverses this list.
        /// </summary>
        public void Reverse()
        {
            Reverse(0, _size);
        }


        /// <summary>
        ///     Reverses the order of a number of elements. Starting a specific index.
        /// </summary>
        /// <param name="startIndex">Start index.</param>
        /// <param name="count">Count of elements to reverse.</param>
        public void Reverse(int startIndex, int count)
        {
            // Handle the bounds of startIndex
            if (startIndex < 0 || startIndex >= _size)
            {
                throw new IndexOutOfRangeException("Please pass a valid starting index.");
            }

            // Handle the bounds of count and startIndex with respect to _size.
            if (count < 0 || startIndex > _size - count)
            {
                throw new ArgumentOutOfRangeException();
            }

            // Use Array.Reverse
            // Running complexity is better than O(N). But unknown.
            // Array.Reverse uses the closed-source function TrySZReverse.
            Array.Reverse(_collection, startIndex, count);
        }


        /// <summary>
        ///     For each element in list, apply the specified action to it.
        /// </summary>
        /// <param name="action">Typed Action.</param>
        public void ForEach(Action<T> action)
        {
            // Null actions are not allowed.
            if (action == null)
            {
                throw new ArgumentNullException();
            }

            for (var i = 0; i < _size; ++i)
            {
                action(_collection[i]);
            }
        }


        /// <summary>
        ///     Checks whether the list contains the specified dataItem.
        /// </summary>
        /// <returns>True if list contains the dataItem, false otherwise.</returns>
        /// <param name="dataItem">Data item.</param>
        public bool Contains(T dataItem)
        {
            // Null-value check
            if (dataItem == null)
            {
                for (var i = 0; i < _size; ++i)
                {
                    if (_collection[i] == null) return true;
                }
            }
            else
            {
                // Construct a default equality comparer for this Type T
                // Use it to get the equal match for the dataItem
                var comparer = EqualityComparer<T>.Default;

                for (var i = 0; i < _size; ++i)
                {
                    if (comparer.Equals(_collection[i], dataItem)) return true;
                }
            }

            return false;
        }


        /// <summary>
        ///     Checks whether the list contains the specified dataItem.
        /// </summary>
        /// <returns>True if list contains the dataItem, false otherwise.</returns>
        /// <param name="dataItem">Data item.</param>
        /// <param name="comparer">The Equality Comparer object.</param>
        public bool Contains(T dataItem, IEqualityComparer<T> comparer)
        {
            // Null comparers are not allowed.
            if (comparer == null)
            {
                throw new ArgumentNullException();
            }

            // Null-value check
            if (dataItem == null)
            {
                for (var i = 0; i < _size; ++i)
                {
                    if (_collection[i] == null) return true;
                }
            }
            else
            {
                for (var i = 0; i < _size; ++i)
                {
                    if (comparer.Equals(_collection[i], dataItem)) return true;
                }
            }

            return false;
        }


        /// <summary>
        ///     Checks whether an item specified via a Predicate<T> function exists exists in list.
        /// </summary>
        /// <param name="searchMatch">Match predicate.</param>
        public bool Exists(Predicate<T> searchMatch)
        {
            // Use the FindIndex to look through the collection
            // If the returned index != -1 then it does exist, otherwise it doesn't.
            return FindIndex(searchMatch) != -1;
        }


        /// <summary>
        ///     Finds the index of the element that matches the predicate.
        /// </summary>
        /// <returns>The index of element if found, -1 otherwise.</returns>
        /// <param name="searchMatch">Match predicate.</param>
        public int FindIndex(Predicate<T> searchMatch)
        {
            return FindIndex(0, _size, searchMatch);
        }


        /// <summary>
        ///     Finds the index of the element that matches the predicate.
        /// </summary>
        /// <returns>The index of the element if found, -1 otherwise.</returns>
        /// <param name="startIndex">Starting index to search from.</param>
        /// <param name="searchMatch">Match predicate.</param>
        public int FindIndex(int startIndex, Predicate<T> searchMatch)
        {
            return FindIndex(startIndex, _size - startIndex, searchMatch);
        }


        /// <summary>
        ///     Finds the index of the first element that matches the given predicate function.
        /// </summary>
        /// <returns>The index of element if found, -1 if not found.</returns>
        /// <param name="startIndex">Starting index of search.</param>
        /// <param name="count">Count of elements to search through.</param>
        /// <param name="searchMatch">Match predicate function.</param>
        public int FindIndex(int startIndex, int count, Predicate<T> searchMatch)
        {
            // Check bound of startIndex
            if (startIndex < 0 || startIndex > _size)
            {
                throw new IndexOutOfRangeException("Please pass a valid starting index.");
            }

            // CHeck the bounds of count and startIndex with respect to _size
            if (count < 0 || startIndex > _size - count)
            {
                throw new ArgumentOutOfRangeException();
            }

            // Null match-predicates are not allowed
            if (searchMatch == null)
            {
                throw new ArgumentNullException();
            }

            // Start the search
            var endIndex = startIndex + count;
            for (var index = startIndex; index < endIndex; ++index)
            {
                if (searchMatch(_collection[index])) return index;
            }

            // Not found, return -1
            return -1;
        }


        /// <summary>
        ///     Returns the index of a given dataItem.
        /// </summary>
        /// <returns>Index of element in list.</returns>
        /// <param name="dataItem">Data item.</param>
        public int IndexOf(T dataItem)
        {
            return IndexOf(dataItem, 0, _size);
        }


        /// <summary>
        ///     Returns the index of a given dataItem.
        /// </summary>
        /// <returns>Index of element in list.</returns>
        /// <param name="dataItem">Data item.</param>
        /// <param name="startIndex">The starting index to search from.</param>
        public int IndexOf(T dataItem, int startIndex)
        {
            return IndexOf(dataItem, startIndex, _size);
        }


        /// <summary>
        ///     Returns the index of a given dataItem.
        /// </summary>
        /// <returns>Index of element in list.</returns>
        /// <param name="dataItem">Data item.</param>
        /// <param name="startIndex">The starting index to search from.</param>
        /// <param name="count">Count of elements to search through.</param>
        public int IndexOf(T dataItem, int startIndex, int count)
        {
            // Check the bound of the starting index.
            if (startIndex < 0 || (uint) startIndex > (uint) _size)
            {
                throw new IndexOutOfRangeException("Please pass a valid starting index.");
            }

            // Check the bounds of count and starting index with respect to _size.
            if (count < 0 || startIndex > _size - count)
            {
                throw new ArgumentOutOfRangeException();
            }

            // Everything is cool, start looking for the index
            // Use the Array.IndexOf
            // Array.IndexOf has a O(n) running time complexity, where: "n = count - size".
            // Array.IndexOf uses EqualityComparer<T>.Default to return the index of element which loops
            // ... over all the elements in the range [startIndex,count) in the array.
            return Array.IndexOf(_collection, dataItem, startIndex, count);
        }


        /// <summary>
        ///     Find the specified element that matches the Search Predication.
        /// </summary>
        /// <param name="searchMatch">Match predicate.</param>
        public T Find(Predicate<T> searchMatch)
        {
            // Null Predicate functions are not allowed. 
            if (searchMatch == null)
            {
                throw new ArgumentNullException();
            }

            // Begin searching, and return the matched element
            for (var i = 0; i < _size; ++i)
            {
                if (searchMatch(_collection[i]))
                {
                    return _collection[i];
                }
            }

            // Not found, return the default value of the type T.
            return default(T);
        }


        /// <summary>
        ///     Finds all the elements that match the typed Search Predicate.
        /// </summary>
        /// <returns>ArrayList<T> of matched elements. Empty list is returned if not element was found.</returns>
        /// <param name="searchMatch">Match predicate.</param>
        public ArrayList<T> FindAll(Predicate<T> searchMatch)
        {
            // Null Predicate functions are not allowed. 
            if (searchMatch == null)
            {
                throw new ArgumentNullException();
            }

            var matchedElements = new ArrayList<T>();

            // Begin searching, and add the matched elements to the new list.
            for (var i = 0; i < _size; ++i)
            {
                if (searchMatch(_collection[i]))
                {
                    matchedElements.Add(_collection[i]);
                }
            }

            // Return the new list of elements.
            return matchedElements;
        }


        /// <summary>
        ///     Get a range of elements, starting from an index..
        /// </summary>
        /// <returns>The range as ArrayList<T>.</returns>
        /// <param name="startIndex">Start index to get range from.</param>
        /// <param name="count">Count of elements.</param>
        public ArrayList<T> GetRange(int startIndex, int count)
        {
            // Handle the bound errors of startIndex
            if (startIndex < 0 || (uint) startIndex > (uint) _size)
            {
                throw new IndexOutOfRangeException("Please provide a valid starting index.");
            }

            // Handle the bound errors of count and startIndex with respect to _size
            if (count < 0 || startIndex > _size - count)
            {
                throw new ArgumentOutOfRangeException();
            }

            var newArrayList = new ArrayList<T>(count);

            // Use Array.Copy to quickly copy the contents from this array to the new list's inner array.
            Array.Copy(_collection, startIndex, newArrayList._collection, 0, count);

            // Assign count to the new list's inner _size counter.
            newArrayList._size = count;

            return newArrayList;
        }


        /// <summary>
        ///     Return an array version of this list.
        /// </summary>
        /// <returns>Array.</returns>
        public T[] ToArray()
        {
            var newArray = new T[Count];

            if (Count > 0)
            {
                Array.Copy(_collection, 0, newArray, 0, Count);
            }

            return newArray;
        }


        /// <summary>
        ///     Return an array version of this list.
        /// </summary>
        /// <returns>Array.</returns>
        public List<T> ToList()
        {
            var newList = new List<T>(Count);

            if (Count > 0)
            {
                for (var i = 0; i < Count; ++i)
                {
                    newList.Add(_collection[i]);
                }
            }

            return newList;
        }


        /// <summary>
        ///     Return a human readable, multi-line, print-out (string) of this list.
        /// </summary>
        /// <returns>The human readable string.</returns>
        /// <param name="addHeader">
        ///     If set to <c>true</c> a header with count and Type is added; otherwise, only elements are
        ///     printed.
        /// </param>
        public string ToHumanReadable(bool addHeader = false)
        {
            var i = 0;
            var listAsString = string.Empty;

            var preLineIndent = addHeader == false ? "" : "\t";

            for (i = 0; i < Count; ++i)
            {
                listAsString = string.Format("{0}{1}[{2}] => {3}\r\n", listAsString, preLineIndent, i, _collection[i]);
            }

            if (addHeader)
            {
                listAsString = string.Format("ArrayList of count: {0}.\r\n(\r\n{1})", Count, listAsString);
            }

            return listAsString;
        }
    }

    /// <summary>
    ///     The Doubly-Linked List Node class.
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    public class DLinkedListNode<T> : IComparable<DLinkedListNode<T>> where T : IComparable<T>
    {
        public DLinkedListNode() : this(default(T))
        {
        }

        public DLinkedListNode(T dataItem) : this(dataItem, null, null)
        {
        }

        public DLinkedListNode(T dataItem, DLinkedListNode<T> next, DLinkedListNode<T> previous)
        {
            Data = dataItem;
            Next = next;
            Previous = previous;
        }

        public virtual T Data { get; set; }

        public virtual DLinkedListNode<T> Next { get; set; }

        public virtual DLinkedListNode<T> Previous { get; set; }

        public int CompareTo(DLinkedListNode<T> other)
        {
            if (other == null) return -1;

            return Data.CompareTo(other.Data);
        }
    }


    /***********************************************************************************/


    /// <summary>
    ///     Doubly-Linked List Data Structure.
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    public class DLinkedList<T> : IEnumerable<T> where T : IComparable<T>
    {
        /// <summary>
        ///     Instance variables.
        /// </summary>
        private int _count;


        /// <summary>
        ///     CONSTRUCTOR
        /// </summary>
        public DLinkedList()
        {
            _count = 0;
            _firstNode = null;
            _lastNode = null;
        }

        private DLinkedListNode<T> _firstNode { get; set; }
        private DLinkedListNode<T> _lastNode { get; set; }

        public virtual DLinkedListNode<T> Head
        {
            get { return _firstNode; }
        }

        public virtual int Count
        {
            get { return _count; }
        }

        /// <summary>
        ///     Getter function that returns the first element
        /// </summary>
        public virtual T First
        {
            get
            {
                if (IsEmpty())
                {
                    throw new Exception("Empty list.");
                }
                return _firstNode.Data;
            }
        }

        /// <summary>
        ///     Getter function that returns the last element
        /// </summary>
        public virtual T Last
        {
            get
            {
                if (IsEmpty())
                {
                    throw new Exception("Empty list.");
                }
                if (_lastNode == null)
                {
                    var currentNode = _firstNode;
                    while (currentNode.Next != null)
                    {
                        currentNode = currentNode.Next;
                    }
                    _lastNode = currentNode;
                    return currentNode.Data;
                }
                return _lastNode.Data;
            }
        }

        /// <summary>
        ///     Implements the collection-index operator.
        ///     Gets or sets the element at the specified index
        /// </summary>
        /// <param name="index">Index of element.</param>
        public virtual T this[int index]
        {
            get { return _getElementAt(index); }
            set { _setElementAt(index, value); }
        }

        /********************************************************************************/

        public IEnumerator<T> GetEnumerator()
        {
            var node = _firstNode;
            while (node != null)
            {
                yield return node.Data;
                node = node.Next;
            }

            // Alternative: IEnumerator class instance
            // return new DLinkedListEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();

            // Alternative: IEnumerator class instance
            // return new DLinkedListEnumerator(this);
        }


        /// <summary>
        ///     Gets the element at the specified index
        /// </summary>
        /// <param name="index">Index of element</param>
        /// <returns>Element</returns>
        protected virtual T _getElementAt(int index)
        {
            if (IsEmpty() || index < 0 || index >= Count)
                throw new IndexOutOfRangeException("List is empty.");

            if (index == 0)
            {
                return First;
            }
            if (index == Count - 1)
            {
                return Last;
            }
            DLinkedListNode<T> currentNode = null;

            // Decide from which reference to traverse the list, and then move the currentNode reference to the index
            // If index > half then traverse it from the end (_lastNode reference)
            // Otherwise, traverse it from the beginning (_firstNode refrence)
            if (index > Count/2)
            {
                currentNode = _lastNode;
                for (var i = Count - 1; i > index; --i)
                {
                    currentNode = currentNode.Previous;
                }
            }
            else
            {
                currentNode = _firstNode;
                for (var i = 0; i < index; ++i)
                {
                    currentNode = currentNode.Next;
                }
            }

            return currentNode.Data;
        }

        /// <summary>
        ///     Sets the value of the element at the specified index
        /// </summary>
        /// <param name="index">Index of element to update.</param>
        /// <returns>Element</returns>
        protected virtual void _setElementAt(int index, T value)
        {
            if (IsEmpty() || index < 0 || index >= Count)
                throw new IndexOutOfRangeException("List is empty.");

            if (index == 0)
            {
                _firstNode.Data = value;
            }
            else if (index == Count - 1)
            {
                _lastNode.Data = value;
            }
            else
            {
                DLinkedListNode<T> currentNode = null;

                // Decide from which reference to traverse the list, and then move the currentNode reference to the index
                // If index > half then traverse it from the end (_lastNode reference)
                // Otherwise, traverse it from the beginning (_firstNode refrence)
                if (index > Count/2)
                {
                    currentNode = _lastNode;
                    for (var i = Count - 1; i > index; --i)
                    {
                        currentNode = currentNode.Previous;
                    }
                }
                else
                {
                    currentNode = _firstNode;
                    for (var i = 0; i < index; ++i)
                    {
                        currentNode = currentNode.Next;
                    }
                }

                currentNode.Data = value;
            }
        }

        /// <summary>
        ///     Determines whether this List is empty.
        /// </summary>
        /// <returns><c>true</c> if this list is empty; otherwise, <c>false</c>.</returns>
        public virtual bool IsEmpty()
        {
            return Count == 0;
        }

        /// <summary>
        ///     Returns the index of an item if exists.
        /// </summary>
        public virtual int IndexOf(T dataItem)
        {
            var i = 0;
            var found = false;
            var currentNode = _firstNode;

            // Get currentNode to reference the element at the index.
            while (i < Count)
            {
                if (currentNode.Data.IsEqualTo(dataItem))
                {
                    found = true;
                    break;
                }

                currentNode = currentNode.Next;
                i++;
            } //end-while

            return found ? i : -1;
        }

        /// <summary>
        ///     Prepend the specified dataItem at the beginning of the list.
        /// </summary>
        /// <param name="dataItem">Data item.</param>
        public virtual void Prepend(T dataItem)
        {
            var newNode = new DLinkedListNode<T>(dataItem);

            if (_firstNode == null)
            {
                _firstNode = _lastNode = newNode;
            }
            else
            {
                var currentNode = _firstNode;
                newNode.Next = currentNode;
                currentNode.Previous = newNode;
                _firstNode = newNode;
            }

            // Increment the count.
            _count++;
        }

        /// <summary>
        ///     Append the specified dataItem at the end of the list.
        /// </summary>
        /// <param name="dataItem">Data item.</param>
        public virtual void Append(T dataItem)
        {
            var newNode = new DLinkedListNode<T>(dataItem);

            if (_firstNode == null)
            {
                _firstNode = _lastNode = newNode;
            }
            else
            {
                var currentNode = _lastNode;
                currentNode.Next = newNode;
                newNode.Previous = currentNode;
                _lastNode = newNode;
            }

            // Increment the count.
            _count++;
        }

        /// <summary>
        ///     Inserts the dataItem at the specified index.
        /// </summary>
        /// <param name="dataItem">Data item.</param>
        /// <param name="index">Index.</param>
        public virtual void InsertAt(T dataItem, int index)
        {
            if (index < 0 || index > Count)
                throw new IndexOutOfRangeException();

            if (index == 0)
            {
                Prepend(dataItem);
            }
            else if (index == Count)
            {
                Append(dataItem);
            }
            else
            {
                DLinkedListNode<T> currentNode = null;
                var newNode = new DLinkedListNode<T>(dataItem);

                currentNode = _firstNode;
                for (var i = 0; i < index - 1; ++i)
                {
                    currentNode = currentNode.Next;
                }

                var oldNext = currentNode.Next;

                if (oldNext != null)
                    currentNode.Next.Previous = newNode;

                newNode.Next = oldNext;
                currentNode.Next = newNode;
                newNode.Previous = currentNode;

                // Increment the count
                _count++;
            }
        }

        /// <summary>
        ///     Inserts the dataItem after specified index.
        /// </summary>
        /// <param name="dataItem">Data item.</param>
        /// <param name="index">Index.</param>
        public virtual void InsertAfter(T dataItem, int index)
        {
            // Insert at previous index.
            InsertAt(dataItem, index - 1);
        }

        /// <summary>
        ///     Remove the specified dataItem.
        /// </summary>
        public virtual void Remove(T dataItem)
        {
            // Handle index out of bound errors
            if (IsEmpty())
                throw new IndexOutOfRangeException();

            if (_firstNode.Data.IsEqualTo(dataItem))
            {
                _firstNode = _firstNode.Next;

                if (_firstNode != null)
                    _firstNode.Previous = null;
            }
            else if (_lastNode.Data.IsEqualTo(dataItem))
            {
                _lastNode = _lastNode.Previous;

                if (_lastNode != null)
                    _lastNode.Next = null;
            }
            else
            {
                // Remove
                var currentNode = _firstNode;

                // Get currentNode to reference the element at the index.
                while (currentNode.Next != null)
                {
                    if (currentNode.Data.IsEqualTo(dataItem))
                        break;

                    currentNode = currentNode.Next;
                } //end-while

                // Throw exception if item was not found
                if (!currentNode.Data.IsEqualTo(dataItem))
                    throw new Exception("Item was not found!");

                // Remove element
                var newPrevious = currentNode.Previous;
                var newNext = currentNode.Next;

                if (newPrevious != null)
                    newPrevious.Next = newNext;

                if (newNext != null)
                    newNext.Previous = newPrevious;

                currentNode = newPrevious;
            }

            // Decrement count.
            _count--;
        }

        /// <summary>
        ///     Remove the specified dataItem.
        /// </summary>
        public virtual void RemoveFirstMatch(Predicate<T> match)
        {
            // Handle index out of bound errors
            if (IsEmpty())
                throw new IndexOutOfRangeException();

            if (match(_firstNode.Data))
            {
                _firstNode = _firstNode.Next;

                if (_firstNode != null)
                    _firstNode.Previous = null;
            }
            else if (match(_lastNode.Data))
            {
                _lastNode = _lastNode.Previous;

                if (_lastNode != null)
                    _lastNode.Next = null;
            }
            else
            {
                // Remove
                var currentNode = _firstNode;

                // Get currentNode to reference the element at the index.
                while (currentNode.Next != null)
                {
                    if (match(currentNode.Data))
                        break;

                    currentNode = currentNode.Next;
                } //end-while

                // If we reached the last node and item was not found
                // Throw exception
                if (!match(currentNode.Data))
                    throw new Exception("Item was not found!");

                // Remove element
                var newPrevious = currentNode.Previous;
                var newNext = currentNode.Next;

                if (newPrevious != null)
                    newPrevious.Next = newNext;

                if (newNext != null)
                    newNext.Previous = newPrevious;

                currentNode = newPrevious;
            }

            // Decrement count.
            _count--;
        }

        /// <summary>
        ///     Removes the item at the specified index.
        /// </summary>
        /// <returns>True if removed successfully, false otherwise.</returns>
        /// <param name="index">Index of item.</param>
        public virtual void RemoveAt(int index)
        {
            // Handle index out of bound errors
            if (IsEmpty() || index < 0 || index >= Count)
                throw new IndexOutOfRangeException();

            // Remove
            if (index == 0)
            {
                _firstNode = _firstNode.Next;

                if (_firstNode != null)
                    _firstNode.Previous = null;
            }
            else if (index == Count - 1)
            {
                _lastNode = _lastNode.Previous;

                if (_lastNode != null)
                    _lastNode.Next = null;
            }
            else
            {
                var i = 0;
                var currentNode = _firstNode;

                // Get currentNode to reference the element at the index.
                while (i < index)
                {
                    currentNode = currentNode.Next;
                    i++;
                } //end-while


                // Remove element
                var newPrevious = currentNode.Previous;
                var newNext = currentNode.Next;
                newPrevious.Next = newNext;

                if (newNext != null)
                    newNext.Previous = newPrevious;

                currentNode = newPrevious;
            } //end-else

            // Decrement count.
            _count--;
        }

        /// <summary>
        ///     Clears the list.
        /// </summary>
        public virtual void Clear()
        {
            _count = 0;
            _firstNode = _lastNode = null;
        }

        /// <summary>
        ///     Chesk whether the specified element exists in the list.
        /// </summary>
        /// <param name="dataItem">Value to check for.</param>
        /// <returns>True if found; false otherwise.</returns>
        public virtual bool Contains(T dataItem)
        {
            try
            {
                return Find(dataItem).IsEqualTo(dataItem);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Find the specified item in the list.
        /// </summary>
        /// <param name="dataItem">Value to find.</param>
        /// <returns>value.</returns>
        public virtual T Find(T dataItem)
        {
            if (IsEmpty())
                throw new Exception("List is empty.");

            var currentNode = _firstNode;
            while (currentNode != null)
            {
                if (currentNode.Data.IsEqualTo(dataItem))
                    return dataItem;

                currentNode = currentNode.Next;
            }

            throw new Exception("Item was not found.");
        }

        /// <summary>
        ///     Tries to find a match for the predicate. Returns true if found; otherwise false.
        /// </summary>
        public virtual bool TryFindFirst(Predicate<T> match, out T found)
        {
            // Initialize the output parameter
            found = default(T);

            if (IsEmpty())
                return false;

            var currentNode = _firstNode;

            try
            {
                while (currentNode != null)
                {
                    if (match(currentNode.Data))
                    {
                        found = currentNode.Data;
                        return true;
                    }

                    currentNode = currentNode.Next;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Find the first element that matches the predicate from all elements in list.
        /// </summary>
        public virtual T FindFirst(Predicate<T> match)
        {
            if (IsEmpty())
                throw new Exception("List is empty.");

            var currentNode = _firstNode;

            while (currentNode != null)
            {
                if (match(currentNode.Data))
                    return currentNode.Data;

                currentNode = currentNode.Next;
            }

            throw new KeyNotFoundException();
        }

        /// <summary>
        ///     Find all elements in list that match the predicate.
        /// </summary>
        /// <param name="match">Predicate function.</param>
        /// <returns>List of elements.</returns>
        public virtual List<T> FindAll(Predicate<T> match)
        {
            if (IsEmpty())
                throw new Exception("List is empty.");

            var currentNode = _firstNode;
            var list = new List<T>();

            while (currentNode != null)
            {
                if (match(currentNode.Data))
                    list.Add(currentNode.Data);

                currentNode = currentNode.Next;
            }

            return list;
        }

        /// <summary>
        ///     Returns a number of elements as specified by countOfElements, starting from the specified index.
        /// </summary>
        /// <param name="index">Starting index.</param>
        /// <param name="countOfElements">The number of elements to return.</param>
        /// <returns>Doubly-Linked List of elements</returns>
        public virtual DLinkedList<T> GetRange(int index, int countOfElements)
        {
            DLinkedListNode<T> currentNode = null;
            var newList = new DLinkedList<T>();

            // Handle Index out of Bound errors
            if (Count == 0)
            {
                return newList;
            }
            if (index < 0 || index > Count)
            {
                throw new IndexOutOfRangeException();
            }

            // Decide from which reference to traverse the list, and then move the currentNode reference to the index
            // If index > half then traverse it from the end (_lastNode reference)
            // Otherwise, traverse it from the beginning (_firstNode refrence)
            if (index > Count/2)
            {
                currentNode = _lastNode;
                for (var i = Count - 1; i > index; --i)
                {
                    currentNode = currentNode.Previous;
                }
            }
            else
            {
                currentNode = _firstNode;
                for (var i = 0; i < index; ++i)
                {
                    currentNode = currentNode.Next;
                }
            }

            // Append the elements to the new list using the currentNode reference
            while (currentNode != null && newList.Count <= countOfElements)
            {
                newList.Append(currentNode.Data);
                currentNode = currentNode.Next;
            }

            return newList;
        }

        /// <summary>
        ///     Sorts the entire list using Selection Sort.
        /// </summary>
        public virtual void SelectionSort()
        {
            if (IsEmpty())
                return;

            var currentNode = _firstNode;
            while (currentNode != null)
            {
                var nextNode = currentNode.Next;
                while (nextNode != null)
                {
                    if (nextNode.Data.IsLessThan(currentNode.Data))
                    {
                        var temp = nextNode.Data;
                        nextNode.Data = currentNode.Data;
                        currentNode.Data = temp;
                    }

                    nextNode = nextNode.Next;
                }

                currentNode = currentNode.Next;
            }
        }

        /// <summary>
        ///     Return an array version of this list.
        /// </summary>
        /// <returns></returns>
        public virtual T[] ToArray()
        {
            var array = new T[Count];

            var currentNode = _firstNode;
            for (var i = 0; i < Count; ++i)
            {
                if (currentNode != null)
                {
                    array[i] = currentNode.Data;
                    currentNode = currentNode.Next;
                }
                else
                {
                    break;
                }
            }

            return array;
        }

        /// <summary>
        ///     Returns a System.List version of this DLList instace.
        /// </summary>
        /// <returns>System.List of elements</returns>
        public virtual List<T> ToList()
        {
            var list = new List<T>(Count);

            var currentNode = _firstNode;
            for (var i = 0; i < Count; ++i)
            {
                if (currentNode != null)
                {
                    list.Add(currentNode.Data);
                    currentNode = currentNode.Next;
                }
                else
                {
                    break;
                }
            }

            return list;
        }

        /// <summary>
        ///     Returns the list items as a readable multi--line string.
        /// </summary>
        /// <returns></returns>
        public virtual string ToReadable()
        {
            var listAsString = string.Empty;
            var i = 0;
            var currentNode = _firstNode;

            while (currentNode != null)
            {
                listAsString = string.Format("{0}[{1}] => {2}\r\n", listAsString, i, currentNode.Data);
                currentNode = currentNode.Next;
                ++i;
            }

            return listAsString;
        }

        /********************************************************************************/

        internal class DLinkedListEnumerator : IEnumerator<T>
        {
            private DLinkedListNode<T> _current;
            private DLinkedList<T> _doublyLinkedList;

            public DLinkedListEnumerator(DLinkedList<T> list)
            {
                _current = list.Head;
                _doublyLinkedList = list;
            }

            public T Current
            {
                get { return _current.Data; }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                if (_current.Next != null)
                    _current = _current.Next;
                else
                    return false;

                return true;
            }

            public void Reset()
            {
                _current = _doublyLinkedList.Head;
            }

            public void Dispose()
            {
                _current = null;
                _doublyLinkedList = null;
            }

            public bool MovePrevious()
            {
                if (_current.Previous != null)
                    _current = _current.Previous;
                else
                    return false;

                return true;
            }
        }
    }

    /// <summary>
    ///     The Doubly-Linked List Node class.
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    public class DLinkedListNode<TKey, TValue> : IComparable<DLinkedListNode<TKey, TValue>>
        where TKey : IComparable<TKey>
    {
        public DLinkedListNode() : this(default(TKey), default(TValue))
        {
        }

        public DLinkedListNode(TKey key, TValue value) : this(key, value, null, null)
        {
        }

        public DLinkedListNode(TKey key, TValue value, DLinkedListNode<TKey, TValue> next,
            DLinkedListNode<TKey, TValue> previous)
        {
            Key = key;
            Value = value;
            Next = next;
            Previous = previous;
        }

        public virtual TKey Key { get; set; }

        public virtual TValue Value { get; set; }

        public virtual DLinkedListNode<TKey, TValue> Next { get; set; }

        public virtual DLinkedListNode<TKey, TValue> Previous { get; set; }

        public int CompareTo(DLinkedListNode<TKey, TValue> other)
        {
            if (other == null) return -1;

            return Key.CompareTo(other.Key);
        }
    }


    /// <summary>
    ///     Doubly-Linked List Data Structure.
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    public class DLinkedList<TKey, TValue> where TKey : IComparable<TKey>
    {
        /// <summary>
        ///     Instance variables.
        /// </summary>
        private int _count;


        /// <summary>
        ///     CONSTRUCTOR
        /// </summary>
        public DLinkedList()
        {
            _count = 0;
            _firstNode = null;
            _lastNode = null;
        }

        private DLinkedListNode<TKey, TValue> _firstNode { get; set; }
        private DLinkedListNode<TKey, TValue> _lastNode { get; set; }

        public virtual DLinkedListNode<TKey, TValue> Head
        {
            get { return _firstNode; }
        }

        public virtual int Count
        {
            get { return _count; }
        }

        /// <summary>
        ///     Getter function that returns the first element
        /// </summary>
        public virtual KeyValuePair<TKey, TValue> First
        {
            get
            {
                if (IsEmpty())
                {
                    throw new Exception("Empty list.");
                }
                return new KeyValuePair<TKey, TValue>(_firstNode.Key, _firstNode.Value);
            }
        }

        /// <summary>
        ///     Getter function that returns the last element
        /// </summary>
        public virtual KeyValuePair<TKey, TValue> Last
        {
            get
            {
                if (IsEmpty())
                {
                    throw new Exception("Empty list.");
                }
                if (_lastNode == null)
                {
                    var currentNode = _firstNode;
                    while (currentNode.Next != null)
                    {
                        currentNode = currentNode.Next;
                    }
                    _lastNode = currentNode;
                }

                return new KeyValuePair<TKey, TValue>(_lastNode.Key, _lastNode.Value);
            }
        }

        /// <summary>
        ///     Returns a list of the keys.
        /// </summary>
        public virtual List<TKey> Keys
        {
            get
            {
                var list = new List<TKey>(Count);

                var currentNode = _firstNode;
                for (var i = 0; i < Count; ++i)
                {
                    if (currentNode != null)
                    {
                        list.Add(currentNode.Key);
                        currentNode = currentNode.Next;
                    }
                    else
                    {
                        break;
                    }
                }

                return list;
            }
        }

        /// <summary>
        ///     Returns a list of the values.
        /// </summary>
        public virtual List<TValue> Values
        {
            get
            {
                var list = new List<TValue>(Count);

                var currentNode = _firstNode;
                for (var i = 0; i < Count; ++i)
                {
                    if (currentNode != null)
                    {
                        list.Add(currentNode.Value);
                        currentNode = currentNode.Next;
                    }
                    else
                    {
                        break;
                    }
                }

                return list;
            }
        }


        /// <summary>
        ///     Gets the element at the specified index
        /// </summary>
        protected virtual DLinkedListNode<TKey, TValue> _getNodeByIndex(int index)
        {
            if (IsEmpty())
                throw new IndexOutOfRangeException("List is empty.");

            if (index == 0)
            {
                return _firstNode;
            }
            if (index == Count - 1)
            {
                return _lastNode;
            }
            if (index > 0 && index < Count - 1)
            {
                DLinkedListNode<TKey, TValue> currentNode = null;

                // Decide from which reference to traverse the list, and then move the currentNode reference to the index
                // If index > half then traverse it from the end (_lastNode reference)
                // Otherwise, traverse it from the beginning (_firstNode refrence)
                if (index > Count/2)
                {
                    currentNode = _lastNode;
                    for (var i = Count - 1; i > index; --i)
                    {
                        currentNode = currentNode.Previous;
                    }
                }
                else
                {
                    currentNode = _firstNode;
                    for (var i = 0; i < index; ++i)
                    {
                        currentNode = currentNode.Next;
                    }
                }

                return currentNode;
            }
            throw new IndexOutOfRangeException();
        }

        /// <summary>
        ///     Gets the element by the specified key
        /// </summary>
        protected virtual DLinkedListNode<TKey, TValue> _getNodeByKey(TKey key)
        {
            if (key.IsEqualTo(_firstNode.Key))
            {
                return _firstNode;
            }
            if (key.IsEqualTo(_lastNode.Key))
            {
                return _lastNode;
            }
            var currentNode = _firstNode;
            while (currentNode != null)
            {
                if (key.IsEqualTo(currentNode.Key))
                    break;

                currentNode = currentNode.Next;
            }

            if (currentNode == null)
                throw new KeyNotFoundException();

            return currentNode;
        }

        /// <summary>
        ///     Sets the node's value by index.
        /// </summary>
        protected virtual void _setValueByIndex(int index, TValue value)
        {
            if (IsEmpty() || index < 0 || index >= Count)
                throw new IndexOutOfRangeException("List is empty.");

            if (index == 0)
            {
                _firstNode.Value = value;
            }
            else if (index == Count - 1)
            {
                _lastNode.Value = value;
            }
            else if (index > 0 && index < Count - 1)
            {
                DLinkedListNode<TKey, TValue> currentNode = null;

                // Decide from which reference to traverse the list, and then move the currentNode reference to the index
                // If index > half then traverse it from the end (_lastNode reference)
                // Otherwise, traverse it from the beginning (_firstNode refrence)
                if (index > Count/2)
                {
                    currentNode = _lastNode;
                    for (var i = Count - 1; i > index; --i)
                    {
                        currentNode = currentNode.Previous;
                    }
                }
                else
                {
                    currentNode = _firstNode;
                    for (var i = 0; i < index; ++i)
                    {
                        currentNode = currentNode.Next;
                    }
                }

                currentNode.Value = value;
            }
        }

        /// <summary>
        ///     Sets the node's value by key.
        /// </summary>
        protected virtual void _setValueByKey(TKey key, TValue value)
        {
            if (IsEmpty())
                throw new IndexOutOfRangeException("List is empty.");

            if (key.IsEqualTo(_firstNode.Key))
            {
                _firstNode.Value = value;
            }
            else if (key.IsEqualTo(_lastNode.Key))
            {
                _lastNode.Value = value;
            }
            else
            {
                var currentNode = _firstNode;
                while (currentNode != null)
                {
                    if (currentNode.Key.IsEqualTo(key))
                        break;

                    currentNode = currentNode.Next;
                }

                if (currentNode == null)
                    throw new KeyNotFoundException();

                currentNode.Value = value;
            }
        }

        /// <summary>
        ///     Sets the node object by index.
        /// </summary>
        protected virtual void _setNodeByIndex(int index, TKey key, TValue value)
        {
            if (IsEmpty() || index < 0 || index >= Count)
                throw new IndexOutOfRangeException("List is empty.");

            if (index == 0)
            {
                _firstNode.Key = key;
                _firstNode.Value = value;
            }
            else if (index == Count - 1)
            {
                _lastNode.Key = key;
                _lastNode.Value = value;
            }
            else if (index > 0 && index < Count - 1)
            {
                DLinkedListNode<TKey, TValue> currentNode = null;

                // Decide from which reference to traverse the list, and then move the currentNode reference to the index
                // If index > half then traverse it from the end (_lastNode reference)
                // Otherwise, traverse it from the beginning (_firstNode refrence)
                if (index > Count/2)
                {
                    currentNode = _lastNode;
                    for (var i = Count - 1; i > index; --i)
                    {
                        currentNode = currentNode.Previous;
                    }
                }
                else
                {
                    currentNode = _firstNode;
                    for (var i = 0; i < index; ++i)
                    {
                        currentNode = currentNode.Next;
                    }
                }

                currentNode.Key = key;
                currentNode.Value = value;
            }
        }

        /// <summary>
        ///     Determines whether this List is empty.
        /// </summary>
        /// <returns><c>true</c> if this list is empty; otherwise, <c>false</c>.</returns>
        public virtual bool IsEmpty()
        {
            return Count == 0;
        }

        /// <summary>
        ///     Prepend the key-value at the beginning of the list.
        /// </summary>
        public virtual void Prepend(TKey key, TValue value)
        {
            var newNode = new DLinkedListNode<TKey, TValue>(key, value);

            if (_firstNode == null)
            {
                _firstNode = _lastNode = newNode;
            }
            else
            {
                var currentNode = _firstNode;
                newNode.Next = currentNode;
                currentNode.Previous = newNode;
                _firstNode = newNode;
            }

            // Increment the count.
            _count++;
        }

        /// <summary>
        ///     Append the key-value item at the end of the list.
        /// </summary>
        public virtual void Append(TKey key, TValue value)
        {
            var newNode = new DLinkedListNode<TKey, TValue>(key, value);

            if (_firstNode == null)
            {
                _firstNode = _lastNode = newNode;
            }
            else
            {
                var currentNode = _lastNode;
                currentNode.Next = newNode;
                newNode.Previous = currentNode;
                _lastNode = newNode;
            }

            // Increment the count.
            _count++;
        }

        /// <summary>
        ///     Inserts the a new key-value item at the specified index.
        /// </summary>
        public virtual void InsertAt(int index, TKey key, TValue value)
        {
            if (index == 0)
            {
                Prepend(key, value);
            }
            else if (index == Count)
            {
                Append(key, value);
            }
            else if (index > 0 && index < Count)
            {
                DLinkedListNode<TKey, TValue> currentNode = null;
                var newNode = new DLinkedListNode<TKey, TValue>(key, value);

                // Decide from which reference to traverse the list, and then move the currentNode reference to the index
                // If index > half then traverse it from the end (_lastNode reference)
                // Otherwise, traverse it from the beginning (_firstNode refrence)
                if (index > Count/2)
                {
                    currentNode = _lastNode;
                    for (var i = Count - 1; i > index - 1; --i)
                    {
                        currentNode = currentNode.Previous;
                    }
                }
                else
                {
                    currentNode = _firstNode;
                    for (var i = 0; i < index - 1; ++i)
                    {
                        currentNode = currentNode.Next;
                    }
                }

                newNode.Next = currentNode.Next;
                currentNode.Next = newNode;
                newNode.Previous = currentNode;

                // Increment the count
                _count++;
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        /// <summary>
        ///     Inserts the key-value after specified index.
        /// </summary>
        public virtual void InsertAfter(int index, TKey key, TValue value)
        {
            // Insert at previous index.
            InsertAt(index - 1, key, value);
        }

        /// <summary>
        ///     Removes the item at the specified index.
        /// </summary>
        public virtual void RemoveAt(int index)
        {
            // Handle index out of bound errors
            if (IsEmpty() || index < 0 || index >= Count)
                throw new IndexOutOfRangeException();

            // Remove
            if (index == 0)
            {
                _firstNode = _firstNode.Next;

                if (_firstNode != null)
                    _firstNode.Previous = null;

                // Decrement count.
                _count--;
            }
            else if (index == Count - 1)
            {
                _lastNode = _lastNode.Previous;

                if (_lastNode != null)
                    _lastNode.Next = null;

                // Decrement count.
                _count--;
            }
            else
            {
                var i = 0;
                var currentNode = _firstNode;

                // Get currentNode to reference the element at the index.
                while (i < index)
                {
                    currentNode = currentNode.Next;
                    i++;
                } //end-while


                // Remove element
                var newPrevious = currentNode.Previous;
                var newNext = currentNode.Next;
                newPrevious.Next = newNext;

                if (newNext != null)
                    newNext.Previous = newPrevious;

                currentNode = newPrevious;

                // Decrement count.
                _count--;
            } //end-else
        }

        /// <summary>
        ///     Removes the item with the specified key.
        /// </summary>
        public virtual void RemoveBy(TKey key)
        {
            // Remove
            if (key.IsEqualTo(_firstNode.Key))
            {
                _firstNode = _firstNode.Next;

                if (_firstNode != null)
                    _firstNode.Previous = null;

                // Decrement count.
                _count--;
            }
            else if (key.IsEqualTo(_lastNode.Key))
            {
                _lastNode = _lastNode.Previous;

                if (_lastNode != null)
                    _lastNode.Next = null;

                // Decrement count.
                _count--;
            }
            else
            {
                var currentNode = _firstNode;

                // Get currentNode to reference the element at the index.
                while (currentNode != null)
                {
                    if (currentNode.Key.IsEqualTo(key))
                        break;

                    currentNode = currentNode.Next;
                } //end-while

                if (currentNode != null)
                {
                    // Remove element
                    var newPrevious = currentNode.Previous;
                    var newNext = currentNode.Next;
                    newPrevious.Next = newNext;

                    if (newNext != null)
                        newNext.Previous = newPrevious;

                    currentNode = newPrevious;

                    // Decrement count.
                    _count--;
                }
                else
                {
                    throw new KeyNotFoundException();
                }
            } //end-else
        }

        /// <summary>
        ///     Updates the value of an element at the specified index.
        /// </summary>
        public virtual void UpdateValueByIndex(int index, TValue value)
        {
            if (IsEmpty() || index < 0 || index >= Count)
                throw new IndexOutOfRangeException();

            _setValueByIndex(index, value);
        }

        /// <summary>
        ///     Updates the value of an element by it's key.
        /// </summary>
        public virtual void UpdateValueByKey(TKey key, TValue value)
        {
            if (IsEmpty())
                throw new IndexOutOfRangeException();

            _setValueByKey(key, value);
        }

        /// <summary>
        ///     Updates the key and value of an element at the specified index.
        /// </summary>
        public virtual void UpdateAtIndex(int index, TKey key, TValue value)
        {
            if (IsEmpty() || index < 0 || index >= Count)
                throw new IndexOutOfRangeException();

            _setNodeByIndex(index, key, value);
        }

        /// <summary>
        ///     Clears the list.
        /// </summary>
        public virtual void Clear()
        {
            _count = 0;
            _firstNode = _lastNode = null;
        }

        /// <summary>
        ///     Chesk whether the specified key exists in the list.
        /// </summary>
        public virtual bool ContainsKey(TKey key)
        {
            if (IsEmpty())
                throw new Exception("List is empty.");

            try
            {
                return Find(key).Key.IsEqualTo(key);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Find the specified item in the list.
        /// </summary>
        public virtual KeyValuePair<TKey, TValue> Find(TKey key)
        {
            if (IsEmpty())
                throw new Exception("List is empty.");

            var currentNode = _firstNode;
            while (currentNode != null)
            {
                if (currentNode.Key.IsEqualTo(key))
                    break;

                currentNode = currentNode.Next;
            }

            if (currentNode != null)
                return new KeyValuePair<TKey, TValue>(currentNode.Key, currentNode.Value);
            throw new KeyNotFoundException("Item was not found.");
        }

        /// <summary>
        ///     Find all elements in list that match the predicate.
        /// </summary>
        /// <param name="match">Predicate function.</param>
        /// <returns>List of elements.</returns>
        public virtual List<KeyValuePair<TKey, TValue>> FindAll(Predicate<TKey> match)
        {
            if (IsEmpty())
                throw new Exception("List is empty.");

            var currentNode = _firstNode;
            var list = new List<KeyValuePair<TKey, TValue>>();

            while (currentNode != null)
            {
                if (match(currentNode.Key))
                    list.Add(new KeyValuePair<TKey, TValue>(currentNode.Key, currentNode.Value));

                currentNode = currentNode.Next;
            }

            return list;
        }

        /// <summary>
        ///     Returns a number of elements as specified by countOfElements, starting from the specified index.
        /// </summary>
        /// <param name="index">Starting index.</param>
        /// <param name="countOfElements">The number of elements to return.</param>
        /// <returns>Doubly-Linked List of elements</returns>
        public virtual List<KeyValuePair<TKey, TValue>> GetRange(int index, int countOfElements)
        {
            DLinkedListNode<TKey, TValue> currentNode = null;
            var newList = new List<KeyValuePair<TKey, TValue>>();

            // Handle Index out of Bound errors
            if (Count == 0)
            {
                return newList;
            }
            if (index < 0 || index > Count)
            {
                throw new IndexOutOfRangeException();
            }

            // Decide from which reference to traverse the list, and then move the currentNode reference to the index
            // If index > half then traverse it from the end (_lastNode reference)
            // Otherwise, traverse it from the beginning (_firstNode refrence)
            if (index > Count/2)
            {
                currentNode = _lastNode;
                for (var i = Count - 1; i > index; --i)
                {
                    currentNode = currentNode.Previous;
                }
            }
            else
            {
                currentNode = _firstNode;
                for (var i = 0; i < index; ++i)
                {
                    currentNode = currentNode.Next;
                }
            }

            // Append the elements to the new list using the currentNode reference
            while (currentNode != null && newList.Count <= countOfElements)
            {
                var keyValue = new KeyValuePair<TKey, TValue>(currentNode.Key, currentNode.Value);
                newList.Add(keyValue);
                currentNode = currentNode.Next;
            }

            return newList;
        }

        /// <summary>
        ///     Sorts the entire list using Selection Sort.
        /// </summary>
        public virtual void SelectionSort()
        {
            if (IsEmpty())
                return;

            var currentNode = _firstNode;
            while (currentNode != null)
            {
                var nextNode = currentNode.Next;
                while (nextNode != null)
                {
                    if (nextNode.Key.IsLessThan(currentNode.Key))
                    {
                        var temp = nextNode.Key;
                        nextNode.Key = currentNode.Key;
                        currentNode.Key = temp;
                    }

                    nextNode = nextNode.Next;
                }

                currentNode = currentNode.Next;
            }
        }

        /// <summary>
        ///     Returns the list items as a readable multi--line string.
        /// </summary>
        /// <returns></returns>
        public virtual string ToReadable()
        {
            var listAsString = string.Empty;
            var i = 0;
            var currentNode = _firstNode;

            while (currentNode != null)
            {
                listAsString = string.Format("{0}[{1}] => {2}\r\n", listAsString, i, currentNode.Key);
                currentNode = currentNode.Next;
                ++i;
            }

            return listAsString;
        }
    }

    public class Queue<T> : IEnumerable<T> where T : IComparable<T>
    {
        private const int _defaultCapacity = 8;

        // The C# Maximum Array Length (before encountering overflow)
        // Reference: http://referencesource.microsoft.com/#mscorlib/system/array.cs,2d2b551eabe74985
        public const int MAXIMUM_ARRAY_LENGTH_x64 = 0X7FEFFFFF; //x64
        public const int MAXIMUM_ARRAY_LENGTH_x86 = 0x8000000; //x86

        // This sets the default maximum array length to refer to MAXIMUM_ARRAY_LENGTH_x64
        // Set the flag IsMaximumCapacityReached to false
        private bool DefaultMaxCapacityIsX64 = true;
        private bool IsMaximumCapacityReached;


        /// <summary>
        ///     CONSTRUCTOR
        /// </summary>
        public Queue() : this(_defaultCapacity)
        {
        }

        public Queue(int initialCapacity)
        {
            if (initialCapacity < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            _size = 0;
            _headPointer = 0;
            _tailPointer = 0;
            _collection = new object[initialCapacity];
        }

        /// <summary>
        ///     INSTANCE VARIABLE.
        /// </summary>
        private int _size { get; set; }

        private int _headPointer { get; set; }
        private int _tailPointer { get; set; }

        // The internal collection.
        private object[] _collection { get; set; }


        /// <summary>
        ///     Returns count of elements in queue
        /// </summary>
        public int Count
        {
            get { return _size; }
        }

        /// <summary>
        ///     Checks whether the queue is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return _size == 0; }
        }

        /// <summary>
        ///     Returns the top element in queue
        /// </summary>
        public T Top
        {
            get
            {
                if (IsEmpty)
                    throw new Exception("Queue is empty.");

                return (T) _collection[_headPointer];
            }
        }


        /********************************************************************************/


        public IEnumerator<T> GetEnumerator()
        {
            return _collection.GetEnumerator() as IEnumerator<T>;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        /// <summary>
        ///     Resize the internal array to a new size.
        /// </summary>
        private void _resize(int newSize)
        {
            if (newSize > _size && !IsMaximumCapacityReached)
            {
                var capacity = _collection.Length == 0 ? _defaultCapacity : _collection.Length*2;

                // Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
                // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
                var maxCapacity = DefaultMaxCapacityIsX64 ? MAXIMUM_ARRAY_LENGTH_x64 : MAXIMUM_ARRAY_LENGTH_x86;

                // Handle the new proper size
                if (capacity < newSize)
                    capacity = newSize;

                if (capacity >= maxCapacity)
                {
                    capacity = maxCapacity - 1;
                    IsMaximumCapacityReached = true;
                }

                // Try resizing and handle overflow
                try
                {
                    //Array.Resize (ref _collection, newSize);

                    var tempCollection = new object[newSize];
                    Array.Copy(_collection, _headPointer, tempCollection, 0, _size);
                    _collection = tempCollection;
                }
                catch (OutOfMemoryException)
                {
                    if (DefaultMaxCapacityIsX64)
                    {
                        DefaultMaxCapacityIsX64 = false;
                        _resize(capacity);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        ///     Inserts an element at the end of the queue
        /// </summary>
        /// <param name="dataItem">Element to be inserted.</param>
        public void Enqueue(T dataItem)
        {
            if (_size == _collection.Length)
            {
                try
                {
                    _resize(_collection.Length*2);
                }
                catch (OutOfMemoryException ex)
                {
                    throw ex;
                }
            }

            // Enqueue item at tail and then increment tail
            _collection[_tailPointer++] = dataItem;

            // Wrap around
            if (_tailPointer == _collection.Length)
                _tailPointer = 0;

            // Increment size
            _size++;
        }

        /// <summary>
        ///     Removes the Top Element from queue, and assigns it's value to the "top" parameter.
        /// </summary>
        /// <return>The top element container.</return>
        public T Dequeue()
        {
            if (IsEmpty)
                throw new Exception("Queue is empty.");

            var topItem = _collection[_headPointer];
            _collection[_headPointer] = null;

            // Decrement the size
            _size--;

            // Increment the head pointer
            _headPointer++;

            // Reset the pointer
            if (_headPointer == _collection.Length)
                _headPointer = 0;

            // Shrink the internal collection
            if (_size > 0 && _collection.Length > _defaultCapacity && _size <= _collection.Length/4)
            {
                // Get head and tail
                var head = _collection[_headPointer];
                var tail = _collection[_tailPointer];

                // Shrink
                _resize(_collection.Length/3*2);

                // Update head and tail pointers
                _headPointer = Array.IndexOf(_collection, head);
                _tailPointer = Array.IndexOf(_collection, tail);
            }

            return (T) topItem;
        }

        /// <summary>
        ///     Returns an array version of this queue.
        /// </summary>
        /// <returns>System.Array.</returns>
        public T[] ToArray()
        {
            var array = new T[_size];

            var j = 0;
            for (var i = 0; i < _size; ++i)
            {
                array[j] = (T) _collection[_headPointer + i];
                j++;
            }

            return array;
        }

        /// <summary>
        ///     Returns a human-readable, multi-line, print-out (string) of this queue.
        /// </summary>
        public string ToHumanReadable()
        {
            var array = ToArray();
            var listAsString = string.Empty;

            var i = 0;
            for (i = 0; i < Count; ++i)
                listAsString = string.Format("{0}[{1}] => {2}\r\n", listAsString, i, array[i]);

            return listAsString;
        }
    }

    public class SkipList<T> : ICollection<T>, IEnumerable<T> where T : IComparable<T>
    {
        // Readonly values
        private readonly int MaxLevel = 32;
        private readonly double Probability = 0.5;


        /// <summary>
        ///     CONSTRUCTOR
        /// </summary>
        public SkipList()
        {
            _count = 0;
            _currentMaxLevel = 1;
            _randomizer = new Random();
            _firstNode = new SkipListNode<T>(default(T), MaxLevel);

            for (var i = 0; i < MaxLevel; ++i)
                _firstNode.Forwards[i] = _firstNode;
        }

        private int _count { get; set; }
        private int _currentMaxLevel { get; set; }
        private Random _randomizer { get; set; }

        // The skip-list root node
        private SkipListNode<T> _firstNode { get; set; }


        /// <summary>
        ///     Getter accessor for the first node
        /// </summary>
        public SkipListNode<T> Root
        {
            get { return _firstNode; }
        }

        /// <summary>
        ///     Checks if list is empty or not
        /// </summary>
        public bool IsEmpty
        {
            get { return Count == 0; }
        }

        /// <summary>
        ///     Return current max-node level
        /// </summary>
        public int Level
        {
            get { return _currentMaxLevel; }
        }

        /// <summary>
        ///     Access elements by index
        /// </summary>
        public T this[int index]
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        ///     Return count of elements
        /// </summary>
        public int Count
        {
            get { return _count; }
        }

        /// <summary>
        ///     Adds item to the list
        /// </summary>
        public void Add(T item)
        {
            var current = _firstNode;
            var toBeUpdated = new SkipListNode<T>[MaxLevel];

            for (var i = _currentMaxLevel - 1; i >= 0; --i)
            {
                while (current.Forwards[i] != _firstNode && current.Forwards[i].Value.IsLessThan(item))
                    current = current.Forwards[i];

                toBeUpdated[i] = current;
            }

            current = current.Forwards[0];

            // Get the next node level, and update list level if required.
            var lvl = _getNextLevel();
            if (lvl > _currentMaxLevel)
            {
                for (var i = _currentMaxLevel; i < lvl; ++i)
                    toBeUpdated[i] = _firstNode;

                _currentMaxLevel = lvl;
            }

            // New node
            var newNode = new SkipListNode<T>(item, lvl);

            // Insert the new node into the skip list
            for (var i = 0; i < lvl; ++i)
            {
                newNode.Forwards[i] = toBeUpdated[i].Forwards[i];
                toBeUpdated[i].Forwards[i] = newNode;
            }

            // Increment the count
            ++_count;
        }

        /// <summary>
        ///     Remove element from the list.
        /// </summary>
        public bool Remove(T item)
        {
            T deleted;
            return Remove(item, out deleted);
        }

        /// <summary>
        ///     Checks if an item is in the list
        /// </summary>
        public bool Contains(T item)
        {
            T itemOut;
            return Find(item, out itemOut);
        }


        /// <summary>
        ///     Private helper. Used in Add method.
        /// </summary>
        /// <returns></returns>
        private int _getNextLevel()
        {
            var lvl = 0;

            while (_randomizer.NextDouble() < Probability && lvl <= _currentMaxLevel && lvl < MaxLevel)
                ++lvl;

            return lvl;
        }

        /// <summary>
        ///     Remove an element from list and then return it
        /// </summary>
        public bool Remove(T item, out T deleted)
        {
            // Find the node in each of the levels
            var current = _firstNode;
            var toBeUpdated = new SkipListNode<T>[MaxLevel];

            // Walk after all the nodes that have values less than the node we are looking for.
            // Mark all nodes as toBeUpdated.
            for (var i = _currentMaxLevel - 1; i >= 0; --i)
            {
                while (current.Forwards[i] != _firstNode && current.Forwards[i].Value.IsLessThan(item))
                    current = current.Forwards[i];

                toBeUpdated[i] = current;
            }

            current = current.Forwards[0];

            // Return default value of T if the item was not found
            if (current.Value.IsEqualTo(item) == false)
            {
                deleted = default(T);
                return false;
            }

            // We know that the node is in the list.
            // Unlink it from the levels where it exists.
            for (var i = 0; i < _currentMaxLevel; ++i)
                if (toBeUpdated[i].Forwards[i] == current)
                    toBeUpdated[i].Forwards[i] = current.Forwards[i];

            // Decrement the count
            --_count;

            // Check to see if we've deleted the highest-level node
            // Decrement level
            while (_currentMaxLevel > 1 && _firstNode.Forwards[_currentMaxLevel - 1] == _firstNode)
                --_currentMaxLevel;

            // Assign the deleted output parameter to the node.Value
            deleted = current.Value;
            return true;
        }

        /// <summary>
        ///     Look for an element and return it if found
        /// </summary>
        public bool Find(T item, out T result)
        {
            var current = _firstNode;

            // Walk after all the nodes that have values less than the node we are looking for
            for (var i = _currentMaxLevel - 1; i >= 0; --i)
                while (current.Forwards[i] != _firstNode && current.Forwards[i].Value.IsLessThan(item))
                    current = current.Forwards[i];

            current = current.Forwards[0];

            // Return true if we found the element; false otherwise
            if (current.Value.IsEqualTo(item))
            {
                result = current.Value;
                return true;
            }
            result = default(T);
            return false;
        }

        /// <summary>
        ///     Deletes the min element if the list is empty; otherwise throws exception
        /// </summary>
        public T DeleteMin()
        {
            T min;

            if (!TryDeleteMin(out min))
            {
                throw new ApplicationException("SkipList is empty.");
            }

            return min;
        }

        /// <summary>
        ///     Tries to delete the min element, returns false if list is empty
        /// </summary>
        public bool TryDeleteMin(out T result)
        {
            if (IsEmpty)
            {
                result = default(T);
                return false;
            }

            return Remove(_firstNode.Forwards[0].Value, out result);
        }

        /// <summary>
        ///     Returns the first element if the list is not empty; otherwise throw an exception
        /// </summary>
        public T Peek()
        {
            T peek;

            if (!TryPeek(out peek))
            {
                throw new ApplicationException("SkipList is empty.");
            }

            return peek;
        }

        /// <summary>
        ///     Tries to return the first element, if the list is empty it returns false
        /// </summary>
        public bool TryPeek(out T result)
        {
            if (IsEmpty)
            {
                result = default(T);
                return false;
            }

            result = _firstNode.Forwards[0].Value;
            return true;
        }

        #region IEnumerable<T> Implementation

        /// <summary>
        ///     IEnumerable method implementation
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            var node = _firstNode;
            while (node.Forwards[0] != null && node.Forwards[0] != _firstNode)
            {
                node = node.Forwards[0];
                yield return node.Value;
            }
        }

        /// <summary>
        ///     IEnumerable method implementation
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IEnumerable<T> Implementation

        #region ICollection<T> Implementation

        /// <summary>
        ///     Checks whether this collection is readonly
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        ///     Copy this list to an array
        /// </summary>
        public void CopyTo(T[] array, int arrayIndex)
        {
            // Validate the array and arrayIndex
            if (array == null)
                throw new ArgumentNullException();
            if (array.Length == 0 || arrayIndex >= array.Length || arrayIndex < 0)
                throw new IndexOutOfRangeException();

            // Get enumerator
            var enumarator = GetEnumerator();

            // Copy elements as long as there is any in the list and as long as the index is within the valid range
            for (var i = arrayIndex; i < array.Length; ++i)
            {
                if (enumarator.MoveNext())
                    array[i] = enumarator.Current;
                else
                    break;
            }
        }

        /// <summary>
        ///     Clears this instance
        /// </summary>
        public void Clear()
        {
            _count = 0;
            _currentMaxLevel = 1;
            _randomizer = new Random();
            _firstNode = new SkipListNode<T>(default(T), MaxLevel);

            for (var i = 0; i < MaxLevel; ++i)
                _firstNode.Forwards[i] = _firstNode;
        }

        #endregion
    }

    public class SkipListNode<T> : IComparable<SkipListNode<T>> where T : IComparable<T>
    {
        /// <summary>
        ///     Instance variables
        /// </summary>
        private T _value;

        /// <summary>
        ///     CONSTRUCTORS
        /// </summary>
        public SkipListNode(T value, int level)
        {
            if (level < 0)
                throw new ArgumentOutOfRangeException("Invalid value for level.");

            Value = value;
            Forwards = new SkipListNode<T>[level];
        }

        /// <summary>
        ///     Get and set node's value
        /// </summary>
        public virtual T Value
        {
            get { return _value; }
            private set { _value = value; }
        }

        /// <summary>
        ///     Get and set node's forwards links
        /// </summary>
        public virtual SkipListNode<T>[] Forwards { get; }

        /// <summary>
        ///     Return level of node.
        /// </summary>
        public virtual int Level
        {
            get { return Forwards.Length; }
        }

        /// <summary>
        ///     IComparable method implementation
        /// </summary>
        public int CompareTo(SkipListNode<T> other)
        {
            if (other == null)
                return -1;

            return Value.CompareTo(other.Value);
        }
    }

    /// <summary>
    ///     The Singly-Linked List Node class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SLinkedListNode<T> : IComparable<SLinkedListNode<T>> where T : IComparable<T>
    {
        public SLinkedListNode()
        {
            Next = null;
            Data = default(T);
        }

        public SLinkedListNode(T dataItem)
        {
            Next = null;
            Data = dataItem;
        }

        public T Data { get; set; }

        public SLinkedListNode<T> Next { get; set; }

        public int CompareTo(SLinkedListNode<T> other)
        {
            if (other == null) return -1;

            return Data.CompareTo(other.Data);
        }
    }


    /// <summary>
    ///     Singly Linked List Data Structure
    /// </summary>
    public class SLinkedList<T> : IEnumerable<T> where T : IComparable<T>
    {
        /// <summary>
        ///     Instance variables
        /// </summary>
        private int _count;

        /// <summary>
        ///     CONSTRUCTOR
        /// </summary>
        public SLinkedList()
        {
            _firstNode = null;
            _lastNode = null;
            _count = 0;
        }

        private SLinkedListNode<T> _firstNode { get; set; }
        private SLinkedListNode<T> _lastNode { get; set; }

        public int Count
        {
            get { return _count; }
        }

        public virtual SLinkedListNode<T> Head
        {
            get { return _firstNode; }
        }

        /// <summary>
        ///     Getter function that returns the first element
        /// </summary>
        public T First
        {
            get { return _firstNode == null ? default(T) : _firstNode.Data; }
        }

        /// <summary>
        ///     Getter function that returns the last element
        /// </summary>
        public T Last
        {
            get
            {
                if (Count == 0)
                {
                    throw new Exception("Empty list.");
                }
                if (_lastNode == null)
                {
                    var currentNode = _firstNode;
                    while (currentNode.Next != null)
                    {
                        currentNode = currentNode.Next;
                    }
                    _lastNode = currentNode;
                    return currentNode.Data;
                }
                return _lastNode.Data;
            }
        }

        /********************************************************************************/

        public IEnumerator<T> GetEnumerator()
        {
            return new SLinkedListEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new SLinkedListEnumerator(this);
        }

        /// <summary>
        ///     The Is List Empty check.
        /// </summary>
        /// <returns>true, if the list is empty, false otherwise.</returns>
        public bool IsEmpty()
        {
            return Count == 0;
        }

        /// <summary>
        ///     Inserts the specified dataItem at the beginning of the list.
        /// </summary>
        /// <param name="dataItem">The data value to be inserted to the list.</param>
        public void Prepend(T dataItem)
        {
            var newNode = new SLinkedListNode<T>(dataItem);

            if (_firstNode == null)
            {
                _firstNode = _lastNode = newNode;
            }
            else
            {
                var currentNode = _firstNode;
                newNode.Next = currentNode;
                _firstNode = newNode;
            }

            // Increment the count.
            _count++;
        }

        /// <summary>
        ///     Inserts the specified dataItem at the end of the list.
        /// </summary>
        /// <param name="dataItem">The data value to be inserted to the list.</param>
        public void Append(T dataItem)
        {
            var newNode = new SLinkedListNode<T>(dataItem);

            if (_firstNode == null)
            {
                _firstNode = _lastNode = newNode;
            }
            else
            {
                var currentNode = _lastNode;
                currentNode.Next = newNode;
                _lastNode = newNode;
            }

            // Increment the count.
            _count++;
        }

        /// <summary>
        ///     Inserts a specified item dataItem at an index.
        /// </summary>
        /// <param name="dataItem">Data item.</param>
        /// <param name="index">Index.</param>
        public void InsertAt(T dataItem, int index)
        {
            // Handle scope of insertion.
            // Prepend? Append? Or Insert in the range?
            if (index == 0)
            {
                Prepend(dataItem);
            }
            else if (index == Count)
            {
                Append(dataItem);
            }
            else if (index > 0 && index < Count)
            {
                var currentNode = _firstNode;
                var newNode = new SLinkedListNode<T>(dataItem);

                for (var i = 1; i < index; ++i)
                {
                    currentNode = currentNode.Next;
                }

                newNode.Next = currentNode.Next;
                currentNode.Next = newNode;

                // Increment the count
                _count++;
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        /// <summary>
        ///     Removes the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the list node to be removed.</param>
        public void RemoveAt(int index)
        {
            // Handle index out of bound errors
            if (IsEmpty() || index < 0 || index >= Count)
                throw new IndexOutOfRangeException();

            // Remove
            if (index == 0)
            {
                _firstNode = _firstNode.Next;

                // Decrement count.
                _count--;
            }
            else if (index == Count - 1)
            {
                var currentNode = _firstNode;

                while (currentNode.Next != null && currentNode.Next != _lastNode)
                    currentNode = currentNode.Next;

                currentNode.Next = null;
                _lastNode = currentNode;

                // Decrement count.
                _count--;
            }
            else
            {
                var i = 0;
                var currentNode = _firstNode;
                while (currentNode.Next != null)
                {
                    if (i + 1 == index)
                    {
                        currentNode.Next = currentNode.Next.Next;

                        // Decrement the count.
                        _count--;
                        break;
                    }

                    ++i;
                    currentNode = currentNode.Next;
                }
            }
        }

        /// <summary>
        ///     Clears all the items in the list.
        /// </summary>
        public void Clear()
        {
            _firstNode = null;
            _lastNode = null;
            _count = 0;
        }

        /// <summary>
        ///     Get the element at the specified index
        /// </summary>
        /// <param name="index">Index of element</param>
        /// <returns>Element</returns>
        public T GetAt(int index)
        {
            if (index == 0)
            {
                return First;
            }
            if (index == Count - 1)
            {
                return Last;
            }
            if (index > 0 && index < Count - 1)
            {
                var currentNode = _firstNode;
                for (var i = 0; i < index; ++i)
                {
                    currentNode = currentNode.Next;
                }
                return currentNode.Data;
            }
            throw new IndexOutOfRangeException();
        }

        /// <summary>
        ///     Returns a number of elements as specified by countOfElements, starting from the specified index.
        /// </summary>
        /// <param name="index">Starting index.</param>
        /// <param name="countOfElements">The number of elements to return.</param>
        /// <returns>Singly-Linked List of elements</returns>
        public SLinkedList<T> GetRange(int index, int countOfElements)
        {
            var newList = new SLinkedList<T>();
            var currentNode = _firstNode;

            // Handle Index out of Bound errors
            if (Count == 0)
            {
                return newList;
            }
            if (index < 0 || index > Count)
            {
                throw new IndexOutOfRangeException();
            }

            // Move the currentNode reference to the specified index
            for (var i = 0; i < index; ++i)
            {
                currentNode = currentNode.Next;
            }

            // Append the elements to the new list using the currentNode reference
            while (currentNode != null && newList.Count <= countOfElements)
            {
                newList.Append(currentNode.Data);
                currentNode = currentNode.Next;
            }

            return newList;
        }

        /// <summary>
        ///     Sorts the entire list using Selection Sort.
        /// </summary>
        public virtual void SelectionSort()
        {
            if (IsEmpty())
                return;

            var currentNode = _firstNode;
            while (currentNode != null)
            {
                var nextNode = currentNode.Next;
                while (nextNode != null)
                {
                    if (nextNode.Data.IsLessThan(currentNode.Data))
                    {
                        var temp = nextNode.Data;
                        nextNode.Data = currentNode.Data;
                        currentNode.Data = temp;
                    }

                    nextNode = nextNode.Next;
                }

                currentNode = currentNode.Next;
            }
        }

        /// <summary>
        ///     Return an array version of this list.
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            var array = new T[Count];

            var currentNode = _firstNode;
            for (var i = 0; i < Count; ++i)
            {
                if (currentNode != null)
                {
                    array[i] = currentNode.Data;
                    currentNode = currentNode.Next;
                }
                else
                {
                    break;
                }
            }

            return array;
        }

        /// <summary>
        ///     Returns a System.List version of this SLinkedList instace.
        /// </summary>
        /// <returns>System.List of elements</returns>
        public List<T> ToList()
        {
            var list = new List<T>();

            var currentNode = _firstNode;
            for (var i = 0; i < Count; ++i)
            {
                if (currentNode != null)
                {
                    list.Add(currentNode.Data);
                    currentNode = currentNode.Next;
                }
                else
                {
                    break;
                }
            }

            return list;
        }

        /// <summary>
        ///     Returns the list items as a readable multi--line string.
        /// </summary>
        /// <returns></returns>
        public string ToReadable()
        {
            var i = 0;
            var currentNode = _firstNode;
            var listAsString = string.Empty;

            while (currentNode != null)
            {
                listAsString = string.Format("{0}[{1}] => {2}\r\n", listAsString, i, currentNode.Data);
                currentNode = currentNode.Next;
                ++i;
            }

            return listAsString;
        }

        /********************************************************************************/

        internal class SLinkedListEnumerator : IEnumerator<T>
        {
            private SLinkedListNode<T> _current;
            private SLinkedList<T> _doublyLinkedList;

            public SLinkedListEnumerator(SLinkedList<T> list)
            {
                _doublyLinkedList = list;
                _current = list.Head;
            }

            public T Current
            {
                get { return _current.Data; }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                _current = _current.Next;

                return _current != null;
            }

            public void Reset()
            {
                _current = _doublyLinkedList.Head;
            }

            public void Dispose()
            {
                _current = null;
                _doublyLinkedList = null;
            }
        }
    }

    public class Stack<T> : IEnumerable<T> where T : IComparable<T>
    {
        public Stack()
        {
            _collection = new ArrayList<T>();
        }


        public Stack(int initialCapacity)
        {
            if (initialCapacity < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            _collection = new ArrayList<T>(initialCapacity);
        }


        private ArrayList<T> _collection { get; }

        public int Count
        {
            get { return _collection.Count; }
        }


        public bool IsEmpty
        {
            get { return _collection.IsEmpty; }
        }


        public T Top
        {
            get
            {
                try
                {
                    return _collection[_collection.Count - 1];
                }
                catch (Exception)
                {
                    throw new Exception("Stack is empty.");
                }
            }
        }


        public IEnumerator<T> GetEnumerator()
        {
            for (var i = _collection.Count - 1; i >= 0; --i)
                yield return _collection[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public void Push(T dataItem)
        {
            _collection.Add(dataItem);
        }


        public T Pop()
        {
            if (Count > 0)
            {
                var top = Top;
                _collection.RemoveAt(_collection.Count - 1);
                return top;
            }
            throw new Exception("Stack is empty.");
        }

        public T[] ToArray()
        {
            return _collection.ToArray();
        }


        public string ToHumanReadable()
        {
            return _collection.ToHumanReadable();
        }
    }
}