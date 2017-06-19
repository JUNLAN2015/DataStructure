/***
 * Trie.
 * 
 * This is the standard/vanilla implementation of a Trie. For an associative version of Trie, checkout the TrieMap<TRecord> class.
 * 
 * This class implements the IEnumerable interface.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DataStructures.Trees
{
    /// <summary>
    ///     The vanila Trie implementation.
    /// </summary>
    public class Trie : IEnumerable<string>
    {
        /// <summary>
        ///     CONSTRUCTOR
        /// </summary>
        public Trie()
        {
            _count = 0;
            _root = new TrieNode(' ', false);
        }

        private int _count { get; set; }
        private TrieNode _root { get; set; }

        /// <summary>
        ///     Return count of words.
        /// </summary>
        public int Count
        {
            get { return _count; }
        }

        /// <summary>
        ///     Checks if element is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return _count == 0; }
        }

        /// <summary>
        ///     Add word to trie
        /// </summary>
        public void Add(string word)
        {
            if (string.IsNullOrEmpty(word))
                throw new ArgumentException("Word is empty or null.");

            var current = _root;

            for (var i = 0; i < word.Length; ++i)
            {
                if (!current.Children.ContainsKey(word[i]))
                {
                    var newTrieNode = new TrieNode(word[i]);
                    newTrieNode.Parent = current;
                    current.Children.Add(word[i], newTrieNode);
                }

                current = current.Children[word[i]];
            }

            if (current.IsTerminal)
                throw new ApplicationException("Word already exists in Trie.");

            ++_count;
            current.IsTerminal = true;
        }

        /// <summary>
        ///     Removes a word from the trie.
        /// </summary>
        public void Remove(string word)
        {
            if (string.IsNullOrEmpty(word))
                throw new ArgumentException("Word is empty or null.");

            var current = _root;

            for (var i = 0; i < word.Length; ++i)
            {
                if (!current.Children.ContainsKey(word[i]))
                    throw new KeyNotFoundException("Word doesn't belong to trie.");

                current = current.Children[word[i]];
            }

            if (!current.IsTerminal)
                throw new KeyNotFoundException("Word doesn't belong to trie.");

            --_count;
            current.Remove();
        }

        /// <summary>
        ///     Checks whether the trie has a specific word.
        /// </summary>
        public bool ContainsWord(string word)
        {
            if (string.IsNullOrEmpty(word))
                throw new ApplicationException("Word is either null or empty.");

            var current = _root;

            for (var i = 0; i < word.Length; ++i)
            {
                if (!current.Children.ContainsKey(word[i]))
                    return false;

                current = current.Children[word[i]];
            }

            return current.IsTerminal;
        }

        /// <summary>
        ///     Checks whether the trie has a specific prefix.
        /// </summary>
        public bool ContainsPrefix(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
                throw new ApplicationException("Prefix is either null or empty.");

            var current = _root;

            for (var i = 0; i < prefix.Length; ++i)
            {
                if (!current.Children.ContainsKey(prefix[i]))
                    return false;

                current = current.Children[prefix[i]];
            }

            return true;
        }

        /// <summary>
        ///     Searches the entire trie for words that has a specific prefix.
        /// </summary>
        public IEnumerable<string> SearchByPrefix(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
                throw new ApplicationException("Prefix is either null or empty.");

            var current = _root;

            for (var i = 0; i < prefix.Length; ++i)
            {
                if (!current.Children.ContainsKey(prefix[i]))
                    return null;

                current = current.Children[prefix[i]];
            }

            return current.GetByPrefix();
        }

        /// <summary>
        ///     Clears this insance.
        /// </summary>
        public void Clear()
        {
            _count = 0;
            _root.Clear();
            _root = new TrieNode(' ', false);
        }

        #region IEnumerable<String> Implementation

        /// <summary>
        ///     IEnumerable\<String\>.IEnumerator implementation.
        /// </summary>
        public IEnumerator<string> GetEnumerator()
        {
            return _root.GetTerminalChildren().Select(node => node.Word).GetEnumerator();
        }

        /// <summary>
        ///     IEnumerable\<String\>.IEnumerator implementation.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IEnumerable<String> Implementation
    }

    public class TrieMap<TRecord> : IDictionary<string, TRecord>, IEnumerable<KeyValuePair<string, TRecord>>
    {
        private readonly EqualityComparer<TRecord> _recordsComparer = EqualityComparer<TRecord>.Default;

        /// <summary>
        ///     CONSTRUCTOR
        /// </summary>
        public TrieMap()
        {
            _count = 0;
            _root = new TrieMapNode<TRecord>(' ', default(TRecord), false);
        }

        private int _count { get; set; }
        private TrieMapNode<TRecord> _root { get; set; }

        /// <summary>
        ///     Checks if element is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return _count == 0; }
        }

        /// <summary>
        ///     Return count of words.
        /// </summary>
        public int Count
        {
            get { return _count; }
        }

        /// <summary>
        ///     Add word to trie
        /// </summary>
        public void Add(string word, TRecord record)
        {
            if (string.IsNullOrEmpty(word))
                throw new ArgumentException("Word is empty or null.");

            var current = _root;

            for (var i = 0; i < word.Length; ++i)
            {
                if (!current.Children.ContainsKey(word[i]))
                {
                    var newTrieNode = new TrieMapNode<TRecord>(word[i], default(TRecord));
                    newTrieNode.Parent = current;
                    current.Children.Add(word[i], newTrieNode);
                }

                current = current.Children[word[i]];
            }

            if (current.IsTerminal)
                throw new ApplicationException("Word already exists in Trie.");

            ++_count;
            current.IsTerminal = true;
            current.Record = record;
        }

        /// <summary>
        ///     Clears this insance.
        /// </summary>
        public void Clear()
        {
            _count = 0;
            _root.Clear();
            _root = new TrieMapNode<TRecord>(' ', default(TRecord), false);
        }

        /// <summary>
        ///     Updates a terminal word with a new record. Throws an exception if word was not found or if it is not a terminal
        ///     word.
        /// </summary>
        public void UpdateWord(string word, TRecord newRecord)
        {
            if (string.IsNullOrEmpty(word))
                throw new ApplicationException("Word is either null or empty.");

            var current = _root;

            for (var i = 0; i < word.Length; ++i)
            {
                if (!current.Children.ContainsKey(word[i]))
                    throw new KeyNotFoundException("Word doesn't belong to trie.");

                current = current.Children[word[i]];
            }

            if (!current.IsTerminal)
                throw new KeyNotFoundException("Word doesn't belong to trie.");

            current.Record = newRecord;
        }

        /// <summary>
        ///     Removes a word from the trie.
        /// </summary>
        public void Remove(string word)
        {
            if (string.IsNullOrEmpty(word))
                throw new ArgumentException("Word is empty or null.");

            var current = _root;

            for (var i = 0; i < word.Length; ++i)
            {
                if (!current.Children.ContainsKey(word[i]))
                    throw new KeyNotFoundException("Word doesn't belong to trie.");

                current = current.Children[word[i]];
            }

            if (!current.IsTerminal)
                throw new KeyNotFoundException("Word doesn't belong to trie.");

            --_count;
            current.Remove();
        }

        /// <summary>
        ///     Checks whether the trie has a specific word.
        /// </summary>
        public bool ContainsWord(string word)
        {
            TRecord record;
            return SearchByWord(word, out record);
        }

        /// <summary>
        ///     Checks whether the trie has a specific prefix.
        /// </summary>
        public bool ContainsPrefix(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
                throw new ApplicationException("Prefix is either null or empty.");

            var current = _root;

            for (var i = 0; i < prefix.Length; ++i)
            {
                if (!current.Children.ContainsKey(prefix[i]))
                    return false;

                current = current.Children[prefix[i]];
            }

            return true;
        }

        /// <summary>
        ///     Searchs the trie for a word and returns the associated record, if found; otherwise returns false.
        /// </summary>
        public bool SearchByWord(string word, out TRecord record)
        {
            if (string.IsNullOrEmpty(word))
                throw new ApplicationException("Word is either null or empty.");

            record = default(TRecord);
            var current = _root;

            for (var i = 0; i < word.Length; ++i)
            {
                if (!current.Children.ContainsKey(word[i]))
                    return false;

                current = current.Children[word[i]];
            }

            if (!current.IsTerminal)
                return false;

            record = current.Record;
            return true;
        }

        /// <summary>
        ///     Searches the entire trie for words that has a specific prefix.
        /// </summary>
        public IEnumerable<KeyValuePair<string, TRecord>> SearchByPrefix(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
                throw new ApplicationException("Prefix is either null or empty.");

            var current = _root;

            for (var i = 0; i < prefix.Length; ++i)
            {
                if (!current.Children.ContainsKey(prefix[i]))
                    return null;

                current = current.Children[prefix[i]];
            }

            return current.GetByPrefix();
        }

        #region IDictionary implementation

        bool ICollection<KeyValuePair<string, TRecord>>.IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        ///     Checks whether a specific key exists in trie as a word (terminal word).
        /// </summary>
        bool IDictionary<string, TRecord>.ContainsKey(string key)
        {
            TRecord record;
            return SearchByWord(key, out record);
        }

        /// <summary>
        ///     Return all terminal words in trie.
        /// </summary>
        ICollection<string> IDictionary<string, TRecord>.Keys
        {
            get
            {
                var collection = new List<string>(Count);

                var terminalNodes = _root.GetTerminalChildren();
                foreach (var node in terminalNodes)
                    collection.Add(node.Word);

                return collection;
            }
        }

        /// <summary>
        ///     Return all the associated records to terminal words.
        /// </summary>
        ICollection<TRecord> IDictionary<string, TRecord>.Values
        {
            get
            {
                var collection = new List<TRecord>(Count);

                var terminalNodes = _root.GetTerminalChildren();
                foreach (var node in terminalNodes)
                    collection.Add(node.Record);

                return collection;
            }
        }

        /// <summary>
        ///     Tries to get the associated record of a terminal word from trie. Returns false if key was not found.
        /// </summary>
        bool IDictionary<string, TRecord>.TryGetValue(string key, out TRecord value)
        {
            return SearchByWord(key, out value);
        }

        /// <summary>
        ///     Checks whether a specific word-record pair exists in trie. The key of item must be a terminal word not a prefix.
        /// </summary>
        bool ICollection<KeyValuePair<string, TRecord>>.Contains(KeyValuePair<string, TRecord> item)
        {
            TRecord record;
            var status = SearchByWord(item.Key, out record);
            return status && _recordsComparer.Equals(item.Value, record);
        }

        void ICollection<KeyValuePair<string, TRecord>>.CopyTo(KeyValuePair<string, TRecord>[] array, int arrayIndex)
        {
            var tempArray = _root.GetTerminalChildren()
                .Select(item => new KeyValuePair<string, TRecord>(item.Word, item.Record))
                .ToArray();

            Array.Copy(tempArray, 0, array, arrayIndex, Count);
        }

        /// <summary>
        ///     Get/Set the associated record of a terminal word in trie.
        /// </summary>
        TRecord IDictionary<string, TRecord>.this[string key]
        {
            get
            {
                TRecord record;
                if (SearchByWord(key, out record))
                    return record;
                throw new KeyNotFoundException();
            }
            set { UpdateWord(key, value); }
        }

        void ICollection<KeyValuePair<string, TRecord>>.Add(KeyValuePair<string, TRecord> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        ///     Remove a word from trie.
        /// </summary>
        bool IDictionary<string, TRecord>.Remove(string key)
        {
            try
            {
                Remove(key);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Removes a word from trie.
        /// </summary>
        bool ICollection<KeyValuePair<string, TRecord>>.Remove(KeyValuePair<string, TRecord> item)
        {
            try
            {
                Remove(item.Key);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion IDictionary implementation

        #region IEnumerable implementation

        public IEnumerator<KeyValuePair<string, TRecord>> GetEnumerator()
        {
            return _root.GetTerminalChildren()
                .Select(item => new KeyValuePair<string, TRecord>(item.Word, item.Record))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IEnumerable implementation
    }

    public class TrieMapNode<TRecord> : IComparable<TrieMapNode<TRecord>>
    {
        /// <summary>
        ///     CONSTRUCTORS
        /// </summary>
        public TrieMapNode(char key, TRecord record) : this(key, record, false)
        {
        }

        public TrieMapNode(char key, TRecord record, bool isTerminal)
        {
            Key = key;
            Record = record;
            IsTerminal = isTerminal;
            Children = new Dictionary<char, TrieMapNode<TRecord>>();
        }

        // Node key
        public virtual char Key { get; set; }

        // Associated record with this node
        public virtual TRecord Record { get; set; }

        // Is Terminal node flag
        public virtual bool IsTerminal { get; set; }

        // Parent pointer
        public virtual TrieMapNode<TRecord> Parent { get; set; }

        // Dictionary of child-nodes
        public virtual Dictionary<char, TrieMapNode<TRecord>> Children { get; set; }

        /// <summary>
        ///     Return the word at this node if the node is terminal; otherwise, return null
        /// </summary>
        public virtual string Word
        {
            get
            {
                if (!IsTerminal)
                    return null;

                var curr = this;
                var stack = new Stack<char>();

                while (curr.Parent != null)
                {
                    stack.Push(curr.Key);
                    curr = curr.Parent;
                }

                return new string(stack.ToArray());
            }
        }

        /// <summary>
        ///     IComparer interface implementation
        /// </summary>
        public int CompareTo(TrieMapNode<TRecord> other)
        {
            if (other == null)
                return -1;

            return Key.CompareTo(other.Key);
        }

        /// <summary>
        ///     Returns an enumerable list of key-value pairs of all the words that start
        ///     with the prefix that maps from the root node until this node.
        /// </summary>
        public virtual IEnumerable<KeyValuePair<string, TRecord>> GetByPrefix()
        {
            if (IsTerminal)
                yield return new KeyValuePair<string, TRecord>(Word, Record);

            foreach (var childKeyVal in Children)
                foreach (var terminalNode in childKeyVal.Value.GetByPrefix())
                    yield return terminalNode;
        }

        /// <summary>
        ///     Returns an enumerable collection of terminal child nodes.
        /// </summary>
        public virtual IEnumerable<TrieMapNode<TRecord>> GetTerminalChildren()
        {
            foreach (var child in Children.Values)
            {
                if (child.IsTerminal)
                    yield return child;

                foreach (var grandChild in child.GetTerminalChildren())
                    if (grandChild.IsTerminal)
                        yield return grandChild;
            }
        }

        /// <summary>
        ///     Remove this element upto its parent.
        /// </summary>
        public virtual void Remove()
        {
            IsTerminal = false;

            if (Children.Count == 0 && Parent != null)
            {
                Parent.Children.Remove(Key);

                if (!Parent.IsTerminal)
                    Parent.Remove();
            }
        }

        /// <summary>
        ///     Clears this node instance
        /// </summary>
        public void Clear()
        {
            Children.Clear();
            Children = null;
        }
    }

    public class TrieNode
    {
        /// <summary>
        ///     CONSTRUCTORS
        /// </summary>
        public TrieNode(char key) : this(key, false)
        {
        }

        public TrieNode(char key, bool isTerminal)
        {
            Key = key;
            IsTerminal = isTerminal;
            Children = new Dictionary<char, TrieNode>();
        }

        /// <summary>
        ///     Instance variables.
        /// </summary>
        public virtual char Key { get; set; }

        public virtual bool IsTerminal { get; set; }
        public virtual TrieNode Parent { get; set; }
        public virtual Dictionary<char, TrieNode> Children { get; set; }

        /// <summary>
        ///     Return the word at this node if the node is terminal; otherwise, return null
        /// </summary>
        public virtual string Word
        {
            get
            {
                if (!IsTerminal)
                    return null;

                var curr = this;
                var stack = new Stack<char>();

                while (curr.Parent != null)
                {
                    stack.Push(curr.Key);
                    curr = curr.Parent;
                }

                return new string(stack.ToArray());
            }
        }

        /// <summary>
        ///     Returns an enumerable list of key-value pairs of all the words that start
        ///     with the prefix that maps from the root node until this node.
        /// </summary>
        public virtual IEnumerable<string> GetByPrefix()
        {
            if (IsTerminal)
                yield return Word;

            foreach (var childKeyVal in Children)
                foreach (var terminalNode in childKeyVal.Value.GetByPrefix())
                    yield return terminalNode;
        }

        /// <summary>
        ///     Returns an enumerable collection of terminal child nodes.
        /// </summary>
        public virtual IEnumerable<TrieNode> GetTerminalChildren()
        {
            foreach (var child in Children.Values)
            {
                if (child.IsTerminal)
                    yield return child;

                foreach (var grandChild in child.GetTerminalChildren())
                    if (grandChild.IsTerminal)
                        yield return grandChild;
            }
        }

        /// <summary>
        ///     Remove this element upto its parent.
        /// </summary>
        public virtual void Remove()
        {
            IsTerminal = false;

            if (Children.Count == 0 && Parent != null)
            {
                Parent.Children.Remove(Key);

                if (!Parent.IsTerminal)
                    Parent.Remove();
            }
        }

        /// <summary>
        ///     IComparer interface implementation
        /// </summary>
        public int CompareTo(TrieNode other)
        {
            if (other == null)
                return -1;

            return Key.CompareTo(other.Key);
        }

        /// <summary>
        ///     Clears this node instance
        /// </summary>
        public void Clear()
        {
            Children.Clear();
            Children = null;
        }
    }
}