using System.Linq;
using CodeVision.Dependencies;
using NUnit.Framework;

namespace CodeVision.Tests
{
    [TestFixture]
    public class DependencyMatrixTests
    {
        [Test]
        public void DependencyMatrix_Basic()
        {
            //    a   b  c
            // d  x
            // e         x
            // f      x
            var dm = new DependencyMatrix();
            dm.AddDependency("d", "a");
            dm.AddDependency("e", "c");
            dm.AddDependency("f", "b");
            dm.Sort();

            CollectionAssert.AreEqual(dm.Rows.Select(s => s.Value), new string[] {"d", "e", "f"});
            CollectionAssert.AreEqual(dm.Columns.Select(s => s.Value), new string[] {"a", "b", "c"}); 
            Assert.IsTrue(dm.Matrix[dm.Rows[0].Id, dm.Columns[0].Id]);
            Assert.IsFalse(dm.Matrix[dm.Rows[0].Id, dm.Columns[1].Id]);
            Assert.IsFalse(dm.Matrix[dm.Rows[0].Id, dm.Columns[2].Id]);
            Assert.IsFalse(dm.Matrix[dm.Rows[1].Id, dm.Columns[0].Id]);
            Assert.IsFalse(dm.Matrix[dm.Rows[1].Id, dm.Columns[1].Id]);
            Assert.IsTrue(dm.Matrix[dm.Rows[1].Id, dm.Columns[2].Id]);
            Assert.IsFalse(dm.Matrix[dm.Rows[2].Id, dm.Columns[0].Id]);
            Assert.IsTrue(dm.Matrix[dm.Rows[2].Id, dm.Columns[1].Id]);
            Assert.IsFalse(dm.Matrix[dm.Rows[2].Id, dm.Columns[2].Id]);
        }

        [Test]
        public void DependencyMatrix_Empty()
        {
            var dm = new DependencyMatrix();
            CollectionAssert.IsEmpty(dm.Matrix);
        }
    }
}