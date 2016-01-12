using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using CodeVision.Dependencies.Nugets;
using CodeVision.Dependencies.SqlStorage;
using Dapper;
using NUnit.Framework;
using Package = CodeVision.Dependencies.Nugets.Package;
using Project = CodeVision.Dependencies.Nugets.Project;

namespace CodeVision.Tests
{
    [TestFixture]
    public class ProjectRepositoryTests
    {
        private ProjectRepository _repository;
        private SqlConnection _connection;
        private DependencyGraphContext _context;

        [TestFixtureSetUp]
        public void Setup()
        {
            var configuration = CodeVisionConfigurationSection.Load();
            var connectionString = configuration.DependencyGraphConnectionString;

            _context = new DependencyGraphContext(connectionString);
            var items = _context.DatabaseObjects.ToList();

            _repository = new ProjectRepository(connectionString);
            
            _connection = new SqlConnection(connectionString);
            _connection.Execute("delete from ProjectPackage;");
            _connection.Execute("delete from Project;");
            _connection.Execute("delete from Package;");
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _connection.Close();
            _context.Dispose();
            _repository.Dispose();
        }

        [Test]
        public void ProjectRepository_CanSaveProject()
        {
            var project = new Project
            {
                Name = "Console App",
                OutputKind = "Console",
                Platform = "Any",
                Packages = new List<Package>
                {
                    new Package { Name = "Nuget1", TargetFramework = "4.5", Version  = "1.0" },
                    new Package { Name = "Nuget2", TargetFramework = "4.5", Version  = "1.0" }
                }
            };

            _repository.SaveProject(project);

            Assert.That(GetTableRowCount("Project"), Is.EqualTo(1));
            Assert.That(GetTableRowCount("Package"), Is.EqualTo(2));
            Assert.That(GetTableRowCount("ProjectPackage"), Is.EqualTo(2));
            
            var anotherProjectThatUsesTheSameNuget = new Project
            {
                Name = "Some DLL",
                OutputKind = "Dll",
                Platform = "x86",
                Packages = new List<Package>
                {
                    new Package { Name = "Nuget1", TargetFramework = "4.5", Version  = "1.0" },
                }
            };

            _repository.SaveProject(anotherProjectThatUsesTheSameNuget);

            Assert.That(GetTableRowCount("Project"), Is.EqualTo(2));
            Assert.That(GetTableRowCount("Package"), Is.EqualTo(2));
            Assert.That(GetTableRowCount("ProjectPackage"), Is.EqualTo(3));

            var sameProjectNewNuget = new Project
            {
                Name = "Console App",
                OutputKind = "Console",
                Platform = "Any",
                Packages = new List<Package>
                {
                    new Package { Name = "Nuget3", TargetFramework = "4.6.1", Version  = "1.1" },
                }
            };

            _repository.SaveProject(sameProjectNewNuget);

            Assert.That(GetTableRowCount("Project"), Is.EqualTo(2));
            Assert.That(GetTableRowCount("Package"), Is.EqualTo(3));
            Assert.That(GetTableRowCount("ProjectPackage"), Is.EqualTo(4));
            
            var duplicateProject = new Project
            {
                Name = "Console App",
                OutputKind = "Console",
                Platform = "Any",
                Packages = new List<Package>
                {
                    new Package { Name = "Nuget3", TargetFramework = "4.6.1", Version  = "1.1" },
                }
            };

            _repository.SaveProject(duplicateProject);

            Assert.That(GetTableRowCount("Project"), Is.EqualTo(2));
            Assert.That(GetTableRowCount("Package"), Is.EqualTo(3));
            Assert.That(GetTableRowCount("ProjectPackage"), Is.EqualTo(4));
        }

        [Test]
        public void ProjectRepository_CanSearch()
        {
            _repository.SaveProject(
                new Project { Name = "Console App",
                    Packages = new List<Package>
                    {
                        new Package { Name = "Console"},
                        new Package { Name = "Console Test"},
                        new Package { Name = "Foo"},
                        new Package { Name = "Foo Test"},
                    }
                });

            Assert.That(_repository.GetPackages("Console").Count, Is.EqualTo(2));
        }

        protected int GetTableRowCount(string tableName)
        {
            return Convert.ToInt32(_connection.ExecuteScalar($"select count(*) from {tableName};"));
        }
    }
}