using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeVision.CSharp.Semantic
{
    public class Digraph
    {
        public int V { get; private set; }
        public int E { get; private set; }

        private HashSet<int>[] _adj;

        const int _initalCapacity = 100;

        public Digraph() : this (null)  
        {            
        }

        public Digraph(Memento<int[][]> memento)
        {
            _adj = new HashSet<int>[_initalCapacity];
            if (memento != null)
            {
                SetMemento(memento);
            }
        }

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

        public IEnumerable<int> GetAdjList(int v)
        {
            if (!(v < _adj.Count()))
            {
                throw new ArgumentException(nameof(v));
            }

            if (_adj[v] != null)
            {
                return _adj[v].AsEnumerable();
            }
            else
            {
                return null;
            }           
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
                jaggedArray[i] = _adj[i].ToArray();
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
    }
}
