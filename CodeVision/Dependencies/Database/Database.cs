using System.Collections.Generic;

namespace CodeVision.Dependencies.Database
{
    public class Database
    {
        public List<Table> Tables { get; set; }
        public List<StoredProcedure> StoredProcedures { get; set; }
    }
}
