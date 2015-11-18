using System.Collections.Generic;

namespace CodeVision.Web.ViewModels
{
    public class StoredProcedure : DatabaseObject
    {
        public StoredProcedure(List<DatabaseObject> columns)
        {
            Columns = columns;
        }
        public List<DatabaseObject> Columns { get; set; }
    }
}