namespace CodeVision.Dependencies.Database
{
    public class Column
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FullyQualifiedName { get; set; } // database.owner.table.column
    }
}
