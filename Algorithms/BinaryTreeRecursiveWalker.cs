using System;
using System.Collections.Generic;
using Algorithms.Common;
using DataStructures.Trees;

namespace Algorithms.Trees
{
    public static class BinaryTreeIterativeWalker
    {
        /// <summary>
        ///     Specifies the mode of travelling through the tree.
        /// </summary>
        public enum TraversalMode
        {
            InOrder = 0,
            PreOrder = 1,
            PostOrder = 2
        }


        /************************************************************************************
         * PRIVATE HELPERS SECTION 
         * 
         */
    }

    /// <summary>
    ///     Simple Recursive Tree Traversal and Search Algorithms.
    /// </summary>
    public static class BinaryTreeRecursiveWalker
    {
        /// <summary>
        ///     Specifies the mode of travelling through the tree.
        /// </summary>
        public enum TraversalMode
        {
            InOrder = 0,
            PreOrder = 1,
            PostOrder = 2
        }


        /************************************************************************************
         * PRIVATE HELPERS SECTION 
         * 
         */

        /// <summary>
        ///     Private helper method for Preorder Traversal.
        /// </summary>
        private static void PreOrderVisitor<T>(BSTNode<T> BinaryTreeRoot, Action<T> Action) where T : IComparable<T>
        {
            if (BinaryTreeRoot == null)
                return;

            Action(BinaryTreeRoot.Value);
            PreOrderVisitor(BinaryTreeRoot.LeftChild, Action);
            PreOrderVisitor(BinaryTreeRoot.RightChild, Action);
        }

        /// <summary>
        ///     Private helper method for Inorder Traversal.
        /// </summary>
        private static void InOrderVisitor<T>(BSTNode<T> BinaryTreeRoot, Action<T> Action) where T : IComparable<T>
        {
            if (BinaryTreeRoot == null)
                return;

            InOrderVisitor(BinaryTreeRoot.LeftChild, Action);
            Action(BinaryTreeRoot.Value);
            InOrderVisitor(BinaryTreeRoot.RightChild, Action);
        }

        /// <summary>
        ///     Private helper method for Preorder Traversal.
        /// </summary>
        private static void PostOrderVisitor<T>(BSTNode<T> BinaryTreeRoot, Action<T> Action) where T : IComparable<T>
        {
            if (BinaryTreeRoot == null)
                return;

            PostOrderVisitor(BinaryTreeRoot.LeftChild, Action);
            PostOrderVisitor(BinaryTreeRoot.RightChild, Action);
            Action(BinaryTreeRoot.Value);
        }

        /// <summary>
        ///     Private helper method for Preorder Searcher.
        /// </summary>
        private static bool PreOrderSearcher<T>(BSTNode<T> BinaryTreeRoot, T Value, bool IsBinarySearchTree = false)
            where T : IComparable<T>
        {
            var current = BinaryTreeRoot.Value;
            var hasLeft = BinaryTreeRoot.HasLeftChild;
            var hasRight = BinaryTreeRoot.HasRightChild;

            if (current.IsEqualTo(Value))
                return true;

            if (IsBinarySearchTree)
            {
                if (BinaryTreeRoot.HasLeftChild && current.IsGreaterThan(Value))
                    return PreOrderSearcher(BinaryTreeRoot.LeftChild, Value);

                if (BinaryTreeRoot.HasRightChild && current.IsLessThan(Value))
                    return PreOrderSearcher(BinaryTreeRoot.RightChild, Value);
            }
            else
            {
                if (hasLeft && PreOrderSearcher(BinaryTreeRoot.LeftChild, Value))
                    return true;

                if (hasRight && PreOrderSearcher(BinaryTreeRoot.RightChild, Value))
                    return true;
            }

            return false;
        }

        /// <summary>
        ///     Private helper method for Inorder Searcher.
        /// </summary>
        private static bool InOrderSearcher<T>(BSTNode<T> BinaryTreeRoot, T Value, bool IsBinarySearchTree = false)
            where T : IComparable<T>
        {
            var current = BinaryTreeRoot.Value;
            var hasLeft = BinaryTreeRoot.HasLeftChild;
            var hasRight = BinaryTreeRoot.HasRightChild;

            if (IsBinarySearchTree)
            {
                if (hasLeft && current.IsGreaterThan(Value))
                    return InOrderSearcher(BinaryTreeRoot.LeftChild, Value);

                if (current.IsEqualTo(Value))
                    return true;

                if (hasRight && current.IsLessThan(Value))
                    return InOrderSearcher(BinaryTreeRoot.RightChild, Value);
            }
            else
            {
                if (hasLeft && InOrderSearcher(BinaryTreeRoot.LeftChild, Value))
                    return true;

                if (current.IsEqualTo(Value))
                    return true;

                if (hasRight && InOrderSearcher(BinaryTreeRoot.RightChild, Value))
                    return true;
            }

            return false;
        }

        /// <summary>
        ///     Private helper method for Inorder Searcher.
        /// </summary>
        private static bool PostOrderSearcher<T>(BSTNode<T> BinaryTreeRoot, T Value, bool IsBinarySearchTree = false)
            where T : IComparable<T>
        {
            var current = BinaryTreeRoot.Value;
            var hasLeft = BinaryTreeRoot.HasLeftChild;
            var hasRight = BinaryTreeRoot.HasRightChild;

            if (IsBinarySearchTree)
            {
                if (hasLeft && current.IsGreaterThan(Value))
                    return PostOrderSearcher(BinaryTreeRoot.LeftChild, Value);

                if (hasRight && current.IsLessThan(Value))
                    return PostOrderSearcher(BinaryTreeRoot.RightChild, Value);

                if (current.IsEqualTo(Value))
                    return true;
            }
            else
            {
                if (hasLeft && PostOrderSearcher(BinaryTreeRoot.LeftChild, Value))
                    return true;

                if (hasRight && PostOrderSearcher(BinaryTreeRoot.RightChild, Value))
                    return true;

                if (current.IsEqualTo(Value))
                    return true;
            }

            return false;
        }


        /************************************************************************************
         * PUBLIC API SECTION 
         * 
         */

        /// <summary>
        ///     Recusrsivley walks the tree and prints the values of all nodes.
        ///     By default this method traverses the tree in inorder fashion.
        /// </summary>
        public static void PrintAll<T>(BSTNode<T> BinaryTreeRoot, TraversalMode Mode = TraversalMode.InOrder)
            where T : IComparable<T>
        {
            if (BinaryTreeRoot == null)
                throw new ArgumentNullException("Tree root cannot be null.");

            var printAction = new Action<T>((T nodeValue) =>
                Console.Write(string.Format("{0} ", nodeValue)));

            ForEach(BinaryTreeRoot, printAction, Mode);
            Console.WriteLine();
        }

        /// <summary>
        ///     Recursively Visits All nodes in tree applying a given action to all nodes.
        ///     By default this method traverses the tree in inorder fashion.
        /// </summary>
        public static void ForEach<T>(BSTNode<T> BinaryTreeRoot, Action<T> Action,
            TraversalMode Mode = TraversalMode.InOrder) where T : IComparable<T>
        {
            if (BinaryTreeRoot == null)
                throw new ArgumentNullException("Tree root cannot be null.");

            if (Action == null)
                throw new ArgumentNullException("Action<T> Action cannot be null.");

            // Traverse
            switch (Mode)
            {
                case TraversalMode.PreOrder:
                    PreOrderVisitor(BinaryTreeRoot, Action);
                    return;
                case TraversalMode.InOrder:
                    InOrderVisitor(BinaryTreeRoot, Action);
                    return;
                case TraversalMode.PostOrder:
                    PostOrderVisitor(BinaryTreeRoot, Action);
                    return;
                default:
                    InOrderVisitor(BinaryTreeRoot, Action);
                    return;
            }
        }

        /// <summary>
        ///     Search the tree for the specified value.
        ///     By default this method traverses the tree in inorder fashion.
        /// </summary>
        public static bool Contains<T>(BSTNode<T> BinaryTreeRoot, T Value, TraversalMode Mode = TraversalMode.InOrder)
            where T : IComparable<T>
        {
            if (BinaryTreeRoot == null)
                throw new ArgumentNullException("Tree root cannot be null.");

            // Traverse
            // Traverse
            switch (Mode)
            {
                case TraversalMode.PreOrder:
                    return PreOrderSearcher(BinaryTreeRoot, Value);
                case TraversalMode.InOrder:
                    return InOrderSearcher(BinaryTreeRoot, Value);
                case TraversalMode.PostOrder:
                    return PostOrderSearcher(BinaryTreeRoot, Value);
                default:
                    return InOrderSearcher(BinaryTreeRoot, Value);
            }
        }

        /// <summary>
        ///     Search the tree for the specified value.
        ///     By default this method traverses the tree in inorder fashion.
        /// </summary>
        public static bool BinarySearch<T>(BSTNode<T> BinaryTreeRoot, T Value,
            TraversalMode Mode = TraversalMode.InOrder) where T : IComparable<T>
        {
            if (BinaryTreeRoot == null)
                throw new ArgumentNullException("Tree root cannot be null.");

            // Traverse
            // Traverse
            switch (Mode)
            {
                case TraversalMode.PreOrder:
                    return PreOrderSearcher(BinaryTreeRoot, Value, true);
                case TraversalMode.InOrder:
                    return InOrderSearcher(BinaryTreeRoot, Value, true);
                case TraversalMode.PostOrder:
                    return PostOrderSearcher(BinaryTreeRoot, Value, true);
                default:
                    return InOrderSearcher(BinaryTreeRoot, Value, true);
            }
        }

        /// <summary>
        ///     Search the tree for all matches for a given predicate function.
        ///     By default this method traverses the tree in inorder fashion.
        /// </summary>
        public static List<T> FindAllMatches<T>(BSTNode<T> BinaryTreeRoot, Predicate<T> Match,
            TraversalMode Mode = TraversalMode.InOrder) where T : IComparable<T>
        {
            throw new NotImplementedException();
        }
    }
}