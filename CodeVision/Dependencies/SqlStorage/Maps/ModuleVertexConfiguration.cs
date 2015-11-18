using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeVision.Dependencies.SqlStorage.Maps
{
    public class ModuleVertexConfiguration : EntityTypeConfiguration<ModuleVertex>
    {
        public ModuleVertexConfiguration()
        {            
            HasKey(h => h.VertexId);
            Property(p => p.VertexId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(p => p.AdjacencyListJson).IsUnicode(false);
            ToTable("ModulesGraph");
        }
    }
}
