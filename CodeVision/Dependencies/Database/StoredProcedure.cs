using System.Collections.Generic;

namespace CodeVision.Dependencies.Database
{
    public class StoredProcedure
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public string Path { get; set; } // database.owner.sproc
        public string TextBody { get; set; }

        public List<Column> Columns { get; set; }
    }
}
