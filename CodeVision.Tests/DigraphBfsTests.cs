using System.Linq;
using NUnit.Framework;
using CodeVision.CSharp.Semantic;

namespace CodeVision.Tests
{
    [TestFixture]
    public class DigraphBfsTests
    {
        [Test]
        public void DigraphBfs_Reachability()
        {
            // Arrange
            // See TinyGraph.png
            var g = new Digraph();
            g = new Digraph();
            g.AddVertex(0);
            g.AddEdge(0, 5);
            g.AddEdge(0, 1);
            g.AddEdge(0, 6);
            g.AddVertex(2);
            g.AddEdge(2, 0);
            g.AddEdge(2, 3);
            g.AddEdge(3, 5);
            g.AddEdge(5, 4);
            g.AddEdge(6, 4);
            g.AddEdge(6, 9);
            g.AddEdge(7, 6);
            g.AddEdge(8, 7);
            g.AddEdge(9, 11);
            g.AddEdge(9, 10);
            g.AddEdge(9, 12);
            g.AddEdge(11, 12);
            
            var bfs = new DigraphBfs(g, 0);
            CollectionAssert.AreEqual(new int[] { 5, 1, 6, 4, 9, 11, 10, 12 }, bfs.Preorder);
            
            bfs = new DigraphBfs(g, 2);
            CollectionAssert.AreEqual(new int[] { 0, 3, 5, 1, 6, 4, 9, 11, 10, 12 }, bfs.Preorder);

            bfs = new DigraphBfs(g, 9);
            CollectionAssert.AreEqual(new int[] { 11, 10, 12 }, bfs.Preorder);
        }
    }
}
