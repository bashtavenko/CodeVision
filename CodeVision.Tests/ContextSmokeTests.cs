using System.Linq;
using CodeVision.Dependencies.SqlStorage;
using NUnit.Framework;

namespace CodeVision.Tests
{
    [TestFixture]
    public class ContextSmokeTests
    {
        private DependencyGraphContext _context;

        [TestFixtureSetUp]
        public void Setup()
        {
            var configuration = CodeVisionConfigurationSection.Load();
            var connectionString = configuration.DependencyGraphConnectionString;
            _context = new DependencyGraphContext(connectionString);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public void Context_SmokeTest()
        {
            var items = _context.DatabaseObjects.ToList();
        }
    }
}