using System.Collections.Generic;

namespace CodeVision.Dependencies.Database
{
    public class Database
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Table> Tables { get; set; }
        public List<StoredProcedure> StoredProcedures { get; set; }

        public Database()
        {
            Tables = new List<Table>();
            StoredProcedures = new List<StoredProcedure>();
        }
    }
}
