using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using CodeVision.Dependencies.SqlStorage.Maps;

namespace CodeVision.Dependencies.SqlStorage
{
    public class DependencyGraphContext : DbContext
    {   
        public DependencyGraphContext(string connectionString) : this (connectionString, new CreateDatabaseIfNotExists<DependencyGraphContext>())
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<DependencyGraphContext>());        
        }

        public DependencyGraphContext(string connectionString, IDatabaseInitializer<DependencyGraphContext> initializer)
                : base(nameOrConnectionString: connectionString)
        {
            Database.SetInitializer(initializer);
        }
                        
        public DbSet<Module> Modules { get; set; }
        public DbSet<Vertex> Vertices { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Configurations.Add(new ModuleConfiguration());
            modelBuilder.Configurations.Add(new VertexConfiguration());
        }
    }
}
