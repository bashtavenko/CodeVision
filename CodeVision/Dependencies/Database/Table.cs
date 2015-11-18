using System.Collections.Generic;

namespace CodeVision.Dependencies.Database
{
    public class Table
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Schema { get; set; }
        public string FullyQualifiedName { get; set; } // database.owner.table

        public List<Column> Columns { get; set; }
        public List<Table> DependentTables { get; set; }

        // These should probably not be in the public interface as FK's are only needed to find dependent tables.
        // or there could be another TableFinal with all fields except for ForeignKeys
        public List<ForeignKey> ForeignKeys { get; set; }

        public Table()
        {
            Columns = new List<Column>();
            ForeignKeys = new List<ForeignKey>();
            DependentTables = new List<Table>();
        }
    }
}
