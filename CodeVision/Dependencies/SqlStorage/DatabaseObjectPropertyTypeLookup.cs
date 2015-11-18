using System.Collections.Generic;

namespace CodeVision.Dependencies.SqlStorage
{
    public class DatabaseObjectPropertyTypeLookup
    {
        public DatabaseObjectPropertyType PropertyType { get; set; }
        public string Name { get; set; }
        public virtual List<DatabaseObjectProperty> DatabaseObjectProperties { get; set; }
    }
}