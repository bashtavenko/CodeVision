using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeVision.Dependencies
{
    public class DependencyGraph
    {
        private readonly Dictionary<Module, int> _st;
        private readonly Dictionary<int, Module> _keys;
        private readonly Digraph _g;

        public IEnumerable<Module> Modules
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

        public DependencyGraph(Memento<Module[]> memento, Digraph g)
        {
            _st = new Dictionary<Module, int>();
            _keys = new Dictionary<int, Module>();

            if (memento != null)
            {
                SetMemento(memento);
            }
            _g = g;
        }

        public void AddModule(Module module)
        {
            int vertexIndex = AddVertexInternal(module);
            _g.AddVertex(vertexIndex);
        }

        public void AddDependency(Module fromModule, Module toModule)
        {
            int vi = AddVertexInternal(fromModule);
            int wi = AddVertexInternal(toModule);
            _g.AddEdge(vi, wi);
        } 

        public List<Module> GetDependencies (Module module, DependencyDirection direction, DependencyLevel level)
        {
            if (!_st.ContainsKey(module))
            {
                throw new ArgumentException(nameof(module));
            }
            return GetDependencies(_st[module], direction, level);
        }
 
        public List<Module> GetDependencies (int moduleId, DependencyDirection direction, DependencyLevel level)
        {
            if (moduleId < 0 || moduleId >= _st.Count)
            {
                throw new ArgumentException(nameof(moduleId));
            }
            Digraph g = direction == DependencyDirection.Downstream ? _g : _g.Reverse();
            IEnumerable<int> moduleIndices;
            if (level == DependencyLevel.DirectOnly)
            {
                // That's easy
                moduleIndices = g.GetAdjList(moduleId);
            }
            else
            {
                // Run BFS to find out
                var bfs = new DigraphBfs(g, moduleId);
                moduleIndices = bfs.Preorder;
            }

            List<Module> modules = moduleIndices
                .Select(i => _keys[i])
                .ToList();

            return modules;
        }

        public List<Module> GetModulesBeginsWith (string name)
        {
            return _st.Keys
                .Where(w => w.Name.StartsWith(name, StringComparison.InvariantCultureIgnoreCase))
                .OrderBy(o => o.Name)
                .ThenBy(o => o.Version)
                .ToList();
        }

        public Memento<Module[]> CreateMemento()
        {            
            return new Memento<Module[]>(_st.Keys.ToArray());
        }

        public void SetMemento(Memento<Module[]> memento)
        {   
            if (memento == null)
            {
                throw new ArgumentNullException(nameof(memento));
            }
                     
            foreach (Module module in memento.State)
            {
                AddKey(module);
            }
        }

        private int AddVertexInternal (Module v)
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

        private int AddKey(Module v)
        {
            int vertexIndex = _st.Count;
            v.Id = vertexIndex;
            _st.Add(v, vertexIndex);
            _keys.Add(vertexIndex, v);
            return vertexIndex;
        }
    }
}
