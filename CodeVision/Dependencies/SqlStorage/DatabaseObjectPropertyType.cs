using System.Collections.Generic;

namespace CodeVision.Dependencies.SqlStorage
{
    public class DatabaseObjectPropertyType
    {
        public int DatabaseObjectPropertyTypeId { get; set; }
        public string Name { get; set; }
        public virtual List<DatabaseObjectProperty> DatabaseObjectProperties { get; set; }
    }
}