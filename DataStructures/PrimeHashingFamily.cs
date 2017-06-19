/***
 * Prime-Hashing Functions Family.
 * 
 * Implements a simple family of hash functions using primes. 
 * The functions are initialized by randomly selecting primes. 
 * Supports re-generation of functions.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataStructures.Common;
using DataStructures.Lists;
using DataStructures.Hashing;

namespace DataStructures.Hashing
{
    /// <summary>
    /// Implements a family of randomized Hash Functions
    /// </summary>
    public class PrimeHashingFamily
    {
        private Random _randomizer { get; set; }
        private int _numberOfHashFunctions { get; set; }
        private int[] _multipliersVector { get; set; }
        private static readonly PrimesList _primes = PrimesList.Instance;

        /// <summary>
        /// Initializes the family with a specified number of hash functions.
        /// </summary>
        public PrimeHashingFamily(int numberOfHashFunctions)
        {
            if (numberOfHashFunctions <= 0)
                throw new ArgumentOutOfRangeException("Number of hash functions should be greater than zero.");

            _randomizer = new Random();
            _numberOfHashFunctions = numberOfHashFunctions;
            _multipliersVector = new int[_numberOfHashFunctions];

            GenerateNewFunctions();
        }

        /// <summary>
        /// Returns number of member hash functions.
        /// </summary>
        public int NumberOfFunctions
        {
            get { return _numberOfHashFunctions; }
        }

        /// <summary>
        /// Generates new hash functions with new randomized multipliers.
        /// </summary>
        public void GenerateNewFunctions()
        {
            // Clear the multipliers vectors
            Array.Clear(_multipliersVector, 0, _multipliersVector.Length);

            for (int i = 0; i < _numberOfHashFunctions; i++)
            {
                var randomIndex = _randomizer.Next(0, _primes.Count - 1);
                _multipliersVector[i] = _primes[randomIndex];
            }
        }

        /// <summary>
        /// Returns hash value of an integer prehash key, given the specified number of the hash function to use.
        /// </summary>
        /// <param name="preHashedKey">Int pre-hash code of an object.</param>
        /// <param name="whichHashFunction">Non-zero, non-negative integer that specified the number of the hash function to use.</param>
        /// <returns></returns>
        public int Hash(int preHashedKey, int whichHashFunction)
        {
            if (whichHashFunction <= 0 || whichHashFunction > _numberOfHashFunctions)
                throw new ArgumentOutOfRangeException("WhichHashFunction parameter should be greater than zero or equal to the number of Hash Functions.");

            int preHashValue = 0;
            int multiplier = _multipliersVector[whichHashFunction - 1];
            var characters = preHashedKey.ToString().ToCharArray();

            return (multiplier * preHashValue);
        }

        /// <summary>
        /// Returns hash value of a string, given the specified number of the hash function to use.
        /// </summary>
        /// <param name="key">string key.</param>
        /// <param name="whichHashFunction">Non-zero, non-negative integer that specified the number of the hash function to use.</param>
        /// <returns></returns>
        public int Hash(string key, int whichHashFunction)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key is either an empty string or null.");

            int preHashValue = 0;
            var characters = key.ToCharArray();

            foreach (var character in characters)
            {
                preHashValue += Convert.ToInt32(Char.GetNumericValue(character));
            }

            return Hash(preHashValue, whichHashFunction);
        }

    }
    public class UniversalHashingFamily
    {
        // A large prime, arbitrarily chosen
        // In decimal = 2,146,435,069;
        private const int BIG_PRIME = 0x7FEFFFFD;

        private Random _randomizer { get; set; }
        private int _numberOfHashFunctions { get; set; }
        private int[] _firstMultipliersVector { get; set; }
        private int[] _secondMultipliersVector { get; set; }
        private static readonly PrimesList _primes = PrimesList.Instance;

        /// <summary>
        /// Initializes the family with a specified number of hash functions.
        /// </summary>
        public UniversalHashingFamily(int numberOfHashFunctions)
        {
            if (numberOfHashFunctions <= 0)
                throw new ArgumentOutOfRangeException("Number of hash functions should be greater than zero.");

            _randomizer = new Random();
            _numberOfHashFunctions = numberOfHashFunctions;
            _firstMultipliersVector = new int[_numberOfHashFunctions];
            _secondMultipliersVector = new int[_numberOfHashFunctions];

            GenerateNewFunctions();
        }

        /// <summary>
        /// Returns number of member hash functions.
        /// </summary>
        public int NumberOfFunctions
        {
            get { return _numberOfHashFunctions; }
        }

        /// <summary>
        /// Generates new hash functions with new randomized multipliers.
        /// </summary>
        public void GenerateNewFunctions()
        {
            // Clear the multipliers vectors
            Array.Clear(_firstMultipliersVector, 0, _firstMultipliersVector.Length);
            Array.Clear(_secondMultipliersVector, 0, _secondMultipliersVector.Length);

            int randomMin = 0;
            int randomMax = _primes.Count - 1;

            for (int i = 0; i < _numberOfHashFunctions; i++)
            {
                // Get only the primes that are smaller than the biggest-chosen prime.
                int randomIndex = _randomizer.Next(randomMin, randomMax);

                while (_primes[randomIndex] >= BIG_PRIME)
                    randomIndex = _randomizer.Next(randomMin, randomMax);

                _firstMultipliersVector[i] = _primes[randomIndex];

                // make sure the next prime we choose is different than the first one and less than the biggest-prime.
                randomIndex = _randomizer.Next(randomMin, randomMax);

                while (_primes[randomIndex] >= BIG_PRIME || _primes[randomIndex] == _firstMultipliersVector[i])
                    randomIndex = _randomizer.Next(randomMin, randomMax);

                _secondMultipliersVector[i] = _primes[randomIndex];
            }
        }

        /// <summary>
        /// Returns hash value of a string, given the specified number of the hash function to use.
        /// </summary>
        /// <param name="preHashedKey">Int pre-hash code of an object.</param>
        /// <param name="whichHashFunction">Non-zero, non-negative integer that specified the number of the hash function to use.</param>
        /// <returns></returns>
        public int UniversalHash(int preHashedKey, int whichHashFunction)
        {
            if (whichHashFunction <= 0 || whichHashFunction > _numberOfHashFunctions)
                throw new ArgumentOutOfRangeException("WhichHashFunction parameter should be greater than zero or equal to the number of Hash Functions.");

            int a = _firstMultipliersVector[whichHashFunction - 1];
            int b = _secondMultipliersVector[whichHashFunction - 1];

            return ((a * preHashedKey) + b) % BIG_PRIME;
        }

        /// <summary>
        /// Returns hash value of a string, given the specified number of the hash function to use.
        /// </summary>
        /// <param name="key">string key.</param>
        /// <param name="whichHashFunction">Non-zero, non-negative integer that specified the number of the hash function to use.</param>
        public int UniversalHash(string key, int whichHashFunction)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key is either an empty string or null.");

            int prehash = 0;
            var characters = key.ToCharArray();
            int n = characters.Length;

            for (int i = 0; i < n; ++i)
            {
                prehash = prehash + (characters[i] ^ (n - 1));
            }

            return UniversalHash(prehash, whichHashFunction);
        }
    }

    public class ChainedHashTable<TKey, TValue> : IDictionary<TKey, TValue> where TKey : IComparable<TKey>
    {
        /// <summary>
        ///     Used in the ensure capacity function
        /// </summary>
        public enum CapacityManagementMode
        {
            Contract = 0,
            Expand = 1
        }

        private const int _defaultCapacity = 8;

        // The C# Maximum Array Length (before encountering overflow)
        // Reference: http://referencesource.microsoft.com/#mscorlib/system/array.cs,2d2b551eabe74985
        private const int MAX_ARRAY_LENGTH = 0X7FEFFFFF;

        // Initial hash value.
        private const uint INITIAL_HASH = 0x9e3779b9;

        private static readonly DLinkedList<TKey, TValue>[] _emptyArray =
            new DLinkedList<TKey, TValue>[_defaultCapacity];

        private int _freeSlotsCount;
        private DLinkedList<TKey, TValue>[] _hashTableStore;

        /// <summary>
        ///     INSTANCE VARIABLES.
        /// </summary>
        private int _size;

        private decimal _slotsLoadFactor;


        /// <summary>
        ///     CONSTRUCTOR
        /// </summary>
        public ChainedHashTable()
        {
            _size = 0;
            _hashTableStore = _emptyArray;
            _freeSlotsCount = _hashTableStore.Length;
            _keysComparer = EqualityComparer<TKey>.Default;
            _valuesComparer = EqualityComparer<TValue>.Default;

            _keysCollection = new List<TKey>();
            _valuesCollection = new List<TValue>();
        }

        private List<TKey> _keysCollection { get; }
        private List<TValue> _valuesCollection { get; }

        // Keys and Values Comparers
        private EqualityComparer<TKey> _keysComparer { get; }
        private EqualityComparer<TValue> _valuesComparer { get; }

        /// <summary>
        ///     Checks if the hash table is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return Count == 0; }
        }

        /// <summary>
        ///     Return count of elements in the hash table.
        /// </summary>
        public int Count
        {
            get { return _size; }
        }

        /// <summary>
        ///     Checks if the hash table is readonly.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        ///     Checks whether key exists in the hash table.
        /// </summary>
        public bool ContainsKey(TKey key)
        {
            // Get hash of the key
            var hashcode = _getHashOfKey(key);

            // The chain of colliding keys are found at _keysValuesMap[hashcode] as a doubly-linked-list.
            if (_hashTableStore[hashcode] != null && _hashTableStore[hashcode].Count > 0)
            {
                return _hashTableStore[hashcode].ContainsKey(key);
            }

            // else
            return false;
        }

        /// <summary>
        ///     Checks whether a key-value pair exist in the hash table.
        /// </summary>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            // Get hash of the key
            var hashcode = _getHashOfKey(item.Key);

            // The chain of colliding keys are found at _keysValuesMap[hashcode] as a doubly-linked-list.
            if (_hashTableStore[hashcode] != null && _hashTableStore[hashcode].Count > 0)
            {
                try
                {
                    var existingPair = _hashTableStore[hashcode].Find(item.Key);

                    if (existingPair.Key.IsEqualTo(item.Key) && _valuesComparer.Equals(existingPair.Value, item.Value))
                        return true;
                }
                catch (KeyNotFoundException)
                {
                    // do nothing
                }
            }

            // else
            return false;
        }

        /// <summary>
        ///     List of hash table keys.
        /// </summary>
        public ICollection<TKey> Keys
        {
            get { return _keysCollection; }
        }

        /// <summary>
        ///     List of hash table values.
        /// </summary>
        public ICollection<TValue> Values
        {
            get { return _valuesCollection; }
        }


        /// <summary>
        ///     Tries to get the value of key which might not be in the dictionary.
        /// </summary>
        public bool TryGetValue(TKey key, out TValue value)
        {
            // Get hash of the key
            var hashcode = _getHashOfKey(key);

            // The chain of colliding keys are found at _keysValuesMap[hashcode] as a doubly-linked-list.
            if (_hashTableStore[hashcode] != null && _hashTableStore[hashcode].Count > 0)
            {
                try
                {
                    var existingPair = _hashTableStore[hashcode].Find(key);
                    value = existingPair.Value;
                    return true;
                }
                catch (KeyNotFoundException)
                {
                    // do nothing
                }
            }

            // NOT FOUND
            value = default(TValue);
            return false;
        }

        /// <summary>
        ///     Gets or sets the value of a key.
        /// </summary>
        public TValue this[TKey key]
        {
            get
            {
                // Get hash of the key
                var hashcode = _getHashOfKey(key);

                // The chain of colliding keys are found at _keysValuesMap[hashcode] as a doubly-linked-list.
                if (_hashTableStore[hashcode] != null && _hashTableStore[hashcode].Count > 0)
                {
                    try
                    {
                        var existingPair = _hashTableStore[hashcode].Find(key);
                        return existingPair.Value;
                    }
                    catch (KeyNotFoundException)
                    {
                        // do nothing
                    }
                }

                // NOT FOUND
                throw new KeyNotFoundException();
            }
            set
            {
                // Get hash of the key
                var hashcode = _getHashOfKey(key);

                // The chain of colliding keys are found at _keysValuesMap[hashcode] as a doubly-linked-list.
                if (_hashTableStore[hashcode] != null && _hashTableStore[hashcode].Count > 0)
                {
                    var exists = _hashTableStore[hashcode].ContainsKey(key);

                    if (exists)
                        _hashTableStore[hashcode].UpdateValueByKey(key, value);
                }

                // NOT FOUND
                throw new KeyNotFoundException();
            }
        }

        /// <summary>
        ///     Add a key and value to the hash table.
        /// </summary>
        public void Add(TKey key, TValue value)
        {
            // Get hash of the key
            var hashcode = _getHashOfKey(key);

            // The chain of colliding keys are found at _keysValuesMap[hashcode] as a doubly-linked-list.
            if (_hashTableStore[hashcode] == null)
            {
                // This is an empty slot. Initialize the chain of collisions.
                _hashTableStore[hashcode] = new DLinkedList<TKey, TValue>();

                // Decrease the number of free slots.
                _freeSlotsCount--;
            }
            else if (_hashTableStore[hashcode] != null && _hashTableStore[hashcode].Count > 0)
            {
                if (_hashTableStore[hashcode].ContainsKey(key))
                    throw new ArgumentException("Key already exists in the hash table.");
            }

            _hashTableStore[hashcode].Append(key, value);
            _size++;

            //Add the key-value to the keys and values collections
            _keysCollection.Add(key);
            _valuesCollection.Add(value);

            _slotsLoadFactor = decimal.Divide(
                Convert.ToDecimal(_size),
                Convert.ToDecimal(_hashTableStore.Length));

            // Capacity management
            if (_slotsLoadFactor.IsGreaterThanOrEqualTo(Convert.ToDecimal(0.90)))
            {
                _ensureCapacity(CapacityManagementMode.Expand, _hashTableStore.Length + 1);
            }
        }

        /// <summary>
        ///     Add a key-value pair to the hash table.
        /// </summary>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        ///     Remove a key from the hash table and return the status.
        /// </summary>
        public bool Remove(TKey key)
        {
            // Get hash of the key
            var hashcode = _getHashOfKey(key);

            // The chain of colliding keys are found at _keysValuesMap[hashcode] as a doubly-linked-list.
            if (_hashTableStore[hashcode] != null && _hashTableStore[hashcode].Count > 0)
            {
                try
                {
                    var keyValuePair = _hashTableStore[hashcode].Find(key);

                    if (keyValuePair.Key.IsEqualTo(key))
                    {
                        _hashTableStore[hashcode].RemoveBy(key);
                        _size--;

                        // check if no other keys exist in this slot.
                        if (_hashTableStore[hashcode].Count == 0)
                        {
                            // Nullify the chain of collisions at this slot.
                            _hashTableStore[hashcode] = null;

                            // Increase the number of free slots.
                            _freeSlotsCount++;

                            // Capacity management
                            _ensureCapacity(CapacityManagementMode.Contract);
                        }

                        _keysCollection.Remove(key);
                        _valuesCollection.Remove(keyValuePair.Value);

                        return true;
                    }
                }
                catch
                {
                    // do nothing
                }
            }

            // else
            return false;
        }

        /// <summary>
        ///     Remove a key-value pair from the hash table and return the status.
        /// </summary>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            // Get hash of the key
            var hashcode = _getHashOfKey(item.Key);

            // The chain of colliding keys are found at _keysValuesMap[hashcode] as a doubly-linked-list.
            if (_hashTableStore[hashcode] != null && _hashTableStore[hashcode].Count > 0)
            {
                try
                {
                    var keyValuePair = _hashTableStore[hashcode].Find(item.Key);

                    if (keyValuePair.Key.IsEqualTo(item.Key) && _valuesComparer.Equals(keyValuePair.Value, item.Value))
                    {
                        _hashTableStore[hashcode].RemoveBy(item.Key);
                        _size--;

                        // check if no other keys exist in this slot.
                        if (_hashTableStore[hashcode].Count == 0)
                        {
                            // Nullify the chain of collisions at this slot.
                            _hashTableStore[hashcode] = null;

                            // Increase the number of free slots.
                            _freeSlotsCount++;

                            // Capacity management
                            _ensureCapacity(CapacityManagementMode.Contract);
                        }

                        _keysCollection.Remove(keyValuePair.Key);
                        _valuesCollection.Remove(keyValuePair.Value);

                        return true;
                    }
                }
                catch
                {
                    // do nothing
                }
            }

            // else
            return false;
        }

        /// <summary>
        ///     Copy the key-value pairs in the hash table to an array starting from the specified index.
        /// </summary>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
                array = new KeyValuePair<TKey, TValue>[_size];

            var i = arrayIndex;
            var hashTableIndex = 0;
            var countOfElements = array.Length - arrayIndex;

            while (true)
            {
                KeyValuePair<TKey, TValue> pair;

                if (i >= array.Length)
                    break;

                if (_hashTableStore[hashTableIndex] != null && _hashTableStore[hashTableIndex].Count > 0)
                {
                    if (_hashTableStore[hashTableIndex].Count == 1)
                    {
                        pair = new KeyValuePair<TKey, TValue>(_hashTableStore[hashTableIndex].First.Key,
                            _hashTableStore[hashTableIndex].First.Value);
                        array[i] = pair;
                        i++;
                        hashTableIndex++;
                    }
                    else
                    {
                        var headOfChain = _hashTableStore[hashTableIndex].Head;

                        while (i < array.Length)
                        {
                            pair = new KeyValuePair<TKey, TValue>(headOfChain.Key, headOfChain.Value);
                            array[i] = pair;
                            i++;
                            hashTableIndex++;

                            headOfChain = headOfChain.Next;
                        }
                    } //end-if-else
                } //end-if
                else
                {
                    hashTableIndex++;
                }
            }
        }

        /// <summary>
        ///     Clears this instance.
        /// </summary>
        public void Clear()
        {
            // Clear the elements in the store
            Array.Clear(_hashTableStore, 0, _hashTableStore.Length);

            // Re-initialize to empty collection.
            _hashTableStore = _emptyArray;

            _size = 0;
            _slotsLoadFactor = 0;
            _freeSlotsCount = _hashTableStore.Length;
        }


        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        ///     Rehash the the current collection elements to a new collection.
        /// </summary>
        private void _rehash(ref DLinkedList<TKey, TValue>[] newHashTableStore, int oldHashTableSize)
        {
            // Reset the free slots count
            _freeSlotsCount = newHashTableStore.Length;

            for (var i = 0; i < oldHashTableSize; ++i)
            {
                var chain = _hashTableStore[i];

                if (chain != null && chain.Count > 0)
                {
                    var head = chain.Head;

                    while (head != null)
                    {
                        var hash = _getHashOfKey(head.Key, newHashTableStore.Length);

                        if (newHashTableStore[hash] == null)
                        {
                            _freeSlotsCount--;
                            newHashTableStore[hash] = new DLinkedList<TKey, TValue>();
                        }

                        newHashTableStore[hash].Append(head.Key, head.Value);

                        head = head.Next;
                    }
                }
            } //end-for
        }

        /// <summary>
        ///     Contracts the capacity of the keys and values arrays.
        /// </summary>
        private void _contractCapacity()
        {
            var oneThird = _hashTableStore.Length / 3;
            var twoThirds = 2 * oneThird;

            if (_size <= oneThird)
            {
                var newCapacity = _hashTableStore.Length == 0 ? _defaultCapacity : twoThirds;

                // Try to expand the size
                var newHashTableStore = new DLinkedList<TKey, TValue>[newCapacity];

                // Rehash
                if (_size > 0)
                {
                    _rehash(ref newHashTableStore, _hashTableStore.Length);
                } //end-if

                _hashTableStore = newHashTableStore;
            }
        }

        /// <summary>
        ///     Expands the capacity of the keys and values arrays.
        /// </summary>
        private void _expandCapacity(int minCapacity)
        {
            if (_hashTableStore.Length < minCapacity)
            {
                var newCapacity = _hashTableStore.Length == 0 ? _defaultCapacity : _hashTableStore.Length * 2;

                // Make sure it doesn't divide by 2 or 10
                if (newCapacity % 2 == 0 || newCapacity % 10 == 0)
                    newCapacity = newCapacity + 1;

                // Handle overflow
                if (newCapacity >= MAX_ARRAY_LENGTH)
                    newCapacity = MAX_ARRAY_LENGTH;
                else if (newCapacity < minCapacity)
                    newCapacity = minCapacity;

                // Try to expand the size
                try
                {
                    var newHashTableStore = new DLinkedList<TKey, TValue>[newCapacity];

                    // Rehash
                    if (_size > 0)
                    {
                        _rehash(ref newHashTableStore, _hashTableStore.Length);
                    } //end-if

                    _hashTableStore = newHashTableStore;
                }
                catch (OutOfMemoryException)
                {
                    throw;
                }
            }
        }

        /// <summary>
        ///     A high-level functon that handles both contraction and expansion of the internal collection.
        /// </summary>
        /// <param name="mode">Contract or Expand.</param>
        /// <param name="newSize">The new expansion size.</param>
        private void _ensureCapacity(CapacityManagementMode mode, int newSize = -1)
        {
            // If the size of the internal collection is less than or equal to third of 
            // ... the total capacity then contract the internal collection
            var oneThird = _hashTableStore.Length / 3;

            if (mode == CapacityManagementMode.Contract && _size <= oneThird)
            {
                _contractCapacity();
            }
            else if (mode == CapacityManagementMode.Expand && newSize > 0)
            {
                _expandCapacity(newSize);
            }
        }

        /// <summary>
        ///     Hash Function.
        ///     The universal hashing principle method.
        /// </summary>
        private uint _universalHashFunction(TKey key, int length)
        {
            if (length < 0)
                throw new IndexOutOfRangeException();

            // Hashes
            uint prehash = 0, hash = INITIAL_HASH;

            // Primes
            int a = 197, b = 4049, p = 7199369;

            prehash = _getPreHashOfKey(key);
            hash = Convert.ToUInt32((a * prehash + b) % p % length);

            return hash;
        }

        /// <summary>
        ///     Hash Function.
        ///     The division method hashing.
        /// </summary>
        private uint _divisionMethodHashFunction(TKey key, int length)
        {
            uint prehash = 0, hash = INITIAL_HASH;

            if (length < 0)
                throw new IndexOutOfRangeException();

            if (key is string && key.IsEqualTo(default(TKey)) == false)
            {
                var stringKey = Convert.ToString(key);

                for (var i = 0; i < stringKey.Length; ++i)
                {
                    hash = (hash ^ stringKey[i]) + (hash << 26) + (hash >> 6);
                }

                if (hash > length)
                    hash = Convert.ToUInt32(hash % length);
            }
            else
            {
                prehash = _getPreHashOfKey(key);
                hash = Convert.ToUInt32(37 * prehash % length);
            }

            return hash;
        }

        /// <summary>
        ///     Returns an integer that represents the key.
        ///     Used in the _hashKey function.
        /// </summary>
        private uint _getPreHashOfKey(TKey key)
        {
            return Convert.ToUInt32(Math.Abs(_keysComparer.GetHashCode(key)));
        }

        /// <summary>
        ///     Returns a key from 0 to m where m is the size of the keys-and-values map. The hash serves as an index.
        /// </summary>
        private uint _getHashOfKey(TKey key, int length)
        {
            return _universalHashFunction(key, length);
        }

        /// <summary>
        ///     Returns a key from 0 to m where m is the size of the keys-and-values map. The hash serves as an index.
        ///     Division Method.
        /// </summary>
        private uint _getHashOfKey(TKey key)
        {
            return _universalHashFunction(key, _hashTableStore.Length);
        }
    }
    public class CuckooHashTable<TKey, TValue> where TKey : IComparable<TKey>
    {
        /// <summary>
        /// THE CUCKOO HASH TABLE ENTERY
        /// </summary>
        private class CHashEntry<TKey, TValue> where TKey : IComparable<TKey>
        {
            public TKey Key { get; set; }
            public TValue Value { get; set; }
            public bool IsActive { get; set; }

            public CHashEntry() : this(default(TKey), default(TValue), false) { }

            public CHashEntry(TKey key, TValue value, bool isActive)
            {
                Key = key;
                Value = value;
                IsActive = isActive;
            }
        }


        /// <summary>
        /// INSTANCE VARIABLES
        /// </summary>
        private const int DEFAULT_CAPACITY = 11;
        private const double MAX_LOAD_FACTOR = 0.45;
        private const int ALLOWED_REHASHES = 5;
        private const int NUMBER_OF_HASH_FUNCTIONS = 7; // number of hash functions to use, selected 7 because it's prime. The choice was arbitrary.
        internal readonly PrimesList PRIMES = PrimesList.Instance;

        // Random number generator
        private Random _randomizer;

        private int _size { get; set; }
        private int _numberOfRehashes { get; set; }
        private CHashEntry<TKey, TValue>[] _collection { get; set; }
        private UniversalHashingFamily _universalHashingFamily { get; set; }
        private EqualityComparer<TKey> _equalityComparer = EqualityComparer<TKey>.Default;

        // The C# Maximum Array Length (before encountering overflow)
        // Reference: http://referencesource.microsoft.com/#mscorlib/system/array.cs,2d2b551eabe74985
        private const int MAX_ARRAY_LENGTH = 0X7FEFFFFF;


        /// <summary>
        /// CONSTRUCTOR
        /// </summary>
        public CuckooHashTable()
        {
            _size = 0;
            _numberOfRehashes = 0;
            _randomizer = new Random();
            _collection = new CHashEntry<TKey, TValue>[DEFAULT_CAPACITY];
            _universalHashingFamily = new UniversalHashingFamily(NUMBER_OF_HASH_FUNCTIONS);
        }

        /// <summary>
        /// Expands the size of internal collection.
        /// </summary>
        private void _expandCapacity(int minCapacity)
        {
            int newCapacity = (_collection.Length == 0 ? DEFAULT_CAPACITY : _collection.Length * 2);

            // Handle overflow
            if (newCapacity >= MAX_ARRAY_LENGTH)
                newCapacity = MAX_ARRAY_LENGTH;
            else if (newCapacity < minCapacity)
                newCapacity = minCapacity;

            _rehash(Convert.ToInt32(newCapacity));
        }

        /// <summary>
        /// Contracts the size of internal collection to half.
        /// </summary>
        private void _contractCapacity()
        {
            _rehash(_size / 2);
        }

        /// <summary>
        /// Rehashes the internal internal collection.
        /// Table size stays the same, but generates new hash functions.
        /// </summary>
        private void _rehash()
        {
            _universalHashingFamily.GenerateNewFunctions();
            _rehash(_collection.Length);
        }

        /// <summary>
        /// Rehashes the internal collection to a new size.
        /// New hash table size, but the hash functions stay the same.
        /// </summary>
        private void _rehash(int newCapacity)
        {
            int primeCapacity = PRIMES.GetNextPrime(newCapacity);

            var oldSize = _size;
            var oldCollection = this._collection;

            try
            {
                this._collection = new CHashEntry<TKey, TValue>[newCapacity];

                // Reset size
                _size = 0;

                for (int i = 0; i < oldCollection.Length; ++i)
                {
                    if (oldCollection[i] != null && oldCollection[i].IsActive == true)
                    {
                        _insertHelper(oldCollection[i].Key, oldCollection[i].Value);
                    }
                }
            }
            catch (OutOfMemoryException ex)
            {
                // In case a memory overflow happens, return the data to it's old state
                // ... then throw the exception.
                _collection = oldCollection;
                _size = oldSize;

                throw ex.InnerException;
            }
        }

        /// <summary>
        /// Hashes a key, using the specified hash function number which belongs to the internal hash functions family.
        /// </summary>
        private int _cuckooHash(TKey key, int whichHashFunction)
        {
            if (whichHashFunction <= 0 || whichHashFunction > _universalHashingFamily.NumberOfFunctions)
                throw new ArgumentOutOfRangeException("Which Hash Function parameter must be betwwen 1 and " + NUMBER_OF_HASH_FUNCTIONS + ".");

            int hashCode = Math.Abs(_universalHashingFamily.UniversalHash(_equalityComparer.GetHashCode(key), whichHashFunction));

            return hashCode % _collection.Length;
        }

        /// <summary>
        /// Checks whether there is an entry at the specified position and that the entry is active.
        /// </summary>
        private bool _isActive(int index)
        {
            if (index < 0 || index > _collection.Length)
                throw new IndexOutOfRangeException();

            return (_collection[index] != null && _collection[index].IsActive == true);
        }

        /// <summary>
        /// Returns the array position (index) of the specified key.
        /// </summary>
        private int _findPosition(TKey key)
        {
            // The hash functions numbers are indexed from 1 not zero
            for (int i = 1; i <= NUMBER_OF_HASH_FUNCTIONS; ++i)
            {
                int index = _cuckooHash(key, i);

                if (_isActive(index) && _collection[index].Key.IsEqualTo(key))
                    return index;
            }

            return -1;
        }

        /// <summary>
        /// Inserts a key-value pair into hash table.
        /// </summary>
        private void _insertHelper(TKey key, TValue value)
        {
            int COUNT_LIMIT = 100;
            var newEntry = new CHashEntry<TKey, TValue>(key, value, isActive: true);

            while (true)
            {
                int position, lastPosition = -1;

                for (int count = 0; count < COUNT_LIMIT; count++)
                {
                    // The hash functions numbers are indexed from 1 not zero
                    for (int i = 1; i <= NUMBER_OF_HASH_FUNCTIONS; i++)
                    {
                        position = _cuckooHash(key, i);

                        if (!_isActive(position))
                        {
                            _collection[position] = newEntry;

                            // Increment size
                            ++_size;

                            return;
                        }
                    }

                    // Eviction strategy:
                    // No available spot was found. Choose a random one.
                    int j = 0;
                    do
                    {
                        position = _cuckooHash(key, _randomizer.Next(1, NUMBER_OF_HASH_FUNCTIONS));
                    } while (position == lastPosition && j++ < NUMBER_OF_HASH_FUNCTIONS);

                    // SWAP ENTRY
                    lastPosition = position;

                    var temp = _collection[position];
                    _collection[position] = newEntry;
                    newEntry = temp;

                }//end-for

                if (++_numberOfRehashes > ALLOWED_REHASHES)
                {
                    // Expand the table.
                    _expandCapacity(_collection.Length + 1);

                    // Reset number of rehashes.
                    _numberOfRehashes = 0;
                }
                else
                {
                    // Rehash the table with the same current size.
                    _rehash();
                }
            }//end-while
        }


        /// <summary>
        /// Returns number of items in hash table.
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return _size;
        }

        /// <summary>
        /// Returns true if hash table is empty; otherwise, false.
        /// </summary>
        public bool IsEmpty()
        {
            return (_size == 0);
        }

        /// <summary>
        /// Returns the value of the specified key, if exists; otherwise, raises an exception.
        /// </summary>
        public TValue this[TKey key]
        {
            get
            {
                int position = _findPosition(key);

                if (position != -1)
                    return _collection[position].Value;

                throw new KeyNotFoundException();
            }
            set
            {
                if (ContainsKey(key) == true)
                    Update(key, value);

                throw new KeyNotFoundException();
            }
        }

        /// <summary>
        /// Checks if a key exists in the hash table.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            return (_findPosition(key) != -1);
        }

        /// <summary>
        /// Insert key-value pair into hash table.
        /// </summary>
        public void Add(TKey key, TValue value)
        {
            if (ContainsKey(key))
                throw new Exception("Key already exists in the hash table.");

            if (_size >= _collection.Length * MAX_LOAD_FACTOR)
                _expandCapacity(_collection.Length + 1);

            _insertHelper(key, value);
        }

        /// <summary>
        /// Updates a key-value pair with a new value.
        /// </summary>
        public void Update(TKey key, TValue value)
        {
            int position = _findPosition(key);

            if (position == -1)
                throw new KeyNotFoundException();

            _collection[position].Value = value;
        }

        /// <summary>
        /// Remove the key-value pair specified by the given key.
        /// </summary>
        public bool Remove(TKey key)
        {
            int currentPosition = _findPosition(key);

            if (!_isActive(currentPosition))
                return false;

            // Mark the entry as not active
            _collection[currentPosition].IsActive = false;

            // Decrease the size
            --_size;

            return true;
        }

        /// <summary>
        /// Clears this hash table.
        /// </summary>
        public void Clear()
        {
            this._size = 0;

            Parallel.ForEach(_collection,
                (item) =>
                {
                    if (item != null && item.IsActive == true)
                    {
                        item.IsActive = false;
                    }
                });
        }
    }

    public class OpenScatterHashTable<TKey, TValue> : IDictionary<TKey, TValue> where TKey : IComparable<TKey>
    {
        // Initialization-related
        private const int _defaultCapacity = 7;

        // This is the maximum prime that is smaller than the C# maximum allowed size of arrays.
        // Check the following reference: 
        // C# Maximum Array Length (before encountering overflow).
        // Link: http://referencesource.microsoft.com/#mscorlib/system/array.cs,2d2b551eabe74985
        private const int MAX_PRIME_ARRAY_LENGTH = 0x7FEFFFFD;

        // Picked the HashPrime to be (101) because it is prime, and if the ‘hashSize - 1’ is not a multiple of this HashPrime, which is 
        // enforced in _getUpperBoundPrime, then expand function has the potential of being every value from 1 to hashSize - 1. 
        // The choice is largely arbitrary.
        private const int HASH_PRIME = 101;

        private static readonly HashTableEntry<TKey, TValue>[] _emptyArray =
            new HashTableEntry<TKey, TValue>[_defaultCapacity];

        // A collection of prime numbers to use as hash table sizes. 
        internal static readonly PrimesList _primes = PrimesList.Instance;
        private HashTableEntry<TKey, TValue>[] _hashTableStore;
        private decimal _loadFactor;


        /// <summary>
        ///     INSTANCE VARIABLES
        /// </summary>
        private int _size;

        // Helper collections.
        private List<TKey> _keysCollection { get; set; }
        private List<TValue> _valuesCollection { get; set; }

        // Keys and Values Comparers
        private EqualityComparer<TKey> _keysComparer { get; set; }
        private EqualityComparer<TValue> _valuesComparer { get; set; }


        public void Add(TKey key, TValue value)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(TKey key)
        {
            throw new NotImplementedException();
        }

        public ICollection<TKey> Keys
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(TKey key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            throw new NotImplementedException();
        }

        public ICollection<TValue> Values
        {
            get { throw new NotImplementedException(); }
        }

        public TValue this[TKey key]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        ///     Returns the next biggest prime that is greater than twice the size of the interal array (size * 2).
        /// </summary>
        private int _getExpandPrime(int oldSize)
        {
            var newSize = 2 * oldSize;

            // Allow the hashtables to grow to maximum possible size (~2G elements) before encoutering capacity overflow.
            // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
            if ((uint)newSize > MAX_PRIME_ARRAY_LENGTH && MAX_PRIME_ARRAY_LENGTH > oldSize)
            {
                return MAX_PRIME_ARRAY_LENGTH;
            }

            return _primes.GetNextPrime(newSize);
        }

        /// <summary>
        ///     Get the next smaller prime that is smaller than half the size of the internal array (size / 2);
        /// </summary>
        private int _getContractPrime(int oldSize)
        {
            var newSize = oldSize / 2;

            return _primes.GetNextPrime(newSize);
        }

        /// <summary>
        ///     Contracts the capacity of the keys and values arrays.
        /// </summary>
        private void _contractCapacity()
        {
            // Only contract the array if the number of elements is less than 1/3 of the total array size.
            var oneThird = _hashTableStore.Length / 3;

            if (_size <= oneThird)
            {
                var newCapacity = _hashTableStore.Length == 0
                    ? _defaultCapacity
                    : _getContractPrime(_hashTableStore.Length);

                // Try to expand the size
                var newKeysMap = new HashTableEntry<TKey, TValue>[newCapacity];

                if (_size > 0)
                {
                    // REHASH
                }

                _hashTableStore = newKeysMap;
            }
        }

        /// <summary>
        ///     Expands the capacity of the keys and values arrays.
        /// </summary>
        private void _expandCapacity(int minCapacity)
        {
            if (_hashTableStore.Length < minCapacity)
            {
                var newCapacity = _hashTableStore.Length == 0
                    ? _defaultCapacity
                    : _getExpandPrime(_hashTableStore.Length * 2);

                if (newCapacity >= MAX_PRIME_ARRAY_LENGTH)
                    newCapacity = MAX_PRIME_ARRAY_LENGTH;

                // Try to expand the size
                try
                {
                    var newKeysMap = new HashTableEntry<TKey, TValue>[newCapacity];

                    if (_size > 0)
                    {
                        // REHASH
                    }

                    _hashTableStore = newKeysMap;
                }
                catch (OutOfMemoryException)
                {
                    throw;
                }
            }
        }

        /// <summary>
        ///     A high-level functon that handles both contraction and expansion of the internal collection.
        /// </summary>
        /// <param name="mode">Contract or Expand.</param>
        /// <param name="newSize">The new expansion size.</param>
        private void _ensureCapacity(CapacityManagementMode mode, int newSize = -1)
        {
            // If the size of the internal collection is less than or equal to third of 
            // ... the total capacity then contract the internal collection
            var oneThird = _hashTableStore.Length / 3;

            if (mode == CapacityManagementMode.Contract && _size <= oneThird)
            {
                _contractCapacity();
            }
            else if (mode == CapacityManagementMode.Expand && newSize > 0)
            {
                _expandCapacity(newSize);
            }
        }

        /// <summary>
        ///     Returns an integer that represents the key.
        ///     Used in the _hashKey function.
        /// </summary>
        private int _getPreHashOfKey(TKey key)
        {
            return Math.Abs(_keysComparer.GetHashCode(key));
        }

        /// <summary>
        ///     Hash Table Cell.
        /// </summary>
        private class HashTableEntry<TKey, TValue> where TKey : IComparable<TKey>
        {
            public HashTableEntry() : this(default(TKey), default(TValue))
            {
            }

            public HashTableEntry(TKey key, TValue value, EntryStatus status = EntryStatus.Occupied)
            {
                Key = key;
                Value = value;
                Status = status;
            }

            public TKey Key { get; set; }
            public TValue Value { get; set; }
            public EntryStatus Status { get; }

            public bool IsEmpty
            {
                get { return Status == EntryStatus.Empty; }
            }

            public bool IsOccupied
            {
                get { return Status == EntryStatus.Occupied; }
            }

            public bool IsDeleted
            {
                get { return Status == EntryStatus.Deleted; }
            }
        }

        /// <summary>
        ///     The hash table cell status modes: Empty cell, Occupied cell, Deleted cell.
        /// </summary>
        private enum EntryStatus
        {
            Empty = 0,
            Occupied = 1,
            Deleted = 2
        }

        /// <summary>
        ///     Used in the ensure capacity function
        /// </summary>
        private enum CapacityManagementMode
        {
            Contract = 0,
            Expand = 1
        }
    }
}
