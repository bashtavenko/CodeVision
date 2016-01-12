using System.Data.Entity.ModelConfiguration;

namespace CodeVision.Dependencies.SqlStorage.Maps
{
    public class ProjectConfiguration : EntityTypeConfiguration<Project>
    {
        public ProjectConfiguration()
        {            
            HasKey(h => h.ProjectId);
            Property(h => h.Name).HasColumnType("varchar").HasMaxLength(255);

            HasMany(c => c.Packages)
                .WithMany(w => w.Projects)
                .Map(c =>
                {
                    c.ToTable("ProjectPackage");
                    c.MapLeftKey("ProjectId");
                    c.MapRightKey("PackageId");
                });
        }
    }
}