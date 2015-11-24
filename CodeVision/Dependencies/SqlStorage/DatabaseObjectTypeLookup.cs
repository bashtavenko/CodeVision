using System.Collections.Generic;
using CodeVision.Dependencies.Database;

namespace CodeVision.Dependencies.SqlStorage
{
    public class DatabaseObjectTypeLookup
    {
        public DatabaseObjectType ObjectType { get; set; }
        public string Name { get; set; }
        public virtual  List<DatabaseObject> DatabaseObjectsType { get; set; }
    }
}
