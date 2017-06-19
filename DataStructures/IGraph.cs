using System;
using System.Collections.Generic;
using DataStructures.Lists;

namespace DataStructures.Graphs
{
    public interface IEdge<TVertex> : IComparable<IEdge<TVertex>> where TVertex : IComparable<TVertex>
    {
        bool IsWeighted { get; }

        TVertex Source { get; set; }


        TVertex Destination { get; set; }
        long Weight { get; set; }
    }
    public interface IGraph<T> where T : IComparable<T>
    {
        bool IsDirected { get; }
        bool IsWeighted { get; }
        int VerticesCount { get; }

        int EdgesCount { get; }

        IEnumerable<T> Vertices { get; }

        IEnumerable<IEdge<T>> Edges { get; }

        IEnumerable<IEdge<T>> IncomingEdges(T vertex);
        IEnumerable<IEdge<T>> OutgoingEdges(T vertex);

        bool AddEdge(T firstVertex, T secondVertex);

        bool RemoveEdge(T firstVertex, T secondVertex);

        void AddVertices(IList<T> collection);

        bool AddVertex(T vertex);

        bool RemoveVertex(T vertex);

        bool HasEdge(T firstVertex, T secondVertex);

        bool HasVertex(T vertex);

        DLinkedList<T> Neighbours(T vertex);

        int Degree(T vertex);

        string ToReadable();

        IEnumerable<T> DepthFirstWalk();

        IEnumerable<T> DepthFirstWalk(T startingVertex);

        IEnumerable<T> BreadthFirstWalk();


        IEnumerable<T> BreadthFirstWalk(T startingVertex);

        void Clear();
    }
    public interface IWeightedGraph<T> where T : IComparable<T>
    {
        /// <summary>
        /// Connects two vertices together with a weight, in the direction: first->second.
        /// </summary>
        bool AddEdge(T source, T destination, long weight);

        /// <summary>
        /// Updates the edge weight from source to destination.
        /// </summary>
        bool UpdateEdgeWeight(T source, T destination, long weight);

        /// <summary>
        /// Get edge object from source to destination.
        /// </summary>
        WeightedEdge<T> GetEdge(T source, T destination);

        /// <summary>
        /// Returns the edge weight from source to destination.
        /// </summary>
        long GetEdgeWeight(T source, T destination);

        /// <summary>
        /// Returns the neighbours of a vertex as a dictionary of nodes-to-weights.
        /// </summary>
        System.Collections.Generic.Dictionary<T, long> NeighboursMap(T vertex);
    }
}
