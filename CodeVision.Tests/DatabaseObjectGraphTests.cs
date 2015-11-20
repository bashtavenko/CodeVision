using System;
using System.Collections.Generic;
using System.Linq;
using CodeVision.Dependencies;
using CodeVision.Dependencies.Database;
using NUnit.Framework;

namespace CodeVision.Tests
{
    [TestFixture]
    public class DatabaseObjectGraphTests
    {
        //       D1
        //  T1   T2  S1   
        //  C1       
        private DatabaseObjectsGraph _g;
        private DatabaseObject _D1;
        private DatabaseObject _T1;
        private DatabaseObject _T2;
        private DatabaseObject _S1;
        private DatabaseObject _C1;

        [TestFixtureSetUp]
        public void Setup()
        {
            _g = new DatabaseObjectsGraph();
            _D1 = new DatabaseObject(DatabaseObjectType.Database, "D1");
            _T1 = new DatabaseObject(DatabaseObjectType.Table, "T1");
            _T2 = new DatabaseObject(DatabaseObjectType.Table, "T2");
            _S1 = new DatabaseObject(DatabaseObjectType.StoredProcedure, "S1");
            _C1 = new DatabaseObject(DatabaseObjectType.Column, "C1");
            
            _g.AddDatabaseObject(_D1);
            _g.AddDependency(_D1, _T1);
            _g.AddDependency(_D1, _T2);
            _g.AddDependency(_D1, _S1);
            _g.AddDependency(_T1, _C1);

            _D1.Properties.Add(new RelevantToFinancialReportingProperty());
            _D1.Properties.Add(new CommentProperty("Comment"));
        }

        [Test]
        public void DatabaseObjectsGraph_Basic()
        {
            Assert.That(_g.ObjectsCount, Is.EqualTo(5));

            var result = _g.GetDependencies(_D1, DependencyDirection.Downstream, DependencyLevel.DirectOnly);
            Assert.That(result.Count, Is.EqualTo(3));
            Assert.IsNotNull(FindObject(result, "T1"));
            Assert.IsNotNull(FindObject(result, "T2"));
            Assert.IsNotNull(FindObject(result, "S1"));

            result = _g.GetDependencies(_D1, DependencyDirection.Downstream, DependencyLevel.Everything);
            Assert.That(result.Count, Is.EqualTo(4));
            Assert.IsNotNull(FindObject(result, "T1"));
            Assert.IsNotNull(FindObject(result, "T2"));
            Assert.IsNotNull(FindObject(result, "S1"));
            Assert.IsNotNull(FindObject(result, "C1"));

            result = _g.GetDependencies(_T1, DependencyDirection.Downstream, DependencyLevel.Everything);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.IsNotNull(FindObject(result, "C1"));

            result = _g.GetDependencies(_C1, DependencyDirection.Upstream, DependencyLevel.DirectOnly);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.IsNotNull(FindObject(result, "T1"));

            result = _g.GetDependencies(_C1, DependencyDirection.Upstream, DependencyLevel.Everything);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.IsNotNull(FindObject(result, "T1"));
            Assert.IsNotNull(FindObject(result, "D1"));
            
            result = _g.GetDependencies(_D1, DependencyDirection.Upstream, DependencyLevel.Everything);
            Assert.That(result.Count, Is.EqualTo(0));                       
        }

        [Test]
        public void DatabaseObjectsGraph_Lookup()
        {
            var result = _g.GetDatabaseObjectsBeginsWith("T");
            Assert.That(result.Count, Is.EqualTo(2));

            result = _g.GetDatabaseObjectsBeginsWith("C");
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public void DatabaseObjectsGraph_Memento()
        {
            var memento = _g.CreateMemento();
            DatabaseObject[] st = memento.State;
            Assert.That(st.Count(), Is.EqualTo(5));

            var anotherGraph = new DatabaseObjectsGraph();
            anotherGraph.SetMemento(memento);
            Assert.That(anotherGraph.ObjectsCount, Is.EqualTo(5));
            // This only tests objects part, and not the graph which is not initialized yet.
        }

        [Test]
        public void DatabaseObjectsGraph_Dups()
        {
            var g = new DatabaseObjectsGraph();
            g.AddDatabaseObject(_D1);
            g.AddDatabaseObject(_D1);
            Assert.That(g.ObjectsCount, Is.EqualTo(1));
        }
        
        [Test]
        public void DependencyGraph_OutOfRange()
        {
            Assert.Throws<ArgumentException>(() => _g.GetDependencies(-1, DependencyDirection.Downstream, DependencyLevel.DirectOnly));
            Assert.Throws<ArgumentException>(() => _g.GetDependencies(100, DependencyDirection.Downstream, DependencyLevel.DirectOnly));
        }

        private DatabaseObject FindObject (IEnumerable<DatabaseObject> list, string name)
        {
            return list.SingleOrDefault(s => s.FullyQualifiedName == name);
        }
    }
}