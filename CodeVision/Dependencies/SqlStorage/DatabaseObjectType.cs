using System.Collections.Generic;

namespace CodeVision.Dependencies.SqlStorage
{
    public class DatabaseObjectType
    {
        public int DatabaseObjectTypeId { get; set; }
        public string Name { get; set; }
        public virtual  List<DatabaseObject> DatabaseObjectsType { get; set; }
    }
}
