using System;
using System.Collections;
using System.Collections.Generic;
using DataStructures.Common;
using DataStructures.Trees;

namespace DataStructures.SortedCollections
{
    public class SortedDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        public SortedDictionary()
        {
            _collection = new RedBlackTreeMap<TKey, TValue>(false);
        }

        /// <summary>
        ///     The internal collection is a Red-Black Tree Map.
        /// </summary>
        private RedBlackTreeMap<TKey, TValue> _collection { get; set; }

        /// <summary>
        ///     Returns true if dictionary is empty; otherwise, false.
        /// </summary>
        public bool IsEmpty
        {
            get { return Count == 0; }
        }


        /// <summary>
        ///     Gets the count of enteries in dictionary.
        /// </summary>
        public int Count
        {
            get { return _collection.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        ///     Determines whether the current dictionary contains an entry with the specified key.
        /// </summary>
        public bool ContainsKey(TKey key)
        {
            return _collection.Contains(key);
        }

        /// <summary>
        ///     Determines whether the current collection contains a specific key-value pair.
        /// </summary>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            try
            {
                var entry = _collection.Find(item.Key);
                return entry.Value.Equals(item.Value);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Try to get the value of a key or default(TValue). Returns true if key exists; otherwise, false.
        /// </summary>
        public bool TryGetValue(TKey key, out TValue value)
        {
            // Set value to the default value of type TValue
            value = default(TValue);

            try
            {
                // Assign the returned object to value
                value = _collection.Find(key).Value;

                // return Success.
                return true;
            }
            catch (KeyNotFoundException)
            {
                // No entry was found with the specified key.
                // return Failure.
                return false;
            }
        }

        /// <summary>
        ///     Gets or sets the value at the specified key.
        /// </summary>
        public TValue this[TKey index]
        {
            get
            {
                // In case dictionary is empty
                if (IsEmpty)
                    throw new Exception("Dictionary is empty.");

                try
                {
                    return _collection.Find(index).Value;
                }
                catch (KeyNotFoundException)
                {
                    // Mask the tree's exception with a new one.
                    throw new KeyNotFoundException("Key doesn't exist in dictionary.");
                }
            }
            set
            {
                if (ContainsKey(index))
                    _collection.Update(index, value);
                else
                    Add(index, value);
            }
        }

        /// <summary>
        ///     Gets the collection of keys in the dictionary.
        /// </summary>
        public ICollection<TKey> Keys
        {
            get
            {
                var keys = new List<TKey>(Count);
                var enumerator = _collection.GetInOrderEnumerator();

                while (enumerator.MoveNext())
                    keys.Add(enumerator.Current.Key);

                return keys;
            }
        }

        /// <summary>
        ///     Gets the collection of values in the dictionary.
        /// </summary>
        public ICollection<TValue> Values
        {
            get
            {
                var values = new List<TValue>(Count);
                var enumerator = _collection.GetInOrderEnumerator();

                while (enumerator.MoveNext())
                    values.Add(enumerator.Current.Value);

                return values;
            }
        }

        /// <summary>
        ///     Add the specified key and value to the dictionary.
        /// </summary>
        public void Add(TKey key, TValue value)
        {
            // Throw an duplicate key exception if an entry with the same key exists
            try
            {
                _collection.Insert(key, value);
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("An entry with the same key already exists in dictionary.");
            }
        }

        /// <summary>
        ///     Removes the item with specific Key from the dictionary.
        /// </summary>
        public bool Remove(TKey key)
        {
            try
            {
                // Try removing it and return Success
                _collection.Remove(key);
                return true;
            }
            catch (Exception)
            {
                // Item was not found. Return Failure.
                return false;
            }
        }

        /// <summary>
        ///     Add the key-value pair to the dictionary.
        /// </summary>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        ///     Removes the first occurrence of an item from the current collection Key and Value will be matched.
        /// </summary>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (IsEmpty)
                return false;

            // Get the entry from collection
            var entry = _collection.Find(item.Key);

            // If the entry's value match the value of the specified item, remove it
            if (entry.Value.Equals(item.Value))
            {
                _collection.Remove(item.Key);
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Copies the key-value pairs to a given array starting from specified index.
        /// </summary>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException();

            var enumerator = _collection.GetInOrderEnumerator();

            while (enumerator.MoveNext() && arrayIndex < array.Length)
            {
                array[arrayIndex] = enumerator.Current;
                arrayIndex++;
            }
        }

        /// <summary>
        ///     Clears this instance.
        /// </summary>
        public void Clear()
        {
            _collection = new RedBlackTreeMap<TKey, TValue>(false);
        }

        #region IEnumerable implementation

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _collection.GetInOrderEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }

    public class SortedList<T> : IEnumerable<T>, ICollection<T>, IList<T> where T : IComparable<T>
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        public SortedList()
        {
            _collection = new RedBlackTree<T>();
        }

        /// <summary>
        ///     The internal collection is a Red-Black Tree.
        /// </summary>
        private RedBlackTree<T> _collection { get; set; }

        /// <summary>
        ///     Returns true if list is empty; otherwise, false.
        /// </summary>
        public bool IsEmpty
        {
            get { return Count == 0; }
        }

        /// <summary>
        ///     Gets the count of items in list.
        /// </summary>
        public int Count
        {
            get { return _collection.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        ///     Determines whether the current collection contains a specific value.
        /// </summary>
        public bool Contains(T item)
        {
            return _collection.Contains(item);
        }

        /// <summary>
        ///     Adds the item to list.
        /// </summary>
        public void Add(T item)
        {
            _collection.Insert(item);
        }

        /// <summary>
        ///     Removes the first occurrence of an item from list.
        /// </summary>
        public bool Remove(T item)
        {
            try
            {
                _collection.Remove(item);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Copies the items in list to an array starting from a given index.
        /// </summary>
        public void CopyTo(T[] array, int arrayIndex)
        {
            // Validate the array argument
            if (array == null)
                throw new ArgumentNullException("Array cannot be Null.");

            var enumerator = _collection.GetInOrderEnumerator();

            // Copy the items from the inorder-walker of the tree to the passed array
            while (enumerator.MoveNext() && arrayIndex < array.Length)
            {
                array[arrayIndex] = enumerator.Current;
                arrayIndex++;
            }
        }

        /// <summary>
        ///     Clears this instance.
        /// </summary>
        public void Clear()
        {
            _collection = new RedBlackTree<T>();
        }

        /// <summary>
        ///     Determines the index of a specific item in the current collection.
        /// </summary>
        public int IndexOf(T item)
        {
            // If the item doesn't exist in collection, return -1
            if (!Contains(item))
                return -1;

            var index = 0;
            var enumerator = _collection.GetInOrderEnumerator();

            while (enumerator.MoveNext())
            {
                // If the current item is found return index
                if (enumerator.Current.IsEqualTo(item))
                    return index;

                // Increment index
                index++;
            }

            return -1;
        }

        /// <summary>
        ///     Gets or sets the item at the specified index.
        /// </summary>
        public T this[int index]
        {
            get
            {
                // In case list is empty
                if (IsEmpty)
                    throw new Exception("List is empty.");

                // Validate index range
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();

                var enumerator = _collection.GetInOrderEnumerator();

                // Keep moving to the next item until index becomes 0
                while (enumerator.MoveNext() && index > 0)
                    index--;

                // Return the enumerator's Current value
                return enumerator.Current;
            }
            set
            {
                try
                {
                    _collection.Remove(this[index]);
                    Add(value);
                }
                catch (IndexOutOfRangeException)
                {
                    // Masks the get method (see above) exception with a new one.
                    throw new IndexOutOfRangeException();
                }
            }
        }

        /// <summary>
        ///     Inserts the item at the specified index.
        /// </summary>
        public void Insert(int index, T item)
        {
            // It is meaningless to insert at a specific index since after every
            // insert operation, the collection will be rebalanced and the insertion
            // operation itself needs to ensure the sorting criteria, therefore the item
            // item insert at index i might not be the same after the operation has completed.
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Removes an item at a specific index.
        /// </summary>
        public void RemoveAt(int index)
        {
            // Validate index range
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException();

            var enumerator = _collection.GetInOrderEnumerator();

            // Keep moving to the next item until index becomes 0
            while (enumerator.MoveNext() && index > 0)
                index--;

            // Remove the enumerator's Current value from collection
            Remove(enumerator.Current);
        }

        #region IEnumerable implementation

        public IEnumerator<T> GetEnumerator()
        {
            return _collection.GetInOrderEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}