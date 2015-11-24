using System.Data.Entity.ModelConfiguration;

namespace CodeVision.Dependencies.SqlStorage.Maps
{
    public class DatabaseObjectPropertyConfiguration : EntityTypeConfiguration<DatabaseObjectProperty>
    {
        public DatabaseObjectPropertyConfiguration()
        {            
            HasKey(h => new {h.DatabaseObjectId, h.DatabaseObjectPropertyTypeId});
            Ignore(h => h.PropertyType);
            ToTable("DatabaseObjectProperty");
        }
    }
}