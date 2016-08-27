using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeVision.Dependencies.Database
{
    public class DatabaseObjectsGraph : SymbolGraph<DatabaseObject>
    {
        public int ObjectsCount => Keys.Count;
        
        public DatabaseObjectsGraph() 
        {            
        }

        public DatabaseObjectsGraph(Memento<DatabaseObject[]> memento) : base(memento)
        {
        }

        public DatabaseObjectsGraph(Memento<DatabaseObject[]> memento, Digraph g) : base (memento, g)
        {
        }
        
        public List<DatabaseObject> GetDatabaseObjectsBeginsWith(string name)
        {
            return GetDatabaseObjectsBeginsWith(name, null);
        }

        public List<DatabaseObject> GetDatabaseObjectsBeginsWith (string name, DatabaseObjectType[] onlyTheseTypes)
        {
            IEnumerable<DatabaseObject> items = SymbolTable.Keys
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
            return Keys[databaseObjectId];
        }

        public int AddDatabaseObject(DatabaseObject databaseObject)
        {
            UpdateObjectState(databaseObject);
            return AddSymbol(databaseObject);
        }

        public void AddDependency(int fromObjectId, int toObjectId)
        {
            EnforceDatabaseObjectId(fromObjectId);
            EnforceDatabaseObjectId(toObjectId);
            AddDependency(Keys[fromObjectId], Keys[toObjectId]);
        }

        public void AddDependency(DatabaseObject fromObject, DatabaseObject toObject)
        {
            UpdateObjectState(fromObject);
            UpdateObjectState(toObject);
            AddEdge(fromObject, toObject);
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
            DatabaseObject databaseObject = Keys[databaseObjectId];

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
            DatabaseObject databaseObject = Keys[databaseObjectId];
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
            if (Keys[storedProcedureId].ObjectType != DatabaseObjectType.StoredProcedure)
            {
                throw new ArgumentException(nameof(storedProcedureId));
            }
            if (Keys[columnId].ObjectType != DatabaseObjectType.Column)
            {
                throw new ArgumentException(nameof(storedProcedureId));
            }
            Digraph.RemoveEdge(storedProcedureId, columnId);
        }

        public void RemoveEdge(int v, int w)
        {
            EnforceDatabaseObjectId(v);
            EnforceDatabaseObjectId(w);
            Digraph.RemoveEdge(v, w);
        }

        public void UpdatedCommentText (int databaseObjectId, string text)
        {
            EnforceDatabaseObjectId(databaseObjectId);
            DatabaseObject databaseObject = Keys[databaseObjectId];
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
            DatabaseObject databaseObject = Keys[databaseObjectId];
            return GetDependencies(databaseObject, direction, level, objectsType);
        }

        public List<DatabaseObject> GetDependencies (DatabaseObject databaseObject, DependencyDirection direction, DependencyLevel level, DatabaseObjectType? objectsType = null)
        {
            if (!SymbolTable.ContainsKey(databaseObject))
            {
                throw new ArgumentException(nameof(databaseObject));
            }

            var items = GetDependencies(SymbolTable[databaseObject], direction, level);
            return objectsType == null ? items : items.Where(w => w.ObjectType == objectsType).ToList();
        }

        public List<DatabaseObject> GetColumnsForStoredProcedure(int databaseObjectId)
        {
            EnforceDatabaseObjectId(databaseObjectId);
            if (Keys[databaseObjectId].ObjectType != DatabaseObjectType.StoredProcedure)
            {
                throw new ArgumentOutOfRangeException(nameof(databaseObjectId));
            }
            var items = GetDependencies(databaseObjectId, DependencyDirection.Downstream, DependencyLevel.DirectOnly);
            return items.Where(w => w.ObjectType == DatabaseObjectType.Column).ToList();
        }

        private void EnforceDatabaseObjectId(int databaseObjectId)
        {
            if (databaseObjectId < 0 || databaseObjectId >= Keys.Count)
            {
                throw new ArgumentException(nameof(databaseObjectId));
            }
        }

        private void UpdateObjectState(DatabaseObject databaseObject)
        {
            if (!SymbolTable.ContainsKey(databaseObject))
            {
                databaseObject.ObjectState = ObjectState.VertexAdded;
            }
        }
    }
}