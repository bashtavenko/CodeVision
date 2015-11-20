using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeVision.Dependencies.SqlStorage
{
    public class DatabaseObject
    {
        public int DatabaseObjectId { get; set; }
        public string FullyQualifiedName { get; set; }
        public virtual List<DatabaseObjectProperty> Properties { get; set; }
    }
}
