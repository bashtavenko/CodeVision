using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeVision.Dependencies.SqlStorage.Maps
{
    public class DatabaseObjectTypeLookupConfiguration : EntityTypeConfiguration<DatabaseObjectTypeLookup>
    {
        public DatabaseObjectTypeLookupConfiguration()
        {            
            HasKey(h => h.ObjectType);

            Property(h => h.ObjectType)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("DatabaseObjectTypeId");

            Property(p => p.Name)
                .HasMaxLength(100)
                .IsRequired()
                .IsUnicode(false);

            ToTable("DatabaseObjectType");
        }
    }
}