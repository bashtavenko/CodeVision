using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeVision.Dependencies.SqlStorage.Maps
{
    public class DatabaseObjectPropertyTypeLookupConfiguration : EntityTypeConfiguration<DatabaseObjectPropertyTypeLookup>
    {
        public DatabaseObjectPropertyTypeLookupConfiguration()
        {            
            HasKey(h => h.DatabaseObjectPropertyTypeId);
            Property(h => h.DatabaseObjectPropertyTypeId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(p => p.Name)
                .HasMaxLength(100)
                .IsRequired()
                .IsUnicode(false);

            ToTable("DatabaseObjectPropertyType");
        }
    }
}