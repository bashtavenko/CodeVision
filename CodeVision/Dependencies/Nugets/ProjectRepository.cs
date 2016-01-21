using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AutoMapper;
using AutoMapper.Mappers;
using CodeVision.Dependencies.SqlStorage;

namespace CodeVision.Dependencies.Nugets
{
    public class ProjectRepository : IDisposable
    {
        private readonly MappingEngine _engine;
        private readonly DependencyGraphContext _context;

        public ProjectRepository(string connectionString)
        {
            var store = new ConfigurationStore(new TypeMapFactory(), MapperRegistry.Mappers);
            store.AssertConfigurationIsValid();
            _engine = new MappingEngine(store);
            CreateMaps(store);
            _context = new DependencyGraphContext(connectionString);
        }
        
        public void SaveProject(Project project)
        {
            SqlStorage.Project sqlProject = _engine.Map<SqlStorage.Project>(project);
            sqlProject = GetOrAddEntity(_context.Projects, sqlProject, m => m.Name == project.Name);
            foreach (var nugetPackage in project.Packages)
            {
                SqlStorage.Package sqlPackage = _engine.Map<SqlStorage.Package>(nugetPackage);
                sqlPackage = GetOrAddEntity(_context.Packages, sqlPackage, m => m.Name == nugetPackage.Name);
                sqlPackage.Projects.Add(sqlProject);
            }
            _context.SaveChanges();
        }

        public IList<SqlStorage.Package> GetPackages(string name)
        {
            return _context.Packages
                .Where(w => w.Name.StartsWith(name))
                .OrderBy(o => o.Name)
                .ThenBy(o => o.Version)
                .Take(10)
                .ToList();
        }

        public IList<SqlStorage.Project> GetProjects(int packageId)
        {
            return _context.Projects.Where(w => w.Packages.Any(a => a.PackageId == packageId)).ToList();
        }

        public DependencyMatrix GetDependencyMatrix()
        {
            return GetDependencyMatrix(null, null);
        }

        public DependencyMatrix GetDependencyMatrix(string projectStartsWith, string packageStartsWith)
        {
            var dependencyMatrix = new DependencyMatrix();

            var packages = _context.Packages.AsQueryable();
            if (!string.IsNullOrEmpty(packageStartsWith))
            {
                packages = packages.Where(s => s.Name.StartsWith(packageStartsWith));
            }

            foreach (var package in packages)
            {
                var projects = package.Projects.AsQueryable();
                if (!string.IsNullOrEmpty(projectStartsWith))
                {
                    // Here we need StringComparison.InvariantCultureIgnoreCase because this goes against in-memory collection and
                    // not SQL which is case-sensitive by default.
                    projects = projects.Where(s => s.Name.StartsWith(projectStartsWith, StringComparison.InvariantCultureIgnoreCase));
                }
                foreach (var project in projects) // This requires MARS MultipleActiveResultSets=True in connection string
                {
                    dependencyMatrix.AddDependency($"{package.Name} {package.Version}", project.Name);
                }
            }
            dependencyMatrix.Sort();
            return dependencyMatrix;
        }

        ~ProjectRepository()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context?.Dispose();
            }
        }


        /// <summary>
        /// Gets existing entity from db or adds one. 
        /// </summary>
        /// <typeparam name="T">Project or Package</typeparam>
        /// <param name="list">List of these entities from the context</param>
        /// <param name="src">Entity itself</param>
        /// <param name="where">Where clause used to search for this entity</param>
        /// <returns>Newly added entity of entity from database which may have an id set.</returns>
        private static T GetOrAddEntity<T>(DbSet<T> list, T src, Func<T, bool> where) where T : class
        {
            var srcFromDb = list.Local.FirstOrDefault(where); // .Local means it exists in context but not in db yet.

            if (srcFromDb == null)
            {
                srcFromDb = list.FirstOrDefault(where); // Maybe it was saved before
                if (srcFromDb == null)
                {
                    list.Add(src); // Brand new
                    return src;
                }
                else
                {
                    return srcFromDb; // From db, has an id
                }
            }
            else
            {
                return srcFromDb; // Local copy without id.
            }
        }

        private void CreateMaps(ConfigurationStore store)
        {
            store.CreateMap<Project, SqlStorage.Project>()
                .ForMember(m => m.Packages, opt => opt.Ignore());

            store.CreateMap<Package, SqlStorage.Package>()
                .ForMember(m => m.Projects, opt => opt.Ignore());
        }
    }
}