using System;
using System.Collections.Generic;
using System.Diagnostics;
using Algorithms.Common;
using DataStructures.Graphs;
using DataStructures.Heaps;

namespace Algorithms.Graphs
{
    /// <summary>
    ///     The Bipartite Colors type.
    /// </summary>
    public enum BipartiteColor
    {
        Red = 0,
        Blue = 1
    }


    /// <summary>
    ///     Bipartite Graph Coloring/Labeling.
    /// </summary>
    public class BipartiteColoring<TGraph, TVertex>
        where TGraph : IGraph<TVertex>
        where TVertex : IComparable<TVertex>
    {
        /// <summary>
        ///     CONTRUSTOR
        /// </summary>
        public BipartiteColoring(IGraph<TVertex> Graph)
        {
            // Validate Graph parameter
            if (Graph == null)
                throw new ArgumentNullException();
            if (Graph.VerticesCount < 2)
                throw new ApplicationException("Graph contains less elements than required.");

            // Init data members
            _initializeDataMembers(Graph);

            // Set bipartite flag to true
            _isBipartite = true;

            // Compute bipartiteness
            foreach (var vertex in Graph.Vertices)
            {
                var vertexIndex = _nodesToIndices[vertex];

                // Check the bipartite from this vertex, if it was not visited
                if (!_visited[vertexIndex])
                {
                    _isBipartite = _isBipartiteHelper(Graph, vertex);

                    // Stop discovery of graph when bipartiteness doesn't hold 
                    if (!_isBipartite)
                        throw new ApplicationException("Graph contains an odd cycle.");
                }
            }
        }

        private bool _isBipartite { get; set; }
        private int _edgesCount { get; set; }
        private int _verticesCount { get; set; }
        private bool[] _visited { get; set; }
        private BipartiteColor[] _nodesColors { get; set; }
        private Stack<TVertex> _cycle { get; set; }

        // A dictionary that maps node-values to integer indeces
        private Dictionary<TVertex, int> _nodesToIndices { get; set; }

        // A dictionary that maps integer index to node-value
        private Dictionary<int, TVertex> _indicesToNodes { get; set; }


        /// <summary>
        ///     Constructors helper function. Initializes some of the data memebers.
        /// </summary>
        private void _initializeDataMembers(IGraph<TVertex> Graph)
        {
            _isBipartite = false;
            _cycle = null;

            _edgesCount = Graph.EdgesCount;
            _verticesCount = Graph.VerticesCount;

            _visited = new bool[_verticesCount];
            _nodesColors = new BipartiteColor[_verticesCount];

            _nodesToIndices = new Dictionary<TVertex, int>();
            _indicesToNodes = new Dictionary<int, TVertex>();

            // Reset the visited, distances and predeccessors arrays
            var i = 0;
            foreach (var node in Graph.Vertices)
            {
                if (i >= _verticesCount)
                    break;

                _visited[i] = false;
                _nodesColors[i] = BipartiteColor.Red;

                _nodesToIndices.Add(node, i);
                _indicesToNodes.Add(i, node);

                ++i;
            }
        }

        /// <summary>
        ///     Constructors helper function. Computes the bipartite of graph from a source vertex.
        /// </summary>
        private bool _isBipartiteHelper(IGraph<TVertex> Graph, TVertex Source)
        {
            var queue = new Queue<TVertex>();
            queue.Enqueue(Source);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var currIndex = _nodesToIndices[current];

                // Visit node
                if (!_visited[currIndex])
                {
                    _visited[currIndex] = true;
                    _nodesColors[currIndex] = BipartiteColor.Red;
                }

                // Discover bfs-level neighbors
                foreach (var adjacent in Graph.Neighbours(current))
                {
                    var adjIndex = _nodesToIndices[adjacent];

                    if (!_visited[adjIndex])
                    {
                        _visited[adjIndex] = true;
                        _nodesColors[adjIndex] = _nodesColors[currIndex] == BipartiteColor.Red
                            ? BipartiteColor.Blue
                            : BipartiteColor.Red;

                        queue.Enqueue(adjacent);
                    }
                    else if (_nodesColors[currIndex] == _nodesColors[adjIndex])
                    {
                        return false;
                    }
                } //end-foreach
            } //end-while

            return true;
        }


        /// <summary>
        ///     Determines the graph is bipartite.
        /// </summary>
        public bool IsBipartite()
        {
            return _isBipartite;
        }

        /// <summary>
        ///     Returns the color of a vertex.
        /// </summary>
        public BipartiteColor ColorOf(TVertex vertex)
        {
            if (!_isBipartite)
                throw new InvalidOperationException("Graph is not bipartite.");
            if (!_nodesToIndices.ContainsKey(vertex))
                throw new ApplicationException("Vertex doesn't belong to graph.");

            return _nodesColors[_nodesToIndices[vertex]];
        }

        /// <summary>
        ///     Returns the odd-cycle in graoh, if any.
        /// </summary>
        /// <returns>The cycle.</returns>
        public IEnumerable<TVertex> OddCycle()
        {
            throw new NotImplementedException();
        }
    }

    public class BellmanFordShortestPaths<TGraph, TVertex>
        where TGraph : IGraph<TVertex>, IWeightedGraph<TVertex>
        where TVertex : IComparable<TVertex>
    {
        // A const that represent an infinite distance
        private const long Infinity = long.MaxValue;
        private const int NilPredecessor = -1;
        private long[] _distances;

        /// <summary>
        ///     INSTANCE VARIABLES
        /// </summary>
        private int _edgesCount;

        private WeightedEdge<TVertex>[] _edgeTo;

        // A dictionary that maps integer index to node-value
        private Dictionary<int, TVertex> _indicesToNodes;

        // A dictionary that maps node-values to integer indeces
        private Dictionary<TVertex, int> _nodesToIndices;
        private int[] _predecessors;
        private int _verticesCount;


        /// <summary>
        ///     CONSTRUCTOR
        /// </summary>
        public BellmanFordShortestPaths(TGraph Graph, TVertex Source)
        {
            if (Graph == null)
                throw new ArgumentNullException();
            if (!Graph.HasVertex(Source))
                throw new ArgumentException("The source vertex doesn't belong to graph.");

            // Init
            _initializeDataMembers(Graph);

            // Traverse the graph
            var status = _bellmanFord(Graph, Source);

            if (status == false)
                throw new Exception("Negative-weight cycle detected.");

            Debug.Assert(_checkOptimalityConditions(Graph, Source));
        }


        /************************************************************************************************************/


        /// <summary>
        ///     The Bellman-Ford Algorithm.
        /// </summary>
        /// <returns>True if shortest-path computation is finished with no negative-weight cycles detected; otehrwise, false.</returns>
        private bool _bellmanFord(TGraph graph, TVertex source)
        {
            var srcIndex = _nodesToIndices[source];
            _distances[srcIndex] = 0;

            var edges = graph.Edges as IEnumerable<WeightedEdge<TVertex>>;

            // First pass
            // Calculate shortest paths and relax all edges.
            for (var i = 1; i < graph.VerticesCount - 1; ++i)
            {
                foreach (var edge in edges)
                {
                    var fromIndex = _nodesToIndices[edge.Source];
                    var toIndex = _nodesToIndices[edge.Destination];

                    // calculate a new possible weighted path if the edge weight is less than infinity
                    var delta = Infinity;
                    if (edge.Weight < Infinity && Infinity - edge.Weight > _distances[fromIndex]) // Handles overflow
                        delta = _distances[fromIndex] + edge.Weight;

                    // Relax the edge
                    // if check is true, a shorter path is found from current to adjacent
                    if (delta < _distances[toIndex])
                    {
                        _edgeTo[toIndex] = edge;
                        _distances[toIndex] = delta;
                        _predecessors[toIndex] = fromIndex;
                    }
                }
            }

            // Second pass
            // Check for negative-weight cycles.
            foreach (var edge in edges)
            {
                var fromIndex = _nodesToIndices[edge.Source];
                var toIndex = _nodesToIndices[edge.Destination];

                // calculate a new possible weighted path if the edge weight is less than infinity
                var delta = Infinity;
                if (edge.Weight < Infinity && Infinity - edge.Weight > _distances[fromIndex]) // Handles overflow
                    delta = _distances[fromIndex] + edge.Weight;

                // if check is true a negative-weight cycle is detected
                // return false;
                if (delta < _distances[toIndex])
                    return false;
            }

            // Completed shortest paths computation.
            // No negative edges were detected.
            return true;
        }

        /// <summary>
        ///     Constructors helper function. Initializes some of the data memebers.
        /// </summary>
        private void _initializeDataMembers(TGraph Graph)
        {
            _edgesCount = Graph.EdgesCount;
            _verticesCount = Graph.VerticesCount;

            _distances = new long[_verticesCount];
            _predecessors = new int[_verticesCount];
            _edgeTo = new WeightedEdge<TVertex>[_edgesCount];

            _nodesToIndices = new Dictionary<TVertex, int>();
            _indicesToNodes = new Dictionary<int, TVertex>();

            // Reset the information arrays
            var i = 0;
            foreach (var node in Graph.Vertices)
            {
                if (i >= _verticesCount)
                    break;

                _edgeTo[i] = null;
                _distances[i] = Infinity;
                _predecessors[i] = NilPredecessor;

                _nodesToIndices.Add(node, i);
                _indicesToNodes.Add(i, node);

                ++i;
            }
        }

        /// <summary>
        ///     Constructors helper function. Checks Optimality Conditions:
        ///     (i) for all edges e:            distTo[e.to()] <= distTo[e.from()] + e.weight()
        ///     (ii) for all edge e on the SPT: distTo[e.to()] == distTo[e.from()] + e.weight()
        /// </summary>
        private bool _checkOptimalityConditions(TGraph graph, TVertex source)
        {
            // Get the source index (to be used with the information arrays).
            var s = _nodesToIndices[source];

            // check that distTo[v] and edgeTo[v] are consistent
            if (_distances[s] != 0 || _predecessors[s] != NilPredecessor)
            {
                Console.WriteLine("distanceTo[s] and edgeTo[s] are inconsistent");
                return false;
            }

            for (var v = 0; v < graph.VerticesCount; v++)
            {
                if (v == s) continue;

                if (_predecessors[v] == NilPredecessor && _distances[v] != Infinity)
                {
                    Console.WriteLine("distanceTo[] and edgeTo[] are inconsistent for at least one vertex.");
                    return false;
                }
            }

            // check that all edges e = v->w satisfy distTo[w] <= distTo[v] + e.weight()
            foreach (var vertex in graph.Vertices)
            {
                var v = _nodesToIndices[vertex];

                foreach (var edge in graph.NeighboursMap(vertex))
                {
                    var w = _nodesToIndices[edge.Key];

                    if (_distances[v] + edge.Value < _distances[w])
                    {
                        Console.WriteLine("edge " + vertex + "-" + edge.Key + " is not relaxed");
                        return false;
                    }
                }
            }

            // check that all edges e = v->w on SPT satisfy distTo[w] == distTo[v] + e.weight()
            foreach (var vertex in graph.Vertices)
            {
                var w = _nodesToIndices[vertex];

                if (_edgeTo[w] == null)
                    continue;

                var edge = _edgeTo[w];
                var v = _nodesToIndices[edge.Source];

                if (!vertex.IsEqualTo(edge.Destination))
                    return false;

                if (_distances[v] + edge.Weight != _distances[w])
                {
                    Console.WriteLine("edge " + edge.Source + "-" + edge.Destination + " on shortest path not tight");
                    return false;
                }
            }

            return true;
        }


        /************************************************************************************************************/


        /// <summary>
        ///     Determines whether there is a path from the source vertex to this specified vertex.
        /// </summary>
        public bool HasPathTo(TVertex destination)
        {
            if (!_nodesToIndices.ContainsKey(destination))
                throw new Exception("Graph doesn't have the specified vertex.");

            var index = _nodesToIndices[destination];
            return _distances[index] != Infinity;
        }

        /// <summary>
        ///     Returns the distance between the source vertex and the specified vertex.
        /// </summary>
        public long DistanceTo(TVertex destination)
        {
            if (!_nodesToIndices.ContainsKey(destination))
                throw new Exception("Graph doesn't have the specified vertex.");

            var index = _nodesToIndices[destination];
            return _distances[index];
        }

        /// <summary>
        ///     Returns an enumerable collection of nodes that specify the shortest path from the source vertex to the destination
        ///     vertex.
        /// </summary>
        public IEnumerable<TVertex> ShortestPathTo(TVertex destination)
        {
            if (!_nodesToIndices.ContainsKey(destination))
                throw new Exception("Graph doesn't have the specified vertex.");
            if (!HasPathTo(destination))
                return null;

            var dstIndex = _nodesToIndices[destination];
            var stack = new DataStructures.Lists.Stack<TVertex>();

            int index;
            for (index = dstIndex; _distances[index] != 0; index = _predecessors[index])
                stack.Push(_indicesToNodes[index]);

            // Push the source vertex
            stack.Push(_indicesToNodes[index]);

            return stack;
        }
    }

    public static class BreadthFirstSearcher
    {
        /// <summary>
        ///     Iterative BFS implementation.
        ///     Traverses nodes in graph starting from a specific node, printing them as they get visited.
        /// </summary>
        public static void PrintAll<T>(IGraph<T> Graph, T StartVertex) where T : IComparable<T>
        {
            // Check if graph is empty
            if (Graph.VerticesCount == 0)
                throw new Exception("Graph is empty!");

            // Check if graph has the starting vertex
            if (!Graph.HasVertex(StartVertex))
                throw new Exception("Starting vertex doesn't belong to graph.");

            var visited = new HashSet<T>();
            var queue = new Queue<T>(Graph.VerticesCount);

            queue.Enqueue(StartVertex);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                Console.Write("({0}) ", current);

                foreach (var adjacent in Graph.Neighbours(current))
                {
                    if (!visited.Contains(adjacent))
                    {
                        visited.Add(adjacent);
                        queue.Enqueue(adjacent);
                    }
                }
            }
        }

        /// <summary>
        ///     Iterative BFS implementation.
        ///     Traverses all the nodes in a graph starting from a specific node, applying the passed action to every node.
        /// </summary>
        public static void VisitAll<T>(ref IGraph<T> Graph, T StartVertex, Action<T> Action) where T : IComparable<T>
        {
            // Check if graph is empty
            if (Graph.VerticesCount == 0)
                throw new Exception("Graph is empty!");

            // Check if graph has the starting vertex
            if (!Graph.HasVertex(StartVertex))
                throw new Exception("Starting vertex doesn't belong to graph.");

            var level = 0; // keeps track of level
            var frontiers = new List<T>(); // keeps track of previous levels, i - 1
            var levels = new Dictionary<T, int>(Graph.VerticesCount);
                // keeps track of visited nodes and their distances
            var parents = new Dictionary<T, object>(Graph.VerticesCount); // keeps track of tree-nodes

            frontiers.Add(StartVertex);
            levels.Add(StartVertex, 0);
            parents.Add(StartVertex, null);

            // BFS VISIT CURRENT NODE
            Action(StartVertex);

            // TRAVERSE GRAPH
            while (frontiers.Count > 0)
            {
                var next = new List<T>(); // keeps track of the current level, i

                foreach (var node in frontiers)
                {
                    foreach (var adjacent in Graph.Neighbours(node))
                    {
                        if (!levels.ContainsKey(adjacent)) // not visited yet
                        {
                            // BFS VISIT NODE STEP
                            Action(adjacent);

                            levels.Add(adjacent, level); // level[node] + 1
                            parents.Add(adjacent, node);
                            next.Add(adjacent);
                        }
                    }
                }

                frontiers = next;
                level = level + 1;
            }
        }

        /// <summary>
        ///     Iterative BFS Implementation.
        ///     Given a predicate function and a starting node, this function searches the nodes of the graph for a first match.
        /// </summary>
        public static T FindFirstMatch<T>(IGraph<T> Graph, T StartVertex, Predicate<T> Match) where T : IComparable<T>
        {
            // Check if graph is empty
            if (Graph.VerticesCount == 0)
                throw new Exception("Graph is empty!");

            // Check if graph has the starting vertex
            if (!Graph.HasVertex(StartVertex))
                throw new Exception("Starting vertex doesn't belong to graph.");

            var level = 0; // keeps track of levels
            var frontiers = new List<T>(); // keeps track of previous levels, i - 1
            var levels = new Dictionary<T, int>(Graph.VerticesCount);
                // keeps track of visited nodes and their distances
            var parents = new Dictionary<T, object>(Graph.VerticesCount); // keeps track of tree-nodes

            frontiers.Add(StartVertex);
            levels.Add(StartVertex, 0);
            parents.Add(StartVertex, null);

            // BFS VISIT CURRENT NODE
            if (Match(StartVertex))
                return StartVertex;

            // TRAVERSE GRAPH
            while (frontiers.Count > 0)
            {
                var next = new List<T>(); // keeps track of the current level, i

                foreach (var node in frontiers)
                {
                    foreach (var adjacent in Graph.Neighbours(node))
                    {
                        if (!levels.ContainsKey(adjacent)) // not visited yet
                        {
                            // BFS VISIT NODE STEP
                            if (Match(adjacent))
                                return adjacent;

                            levels.Add(adjacent, level); // level[node] + 1
                            parents.Add(adjacent, node);
                            next.Add(adjacent);
                        }
                    }
                }

                frontiers = next;
                level = level + 1;
            }

            throw new Exception("Item was not found!");
        }
    }

    public class BreadthFirstShortestPaths<T> where T : IComparable<T>
    {
        // A const that represent an infinite distance
        private const long INFINITY = long.MaxValue;


        /// <summary>
        ///     CONSTRUCTOR.
        ///     Breadth First Searcher from Single Source.
        /// </summary>
        public BreadthFirstShortestPaths(IGraph<T> Graph, T Source)
        {
            if (Graph == null)
                throw new ArgumentNullException();
            if (!Graph.HasVertex(Source))
                throw new ArgumentException("The source vertex doesn't belong to graph.");

            // Init
            _initializeDataMembers(Graph);

            // Single source BFS
            _breadthFirstSearch(Graph, Source);

            //bool optimalityConditionsSatisfied = checkOptimalityConditions (Graph, Source);
            Debug.Assert(checkOptimalityConditions(Graph, Source));
        }


        /// <summary>
        ///     CONSTRUCTOR.
        ///     Breadth First Searcher from Multiple Sources.
        /// </summary>
        public BreadthFirstShortestPaths(IGraph<T> Graph, IList<T> Sources)
        {
            if (Graph == null)
                throw new ArgumentNullException();
            if (Sources == null || Sources.Count == 0)
                throw new ArgumentException("Sources list is either null or empty.");

            // Init
            _initializeDataMembers(Graph);

            // Multiple sources BFS
            _breadthFirstSearch(Graph, Sources);
        }

        private int _edgesCount { get; set; }
        private int _verticesCount { get; set; }
        private bool[] _visited { get; set; }
        private long[] _distances { get; set; }
        private int[] _predecessors { get; set; }

        // A dictionary that maps node-values to integer indeces
        private Dictionary<T, int> _nodesToIndices { get; set; }

        // A dictionary that maps integer index to node-value
        private Dictionary<int, T> _indicesToNodes { get; set; }


        /************************************************************************************************************/


        /// <summary>
        ///     Constructors helper function. Initializes some of the data memebers.
        /// </summary>
        private void _initializeDataMembers(IGraph<T> Graph)
        {
            _edgesCount = Graph.EdgesCount;
            _verticesCount = Graph.VerticesCount;

            _visited = new bool[_verticesCount];
            _distances = new long[_verticesCount];
            _predecessors = new int[_verticesCount];

            _nodesToIndices = new Dictionary<T, int>();
            _indicesToNodes = new Dictionary<int, T>();

            // Reset the visited, distances and predeccessors arrays
            var i = 0;
            foreach (var node in Graph.Vertices)
            {
                if (i >= _verticesCount)
                    break;

                _visited[i] = false;
                _distances[i] = INFINITY;
                _predecessors[i] = -1;

                _nodesToIndices.Add(node, i);
                _indicesToNodes.Add(i, node);

                ++i;
            }
        }

        /// <summary>
        ///     Privat helper. Breadth First Search from Single Source.
        /// </summary>
        private void _breadthFirstSearch(IGraph<T> graph, T source)
        {
            // Set distance to current to zero
            _distances[_nodesToIndices[source]] = 0;

            // Set current to visited: true.
            _visited[_nodesToIndices[source]] = true;

            var queue = new Queue<T>(_verticesCount);
            queue.Enqueue(source);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var indexOfCurrent = _nodesToIndices[current];

                foreach (var adjacent in graph.Neighbours(current))
                {
                    var indexOfAdjacent = _nodesToIndices[adjacent];

                    if (!_visited[indexOfAdjacent])
                    {
                        _predecessors[indexOfAdjacent] = indexOfCurrent;
                        _distances[indexOfAdjacent] = _distances[indexOfCurrent] + 1;
                        _visited[indexOfAdjacent] = true;

                        queue.Enqueue(adjacent);
                    }
                } //end-foreach
            } //end-while
        }

        /// <summary>
        ///     Privat helper. Breadth First Search from Multiple Sources.
        /// </summary>
        private void _breadthFirstSearch(IGraph<T> graph, IList<T> sources)
        {
            // Define helper variables.
            var queue = new Queue<T>(_verticesCount);

            foreach (var source in sources)
            {
                if (!graph.HasVertex(source))
                    throw new Exception("Graph doesn't has a vertex '" + source + "'");

                var index = _nodesToIndices[source];
                _distances[index] = 0;
                _visited[index] = true;
                queue.Enqueue(source);
            }

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var indexOfCurrent = _nodesToIndices[current];

                foreach (var adjacent in graph.Neighbours(current))
                {
                    var indexOfAdjacent = _nodesToIndices[adjacent];

                    if (!_visited[indexOfAdjacent])
                    {
                        _predecessors[indexOfAdjacent] = indexOfCurrent;
                        _distances[indexOfAdjacent] = _distances[indexOfCurrent] + 1;
                        _visited[indexOfAdjacent] = true;

                        queue.Enqueue(adjacent);
                    }
                } //end-foreach
            } //end-while
        }

        /// <summary>
        ///     Private helper. Checks optimality conditions for single source
        /// </summary>
        private bool checkOptimalityConditions(IGraph<T> graph, T source)
        {
            var indexOfSource = _nodesToIndices[source];

            // check that the distance of s = 0
            if (_distances[indexOfSource] != 0)
            {
                Console.WriteLine("Distance of source '" + source + "' to itself = " + _distances[indexOfSource]);
                return false;
            }

            // check that for each edge v-w dist[w] <= dist[v] + 1
            // provided v is reachable from s
            foreach (var node in graph.Vertices)
            {
                var v = _nodesToIndices[node];

                foreach (var adjacent in graph.Neighbours(node))
                {
                    var w = _nodesToIndices[adjacent];

                    if (HasPathTo(node) != HasPathTo(adjacent))
                    {
                        Console.WriteLine("edge " + node + "-" + adjacent);
                        Console.WriteLine("hasPathTo(" + node + ") = " + HasPathTo(node));
                        Console.WriteLine("hasPathTo(" + adjacent + ") = " + HasPathTo(adjacent));
                        return false;
                    }
                    if (HasPathTo(node) && (_distances[w] > _distances[v] + 1))
                    {
                        Console.WriteLine("edge " + node + "-" + adjacent);
                        Console.WriteLine("distanceTo[" + node + "] = " + _distances[v]);
                        Console.WriteLine("distanceTo[" + adjacent + "] = " + _distances[w]);
                        return false;
                    }
                }
            }

            // check that v = edgeTo[w] satisfies distTo[w] + distTo[v] + 1
            // provided v is reachable from source
            foreach (var node in graph.Vertices)
            {
                var w = _nodesToIndices[node];

                if (!HasPathTo(node) || node.IsEqualTo(source))
                    continue;

                var v = _predecessors[w];

                if (_distances[w] != _distances[v] + 1)
                {
                    Console.WriteLine("shortest path edge " + v + "-" + w);
                    Console.WriteLine("distanceTo[" + v + "] = " + _distances[v]);
                    Console.WriteLine("distanceTo[" + w + "] = " + _distances[w]);
                    return false;
                }
            }

            return true;
        }


        /************************************************************************************************************/


        /// <summary>
        ///     Determines whether there is a path from the source vertex to this specified vertex.
        /// </summary>
        public bool HasPathTo(T destination)
        {
            if (!_nodesToIndices.ContainsKey(destination))
                throw new Exception("Graph doesn't have the specified vertex.");

            var dstIndex = _nodesToIndices[destination];
            return _visited[dstIndex];
        }

        /// <summary>
        ///     Returns the distance between the source vertex and the specified vertex.
        /// </summary>
        public long DistanceTo(T destination)
        {
            if (!_nodesToIndices.ContainsKey(destination))
                throw new Exception("Graph doesn't have the specified vertex.");

            var dstIndex = _nodesToIndices[destination];
            return _distances[dstIndex];
        }

        /// <summary>
        ///     Returns an enumerable collection of nodes that specify the shortest path from the source vertex to the destination
        ///     vertex.
        /// </summary>
        public IEnumerable<T> ShortestPathTo(T destination)
        {
            if (!_nodesToIndices.ContainsKey(destination))
                throw new Exception("Graph doesn't have the specified vertex.");
            if (!HasPathTo(destination))
                return null;

            var dstIndex = _nodesToIndices[destination];
            var stack = new DataStructures.Lists.Stack<T>();

            int index;
            for (index = dstIndex; _distances[index] != 0; index = _predecessors[index])
                stack.Push(_indicesToNodes[index]);

            // Push the source vertex
            stack.Push(_indicesToNodes[index]);

            return stack;
        }
    }

    public static class ConnectedComponents
    {
        /// <summary>
        ///     Private helper. Discovers a connected component and return from a source vertex in a graph.
        /// </summary>
        private static List<TVertex> _bfsConnectedComponent<TVertex>(IGraph<TVertex> graph, TVertex source,
            ref HashSet<TVertex> visited) where TVertex : IComparable<TVertex>
        {
            var component = new List<TVertex>();
            var queue = new Queue<TVertex>();

            queue.Enqueue(source);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (!visited.Contains(current))
                {
                    component.Add(current);
                    visited.Add(current);

                    foreach (var adjacent in graph.Neighbours(current))
                        if (!visited.Contains(adjacent))
                            queue.Enqueue(adjacent);
                }
            }

            return component;
        }


        /// <summary>
        ///     Return the the connected components in graph as list of lists of nodes. Each list represents a connected component.
        /// </summary>
        public static List<List<TVertex>> Compute<TVertex>(IGraph<TVertex> Graph) where TVertex : IComparable<TVertex>
        {
            var components = new List<List<TVertex>>();
            var visited = new HashSet<TVertex>();

            // Validate the graph parameter
            if (Graph == null)
                throw new ArgumentNullException();
            if (Graph.IsDirected)
                throw new NotSupportedException("Directed Graphs are not supported.");
            if (Graph.VerticesCount == 0)
                return components;

            // Get connected components using BFS
            foreach (var vertex in Graph.Vertices)
                if (!visited.Contains(vertex))
                    components.Add(_bfsConnectedComponent(Graph, vertex, ref visited));

            return components;
        }
    }

    public static class CyclesDetector
    {
        /// <summary>
        ///     [Undirected DFS Forest].
        ///     Helper function used to decide whether the graph explored from a specific vertex contains a cycle.
        /// </summary>
        /// <param name="graph">The graph to explore.</param>
        /// <param name="source">The vertex to explore graph from.</param>
        /// <param name="parent">The predecessor node to the vertex we are exploring the graph from.</param>
        /// <param name="visited">A hash set of the explored nodes so far.</param>
        /// <returns>True if there is a cycle; otherwise, false.</returns>
        private static bool _isUndirectedCyclic<T>(IGraph<T> graph, T source, object parent, ref HashSet<T> visited)
            where T : IComparable<T>
        {
            if (!visited.Contains(source))
            {
                // Mark the current node as visited
                visited.Add(source);

                // Recur for all the vertices adjacent to this vertex
                foreach (var adjacent in graph.Neighbours(source))
                {
                    // If an adjacent node was not visited, then check the DFS forest of the adjacent for UNdirected cycles.
                    if (!visited.Contains(adjacent) && _isUndirectedCyclic(graph, adjacent, source, ref visited))
                        return true;

                        // If an adjacent is visited and NOT parent of current vertex, then there is a cycle.
                    if (parent != null && !adjacent.IsEqualTo((T) parent))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     [Directed DFS Forest]
        ///     Helper function used to decide whether the graph explored from a specific vertex contains a cycle.
        /// </summary>
        /// <param name="graph">The graph to explore.</param>
        /// <param name="source">The vertex to explore graph from.</param>
        /// <param name="parent">The predecessor node to the vertex we are exploring the graph from.</param>
        /// <param name="visited">A hash set of the explored nodes so far.</param>
        /// <param name="recursionStack">A set of element that are currently being processed.</param>
        /// <returns>True if there is a cycle; otherwise, false.</returns>
        private static bool _isDirectedCyclic<T>(IGraph<T> graph, T source, ref HashSet<T> visited,
            ref HashSet<T> recursionStack) where T : IComparable<T>
        {
            if (!visited.Contains(source))
            {
                // Mark the current node as visited and add it to the recursion stack
                visited.Add(source);
                recursionStack.Add(source);

                // Recur for all the vertices adjacent to this vertex
                foreach (var adjacent in graph.Neighbours(source))
                {
                    // If an adjacent node was not visited, then check the DFS forest of the adjacent for directed cycles.
                    if (!visited.Contains(adjacent) &&
                        _isDirectedCyclic(graph, adjacent, ref visited, ref recursionStack))
                        return true;

                        // If an adjacent is visited and is on the recursion stack then there is a cycle.
                    if (recursionStack.Contains(adjacent))
                        return true;
                }
            }

            // Remove the source vertex from the recursion stack
            recursionStack.Remove(source);
            return false;
        }


        /// <summary>
        ///     Returns true if Graph has cycle.
        /// </summary>
        public static bool IsCyclic<T>(IGraph<T> Graph) where T : IComparable<T>
        {
            if (Graph == null)
                throw new ArgumentNullException();

            var visited = new HashSet<T>();
            var recursionStack = new HashSet<T>();

            if (Graph.IsDirected)
            {
                foreach (var vertex in Graph.Vertices)
                    if (_isDirectedCyclic(Graph, vertex, ref visited, ref recursionStack))
                        return true;
            }
            else
            {
                foreach (var vertex in Graph.Vertices)
                    if (_isUndirectedCyclic(Graph, vertex, null, ref visited))
                        return true;
            }

            return false;
        }
    }

    public static class DepthFirstSearcher
    {
        /// <summary>
        ///     DFS Recursive Helper function.
        ///     Visits the neighbors of a given vertex recusively, and applies the given Action<T> to each one of them.
        /// </summary>
        private static void _visitNeighbors<T>(T Vertex, ref IGraph<T> Graph, ref Dictionary<T, object> Parents,
            Action<T> Action) where T : IComparable<T>
        {
            foreach (var adjacent in Graph.Neighbours(Vertex))
            {
                if (!Parents.ContainsKey(adjacent))
                {
                    // DFS VISIT NODE
                    Action(adjacent);

                    // Save adjacents parent into dictionary
                    Parents.Add(adjacent, Vertex);

                    // Recusively visit adjacent nodes
                    _visitNeighbors(adjacent, ref Graph, ref Parents, Action);
                }
            }
        }

        /// <summary>
        ///     Recursive DFS Implementation with helper.
        ///     Traverses all the nodes in a graph starting from a specific node, applying the passed action to every node.
        /// </summary>
        public static void VisitAll<T>(ref IGraph<T> Graph, T StartVertex, Action<T> Action) where T : IComparable<T>
        {
            // Check if graph is empty
            if (Graph.VerticesCount == 0)
                throw new Exception("Graph is empty!");

            // Check if graph has the starting vertex
            if (!Graph.HasVertex(StartVertex))
                throw new Exception("Starting vertex doesn't belong to graph.");

            var parents = new Dictionary<T, object>(Graph.VerticesCount); // keeps track of visited nodes and tree-edges

            foreach (var vertex in Graph.Neighbours(StartVertex))
            {
                if (!parents.ContainsKey(vertex))
                {
                    // DFS VISIT NODE
                    Action(vertex);

                    // Add to parents dictionary
                    parents.Add(vertex, null);

                    // Visit neighbors using recusrive helper
                    _visitNeighbors(vertex, ref Graph, ref parents, Action);
                }
            }
        }

        /// <summary>
        ///     Iterative DFS Implementation.
        ///     Given a starting node, dfs the graph and print the nodes as they get visited.
        /// </summary>
        public static void PrintAll<T>(IGraph<T> Graph, T StartVertex) where T : IComparable<T>
        {
            // Check if graph is empty
            if (Graph.VerticesCount == 0)
                throw new Exception("Graph is empty!");

            // Check if graph has the starting vertex
            if (!Graph.HasVertex(StartVertex))
                throw new Exception("Starting vertex doesn't belong to graph.");

            var visited = new HashSet<T>();
            var stack = new Stack<T>(Graph.VerticesCount);

            stack.Push(StartVertex);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                if (!visited.Contains(current))
                {
                    // DFS VISIT NODE STEP
                    Console.Write("({0}) ", current);
                    visited.Add(current);

                    // Get the adjacent nodes of current
                    foreach (var adjacent in Graph.Neighbours(current))
                        if (!visited.Contains(adjacent))
                            stack.Push(adjacent);
                }
            }
        }

        /// <summary>
        ///     Iterative DFS Implementation.
        ///     Given a predicate function and a starting node, this function searches the nodes of the graph for a first match.
        /// </summary>
        public static T FindFirstMatch<T>(IGraph<T> Graph, T StartVertex, Predicate<T> Match) where T : IComparable<T>
        {
            // Check if graph is empty
            if (Graph.VerticesCount == 0)
                throw new Exception("Graph is empty!");

            // Check if graph has the starting vertex
            if (!Graph.HasVertex(StartVertex))
                throw new Exception("Starting vertex doesn't belong to graph.");

            var stack = new Stack<T>();
            var parents = new Dictionary<T, object>(Graph.VerticesCount); // keeps track of visited nodes and tree-edges

            object currentParent = null;
            stack.Push(StartVertex);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                // Skip loop if node was already visited
                if (!parents.ContainsKey(current))
                {
                    // Save its parent into the dictionary
                    // Mark it as visited
                    parents.Add(current, currentParent);

                    // DFS VISIT NODE STEP
                    if (Match(current))
                        return current;

                    // Get currents adjacent nodes (might add already visited nodes).
                    foreach (var adjacent in Graph.Neighbours(current))
                        if (!parents.ContainsKey(adjacent))
                            stack.Push(adjacent);

                    // Mark current as the father of its adjacents. This helps keep track of tree-nodes.
                    currentParent = current;
                }
            } //end-while

            throw new Exception("Item was not found!");
        }
    }

    public class DijkstraAllPairsShortestPaths<TGraph, TVertex>
        where TGraph : IGraph<TVertex>, IWeightedGraph<TVertex>
        where TVertex : IComparable<TVertex>
    {
        /// <summary>
        ///     INSTANCE VARIABLES
        /// </summary>
        private readonly Dictionary<TVertex, DijkstraShortestPaths<TGraph, TVertex>> _allPairsDjkstra;


        /// <summary>
        ///     CONSTRUCTOR
        /// </summary>
        public DijkstraAllPairsShortestPaths(TGraph Graph)
        {
            if (Graph == null)
                throw new ArgumentNullException();

            // Initialize the all pairs dictionary
            _allPairsDjkstra = new Dictionary<TVertex, DijkstraShortestPaths<TGraph, TVertex>>();

            var vertices = Graph.Vertices;

            foreach (var vertex in vertices)
            {
                var dijkstra = new DijkstraShortestPaths<TGraph, TVertex>(Graph, vertex);
                _allPairsDjkstra.Add(vertex, dijkstra);
            }
        }


        /// <summary>
        ///     Determines whether there is a path from source vertex to destination vertex.
        /// </summary>
        public bool HasPath(TVertex source, TVertex destination)
        {
            if (!_allPairsDjkstra.ContainsKey(source) || !_allPairsDjkstra.ContainsKey(destination))
                throw new Exception("Either one of the vertices or both of them don't belong to Graph.");

            return _allPairsDjkstra[source].HasPathTo(destination);
        }

        /// <summary>
        ///     Returns the distance between source vertex and destination vertex.
        /// </summary>
        public long PathDistance(TVertex source, TVertex destination)
        {
            if (!_allPairsDjkstra.ContainsKey(source) || !_allPairsDjkstra.ContainsKey(destination))
                throw new Exception("Either one of the vertices or both of them don't belong to Graph.");

            return _allPairsDjkstra[source].DistanceTo(destination);
        }

        /// <summary>
        ///     Returns an enumerable collection of nodes that specify the shortest path from source vertex to destination vertex.
        /// </summary>
        public IEnumerable<TVertex> ShortestPath(TVertex source, TVertex destination)
        {
            if (!_allPairsDjkstra.ContainsKey(source) || !_allPairsDjkstra.ContainsKey(destination))
                throw new Exception("Either one of the vertices or both of them don't belong to Graph.");

            return _allPairsDjkstra[source].ShortestPathTo(destination);
        }
    }

    public class DijkstraShortestPaths<TGraph, TVertex>
        where TGraph : IGraph<TVertex>, IWeightedGraph<TVertex>
        where TVertex : IComparable<TVertex>
    {
        // A const that represent an infinite distance
        private const long Infinity = long.MaxValue;
        private const int NilPredecessor = -1;
        private long[] _distances;

        /// <summary>
        ///     INSTANCE VARIABLES
        /// </summary>
        private int _edgesCount;

        private WeightedEdge<TVertex>[] _edgeTo;

        // A dictionary that maps integer index to node-value
        private Dictionary<int, TVertex> _indicesToNodes;

        // A dictionary that maps node-values to integer indeces
        private Dictionary<TVertex, int> _nodesToIndices;
        private int[] _predecessors;
        private int _verticesCount;


        /// <summary>
        ///     CONSTRUCTOR
        /// </summary>
        /// <param name="Graph"></param>
        public DijkstraShortestPaths(TGraph Graph, TVertex Source)
        {
            if (Graph == null)
                throw new ArgumentNullException();
            if (!Graph.HasVertex(Source))
                throw new ArgumentException("The source vertex doesn't belong to graph.");

            // Init
            _initializeDataMembers(Graph);

            // Traverse the graph
            _dijkstra(Graph, Source);

            // check for the acyclic invariant
            //if (!_checkOptimalityConditions(Graph, Source))
            //    throw new Exception("Graph doesn't match optimality conditions.");
            Debug.Assert(_checkOptimalityConditions(Graph, Source));
        }


        /************************************************************************************************************/


        /// <summary>
        ///     The Dijkstra's algorithm.
        /// </summary>
        private void _dijkstra(TGraph graph, TVertex source)
        {
            var minPQ = new MinPriorityQueue<TVertex, long>((uint) _verticesCount);

            var srcIndex = _nodesToIndices[source];
            _distances[srcIndex] = 0;

            minPQ.Enqueue(source, _distances[srcIndex]);

            // Main loop
            while (!minPQ.IsEmpty)
            {
                var current = minPQ.DequeueMin(); // get vertex with min weight
                var currentIndex = _nodesToIndices[current]; // get its index
                var edges = graph.OutgoingEdges(current) as IEnumerable<WeightedEdge<TVertex>>;
                    // get its outgoing weighted edges

                foreach (var edge in edges)
                {
                    var adjacentIndex = _nodesToIndices[edge.Destination];

                    // calculate a new possible weighted path if the edge weight is less than infinity
                    var delta = Infinity;
                    if (edge.Weight < Infinity && Infinity - edge.Weight > _distances[currentIndex]) // Handles overflow
                        delta = _distances[currentIndex] + edge.Weight;

                    // Relax the edge
                    // if check is true, a shorter path is found from current to adjacent
                    if (delta < _distances[adjacentIndex])
                    {
                        _edgeTo[adjacentIndex] = edge;
                        _distances[adjacentIndex] = delta;
                        _predecessors[adjacentIndex] = currentIndex;

                        // decrease priority with a new distance if it exists; otherwise enqueque it
                        if (minPQ.Contains(edge.Destination))
                            minPQ.UpdatePriority(edge.Destination, delta);
                        else
                            minPQ.Enqueue(edge.Destination, delta);
                    }
                } //end-foreach
            } //end-while
        }

        /// <summary>
        ///     Constructors helper function. Initializes some of the data memebers.
        /// </summary>
        private void _initializeDataMembers(TGraph Graph)
        {
            _edgesCount = Graph.EdgesCount;
            _verticesCount = Graph.VerticesCount;

            _distances = new long[_verticesCount];
            _predecessors = new int[_verticesCount];
            _edgeTo = new WeightedEdge<TVertex>[_edgesCount];

            _nodesToIndices = new Dictionary<TVertex, int>();
            _indicesToNodes = new Dictionary<int, TVertex>();

            // Reset the information arrays
            var i = 0;
            foreach (var node in Graph.Vertices)
            {
                if (i >= _verticesCount)
                    break;

                _edgeTo[i] = null;
                _distances[i] = Infinity;
                _predecessors[i] = NilPredecessor;

                _nodesToIndices.Add(node, i);
                _indicesToNodes.Add(i, node);

                ++i;
            }
        }

        /// <summary>
        ///     Constructors helper function. Checks Optimality Conditions:
        ///     (i) for all edges e:            distTo[e.to()] <= distTo[e.from()] + e.weight()
        ///     (ii) for all edge e on the SPT: distTo[e.to()] == distTo[e.from()] + e.weight()
        /// </summary>
        private bool _checkOptimalityConditions(TGraph graph, TVertex source)
        {
            // Get the source index (to be used with the information arrays).
            var s = _nodesToIndices[source];

            // check that edge weights are nonnegative
            foreach (var edge in graph.Edges)
            {
                if (edge.Weight < 0)
                {
                    Console.WriteLine("Negative edge weight detected.");
                    return false;
                }
            }

            // check that distTo[v] and edgeTo[v] are consistent
            if (_distances[s] != 0 || _predecessors[s] != NilPredecessor)
            {
                Console.WriteLine("distanceTo[s] and edgeTo[s] are inconsistent");
                return false;
            }

            for (var v = 0; v < graph.VerticesCount; v++)
            {
                if (v == s)
                    continue;
                if (_predecessors[v] == NilPredecessor && _distances[v] != Infinity)
                {
                    Console.WriteLine("distanceTo[] and edgeTo[] are inconsistent for at least one vertex.");
                    return false;
                }
            }

            // check that all edges e = v->w satisfy distTo[w] <= distTo[v] + e.weight()
            foreach (var vertex in graph.Vertices)
            {
                var v = _nodesToIndices[vertex];

                foreach (var edge in graph.NeighboursMap(vertex))
                {
                    var w = _nodesToIndices[edge.Key];

                    if (_distances[v] + edge.Value < _distances[w])
                    {
                        Console.WriteLine("edge " + vertex + "-" + edge.Key + " is not relaxed");
                        return false;
                    }
                }
            }

            // check that all edges e = v->w on SPT satisfy distTo[w] == distTo[v] + e.weight()
            foreach (var vertex in graph.Vertices)
            {
                var w = _nodesToIndices[vertex];

                if (_edgeTo[w] == null)
                    continue;

                var edge = _edgeTo[w];
                var v = _nodesToIndices[edge.Source];

                if (!vertex.IsEqualTo(edge.Destination))
                    return false;

                if (_distances[v] + edge.Weight != _distances[w])
                {
                    Console.WriteLine("edge " + edge.Source + "-" + edge.Destination + " on shortest path not tight");
                    return false;
                }
            }

            return true;
        }


        /************************************************************************************************************/


        /// <summary>
        ///     Determines whether there is a path from the source vertex to this specified vertex.
        /// </summary>
        public bool HasPathTo(TVertex destination)
        {
            if (!_nodesToIndices.ContainsKey(destination))
                throw new Exception("Graph doesn't have the specified vertex.");

            var index = _nodesToIndices[destination];
            return _distances[index] != Infinity;
        }

        /// <summary>
        ///     Returns the distance between the source vertex and the specified vertex.
        /// </summary>
        public long DistanceTo(TVertex destination)
        {
            if (!_nodesToIndices.ContainsKey(destination))
                throw new Exception("Graph doesn't have the specified vertex.");

            var index = _nodesToIndices[destination];
            return _distances[index];
        }

        /// <summary>
        ///     Returns an enumerable collection of nodes that specify the shortest path from the source vertex to the destination
        ///     vertex.
        /// </summary>
        public IEnumerable<TVertex> ShortestPathTo(TVertex destination)
        {
            if (!_nodesToIndices.ContainsKey(destination))
                throw new Exception("Graph doesn't have the specified vertex.");
            if (!HasPathTo(destination))
                return null;

            var dstIndex = _nodesToIndices[destination];
            var stack = new DataStructures.Lists.Stack<TVertex>();

            int index;
            for (index = dstIndex; _distances[index] != 0; index = _predecessors[index])
                stack.Push(_indicesToNodes[index]);

            // Push the source vertex
            stack.Push(_indicesToNodes[index]);

            return stack;
        }
    }

    public static class TopologicalSorter
    {
        /// <summary>
        ///     Private recursive helper.
        /// </summary>
        private static void _topoSortHelper<T>(IGraph<T> graph, T source,
            ref DataStructures.Lists.Stack<T> topoSortStack, ref HashSet<T> visited) where T : IComparable<T>
        {
            visited.Add(source);

            foreach (var adjacent in graph.Neighbours(source))
                if (!visited.Contains(adjacent))
                    _topoSortHelper(graph, adjacent, ref topoSortStack, ref visited);

            topoSortStack.Push(source);
        }


        /// <summary>
        ///     The Topological Sorting algorithm
        /// </summary>
        public static IEnumerable<T> Sort<T>(IGraph<T> Graph) where T : IComparable<T>
        {
            // If the graph is either null or is not a DAG, throw exception.
            if (Graph == null)
                throw new ArgumentNullException();
            if (!Graph.IsDirected || CyclesDetector.IsCyclic(Graph))
                throw new Exception("The graph is not a DAG.");

            var visited = new HashSet<T>();
            var topoSortStack = new DataStructures.Lists.Stack<T>();

            foreach (var vertex in Graph.Vertices)
                if (!visited.Contains(vertex))
                    _topoSortHelper(Graph, vertex, ref topoSortStack, ref visited);

            return topoSortStack;
        }
    }
}