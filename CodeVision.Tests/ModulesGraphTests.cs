using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using CodeVision.Dependencies;
using CodeVision.Dependencies.Modules;

namespace CodeVision.Tests
{
    [TestFixture]
    public class ModulesGraphTests
    {
        //       A
        //  B1   B2  B3   
        //  C1       
        private ModulesGraph _g;
        private Module _a;
        private Module _b1;
        private Module _b2;
        private Module _b3;
        private Module _c1;

        [TestFixtureSetUp]
        public void Setup()
        {
            _g = new ModulesGraph();
            _a = new Module("A", "1");
            _b1 = new Module("B1", "1");
            _b2 = new Module("B2", "1");
            _b3 = new Module("B3", "1");
            _c1 = new Module("C1", "1");
            _g.AddModule(_a);
            _g.AddDependency(_a, _b1);
            _g.AddDependency(_a, _b2);
            _g.AddDependency(_a, _b3);
            _g.AddDependency(_b1, _c1);
        }

        [Test]
        public void ModulesGraph_Basic()
        {
            Assert.That(_g.Modules.Count, Is.EqualTo(5));

            var result = _g.GetDependencies(_a, DependencyDirection.Downstream, DependencyLevel.DirectOnly);
            Assert.That(result.Count, Is.EqualTo(3));
            Assert.IsNotNull(FindModule(result, "B1"));
            Assert.IsNotNull(FindModule(result, "B2"));
            Assert.IsNotNull(FindModule(result, "B3"));
            

            result = _g.GetDependencies(_a, DependencyDirection.Downstream, DependencyLevel.Everything);
            Assert.That(result.Count, Is.EqualTo(4));
            Assert.IsNotNull(FindModule(result, "B1"));
            Assert.IsNotNull(FindModule(result, "B2"));
            Assert.IsNotNull(FindModule(result, "B3"));
            Assert.IsNotNull(FindModule(result, "C1"));


            result = _g.GetDependencies(_b1, DependencyDirection.Downstream, DependencyLevel.Everything);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.IsNotNull(FindModule(result, "C1"));

            result = _g.GetDependencies(_c1, DependencyDirection.Upstream, DependencyLevel.DirectOnly);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.IsNotNull(FindModule(result, "B1"));

            result = _g.GetDependencies(_c1, DependencyDirection.Upstream, DependencyLevel.Everything);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.IsNotNull(FindModule(result, "B1"));
            Assert.IsNotNull(FindModule(result, "A"));
            
            result = _g.GetDependencies(_a, DependencyDirection.Upstream, DependencyLevel.Everything);
            Assert.That(result.Count, Is.EqualTo(0));                       
        }

        [Test]
        public void ModulesGraph_Lookup()
        {
            var result = _g.GetModulesBeginsWith("B");
            Assert.That(result.Count, Is.EqualTo(3));

            result = _g.GetModulesBeginsWith("b");
            Assert.That(result.Count, Is.EqualTo(3));
        }

        [Test]
        public void ModulesGraph_Memento()
        {
            var memento = _g.CreateMemento();
            Module[] st = memento.State;
            Assert.That(st.Count(), Is.EqualTo(5));

            var anotherGraph = new ModulesGraph();
            anotherGraph.SetMemento(memento);
            Assert.That(anotherGraph.Modules.Count, Is.EqualTo(5));
        }

        [Test]
        public void ModulesGraph_Dups()
        {
            var g = new ModulesGraph();
            g.AddModule(_a);
            g.AddModule(_a);
            Assert.That(g.Modules.Count(), Is.EqualTo(1));
        }
        
        [Test]
        public void ModulesGraph_OutOfRange()
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
