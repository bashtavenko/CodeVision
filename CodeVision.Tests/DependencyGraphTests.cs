using CodeVision.CSharp.Semantic;
using NUnit.Framework;
using System;
using System.Collections.Generic;
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
        private Module _A;
        private Module _B1;
        private Module _B2;
        private Module _B3;
        private Module _C1;

        [TestFixtureSetUp]
        public void Setup()
        {
            _g = new DependencyGraph();
            _A = new Module("A", "1");
            _B1 = new Module("B1", "1");
            _B2 = new Module("B2", "1");
            _B3 = new Module("B3", "1");
            _C1 = new Module("C1", "1");
            _g.AddModule(_A);
            _g.AddDependency(_A, _B1);
            _g.AddDependency(_A, _B2);
            _g.AddDependency(_A, _B3);
            _g.AddDependency(_B1, _C1);
        }

        [Test]
        public void DependencyGraph_Basic()
        {
            Assert.That(_g.Modules.Count, Is.EqualTo(5));

            var result = _g.GetDependencies(_A, DependencyDirection.Downstream, DependencyLevel.DirectOnly);
            Assert.That(result.Count, Is.EqualTo(3));
            Assert.IsNotNull(FindModule(result, "B1"));
            Assert.IsNotNull(FindModule(result, "B2"));
            Assert.IsNotNull(FindModule(result, "B3"));
            

            result = _g.GetDependencies(_A, DependencyDirection.Downstream, DependencyLevel.Everything);
            Assert.That(result.Count, Is.EqualTo(4));
            Assert.IsNotNull(FindModule(result, "B1"));
            Assert.IsNotNull(FindModule(result, "B2"));
            Assert.IsNotNull(FindModule(result, "B3"));
            Assert.IsNotNull(FindModule(result, "C1"));


            result = _g.GetDependencies(_B1, DependencyDirection.Downstream, DependencyLevel.Everything);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.IsNotNull(FindModule(result, "C1"));

            result = _g.GetDependencies(_C1, DependencyDirection.Upstream, DependencyLevel.DirectOnly);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.IsNotNull(FindModule(result, "B1"));

            result = _g.GetDependencies(_C1, DependencyDirection.Upstream, DependencyLevel.Everything);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.IsNotNull(FindModule(result, "B1"));
            Assert.IsNotNull(FindModule(result, "A"));
            
            result = _g.GetDependencies(_A, DependencyDirection.Upstream, DependencyLevel.Everything);
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
            Module[] st = memento.State;
            Assert.That(st.Count(), Is.EqualTo(5));

            var anotherGraph = new DependencyGraph();
            anotherGraph.SetMemento(memento);
            Assert.That(anotherGraph.Modules.Count, Is.EqualTo(5));
        }

        [Test]
        public void DependencyGraph_Dups()
        {
            var g = new DependencyGraph();
            g.AddModule(_A);
            g.AddModule(_A);
            Assert.That(g.Modules.Count(), Is.EqualTo(1));
        }
        
        [Test]
        public void DependencyGraph_OutOfRange()
        {
            Assert.Throws<ArgumentException>(() => _g.GetDependencies(-1, DependencyDirection.Downstream, DependencyLevel.DirectOnly));
            Assert.Throws<ArgumentException>(() => _g.GetDependencies(100, DependencyDirection.Downstream, DependencyLevel.DirectOnly));
        }

        private Module FindModule (IEnumerable<Module> list, string name)
        {
            return list.SingleOrDefault(s => s.Name == name);
        }
    }
}
