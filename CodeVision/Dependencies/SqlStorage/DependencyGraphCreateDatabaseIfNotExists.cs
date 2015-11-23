using System.Data.Entity;

namespace CodeVision.Dependencies.SqlStorage
{
    public class DependencyGraphCreateDatabaseIfNotExists : CreateDatabaseIfNotExists<DependencyGraphContext>
    {
        protected override void Seed(DependencyGraphContext context)
        {
            new DependencyGraphSeeder(context).Seed();
        }
    }
}
