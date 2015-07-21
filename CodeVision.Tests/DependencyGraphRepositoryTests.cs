using CodeVision.CSharp.Semantic;
using NUnit.Framework;
using System.Linq;

namespace CodeVision.Tests
{
    [TestFixture]
    public class DependencyGraphRepositoryTests
    {
        private string _connectionString;
        
        private DependencyGraphRepository _repository;
        private DependencyGraph _g;

        [TestFixtureSetUp]
        public void Setup()
        {
            var configuration = CodeVisionConfigurationSection.Load();
            _connectionString = configuration.DependencyGraphConnectionString;
            _repository = new DependencyGraphRepository(_connectionString);

            //       A
            //  B1   B2  B3   
            //  C1    
            _g = new DependencyGraph();
            _g.AddModule("A");
            _g.AddDependency("A", "B1");
            _g.AddDependency("A", "B2");
            _g.AddDependency("A", "B3");
            _g.AddDependency("B1", "C1");
        }

        [Test]
        public void DependencyGraphRepository_CanSaveAndLoad()
        {
            _repository.SaveState(_g);

            var anothergraph = _repository.LoadState();
            Assert.That(anothergraph.Modules.Count, Is.EqualTo(5));
            Assert.That(anothergraph.Digraph.V, Is.EqualTo(5));
            Assert.That(anothergraph.Digraph.E, Is.EqualTo(4));

            var result = anothergraph.GetDependencies("A", DependencyDirection.Downstream, DependencyLevels.DirectOnly);
            Assert.That(result.Count, Is.EqualTo(3));
            CollectionAssert.Contains(result, "B1");
            CollectionAssert.Contains(result, "B2");
            CollectionAssert.Contains(result, "B3");
            
        }
    }
}
