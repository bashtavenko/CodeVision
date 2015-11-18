using System;
using System.Collections.Generic;
using System.Linq;
using CodeVision.Dependencies.SqlStorage;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CodeVision.Dependencies.Database
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

        public DatabaseObjectsGraph(Memento<DatabaseObject[]> memento, Digraph g)
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
        
        public List<DatabaseObject> GetDatabaseObjectsBeginsWith(string name)
        {
            return GetDatabaseObjectsBeginsWith(name, null);
        }

        public List<DatabaseObject> GetDatabaseObjectsBeginsWith (string name, DatabaseObjectType[] onlyTheseTypes)
        {
            IEnumerable<DatabaseObject> items = _st.Keys
                .Where(w => w.FullyQualifiedName.StartsWith(name, StringComparison.InvariantCultureIgnoreCase));

            if (onlyTheseTypes ?.Length > 0)
            {
                items = items
                    .Where(w => onlyTheseTypes.Contains(w.ObjectType));
            }

            return items
                .OrderBy(o => o.FullyQualifiedName)
                .Take(10)
                .ToList();
        }

        public DatabaseObject GetDatabaseObject(int databaseObjectId)
        {
            EnforceDatabaseObjectId(databaseObjectId);
            return _keys[databaseObjectId];
        }

        public int AddDatabaseObject(DatabaseObject databaseObject)
        {
            int vertexIndex = AddVertexInternal(databaseObject);
            Digraph.AddVertex(vertexIndex);
            return vertexIndex;
        }

        public void AddDependency(int fromObjectId, int toObjectId)
        {
            EnforceDatabaseObjectId(fromObjectId);
            EnforceDatabaseObjectId(toObjectId);
            AddDependency(_keys[fromObjectId], _keys[toObjectId]);
        }
        
        public void AddDependency(DatabaseObject fromObject, DatabaseObject toObject)
        {
            int vi = AddVertexInternal(fromObject);
            int wi = AddVertexInternal(toObject);
            Digraph.AddEdge(vi, wi);
            // Since graph is overwritten completely in repository, there's no need to set object state
            // which supports adding new objects to a previously loaded graph.
        }

        public void AddProperty(DatabaseObject databaseObject, ObjectProperty property)
        {
            if (databaseObject.Id == null)
            {
                throw new ArgumentException("Must have an object Id");
            }
            AddProperty(databaseObject.Id.Value, property);
        }

        public void AddProperty(int databaseObjectId, ObjectProperty property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            EnforceDatabaseObjectId(databaseObjectId);
            DatabaseObject databaseObject = _keys[databaseObjectId];

            if (databaseObject.Properties.Contains(property))
            {
                throw new ArgumentException($"Property {property} already exists.");
            }

            databaseObject.Properties.Add(property);
            databaseObject.ObjectState = ObjectState.PropertiesModified;
        }

        public void RemoveProperty(int databaseObjectId, ObjectProperty property)
        {
            EnforceDatabaseObjectId(databaseObjectId);
            DatabaseObject databaseObject = _keys[databaseObjectId];
            RemoveProperty(databaseObject, property);
        }

        public void RemoveProperty(DatabaseObject databaseObject, ObjectProperty property)
        {
            if (databaseObject == null)
            {
                throw new ArgumentNullException(nameof(databaseObject));
            }

            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            int propertyIndex =  databaseObject.Properties.IndexOf(property);
            if (propertyIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(property));
            }

            databaseObject.Properties.RemoveAt(propertyIndex);
            databaseObject.ObjectState = ObjectState.PropertiesModified;
        }

        public void RemoveColumn(int storedProcedureId, int columnId)
        {
            EnforceDatabaseObjectId(storedProcedureId);
            EnforceDatabaseObjectId(columnId);
            if (_keys[storedProcedureId].ObjectType != DatabaseObjectType.StoredProcedure)
            {
                throw new ArgumentException(nameof(storedProcedureId));
            }
            if (_keys[columnId].ObjectType != DatabaseObjectType.Column)
            {
                throw new ArgumentException(nameof(storedProcedureId));
            }
            Digraph.RemoveEdge(storedProcedureId, columnId);
        }

        public void UpdatedCommentText (int databaseObjectId, string text)
        {
            EnforceDatabaseObjectId(databaseObjectId);
            DatabaseObject databaseObject = _keys[databaseObjectId];
            UpdatedCommentText(databaseObject, text);
        }

        public void UpdatedCommentText (DatabaseObject databaseObject, string text)
        {
            if (databaseObject == null)
            {
                throw new ArgumentNullException(nameof(databaseObject));
            }

            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException(nameof(text));
            }

            var commentProperty = databaseObject.Properties.SingleOrDefault(s => s is CommentProperty) as CommentProperty;
            if (commentProperty == null)
            {
                throw new ArgumentException("Object doesn't have comment property");
            }
            commentProperty.Text = text;
            databaseObject.ObjectState = ObjectState.PropertiesModified;
        }

        public List<DatabaseObject> GetDependencies(int databaseObjectId, DependencyDirection direction, DependencyLevel level, DatabaseObjectType? objectsType)
        {
            EnforceDatabaseObjectId(databaseObjectId);
            DatabaseObject databaseObject = _keys[databaseObjectId];
            return GetDependencies(databaseObject, direction, level, objectsType);
        }


        public List<DatabaseObject> GetDependencies (DatabaseObject databaseObject, DependencyDirection direction, DependencyLevel level, DatabaseObjectType? objectsType = null)
        {
            if (!_st.ContainsKey(databaseObject))
            {
                throw new ArgumentException(nameof(databaseObject));
            }

            var items = GetDependencies(_st[databaseObject], direction, level);
            return objectsType == null ? items : items.Where(w => w.ObjectType == objectsType).ToList();
        }

        public List<DatabaseObject> GetDependencies (int databaseObjectId, DependencyDirection direction, DependencyLevel level)
        {
            EnforceDatabaseObjectId(databaseObjectId);
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

        public List<DatabaseObject> GetColumnsForStoredProcedure(int databaseObjectId)
        {
            EnforceDatabaseObjectId(databaseObjectId);
            if (_keys[databaseObjectId].ObjectType != DatabaseObjectType.StoredProcedure)
            {
                throw new ArgumentOutOfRangeException(nameof(databaseObjectId));
            }
            var items = GetDependencies(databaseObjectId, DependencyDirection.Downstream, DependencyLevel.DirectOnly);
            return items.Where(w => w.ObjectType == DatabaseObjectType.Column).ToList();
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
                v.ObjectState = ObjectState.VertexAdded;
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

        private void EnforceDatabaseObjectId(int databaseObjectId)
        {
            if (databaseObjectId < 0 || databaseObjectId >= _keys.Count)
            {
                throw new ArgumentException(nameof(databaseObjectId));
            }
        }
    }
}