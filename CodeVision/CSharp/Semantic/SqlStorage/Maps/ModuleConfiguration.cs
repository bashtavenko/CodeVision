using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeVision.CSharp.Semantic.SqlStorage.Maps
{
    public class ModuleConfiguration : EntityTypeConfiguration<Module>
    {
        public ModuleConfiguration()
        {            
            HasKey(h => h.ModuleId);
            Property(p => p.ModuleId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            ToTable("DependencyGraphModule");
        }
    }
}
