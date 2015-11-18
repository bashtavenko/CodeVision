namespace CodeVision.Dependencies.Database
{
    public class Column
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; } // database.owner.table.column
    }
}
