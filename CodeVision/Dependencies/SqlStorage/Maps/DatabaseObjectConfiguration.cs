using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeVision.Dependencies.SqlStorage.Maps
{
    public class DatabaseObjectConfiguration : EntityTypeConfiguration<DatabaseObject>
    {
        public DatabaseObjectConfiguration()
        {            
            HasKey(h => h.DatabaseObjectId);
            Ignore(t => t.ObjectType);

            Property(p => p.FullyQualifiedName)
                .HasMaxLength(500)
                .IsRequired()
                .IsUnicode(false);

            ToTable("DatabaseObject");
        }
    }
}