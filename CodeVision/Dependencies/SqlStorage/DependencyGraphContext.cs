using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using CodeVision.Dependencies.SqlStorage.Maps;

namespace CodeVision.Dependencies.SqlStorage
{
    public class DependencyGraphContext : DbContext
    {   
        public DependencyGraphContext(string connectionString) : this (connectionString, new CreateDatabaseIfNotExists<DependencyGraphContext>())
        {
            System.Data.Entity.Database.SetInitializer(new CreateDatabaseIfNotExists<DependencyGraphContext>());        
        }

        public DependencyGraphContext(string connectionString, IDatabaseInitializer<DependencyGraphContext> initializer)
                : base(nameOrConnectionString: connectionString)
        {
            System.Data.Entity.Database.SetInitializer(initializer);
        }
                        
        public DbSet<Module> Modules { get; set; }
        public DbSet<ModuleVertex> ModuleVertices { get; set; }
        public DbSet<DatabaseObjectVertex> DatabaseObjectVerticesVertices { get; set; }

        public DbSet<DatabaseObjectType> DatabaseObjectTypes { get; set; }
        public DbSet<DatabaseObjectPropertyType> DatabaseObjectPropertyTypes { get; set; }
        public DbSet<DatabaseObject> DatabaseObjects { get; set; }
        public DbSet<DatabaseObjectProperty> DatabaseObjectProperties { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Configurations.Add(new ModuleConfiguration());
            modelBuilder.Configurations.Add(new ModuleVertexConfiguration());

            modelBuilder.Configurations.Add(new DatabaseObjectVertexConfiguration());
        }
    }
}
