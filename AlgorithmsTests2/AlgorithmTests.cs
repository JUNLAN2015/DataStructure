using System.Collections.Generic;
using Algorithms.Sorting;
using DataStructures.Graphs;
using NUnit.Framework;

namespace Algorithms.Graphs.Tests
{
    [TestFixture]
    public class AlgorithmTests
    {
        [Test]
        private static void _initializeFirstCaseGraph(ref IGraph<string> graph)
        {
            // Clear the graph
            graph.Clear();

            //
            // Add vertices
            var verticesSet = new[] {"a", "b", "c"};
            graph.AddVertices(verticesSet);

            // 
            // Add Edges
            graph.AddEdge("a", "b");
            graph.AddEdge("b", "c");
            graph.AddEdge("c", "a");
        }

        [Test]
        public static void BinarySearchTreeSorterTest()
        {
            var numbers = new List<int> {23, 42, 4, 16, 8, 15, 3, 9, 55, 0, 34, 12, 2, 46, 25};
            numbers.UnbalancedBSTSort();
            Assert.AreEqual(numbers[0], 0);
        }


        [Test]
        public void OddCycleTest()
        {
            // The graph
            IGraph<string> graph = new UndirectedSparseGraph<string>();

            // The bipartite wrapper
            BipartiteColoring<UndirectedSparseGraph<string>, string> bipartiteGraph;

            // The status for checking bipartiteness
            bool initBipartiteStatus;


            //
            // Prepare the graph for the first case of testing
            _initializeFirstCaseGraph(ref graph);

            //
            // Test initializing the bipartite
            // This initialization must fail. The graph contains an odd cycle
            initBipartiteStatus = false;
            bipartiteGraph = null;

            try
            {
                bipartiteGraph = new BipartiteColoring<UndirectedSparseGraph<string>, string>(graph);
                initBipartiteStatus = bipartiteGraph.IsBipartite();
            }
            catch
            {
                initBipartiteStatus = false;
            }

            Assert.AreEqual(initBipartiteStatus, false);
        }

        [Test]
        public void Return_Min_Value_Via_Merge_Sort()
        {
            var numbersList = new List<int> {23, 42, 4, 16, 8, 15, 3, 9, 55, 0, 34, 12, 2, 46, 25};
            var sortedList = numbersList.MergeSort();
            Assert.AreEqual(sortedList[0], 0);
        }

        [Test]
        public void Return_Min_Value_Via_Quick_Sort()
        {
            var list = new List<long> {23, 42, 4, 16, 8, 15, 3, 9, 55, 0, 34, 12, 2, 46, 25};
            list.QuickSort();
            Assert.AreEqual(list[0], 0);
        }

        [Test]
        public static void Should_Greater_Ascending()
        {
            int[] numbersList1 = {23, 42, 4, 16, 8, 15, 3, 9, 55, 0, 34, 12, 2, 46, 25};
            var numbersList2 = new List<long> {23, 42, 4, 16, 8, 15, 3, 9, 55, 0, 34, 12, 2, 46, 25};

            numbersList1.HeapSort();
            // Sort Ascending (same as the method above);
            numbersList2.HeapSortAscending();
            Assert.Greater(numbersList2[numbersList2.Count - 1], numbersList2[0]);
        }

        [Test]
        public static void Should_Less_Descending()
        {
            int[] numbersList1 = {23, 42, 4, 16, 8, 15, 3, 9, 55, 0, 34, 12, 2, 46, 25};
            var numbersList2 = new List<long> {23, 42, 4, 16, 8, 15, 3, 9, 55, 0, 34, 12, 2, 46, 25};

            numbersList1.HeapSort();
            numbersList2.HeapSortDescending();

            Assert.Greater(numbersList2[0], numbersList2[numbersList2.Count - 1]);
        }
    }
}