using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeVision.CSharp.Semantic.SqlStorage.Maps
{
    public class VertexConfiguration : EntityTypeConfiguration<Vertex>
    {
        public VertexConfiguration()
        {            
            HasKey(h => h.VertexId);
            Property(p => p.VertexId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            ToTable("DependencyGraph");
        }
    }
}
