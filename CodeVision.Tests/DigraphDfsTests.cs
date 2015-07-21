using System.Linq;
using NUnit.Framework;
using CodeVision.CSharp.Semantic;

namespace CodeVision.Tests
{
    [TestFixture]
    public class DigraphDfsTests
    {
        [Test]
        //    1
        // 0  2 3
        //      4        
        public void DigraphDfs_Reachability()
        {
            // Arrange
            var digraph = new Digraph();            
            digraph.AddVertex(0);
            digraph.AddEdge(0, 1);
            digraph.AddEdge(0, 2);
            digraph.AddEdge(2, 3);
            digraph.AddEdge(2, 4);

            var dfs = new DigraphDfs(digraph, 0);
            Assert.That(dfs.ReachableVertices.Count, Is.EqualTo(4));
            CollectionAssert.Contains(dfs.ReachableVertices, 1);
            CollectionAssert.Contains(dfs.ReachableVertices, 2);
            CollectionAssert.Contains(dfs.ReachableVertices, 3);
            CollectionAssert.Contains(dfs.ReachableVertices, 4);

            dfs = new DigraphDfs(digraph, 2);
            Assert.That(dfs.ReachableVertices.Count, Is.EqualTo(2));            
            CollectionAssert.Contains(dfs.ReachableVertices, 3);
            CollectionAssert.Contains(dfs.ReachableVertices, 4);
        }
    }
}
