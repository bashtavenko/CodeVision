using System.Data.Entity.ModelConfiguration;

namespace CodeVision.Dependencies.SqlStorage.Maps
{
    public class PackageConfiguration : EntityTypeConfiguration<Package>
    {
        public PackageConfiguration()
        {            
            HasKey(h => h.PackageId);
            Property(h => h.Name).HasColumnType("varchar").HasMaxLength(255);
        }
    }
}