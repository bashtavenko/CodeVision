using NUnit.Framework;
using System.Linq;
using CodeVision.Dependencies;
using CodeVision.Dependencies.Modules;

namespace CodeVision.Tests
{
    [TestFixture]
    public class DependencyGraphRepositoryTests
    {
        private string _connectionString;
        
        private ModulesGraphRepository _repository;
        private ModulesGraph _g;

        [TestFixtureSetUp]
        public void Setup()
        {
            var configuration = CodeVisionConfigurationSection.Load();
            _connectionString = configuration.DependencyGraphConnectionString;
            _repository = new ModulesGraphRepository(_connectionString);

            //       A
            //  B1   B2  B3   
            //  C1    
            _g = new ModulesGraph();
            var moduleA = new Module("A", "1");
            var moduleB1 = new Module("B1", "1");
            var moduleB2 = new Module("B2", "1");
            var moduleB3 = new Module("B3", "1");
            var moduleC1 = new Module("C1", "1");
            _g.AddModule(moduleA);
            _g.AddDependency(moduleA, moduleB1);
            _g.AddDependency(moduleA, moduleB2);
            _g.AddDependency(moduleA, moduleB3);
            _g.AddDependency(moduleB1, moduleC1);
        }

        [Test]
        public void DependencyGraphRepository_CanSaveAndLoad()
        {
            _repository.SaveState(_g);

            var anothergraph = _repository.LoadState();
            Assert.That(anothergraph.Modules.Count, Is.EqualTo(5));
            Assert.That(anothergraph.Digraph.V, Is.EqualTo(5));
            Assert.That(anothergraph.Digraph.E, Is.EqualTo(4));

            var moduleA = anothergraph.GetModulesBeginsWith("A").Single(s => s.Name == "A");
            var moduleB1 = anothergraph.GetModulesBeginsWith("B1").Single(s => s.Name == "B1");
            var moduleB2 = anothergraph.GetModulesBeginsWith("B2").Single(s => s.Name == "B2");
            var moduleB3 = anothergraph.GetModulesBeginsWith("B3").Single(s => s.Name == "B3");

            var result = anothergraph.GetDependencies(moduleA.Id.Value, DependencyDirection.Downstream, DependencyLevel.DirectOnly);
            Assert.That(result.Count, Is.EqualTo(3));
            Assert.IsNotNull(result.SingleOrDefault(s => s.Name == "B1"));
            Assert.IsNotNull(result.SingleOrDefault(s => s.Name == "B2"));
            Assert.IsNotNull(result.SingleOrDefault(s => s.Name == "B3"));            
        }
    }
}
