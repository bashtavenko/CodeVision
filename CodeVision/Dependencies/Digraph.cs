using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeVision.Dependencies
{
    public class Digraph
    {
        public int V { get; private set; }
        public int E { get; private set; }

        private HashSet<int>[] _adj;

        const int InitalCapacity = 100;

        public Digraph() : this (null)  
        {            
        }

        public Digraph(Memento<int[][]> memento)
        {
            _adj = new HashSet<int>[InitalCapacity];
            // We don't want to initialize individual arrays here because
            // it is done in AddVertexInternal and if we rather do it here, 
            // we would also need to initiaze those arrays after resizing.
            // The downside is that adjacency list can be null as opposed to HashSet with zero elements
            // for the vertices that have gaps (adding verex 10 to an empty graph).

            if (memento != null && memento.State.Length > 0)
            {
                SetMemento(memento);
            }
        }

        /// <summary>
        /// Adds vertex with a given index to the graph.
        /// </summary>
        /// <param name="v">Vertex index. It can be anything, not neccessarly sequential 0,1,2... but if it is not, graph will have phantom vertices.</param>
        public void AddVertex (int v)
        {            
            AddVertexInternal(v);
        }

        public void AddEdge (int v, int w)
        {
            if (v < 0)
            {
                throw new ArgumentException(nameof(v));
            }
            if (w < 0)
            {
                throw new ArgumentException(nameof(w));
            }
            AddVertexInternal(w);
            var edges = AddVertexInternal(v);
            edges.Add(w);
            E++;
        }

        public void RemoveEdge(int v, int w)
        {
            EnsureVertexExists(v);
            EnsureVertexExists(w);
            _adj[v].Remove(w);
            E--;
        }

        public IEnumerable<int> GetAdjList(int v)
        {
            EnsureVertexExists(v);
            return _adj[v] ?.AsEnumerable() ?? Enumerable.Empty<int>();
        }

        public Digraph Reverse()
        {
            Digraph r = new Digraph();
            for (int v = 0; v < V; v++)
            {
                foreach (int w in GetAdjList(v))
                {
                    r.AddEdge(w, v);
                }
            }
            return r;
        }

        public Memento<int[][]> CreateMemento ()
        {            
            var jaggedArray = new int[V][];
            for (int i = 0; i < V; i++)
            {
                jaggedArray[i] = _adj[i]?.ToArray() ?? Enumerable.Empty<int>().ToArray();
            }            
            return new Memento<int[][]>(jaggedArray);
        }

        public void SetMemento (Memento<int[][]> memento)
        {            
            if (memento == null)
            {
                throw new ArgumentNullException(nameof(memento));
            }

            var jaggedArray = memento.State;
            V = jaggedArray.Count();
            _adj = new HashSet<int>[V];
            for (int v = 0; v < V; v++)
            {
                _adj[v] = new HashSet<int>(jaggedArray[v]);
                E += jaggedArray[v].Count();
            }
        }

        private HashSet<int> AddVertexInternal (int v)
        {
            ResizeArrayIfNeeded(v);
            HashSet<int> edges;
            if (_adj[v] == null)
            {
                edges = new HashSet<int>();
                _adj[v] = edges;
                if (v >= V)
                {
                    V = v + 1;
                }
            }
            else
            {
                edges = _adj[v];
            }
            
            return edges;
        }

        private void ResizeArrayIfNeeded (int v)
        {
            while (v >=_adj.Count())
            {
                Array.Resize<HashSet<int>>(ref _adj, _adj.Count() * 2);
            }
        }

        private void EnsureVertexExists(int v)
        {
            if ((v >= V || v < 0))
            {
                throw new ArgumentException(nameof(v));
            }
        }
    }
}
