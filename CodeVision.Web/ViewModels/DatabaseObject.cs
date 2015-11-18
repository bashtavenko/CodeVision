using System.Collections.Generic;
using CodeVision.Dependencies.Database;

namespace CodeVision.Web.ViewModels
{
    public class DatabaseObject
    {
        public int Id { get; set; }

        public string Name
        {
            get
            {
                if (!string.IsNullOrEmpty(FullyQualifiedName))
                {
                    var nameParts = FullyQualifiedName.Split('.');
                    return nameParts.Length > 0 ? nameParts[nameParts.Length - 1] : string.Empty;
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public string FullyQualifiedName { get; set; }
        public DatabaseObjectType ObjectType { get; set; }
        public string ObjectTypeName => ObjectType.ToString();
        public List<DatabaseObjectProperty> Properties { get; set; }
    }
}