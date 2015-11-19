using CodeVision.Dependencies.Database;
using NUnit.Framework;

namespace CodeVision.Tests
{
    [TestFixture]
    public class DatabaseCollectorTests
    {
        private DatabaseCollector _databaseCollector;

        [TestFixtureSetUp]
        public void Setup()
        {
            var connectionString = CodeVisionConfigurationSection.Load().DependencyGraphConnectionString;
            _databaseCollector = new DatabaseCollector(connectionString);
        }

        [TearDown]
        public void TearDown()
        {
            _databaseCollector.Dispose();
        }

        [Test]
        public void DatabaseCollector_Collect()
        {
            var db = _databaseCollector.Collect("AdventureWorks2012");
            Assert.That(db.StoredProcedures.Count, Is.EqualTo(10));
            Assert.That(db.Tables.Count, Is.EqualTo(71));
        }
    }
}
