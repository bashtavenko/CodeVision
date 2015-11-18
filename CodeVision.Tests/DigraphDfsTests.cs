using System.Linq;
using NUnit.Framework;
using CodeVision.Dependencies;

namespace CodeVision.Tests
{
    [TestFixture]
    public class DigraphDfsTests
    {
        [Test]
        //    4
        // 0  3
        // 2  
        // 1               
        public void DigraphDfs_Reachability()
        {
            // Arrange
            var digraph = new Digraph();            
            digraph.AddVertex(4);
            digraph.AddEdge(4, 0);
            digraph.AddEdge(4, 3);
            digraph.AddEdge(0, 2);
            digraph.AddEdge(2, 1);

            var dfs = new DigraphDfs(digraph, 4);
            Assert.That(dfs.ReachableVertices.Count(), Is.EqualTo(4));

            dfs = new DigraphDfs(digraph, 0);
            Assert.That(dfs.ReachableVertices.Count(), Is.EqualTo(2));
        }
    }
}
