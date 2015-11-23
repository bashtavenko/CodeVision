using System.Collections.Generic;
using CodeVision.Dependencies.Database;

namespace CodeVision.Dependencies.SqlStorage
{
    public class DatabaseObject
    {
        public int DatabaseObjectId { get; set; }
        public string FullyQualifiedName { get; set; }
        public DatabaseObjectType ObjectType { get; set; }
        public virtual List<DatabaseObjectProperty> Properties { get; set; }

        public DatabaseObject()
        {
            Properties = new List<DatabaseObjectProperty>();
        }
    }
}
