using CodeVision.CSharp.Semantic;
using NUnit.Framework;
using System.Linq;

namespace CodeVision.Tests
{
    [TestFixture]
    public class DependencyGraphTests
    {
        //       A
        //  B1   B2  B3   
        //  C1       
        private DependencyGraph _g;

        [TestFixtureSetUp]
        public void Setup()
        {
            _g = new DependencyGraph();
            _g.AddModule("A");
            _g.AddDependency("A", "B1");
            _g.AddDependency("A", "B2");
            _g.AddDependency("A", "B3");
            _g.AddDependency("B1", "C1");
        }

        [Test]
        public void DependencyGraph_Basic()
        {
            Assert.That(_g.Modules.Count, Is.EqualTo(5));

            var result = _g.GetDependencies("A", DependencyDirection.Downstream, DependencyLevels.DirectOnly);
            Assert.That(result.Count, Is.EqualTo(3));
            CollectionAssert.Contains(result, "B1");
            CollectionAssert.Contains(result, "B2");
            CollectionAssert.Contains(result, "B3");

            result = _g.GetDependencies("A", DependencyDirection.Downstream, DependencyLevels.Everything);
            Assert.That(result.Count, Is.EqualTo(4));
            CollectionAssert.Contains(result, "B1");
            CollectionAssert.Contains(result, "B2");
            CollectionAssert.Contains(result, "B3");
            CollectionAssert.Contains(result, "C1");

            result = _g.GetDependencies("B1", DependencyDirection.Downstream, DependencyLevels.Everything);
            Assert.That(result.Count, Is.EqualTo(1));
            CollectionAssert.Contains(result, "C1");

            result = _g.GetDependencies("C1", DependencyDirection.Upstream, DependencyLevels.DirectOnly);
            Assert.That(result.Count, Is.EqualTo(1));
            CollectionAssert.Contains(result, "B1");

            result = _g.GetDependencies("C1", DependencyDirection.Upstream, DependencyLevels.Everything);
            Assert.That(result.Count, Is.EqualTo(2));
            CollectionAssert.Contains(result, "B1");
            CollectionAssert.Contains(result, "A");

            result = _g.GetDependencies("A", DependencyDirection.Upstream, DependencyLevels.Everything);
            Assert.That(result.Count, Is.EqualTo(0));

            result = _g.GetDependencies("a", DependencyDirection.Downstream, DependencyLevels.DirectOnly);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public void DependencyGraph_Lookup()
        {
            var result = _g.GetModulesBeginsWith("B");
            Assert.That(result.Count, Is.EqualTo(3));

            result = _g.GetModulesBeginsWith("b");
            Assert.That(result.Count, Is.EqualTo(3));
        }

        [Test]
        public void DependencyGraph_Memento()
        {
            var memento = _g.CreateMemento();
            string[] st = memento.State;
            Assert.That(st.Count(), Is.EqualTo(5));

            var anotherGraph = new DependencyGraph();
            anotherGraph.SetMemento(memento);
            Assert.That(anotherGraph.Modules.Count, Is.EqualTo(5));
        }

        [Test]
        public void DependencyGraph_Dups()
        {
            var g = new DependencyGraph();
            g.AddModule("A");
            g.AddModule("A");
            Assert.That(g.Modules.Count(), Is.EqualTo(1));
        }
    }
}
