using System.Collections.Generic;
using System.Linq;
using CodeVision.Dependencies;
using CodeVision.Dependencies.Database;
using Moq;
using NUnit.Framework;

namespace CodeVision.Tests
{
    [TestFixture]
    public class DatabaseObjectsGraphCollectorTests : DatabaseTest
    {
        private DatabaseObjectGraphCollector _collector;
        private Mock<ILogger> _loggerMock;
        private DatabaseObjectsGraphRepository _repository;

        [TestFixtureSetUp]
        public void Setup()
        {
            WipeoutAll();
            var configuration = CodeVisionConfigurationSection.Load();
            _loggerMock = new Mock<ILogger>();
            _collector = new DatabaseObjectGraphCollector(configuration.TargetDatabaseConnectionString, ConnectionString, _loggerMock.Object);
            _repository = new DatabaseObjectsGraphRepository(ConnectionString);
        }

        [Test]
        public void DatabaseObjectCollector_AdventureWorks()
        {
            _collector.CollectDependencies(new List<string> { "AdventureWorks2012" });
            Assert.That(GetTableRowCount("DatabaseObject"), Is.EqualTo(568));
            Assert.That(GetTableRowCount("DatabaseObjectsGraph"), Is.EqualTo(568));

            var graph = _repository.LoadState();
            var result = graph.GetDependencies(new DatabaseObject(DatabaseObjectType.Table, "AdventureWorks2012.Person.Person"), DependencyDirection.Downstream, DependencyLevel.DirectOnly);
            var tables = result.Where(w => w.ObjectType == DatabaseObjectType.Table).ToList();
            Assert.That(tables.Count, Is.EqualTo(7));
            
            result = graph.GetDependencies(new DatabaseObject(DatabaseObjectType.Table, "AdventureWorks2012.Person.Person"), DependencyDirection.Upstream, DependencyLevel.DirectOnly);
            tables = result.Where(w => w.ObjectType == DatabaseObjectType.Table).ToList();
            Assert.That(tables.Count, Is.EqualTo(1));
        }
    }
}