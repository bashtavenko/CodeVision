using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using CodeVision.Dependencies.SqlStorage.Maps;

namespace CodeVision.Dependencies.SqlStorage
{
    public class DependencyGraphContext : DbContext
    {
        public DependencyGraphContext(string connectionString, IDatabaseInitializer<DependencyGraphContext> initializer)
            : base(connectionString)
        {
            System.Data.Entity.Database.SetInitializer(initializer);
        }

        public DependencyGraphContext(string connectionString)
            : base(connectionString)
        {
            System.Data.Entity.Database.SetInitializer<DependencyGraphContext>(new DependencyGraphCreateDatabaseIfNotExists());
        }

        public DependencyGraphContext() 
        {
            System.Data.Entity.Database.SetInitializer<DependencyGraphContext>(null);
        }

        public DbSet<Module> Modules { get; set; }
        public DbSet<ModuleVertex> ModuleVertices { get; set; }
        public DbSet<DatabaseObjectVertex> DatabaseObjectVertices { get; set; }

        public DbSet<DatabaseObjectTypeLookup> DatabaseObjectTypeLookups { get; set; }
        public DbSet<DatabaseObjectPropertyTypeLookup> DatabaseObjectPropertyTypeLookups { get; set; }
        public DbSet<DatabaseObject> DatabaseObjects { get; set; }
        public DbSet<DatabaseObjectProperty> DatabaseObjectProperties { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Configurations.Add(new ModuleConfiguration());
            modelBuilder.Configurations.Add(new ModuleVertexConfiguration());

            modelBuilder.Configurations.Add(new DatabaseObjectVertexConfiguration());
            modelBuilder.Configurations.Add(new DatabaseObjectTypeLookupConfiguration());
            modelBuilder.Configurations.Add(new DatabaseObjectPropertyTypeLookupConfiguration());
            modelBuilder.Configurations.Add(new DatabaseObjectConfiguration());
            modelBuilder.Configurations.Add(new DatabaseObjectPropertyConfiguration());
        }
    }
}
