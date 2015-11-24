using System.Collections.Generic;
using CodeVision.Dependencies.Database;

namespace CodeVision.Dependencies.SqlStorage
{
    public class DatabaseObject
    {
        public int DatabaseObjectId { get; set; }
        public string FullyQualifiedName { get; set; }
        
        public int DatabaseObjectTypeId { get; set; }

        // EF has enum support but it's not very helpful because it ignores lookup tables
        // It is possible to use it without establishing relationship between lookup and main table and / or foreign keys
        public DatabaseObjectType ObjectType
        {
            get { return (DatabaseObjectType) DatabaseObjectTypeId; }
            set { DatabaseObjectTypeId = (int) value; }
        }

        public virtual List<DatabaseObjectProperty> Properties { get; set; }

        public DatabaseObject()
        {
            Properties = new List<DatabaseObjectProperty>();
        }
    }
}
