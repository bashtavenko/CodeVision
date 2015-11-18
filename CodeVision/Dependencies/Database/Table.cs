using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeVision.Dependencies.Database
{
    public class Table
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public string Path { get; set; } // database.owner.table

        public List<Column> Columns { get; set; }
        public List<ForeignKey> ForeignKeys { get; set; }

        public List<Table> DependentTables { get; set; }
    }
}
