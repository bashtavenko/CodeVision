using System.Collections.Generic;

namespace CodeVision.Dependencies.Database
{
    public class DatabaseObject : Symbol
    {
        public string FullyQualifiedName { get; }
        public DatabaseObjectType ObjectType { get; }
        public List<ObjectProperty> Properties { get; }
        public ObjectState ObjectState { get; set; }

        public DatabaseObject(DatabaseObjectType objectType, string fullyQualifiedName)
            : this (objectType, fullyQualifiedName, null)
        {
        }

        public DatabaseObject(DatabaseObjectType objectType, string fullyQualifiedName, int? id)
        {
            ObjectType = objectType;
            FullyQualifiedName = fullyQualifiedName;
            Properties = new List<ObjectProperty>();
            Id = id;
            ObjectState = ObjectState.Unchanged;
        }

        public override int GetHashCode()
        {
            return FullyQualifiedName.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var anotherDatabaseObject = obj as DatabaseObject;
            return anotherDatabaseObject != null && anotherDatabaseObject.GetHashCode() == GetHashCode();
        }
    }
}
