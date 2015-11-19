using System.Collections.Generic;

namespace CodeVision.Dependencies.Database
{
    public class Table
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public string FullyQualifiedName { get; set; } // database.owner.table

        public List<Column> Columns { get; set; }
        public List<ForeignKey> ForeignKeys { get; set; }

        public List<Table> DependentTables { get; set; }

        public Table()
        {
            Columns = new List<Column>();
            ForeignKeys = new List<ForeignKey>();
        }
    }
}
