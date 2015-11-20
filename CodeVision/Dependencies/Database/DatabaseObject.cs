using System.Collections.Generic;

namespace CodeVision.Dependencies.Database
{
    public class DatabaseObject
    {
        public int? Id { get; set; }
        public string FullyQualifiedName { get; }
        public DatabaseObjectType ObjectType { get; }
        public List<ObjectProperty> Properties { get; }

        public DatabaseObject(DatabaseObjectType objectType, string fullyQualifiedName)
        {
            ObjectType = objectType;
            FullyQualifiedName = fullyQualifiedName;
            Properties = new List<ObjectProperty>();
            Id = null;
        }
    }
}
