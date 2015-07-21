using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeVision.CSharp.Semantic
{
    public class DependencyGraph
    {
        private Dictionary<string, int> _st;
        private Dictionary<int, string> _keys;
        private Digraph _g;

        public IEnumerable<string> Modules
        {
            get
            {
                return _st.Keys.AsEnumerable();
            }
        }

        public Digraph Digraph {  get { return _g; } }

        public DependencyGraph() : this (null, new Digraph())
        {            
        }

        public DependencyGraph(Memento<string[]> memento, Digraph g)
        {
            _st = new Dictionary<string, int>();
            _keys = new Dictionary<int, string>();

            if (memento != null)
            {
                SetMemento(memento);
            }
            _g = g;
        }

        public void AddModule(string name)
        {
            int vertexIndex = AddVertexInternal(name);
            _g.AddVertex(vertexIndex);
        }

        public void AddDependency(string fromModule, string toModule)
        {
            int vi = AddVertexInternal(fromModule);
            int wi = AddVertexInternal(toModule);
            _g.AddEdge(vi, wi);
        } 

        public List<string> GetDependencies (string moduleName, DependencyDirection direction, DependencyLevels levels)
        {            
            if (!_st.ContainsKey(moduleName)) // Supposed to have correct case
            {
                return new List<string>();
            }

            Digraph g = direction == DependencyDirection.Downstream ? _g : _g.Reverse();
            int moduleInex = _st[moduleName];
            IEnumerable<int> dependencyIndices;
            if (levels == DependencyLevels.DirectOnly)
            {
                // That's easy
                dependencyIndices = g.GetAdjList(moduleInex);
            }
            else
            {
                // Run DFS to find out
                DigraphDfs dfs = new DigraphDfs(g, moduleInex);
                dependencyIndices = dfs.ReachableVertices;
            }

            List<string> r = new List<string>();
            foreach (var i in dependencyIndices)
            {
                r.Add(_keys[i]);
            }
                                              
            return r;
        }

        public List<string> GetModulesBeginsWith (string name)
        {
            return _st.Keys.Where(w => w.StartsWith(name, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        public Memento<string[]> CreateMemento()
        {            
            return new Memento<string[]>(_st.Keys.ToArray());
        }

        public void SetMemento(Memento<string[]> memento)
        {   
            if (memento == null)
            {
                throw new ArgumentNullException(nameof(memento));
            }
                     
            foreach (string module in memento.State)
            {
                AddKey(module);
            }
        }

        private int AddVertexInternal (string v)
        {
            int vertexIndex;
            if (!_st.ContainsKey(v))
            {
                vertexIndex = AddKey(v);
            }
            else
            {
                vertexIndex = _st[v];
            }
            return vertexIndex;
        }

        private int AddKey(string v)
        {
            int vertexIndex = _st.Count;
            _st.Add(v, vertexIndex);
            _keys.Add(vertexIndex, v);
            return vertexIndex;
        }
    }
}
