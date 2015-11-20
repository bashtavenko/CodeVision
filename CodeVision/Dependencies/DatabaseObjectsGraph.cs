using System;
using System.Collections.Generic;
using System.Linq;
using CodeVision.Dependencies.Database;

namespace CodeVision.Dependencies
{
    public class DatabaseObjectsGraph
    {
        public Digraph Digraph { get; }
        public bool IsModified { get; private set; }
        public int ObjectsCount => _keys.Count;

        private readonly Dictionary<DatabaseObject, int> _st;
        private readonly Dictionary<int, DatabaseObject> _keys;

        public DatabaseObjectsGraph() : this (null, new Digraph())
        {            
        }

        public DatabaseObjectsGraph(Memento<DatabaseObject[]> memento) : this(memento, new Digraph())
        {
        }

        private DatabaseObjectsGraph(Memento<DatabaseObject[]> memento, Digraph g)
        {
            _st = new Dictionary<DatabaseObject, int>();
            _keys = new Dictionary<int, DatabaseObject>();

            if (memento != null)
            {
                SetMemento(memento);
            }
            Digraph = g;
            IsModified = false;
        }

        public void AddDatabaseObject(DatabaseObject databaseObject)
        {
            int vertexIndex = AddVertexInternal(databaseObject);
            Digraph.AddVertex(vertexIndex);
        }

        public void AddDependency(DatabaseObject fromObject, DatabaseObject toObject)
        {
            int vi = AddVertexInternal(fromObject);
            int wi = AddVertexInternal(toObject);
            Digraph.AddEdge(vi, wi);
        } 

        public List<DatabaseObject> GetDependencies (DatabaseObject databaseObject, DependencyDirection direction, DependencyLevel level)
        {
            if (!_st.ContainsKey(databaseObject))
            {
                throw new ArgumentException(nameof(databaseObject));
            }
            return GetDependencies(_st[databaseObject], direction, level);
        }
 
        public List<DatabaseObject> GetDependencies (int databaseObjectId, DependencyDirection direction, DependencyLevel level)
        {
            if (databaseObjectId < 0 || databaseObjectId >= _st.Count)
            {
                throw new ArgumentException(nameof(databaseObjectId));
            }
            Digraph g = direction == DependencyDirection.Downstream ? Digraph : Digraph.Reverse();
            IEnumerable<int> moduleIndices;
            if (level == DependencyLevel.DirectOnly)
            {
                // That's easy
                moduleIndices = g.GetAdjList(databaseObjectId);
            }
            else
            {
                // Run BFS to find out
                var bfs = new DigraphBfs(g, databaseObjectId);
                moduleIndices = bfs.Preorder;
            }

            List<DatabaseObject> databaseObjects = moduleIndices
                .Select(i => _keys[i])
                .ToList();

            return databaseObjects;
        }

        public List<DatabaseObject> GetDatabaseObjectsBeginsWith (string name)
        {
            return _st.Keys
                .Where(w => w.FullyQualifiedName.StartsWith(name, StringComparison.InvariantCultureIgnoreCase))
                .OrderBy(o => o.FullyQualifiedName)
                .ToList();
        }

        public Memento<DatabaseObject[]> CreateMemento()
        {            
            return new Memento<DatabaseObject[]>(_st.Keys.ToArray());
        }

        public void SetMemento(Memento<DatabaseObject[]> memento)
        {   
            if (memento == null)
            {
                throw new ArgumentNullException(nameof(memento));
            }
                     
            foreach (DatabaseObject databaseObject in memento.State)
            {
                AddKey(databaseObject);
            }
        }

        private int AddVertexInternal (DatabaseObject v)
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

        private int AddKey(DatabaseObject v)
        {
            int vertexIndex = _st.Count;
            v.Id = vertexIndex;
            _st.Add(v, vertexIndex);
            _keys.Add(vertexIndex, v);
            IsModified = true;
            return vertexIndex;
        }
    }
}