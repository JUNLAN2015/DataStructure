using System;
using System.Collections;
using System.Collections.Generic;
using DataStructures.Common;

namespace DataStructures.Trees
{
    public class BinarySearchTree<T> : IBinarySearchTree<T> where T : IComparable<T>
    {
       public enum TraversalMode
        {
            InOrder = 0,
            PreOrder = 1,
            PostOrder = 2
        }

        public BinarySearchTree()
        {
            _count = 0;
            _allowDuplicates = true;
            Root = null;
        }

        public BinarySearchTree(bool allowDuplicates)
        {
            _count = 0;
            _allowDuplicates = allowDuplicates;
            Root = null;
        }

        protected int _count { get; set; }

        protected bool _allowDuplicates { get; set; }
        protected virtual BSTNode<T> _root { get; set; }

        public virtual BSTNode<T> Root
        {
            get { return _root; }
            internal set { _root = value; }
        }
        
        public virtual int Count
        {
            get { return _count; }
        }

        public virtual bool IsEmpty
        {
            get { return _count == 0; }
        }

      
        public virtual int Height
        {
            get
            {
                if (IsEmpty)
                    return 0;

                var currentNode = Root;
                return _getTreeHeight(currentNode);
            }
        }

        public virtual bool AllowsDuplicates
        {
            get { return _allowDuplicates; }
        }

        public virtual void Insert(T item)
        {
            var newNode = new BSTNode<T>(item);
            var success = _insertNode(newNode);
            if (success == false && _allowDuplicates == false)
                throw new InvalidOperationException("Tree does not allow inserting duplicate elements.");
        }

        /// <summary>
        ///     Inserts an array of elements to the tree.
        /// </summary>
        public virtual void Insert(T[] collection)
        {
            if (collection == null)
                throw new ArgumentNullException();

            if (collection.Length > 0)
            {
                for (var i = 0; i < collection.Length; ++i)
                {
                    Insert(collection[i]);
                }
            }
        }

        /// <summary>
        ///     Inserts a list of elements to the tree.
        /// </summary>
        public virtual void Insert(List<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException();

            if (collection.Count > 0)
            {
                for (var i = 0; i < collection.Count; ++i)
                {
                    Insert(collection[i]);
                }
            }
        }

        /// <summary>
        ///     Deletes an element from the tree
        /// </summary>
        /// <param name="item">item to remove.</param>
        public virtual void Remove(T item)
        {
            if (IsEmpty)
                throw new Exception("Tree is empty.");

            var node = _findNode(Root, item);
            var status = _remove(node);

            // If the element was found, remove it.
            if (status == false)
                throw new Exception("Item was not found.");
        }

        /// <summary>
        ///     Removes the min value from tree.
        /// </summary>
        public virtual void RemoveMin()
        {
            if (IsEmpty)
                throw new Exception("Tree is empty.");

            var node = _findMinNode(Root);
            _remove(node);
        }

        /// <summary>
        ///     Removes the max value from tree.
        /// </summary>
        public virtual void RemoveMax()
        {
            if (IsEmpty)
                throw new Exception("Tree is empty.");

            var node = _findMaxNode(Root);
            _remove(node);
        }

        /// <summary>
        ///     Clears all elements from tree.
        /// </summary>
        public virtual void Clear()
        {
            Root = null;
            _count = 0;
        }

        /// <summary>
        ///     Checks for the existence of an item
        /// </summary>
        public virtual bool Contains(T item)
        {
            return _findNode(_root, item) != null;
        }

        /// <summary>
        ///     Finds the minimum in tree
        /// </summary>
        /// <returns>Min</returns>
        public virtual T FindMin()
        {
            if (IsEmpty)
                throw new Exception("Tree is empty.");

            return _findMinNode(Root).Value;
        }

        /// <summary>
        ///     Finds the maximum in tree
        /// </summary>
        /// <returns>Max</returns>
        public virtual T FindMax()
        {
            if (IsEmpty)
                throw new Exception("Tree is empty.");

            return _findMaxNode(Root).Value;
        }

        /// <summary>
        ///     Find the item in the tree. Throws an exception if not found.
        /// </summary>
        /// <param name="item">Item to find.</param>
        /// <returns>Item.</returns>
        public virtual T Find(T item)
        {
            if (IsEmpty)
                throw new Exception("Tree is empty.");

            var node = _findNode(Root, item);

            if (node != null)
                return node.Value;
            throw new Exception("Item was not found.");
        }

        /// <summary>
        ///     Given a predicate function, find all the elements that match it.
        /// </summary>
        /// <param name="searchPredicate">The search predicate</param>
        /// <returns>ArrayList<T> of elements.</returns>
        public virtual IEnumerable<T> FindAll(Predicate<T> searchPredicate)
        {
            var list = new List<T>();
            _findAll(Root, searchPredicate, ref list);

            return list;
        }

        /// <summary>
        ///     Returns an array of nodes' values.
        /// </summary>
        /// <returns>The array.</returns>
        public virtual T[] ToArray()
        {
            return ToList().ToArray();
        }

        /// <summary>
        ///     Returns a list of the nodes' value.
        /// </summary>
        public virtual List<T> ToList()
        {
            var list = new List<T>();
            _inOrderTraverse(Root, ref list);

            return list;
        }


        /*********************************************************************/


        /// <summary>
        ///     Returns an enumerator that visits node in the order: parent, left child, right child
        /// </summary>
        public virtual IEnumerator<T> GetPreOrderEnumerator()
        {
            return new BinarySearchTreePreOrderEnumerator(this);
        }

        /// <summary>
        ///     Returns an enumerator that visits node in the order: left child, parent, right child
        /// </summary>
        public virtual IEnumerator<T> GetInOrderEnumerator()
        {
            return new BinarySearchTreeInOrderEnumerator(this);
        }

        /// <summary>
        ///     Returns an enumerator that visits node in the order: left child, right child, parent
        /// </summary>
        public virtual IEnumerator<T> GetPostOrderEnumerator()
        {
            return new BinarySearchTreePostOrderEnumerator(this);
        }


        /// <summary>
        ///     Replaces the node's value from it's parent node object with the newValue.
        ///     Used in the recusive _remove function.
        /// </summary>
        /// <param name="node">BST node.</param>
        /// <param name="newNode">New value.</param>
        protected virtual void _replaceNodeInParent(BSTNode<T> node, BSTNode<T> newNode = null)
        {
            if (node.Parent != null)
            {
                if (node.IsLeftChild)
                    node.Parent.LeftChild = newNode;
                else
                    node.Parent.RightChild = newNode;
            }

            if (newNode != null)
                newNode.Parent = node.Parent;
        }

        /// <summary>
        ///     Remove the specified node.
        /// </summary>
        /// <param name="node">Node.</param>
        /// <returns>>True if removed successfully; false if node wasn't found.</returns>
        protected virtual bool _remove(BSTNode<T> node)
        {
            if (node == null)
                return false;

            var parent = node.Parent;

            if (node.ChildrenCount == 2) // if both children are present
            {
                var successor = node.RightChild;
                node.Value = successor.Value;
                return true && _remove(successor);
            }
            if (node.HasLeftChild) // if the node has only a LEFT child
            {
                _replaceNodeInParent(node, node.LeftChild);
                _count--;
            }
            else if (node.HasRightChild) // if the node has only a RIGHT child
            {
                _replaceNodeInParent(node, node.RightChild);
                _count--;
            }
            else //this node has no children
            {
                _replaceNodeInParent(node, null);
                _count--;
            }

            return true;
        }

        /// <summary>
        ///     Inserts a new node to the tree.
        /// </summary>
        /// <param name="currentNode">Current node to insert afters.</param>
        /// <param name="newNode">New node to be inserted.</param>
        protected virtual bool _insertNode(BSTNode<T> newNode)
        {
         if (Root == null)
            {
                Root = newNode;
                _count++;
                return true;
            }
            if (newNode.Parent == null)
               newNode.Parent = Root;
            if (_allowDuplicates == false && newNode.Parent.Value.IsEqualTo(newNode.Value))
            {
                return false;
            }

            // Go Left
            if (newNode.Parent.Value.IsGreaterThan(newNode.Value)) // newNode < parent
            {
                if (newNode.Parent.HasLeftChild == false)
                {
                    newNode.Parent.LeftChild = newNode;
                    _count++;
                    return true;
                }
                newNode.Parent = newNode.Parent.LeftChild;
                return _insertNode(newNode);
            }
                // Go Right
            if (newNode.Parent.HasRightChild == false)
            {
                newNode.Parent.RightChild = newNode;
                _count++;
                return true;
            }
            newNode.Parent = newNode.Parent.RightChild;
            return _insertNode(newNode);
        }

        /// <summary>
        ///   Time-complexity: O(n), where n = number of nodes.
       protected virtual int _getTreeHeight(BSTNode<T> node)
        {
            if (node == null)
                return 0;
                // Is leaf node
            if (node.IsLeafNode)
                return 1;
                // Has two children
            if (node.ChildrenCount == 2)
                return 1 + Math.Max(_getTreeHeight(node.LeftChild), _getTreeHeight(node.RightChild));
                // Has only left
            if (node.HasLeftChild)
                return 1 + _getTreeHeight(node.LeftChild);
                // Has only right
            return 1 + _getTreeHeight(node.RightChild);
        }

        protected virtual BSTNode<T> _findNode(BSTNode<T> currentNode, T item)
        {
            if (currentNode == null)
                return currentNode;

            if (item.IsEqualTo(currentNode.Value))
            {
                return currentNode;
            }
            if (currentNode.HasLeftChild && item.IsLessThan(currentNode.Value))
            {
                return _findNode(currentNode.LeftChild, item);
            }
            if (currentNode.HasRightChild && item.IsGreaterThan(currentNode.Value))
            {
                return _findNode(currentNode.RightChild, item);
            }

            // Return-functions-fix
            return null;
        }

        protected virtual BSTNode<T> _findMinNode(BSTNode<T> node)
        {
            if (node == null)
                return node;

            var currentNode = node;

            while (currentNode.HasLeftChild)
                currentNode = currentNode.LeftChild;

            return currentNode;
        }

        protected virtual BSTNode<T> _findMaxNode(BSTNode<T> node)
        {
            if (node == null)
                return node;

            var currentNode = node;

            while (currentNode.HasRightChild)
                currentNode = currentNode.RightChild;

            return currentNode;
        }

        protected virtual BSTNode<T> _findNextSmaller(BSTNode<T> node)
        {
            if (node == null)
                return node;

            if (node.HasLeftChild)
                return _findMaxNode(node.LeftChild);

            var currentNode = node;
            while (currentNode.Parent != null && currentNode.IsLeftChild)
                currentNode = currentNode.Parent;

            return currentNode.Parent;
        }

        protected virtual BSTNode<T> _findNextLarger(BSTNode<T> node)
        {
            if (node == null)
                return node;

            if (node.HasRightChild)
                return _findMinNode(node.RightChild);

            var currentNode = node;
            while (currentNode.Parent != null && currentNode.IsRightChild)
                currentNode = currentNode.Parent;

            return currentNode.Parent;
        }


        protected virtual void _findAll(BSTNode<T> currentNode, Predicate<T> match, ref List<T> list)
        {
            if (currentNode == null)
                return;

            // call the left child
            _findAll(currentNode.LeftChild, match, ref list);

            if (match(currentNode.Value)) // match
            {
                list.Add(currentNode.Value);
            }

            // call the right child
            _findAll(currentNode.RightChild, match, ref list);
        }

        protected virtual void _inOrderTraverse(BSTNode<T> currentNode, ref List<T> list)
        {
            if (currentNode == null)
                return;
            _inOrderTraverse(currentNode.LeftChild, ref list);
            list.Add(currentNode.Value);
            _inOrderTraverse(currentNode.RightChild, ref list);
        }

        public virtual T FindNextSmaller(T item)
        {
            var node = _findNode(Root, item);
            var nextSmaller = _findNextSmaller(node);

            if (nextSmaller == null)
                throw new Exception("Item was not found.");

            return nextSmaller.Value;
        }

        public virtual T FindNextLarger(T item)
        {
            var node = _findNode(Root, item);
            var nextLarger = _findNextLarger(node);

            if (nextLarger == null)
                throw new Exception("Item was not found.");

            return nextLarger.Value;
        }


        internal class BinarySearchTreePreOrderEnumerator : IEnumerator<T>
        {
            private BSTNode<T> current;
            internal Queue<BSTNode<T>> traverseQueue;
            private BinarySearchTree<T> tree;

            public BinarySearchTreePreOrderEnumerator(BinarySearchTree<T> tree)
            {
                this.tree = tree;

                //Build queue
                traverseQueue = new Queue<BSTNode<T>>();
                visitNode(this.tree.Root);
            }

            public T Current
            {
                get { return current.Value; }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public void Dispose()
            {
                current = null;
                tree = null;
            }

            public void Reset()
            {
                current = null;
            }

            public bool MoveNext()
            {
                if (traverseQueue.Count > 0)
                    current = traverseQueue.Dequeue();
                else
                    current = null;

                return current != null;
            }

            private void visitNode(BSTNode<T> node)
            {
                if (node == null)
                    return;
                traverseQueue.Enqueue(node);
                visitNode(node.LeftChild);
                visitNode(node.RightChild);
            }
        }
        internal class BinarySearchTreeInOrderEnumerator : IEnumerator<T>
        {
            private BSTNode<T> current;
            internal Queue<BSTNode<T>> traverseQueue;
            private BinarySearchTree<T> tree;

            public BinarySearchTreeInOrderEnumerator(BinarySearchTree<T> tree)
            {
                this.tree = tree;

                //Build queue
                traverseQueue = new Queue<BSTNode<T>>();
                visitNode(this.tree.Root);
            }

            public T Current
            {
                get { return current.Value; }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public void Dispose()
            {
                current = null;
                tree = null;
            }

            public void Reset()
            {
                current = null;
            }

            public bool MoveNext()
            {
                if (traverseQueue.Count > 0)
                    current = traverseQueue.Dequeue();
                else
                    current = null;

                return current != null;
            }

            private void visitNode(BSTNode<T> node)
            {
                if (node == null)
                    return;
                visitNode(node.LeftChild);
                traverseQueue.Enqueue(node);
                visitNode(node.RightChild);
            }
        }

     
        internal class BinarySearchTreePostOrderEnumerator : IEnumerator<T>
        {
            private BSTNode<T> current;
            internal Queue<BSTNode<T>> traverseQueue;
            private BinarySearchTree<T> tree;

            public BinarySearchTreePostOrderEnumerator(BinarySearchTree<T> tree)
            {
                this.tree = tree;

                //Build queue
                traverseQueue = new Queue<BSTNode<T>>();
                visitNode(this.tree.Root);
            }

            public T Current
            {
                get { return current.Value; }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public void Dispose()
            {
                current = null;
                tree = null;
            }

            public void Reset()
            {
                current = null;
            }

            public bool MoveNext()
            {
                if (traverseQueue.Count > 0)
                    current = traverseQueue.Dequeue();
                else
                    current = null;

                return current != null;
            }

            private void visitNode(BSTNode<T> node)
            {
                if (node == null)
                    return;
                visitNode(node.LeftChild);
                visitNode(node.RightChild);
                traverseQueue.Enqueue(node);
            }
        }
    } //end-of-binary-search-tree

    public class BinarySearchTreeMap<TKey, TValue> : IBinarySearchTree<TKey, TValue> where TKey : IComparable<TKey>
    {
      
        public enum TraversalMode
        {
            InOrder = 0,
            PreOrder = 1,
            PostOrder = 2
        }


        public BinarySearchTreeMap()
        {
            _count = 0;
            _allowDuplicates = true;
            Root = null;
        }

        public BinarySearchTreeMap(bool allowDuplicates)
        {
            _count = 0;
            _allowDuplicates = allowDuplicates;
            Root = null;
        }


      
        protected int _count { get; set; }

        protected bool _allowDuplicates { get; set; }
        protected virtual BSTMapNode<TKey, TValue> _root { get; set; }

        public virtual BSTMapNode<TKey, TValue> Root
        {
            get { return _root; }
            internal set { _root = value; }
        }

        public virtual int Count
        {
            get { return _count; }
        }

        public virtual bool IsEmpty
        {
            get { return _count == 0; }
        }

      
       ///     Time-complexity: O(n), where n = number of nodes.
      public virtual int Height
        {
            get
            {
                if (IsEmpty)
                    return 0;

                var currentNode = Root;
                return _getTreeHeight(currentNode);
            }
        }

        public virtual bool AllowsDuplicates
        {
            get { return _allowDuplicates; }
        }

        public virtual void Insert(TKey key, TValue value)
        {
            var newNode = new BSTMapNode<TKey, TValue>(key, value);

            // Insert node recursively starting from the root. check for success status.
            var success = _insertNode(newNode);

            if (success == false && _allowDuplicates == false)
                throw new InvalidOperationException("Tree does not allow inserting duplicate elements.");
        }

        public virtual void Insert(KeyValuePair<TKey, TValue> keyValuePair)
        {
            Insert(keyValuePair.Key, keyValuePair.Value);
        }

      
        public virtual void Insert(KeyValuePair<TKey, TValue>[] collection)
        {
            if (collection == null)
                throw new ArgumentNullException();

            if (collection.Length > 0)
            {
                for (var i = 0; i < collection.Length; ++i)
                {
                    Insert(collection[i].Key, collection[i].Value);
                }
            }
        }

      
        public virtual void Insert(List<KeyValuePair<TKey, TValue>> collection)
        {
            if (collection == null)
                throw new ArgumentNullException();

            if (collection.Count > 0)
            {
                for (var i = 0; i < collection.Count; ++i)
                {
                    Insert(collection[i].Key, collection[i].Value);
                }
            }
        }

      
        public virtual void Remove(TKey key)
        {
            if (IsEmpty)
                throw new Exception("Tree is empty.");

            var node = _findNode(Root, key);

            if (node == null)
                throw new KeyNotFoundException("Key doesn't exist in tree.");

            _remove(node);
        }

      
        public virtual void RemoveMin()
        {
            if (IsEmpty)
                throw new Exception("Tree is empty.");

            var node = _findMinNode(Root);
            _remove(node);
        }

        public virtual void RemoveMax()
        {
            if (IsEmpty)
                throw new Exception("Tree is empty.");

            var node = _findMaxNode(Root);
            _remove(node);
        }

        
        public virtual void Clear()
        {
            Root = null;
            _count = 0;
        }

      
        public virtual bool Contains(TKey key)
        {
            return _findNode(_root, key) != null;
        }

        public virtual KeyValuePair<TKey, TValue> FindMin()
        {
            if (IsEmpty)
                throw new Exception("Tree is empty.");

            var minNode = _findMinNode(Root);
            return new KeyValuePair<TKey, TValue>(minNode.Key, minNode.Value);
        }

      
        public virtual KeyValuePair<TKey, TValue> FindMax()
        {
            if (IsEmpty)
                throw new Exception("Tree is empty.");

            var maxNode = _findMaxNode(Root);
            return new KeyValuePair<TKey, TValue>(maxNode.Key, maxNode.Value);
        }

        
        public virtual KeyValuePair<TKey, TValue> Find(TKey key)
        {
            if (IsEmpty)
                throw new Exception("Tree is empty.");

            var node = _findNode(Root, key);

            if (node != null)
                return new KeyValuePair<TKey, TValue>(node.Key, node.Value);
            throw new KeyNotFoundException("Item was not found.");
        }

        public virtual IEnumerable<KeyValuePair<TKey, TValue>> FindAll(Predicate<TKey> searchPredicate)
        {
            var list = new List<KeyValuePair<TKey, TValue>>();
            _findAll(Root, searchPredicate, ref list);
            return list;
        }

        public virtual KeyValuePair<TKey, TValue>[] ToArray()
        {
            return ToList().ToArray();
        }

    
        public virtual List<KeyValuePair<TKey, TValue>> ToList()
        {
            var list = new List<KeyValuePair<TKey, TValue>>();
            _inOrderTraverse(Root, ref list);
            return list;
        }


        protected virtual int _getTreeHeight(BSTMapNode<TKey, TValue> node)
        {
            if (node == null)
                return 0;
                // Is leaf node
            if (node.IsLeafNode)
                return 1;
                // Has two children
            if (node.ChildrenCount == 2)
                return 1 + Math.Max(_getTreeHeight(node.LeftChild), _getTreeHeight(node.RightChild));
                // Has only left
            if (node.HasLeftChild)
                return 1 + _getTreeHeight(node.LeftChild);
                // Has only right
            return 1 + _getTreeHeight(node.RightChild);
        }

        /// <summary>
        ///     Inserts a new node to the tree.
        /// </summary>
        protected virtual bool _insertNode(BSTMapNode<TKey, TValue> newNode)
        {
            // Handle empty trees
            if (Root == null)
            {
                Root = newNode;
                _count++;
                return true;
            }
            if (newNode.Parent == null)
                newNode.Parent = Root;

            // Check for value equality and whether inserting duplicates is allowed
            if (_allowDuplicates == false && newNode.Parent.Key.IsEqualTo(newNode.Key))
            {
                return false;
            }

            // Go Left
            if (newNode.Parent.Key.IsGreaterThan(newNode.Key)) // newNode < parent
            {
                if (newNode.Parent.HasLeftChild == false)
                {
                    newNode.Parent.LeftChild = newNode;

                    // Increment count.
                    _count++;

                    return true;
                }
                newNode.Parent = newNode.Parent.LeftChild;
                return _insertNode(newNode);
            }
                // Go Right
            if (newNode.Parent.HasRightChild == false)
            {
                newNode.Parent.RightChild = newNode;

                // Increment count.
                _count++;

                return true;
            }
            newNode.Parent = newNode.Parent.RightChild;
            return _insertNode(newNode);
        }

        /// <summary>
        ///     Replaces the node's value from it's parent node object with the newValue.
        ///     Used in the recusive _remove function.
        /// </summary>
        protected virtual void _replaceNodeInParent(BSTMapNode<TKey, TValue> node,
            BSTMapNode<TKey, TValue> newNode = null)
        {
            if (node.Parent != null)
            {
                if (node.IsLeftChild)
                    node.Parent.LeftChild = newNode;
                else
                    node.Parent.RightChild = newNode;
            }

            if (newNode != null)
                newNode.Parent = node.Parent;
        }

        /// <summary>
        ///     Remove the specified node.
        /// </summary>
        protected virtual bool _remove(BSTMapNode<TKey, TValue> node)
        {
            if (node == null)
                return false;

            var parent = node.Parent;

            if (node.ChildrenCount == 2) // if both children are present
            {
                var successor = node.RightChild;
                node.Key = successor.Key;
                node.Value = successor.Value;
                return true && _remove(successor);
            }
            if (node.HasLeftChild) // if the node has only a LEFT child
            {
                _replaceNodeInParent(node, node.LeftChild);
                _count--;
            }
            else if (node.HasRightChild) // if the node has only a RIGHT child
            {
                _replaceNodeInParent(node, node.RightChild);
                _count--;
            }
            else //this node has no children
            {
                _replaceNodeInParent(node, null);
                _count--;
            }

            return true;
        }

        /// <summary>
        ///     Finds a node inside another node's subtrees, given it's value.
        /// </summary>
        protected virtual BSTMapNode<TKey, TValue> _findNode(BSTMapNode<TKey, TValue> currentNode, TKey key)
        {
            if (currentNode == null)
                return currentNode;

            if (key.IsEqualTo(currentNode.Key))
            {
                return currentNode;
            }
            if (currentNode.HasLeftChild && key.IsLessThan(currentNode.Key))
            {
                return _findNode(currentNode.LeftChild, key);
            }
            if (currentNode.HasRightChild && key.IsGreaterThan(currentNode.Key))
            {
                return _findNode(currentNode.RightChild, key);
            }

            // Return-functions-fix
            return null;
        }

        /// <summary>
        ///     Returns the min-node in a subtree.
        ///     Used in the recusive _remove function.
        /// </summary>
        protected virtual BSTMapNode<TKey, TValue> _findMinNode(BSTMapNode<TKey, TValue> node)
        {
            if (node == null)
                return node;

            var currentNode = node;

            while (currentNode.HasLeftChild)
                currentNode = currentNode.LeftChild;

            return currentNode;
        }

        /// <summary>
        ///     Returns the max-node in a subtree.
        ///     Used in the recusive _remove function.
        /// </summary>
        protected virtual BSTMapNode<TKey, TValue> _findMaxNode(BSTMapNode<TKey, TValue> node)
        {
            if (node == null)
                return node;

            var currentNode = node;
            while (currentNode.HasRightChild)
                currentNode = currentNode.RightChild;

            return currentNode;
        }

        /// <summary>
        ///     Finds the next smaller node in value compared to the specified node.
        /// </summary>
        protected virtual BSTMapNode<TKey, TValue> _findNextSmaller(BSTMapNode<TKey, TValue> node)
        {
            if (node == null)
                return node;

            if (node.HasLeftChild)
                return _findMaxNode(node.LeftChild);

            var currentNode = node;
            while (currentNode.Parent != null && currentNode.IsLeftChild)
                currentNode = currentNode.Parent;

            return currentNode.Parent;
        }

        /// <summary>
        ///     Finds the next larger node in value compared to the specified node.
        /// </summary>
        protected virtual BSTMapNode<TKey, TValue> _findNextLarger(BSTMapNode<TKey, TValue> node)
        {
            if (node == null)
                return node;

            if (node.HasRightChild)
                return _findMinNode(node.RightChild);

            var currentNode = node;
            while (currentNode.Parent != null && currentNode.IsRightChild)
                currentNode = currentNode.Parent;

            return currentNode.Parent;
        }

        /// <summary>
        ///     A recursive private method. Used in the public FindAll(predicate) functions.
        ///     Implements in-order traversal to find all the matching elements in a subtree.
        /// </summary>
        protected virtual void _findAll(BSTMapNode<TKey, TValue> currentNode, Predicate<TKey> match,
            ref List<KeyValuePair<TKey, TValue>> list)
        {
            if (currentNode == null)
                return;

            // call the left child
            _findAll(currentNode.LeftChild, match, ref list);

            if (match(currentNode.Key)) // match
                list.Add(new KeyValuePair<TKey, TValue>(currentNode.Key, currentNode.Value));

            // call the right child
            _findAll(currentNode.RightChild, match, ref list);
        }

        /// <summary>
        ///     In-order traversal of the subtrees of a node. Returns every node it vists.
        /// </summary>
        protected virtual void _inOrderTraverse(BSTMapNode<TKey, TValue> currentNode,
            ref List<KeyValuePair<TKey, TValue>> list)
        {
            if (currentNode == null)
                return;

            // call the left child
            _inOrderTraverse(currentNode.LeftChild, ref list);

            // visit node
            list.Add(new KeyValuePair<TKey, TValue>(currentNode.Key, currentNode.Value));

            // call the right child
            _inOrderTraverse(currentNode.RightChild, ref list);
        }

        /// <summary>
        ///     Inserts an array of elements to the tree.
        /// </summary>
        public virtual void Insert(TKey[] collection)
        {
            if (collection == null)
                throw new ArgumentNullException();

            if (collection.Length > 0)
            {
                for (var i = 0; i < collection.Length; ++i)
                {
                    Insert(collection[i], default(TValue));
                }
            }
        }

        /// <summary>
        ///     Inserts a list of elements to the tree.
        /// </summary>
        public virtual void Insert(List<TKey> collection)
        {
            if (collection == null)
                throw new ArgumentNullException();

            if (collection.Count > 0)
            {
                for (var i = 0; i < collection.Count; ++i)
                {
                    Insert(collection[i], default(TValue));
                }
            }
        }

        /// <summary>
        ///     Updates the node of a specific key with a new value.
        /// </summary>
        public virtual void Update(TKey key, TValue newValue)
        {
            if (IsEmpty)
                throw new Exception("Tree is empty.");

            var node = _findNode(Root, key);

            if (node == null)
                throw new KeyNotFoundException("Key doesn't exist in tree.");

            node.Value = newValue;
        }

        /// <summary>
        ///     Finds the next smaller element in tree, compared to the specified item.
        /// </summary>
        public virtual KeyValuePair<TKey, TValue> FindNextSmaller(TKey key)
        {
            var node = _findNode(Root, key);
            var nextSmaller = _findNextSmaller(node);

            if (nextSmaller == null)
                throw new Exception("Item was not found.");

            return new KeyValuePair<TKey, TValue>(nextSmaller.Key, nextSmaller.Value);
        }

        /// <summary>
        ///     Finds the next larger element in tree, compared to the specified item.
        /// </summary>
        public virtual KeyValuePair<TKey, TValue> FindNextLarger(TKey item)
        {
            var node = _findNode(Root, item);
            var nextLarger = _findNextLarger(node);

            if (nextLarger == null)
                throw new Exception("Item was not found.");

            return new KeyValuePair<TKey, TValue>(nextLarger.Key, nextLarger.Value);
        }


        /*********************************************************************/


        /// <summary>
        ///     Returns an enumerator that visits node in the order: parent, left child, right child
        /// </summary>
        public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetPreOrderEnumerator()
        {
            return new BinarySearchTreePreOrderEnumerator(this);
        }

        /// <summary>
        ///     Returns an enumerator that visits node in the order: left child, parent, right child
        /// </summary>
        public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetInOrderEnumerator()
        {
            return new BinarySearchTreeInOrderEnumerator(this);
        }

        /// <summary>
        ///     Returns an enumerator that visits node in the order: left child, right child, parent
        /// </summary>
        public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetPostOrderEnumerator()
        {
            return new BinarySearchTreePostOrderEnumerator(this);
        }


        /*********************************************************************/


        /// <summary>
        ///     Returns an preorder-traversal enumerator for the tree values
        /// </summary>
        internal class BinarySearchTreePreOrderEnumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private BSTMapNode<TKey, TValue> current;
            internal Queue<BSTMapNode<TKey, TValue>> traverseQueue;
            private BinarySearchTreeMap<TKey, TValue> tree;

            public BinarySearchTreePreOrderEnumerator(BinarySearchTreeMap<TKey, TValue> tree)
            {
                this.tree = tree;

                //Build queue
                traverseQueue = new Queue<BSTMapNode<TKey, TValue>>();
                visitNode(this.tree.Root);
            }

            public KeyValuePair<TKey, TValue> Current
            {
                get { return new KeyValuePair<TKey, TValue>(current.Key, current.Value); }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public void Dispose()
            {
                current = null;
                tree = null;
            }

            public void Reset()
            {
                current = null;
            }

            public bool MoveNext()
            {
                if (traverseQueue.Count > 0)
                    current = traverseQueue.Dequeue();
                else
                    current = null;

                return current != null;
            }

            private void visitNode(BSTMapNode<TKey, TValue> node)
            {
                if (node == null)
                    return;
                traverseQueue.Enqueue(node);
                visitNode(node.LeftChild);
                visitNode(node.RightChild);
            }
        }


        /// <summary>
        ///     Returns an inorder-traversal enumerator for the tree values
        /// </summary>
        internal class BinarySearchTreeInOrderEnumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private BSTMapNode<TKey, TValue> current;
            internal Queue<BSTMapNode<TKey, TValue>> traverseQueue;
            private BinarySearchTreeMap<TKey, TValue> tree;

            public BinarySearchTreeInOrderEnumerator(BinarySearchTreeMap<TKey, TValue> tree)
            {
                this.tree = tree;

                //Build queue
                traverseQueue = new Queue<BSTMapNode<TKey, TValue>>();
                visitNode(this.tree.Root);
            }

            public KeyValuePair<TKey, TValue> Current
            {
                get { return new KeyValuePair<TKey, TValue>(current.Key, current.Value); }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public void Dispose()
            {
                current = null;
                tree = null;
            }

            public void Reset()
            {
                current = null;
            }

            public bool MoveNext()
            {
                if (traverseQueue.Count > 0)
                    current = traverseQueue.Dequeue();
                else
                    current = null;

                return current != null;
            }

            private void visitNode(BSTMapNode<TKey, TValue> node)
            {
                if (node == null)
                    return;
                visitNode(node.LeftChild);
                traverseQueue.Enqueue(node);
                visitNode(node.RightChild);
            }
        }

        /// <summary>
        ///     Returns a postorder-traversal enumerator for the tree values
        /// </summary>
        internal class BinarySearchTreePostOrderEnumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private BSTMapNode<TKey, TValue> current;
            internal Queue<BSTMapNode<TKey, TValue>> traverseQueue;
            private BinarySearchTreeMap<TKey, TValue> tree;

            public BinarySearchTreePostOrderEnumerator(BinarySearchTreeMap<TKey, TValue> tree)
            {
                this.tree = tree;

                //Build queue
                traverseQueue = new Queue<BSTMapNode<TKey, TValue>>();
                visitNode(this.tree.Root);
            }

            public KeyValuePair<TKey, TValue> Current
            {
                get { return new KeyValuePair<TKey, TValue>(current.Key, current.Value); }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public void Dispose()
            {
                current = null;
                tree = null;
            }

            public void Reset()
            {
                current = null;
            }

            public bool MoveNext()
            {
                if (traverseQueue.Count > 0)
                    current = traverseQueue.Dequeue();
                else
                    current = null;

                return current != null;
            }

            private void visitNode(BSTMapNode<TKey, TValue> node)
            {
                if (node == null)
                    return;
                visitNode(node.LeftChild);
                visitNode(node.RightChild);
                traverseQueue.Enqueue(node);
            }
        }
    } //end-of-binary-search-tree

    public class BSTMapNode<TKey, TValue> : IComparable<BSTMapNode<TKey, TValue>> where TKey : IComparable<TKey>
    {
        public BSTMapNode()
        {
        }

        public BSTMapNode(TKey key) : this(key, default(TValue), 0, null, null, null)
        {
        }

        public BSTMapNode(TKey key, TValue value) : this(key, value, 0, null, null, null)
        {
        }

        public BSTMapNode(TKey key, TValue value, int subTreeSize, BSTMapNode<TKey, TValue> parent,
            BSTMapNode<TKey, TValue> left, BSTMapNode<TKey, TValue> right)
        {
            Key = key;
            Value = value;
            Parent = parent;
            LeftChild = left;
            RightChild = right;
        }

        public virtual TKey Key { get; set; }

        public virtual TValue Value { get; set; }

        public virtual BSTMapNode<TKey, TValue> Parent { get; set; }

        public virtual BSTMapNode<TKey, TValue> LeftChild { get; set; }

        public virtual BSTMapNode<TKey, TValue> RightChild { get; set; }

        /// <summary>
        ///     Checks whether this node has any children.
        /// </summary>
        public virtual bool HasChildren
        {
            get { return ChildrenCount > 0; }
        }

        /// <summary>
        ///     Checks whether this node has left child.
        /// </summary>
        public virtual bool HasLeftChild
        {
            get { return LeftChild != null; }
        }

        /// <summary>
        ///     Checks whether this node has right child.
        /// </summary>
        public virtual bool HasRightChild
        {
            get { return RightChild != null; }
        }

        /// <summary>
        ///     Checks whether this node is the left child of it's parent.
        /// </summary>
        public virtual bool IsLeftChild
        {
            get { return Parent != null && Parent.LeftChild == this; }
        }

        /// <summary>
        ///     Checks whether this node is the left child of it's parent.
        /// </summary>
        public virtual bool IsRightChild
        {
            get { return Parent != null && Parent.RightChild == this; }
        }

        /// <summary>
        ///     Checks whether this node is a leaf node.
        /// </summary>
        public virtual bool IsLeafNode
        {
            get { return ChildrenCount == 0; }
        }

        /// <summary>
        ///     Returns number of direct descendents: 0, 1, 2 (none, left or right, or both).
        /// </summary>
        /// <returns>Number (0,1,2)</returns>
        public virtual int ChildrenCount
        {
            get
            {
                var count = 0;

                if (HasLeftChild)
                    count++;

                if (HasRightChild)
                    count++;

                return count;
            }
        }

        /// <summary>
        ///     Compares to.
        /// </summary>
        public virtual int CompareTo(BSTMapNode<TKey, TValue> other)
        {
            if (other == null)
                return -1;

            return Key.CompareTo(other.Key);
        }
    } //end-of-bstnode

    public class BSTNode<T> : IComparable<BSTNode<T>> where T : IComparable<T>
    {
        public BSTNode() : this(default(T), 0, null, null, null)
        {
        }

        public BSTNode(T value) : this(value, 0, null, null, null)
        {
        }

        public BSTNode(T value, int subTreeSize, BSTNode<T> parent, BSTNode<T> left, BSTNode<T> right)
        {
            Value = value;
            Parent = parent;
            LeftChild = left;
            RightChild = right;
        }

        public virtual T Value { get; set; }

        public virtual BSTNode<T> Parent { get; set; }

        public virtual BSTNode<T> LeftChild { get; set; }

        public virtual BSTNode<T> RightChild { get; set; }

        /// <summary>
        ///     Checks whether this node has any children.
        /// </summary>
        public virtual bool HasChildren
        {
            get { return ChildrenCount > 0; }
        }

        /// <summary>
        ///     Checks whether this node has left child.
        /// </summary>
        public virtual bool HasLeftChild
        {
            get { return LeftChild != null; }
        }

        /// <summary>
        ///     Checks whether this node has right child.
        /// </summary>
        public virtual bool HasRightChild
        {
            get { return RightChild != null; }
        }

        /// <summary>
        ///     Checks whether this node is the left child of it's parent.
        /// </summary>
        public virtual bool IsLeftChild
        {
            get { return Parent != null && Parent.LeftChild == this; }
        }

        /// <summary>
        ///     Checks whether this node is the left child of it's parent.
        /// </summary>
        public virtual bool IsRightChild
        {
            get { return Parent != null && Parent.RightChild == this; }
        }

        /// <summary>
        ///     Checks whether this node is a leaf node.
        /// </summary>
        public virtual bool IsLeafNode
        {
            get { return ChildrenCount == 0; }
        }

        /// <summary>
        ///     Returns number of direct descendents: 0, 1, 2 (none, left or right, or both).
        /// </summary>
        /// <returns>Number (0,1,2)</returns>
        public virtual int ChildrenCount
        {
            get
            {
                var count = 0;

                if (HasLeftChild)
                    count++;
                if (HasRightChild)
                    count++;

                return count;
            }
        }

        /// <summary>
        ///     Compares to.
        /// </summary>
        public virtual int CompareTo(BSTNode<T> other)
        {
            if (other == null)
                return -1;

            return Value.CompareTo(other.Value);
        }
    } //end-of-bstnode

    public interface IBinarySearchTree<T> where T : IComparable<T>
    {
        // Returns a copy of the tree root
        BSTNode<T> Root { get; }

        // Returns the number of elements in the Tree
        int Count { get; }

        // Checks if the tree is empty.
        bool IsEmpty { get; }

        // Returns the height of the tree.
        int Height { get; }

        // Returns true if tree allows inserting duplicates; otherwise, false
        bool AllowsDuplicates { get; }

        // Inserts an element to the tree
        void Insert(T item);

        // Inserts an array of elements to the tree.
        void Insert(T[] collection);

        // Inserts a list of items to the tree.
        void Insert(List<T> collection);

        // Removes the min value from tree
        void RemoveMin();

        // Removes the max value from tree
        void RemoveMax();

        // Remove an element from tree
        void Remove(T item);

        // Check for the existence of an item
        bool Contains(T item);

        // Finds the minimum element.
        T FindMin();

        // Finds the maximum element.
        T FindMax();

        // Find the element in the tree, returns null if not found.
        T Find(T item);

        // Finds all the elements in the tree that match the predicate.
        IEnumerable<T> FindAll(Predicate<T> searchPredicate);

        // Return an array of the tree elements
        T[] ToArray();

        // Return an array of the tree elements
        List<T> ToList();

        // Returns an enumerator that visits node in the order: parent, left child, right child
        IEnumerator<T> GetPreOrderEnumerator();

        // Returns an enumerator that visits node in the order: left child, parent, right child
        IEnumerator<T> GetInOrderEnumerator();

        // Returns an enumerator that visits node in the order: left child, right child, parent
        IEnumerator<T> GetPostOrderEnumerator();

        // Clear this tree.
        void Clear();
    }

   public interface IBinarySearchTree<TKey, TValue> where TKey : IComparable<TKey>
    {
        BSTMapNode<TKey, TValue> Root { get; }
        int Count { get; }
        bool IsEmpty { get; }
        int Height { get; }
        bool AllowsDuplicates { get; }
        void Insert(TKey key, TValue value);
        void Insert(KeyValuePair<TKey, TValue> keyValuePair);
        void Insert(KeyValuePair<TKey, TValue>[] collection);
        void Insert(List<KeyValuePair<TKey, TValue>> collection);
        void RemoveMin();
        void RemoveMax();
        void Remove(TKey item);
        bool Contains(TKey item);
        KeyValuePair<TKey, TValue> FindMin();
        KeyValuePair<TKey, TValue> FindMax();
        KeyValuePair<TKey, TValue> Find(TKey item);
        IEnumerable<KeyValuePair<TKey, TValue>> FindAll(Predicate<TKey> searchPredicate);
        KeyValuePair<TKey, TValue>[] ToArray();
        List<KeyValuePair<TKey, TValue>> ToList();
        void Clear();
    }
}