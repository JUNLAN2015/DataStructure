using DataStructures.Graphs;
using NUnit.Framework;

namespace Algorithms.Graphs.Tests
{
    [TestFixture]
    public class BipartiteColoringTests
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
    }
}