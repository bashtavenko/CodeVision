using CodeVision.Dependencies.SqlStorage;

namespace CodeVision.Web.ViewModels
{
    public class DatabaseObjectProperty
    {
        public DatabaseObjectPropertyType PropertyType { get; set; }
        public string PropertyName { get; set; }
        public string PropertyValue { get; set; } 
    }
}