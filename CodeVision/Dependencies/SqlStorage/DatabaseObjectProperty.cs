namespace CodeVision.Dependencies.SqlStorage
{
    public class DatabaseObjectProperty
    {
        public int DatabaseObjectId { get; set; }
        public DatabaseObjectPropertyType PropertyType { get; set; }
        public string PropertyValue { get; set; }
    }
}
