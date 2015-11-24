namespace CodeVision.Dependencies.SqlStorage
{
    public class DatabaseObjectProperty
    {
        public int DatabaseObjectId { get; set; }
        public int DatabaseObjectPropertyTypeId { get; set; }

        public DatabaseObjectPropertyType PropertyType
        {
            get { return (DatabaseObjectPropertyType) DatabaseObjectPropertyTypeId; }
            set { DatabaseObjectPropertyTypeId = (int) value; }
        }

        public string PropertyValue { get; set; }
    }
}
