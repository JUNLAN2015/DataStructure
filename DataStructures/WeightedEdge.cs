using System;
using DataStructures.Common;

namespace DataStructures.Graphs
{
    /// <summary>
    ///     The graph weighted edge class.
    /// </summary>
    public class WeightedEdge<TVertex> : IEdge<TVertex> where TVertex : IComparable<TVertex>
    {
        /// <summary>
        ///     CONSTRUCTOR
        /// </summary>
        public WeightedEdge(TVertex src, TVertex dst, long weight)
        {
            Source = src;
            Destination = dst;
            Weight = weight;
        }

        /// <summary>
        ///     Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        public TVertex Source { get; set; }

        /// <summary>
        ///     Gets or sets the destination.
        /// </summary>
        /// <value>The destination.</value>
        public TVertex Destination { get; set; }

        /// <summary>
        ///     Gets or sets the weight of edge.
        /// </summary>
        /// <value>The weight.</value>
        public long Weight { get; set; }

        /// <summary>
        ///     Gets a value indicating whether this edge is weighted.
        /// </summary>
        public bool IsWeighted
        {
            get { return false; }
        }

        #region IComparable implementation

        public int CompareTo(IEdge<TVertex> other)
        {
            if (other == null)
                return -1;

            var areNodesEqual = Source.IsEqualTo(other.Source) && Destination.IsEqualTo(other.Destination);

            if (!areNodesEqual)
                return -1;
            return Weight.CompareTo(other.Weight);
        }

        #endregion
    }
}