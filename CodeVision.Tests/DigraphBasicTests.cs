using System.Linq;

using CodeVision.CSharp.Semantic;

using NUnit.Framework;

namespace CodeVision.Tests
{
    [TestFixture]
    public class DigraphBasicTests
    {
        [Test]
        public void Digraph_BasicOperations()
        {
            var digraph = new Digraph();
            digraph.AddVertex(0);
            digraph.AddVertex(1);
            digraph.AddVertex(2);
            digraph.AddEdge(0, 1);
            digraph.AddEdge(0, 2);

            Assert.That(digraph.V, Is.EqualTo(3));
            Assert.That(digraph.E, Is.EqualTo(2));
            Assert.That(digraph.GetAdjList(0).Count, Is.EqualTo(2));
            Assert.That(digraph.GetAdjList(1).Count, Is.EqualTo(0));
        }

        [Test]
        public void Digraph_BasicOperations_2()
        {
            var digraph = new Digraph();
            digraph.AddVertex(0);
            digraph.AddEdge(0, 1);
            digraph.AddEdge(0, 2);

            Assert.That(digraph.V, Is.EqualTo(3));
            Assert.That(digraph.E, Is.EqualTo(2));
            Assert.That(digraph.GetAdjList(0).Count, Is.EqualTo(2));
        }

        [Test]
        public void Digraph_Memento()
        {
            var digraph = new Digraph();
            digraph.AddVertex(0);
            digraph.AddVertex(1);
            digraph.AddVertex(2);
            digraph.AddEdge(0, 1);
            digraph.AddEdge(0, 2);

            var memento = digraph.CreateMemento();
            var state = memento.State;
            Assert.That(state.Count(), Is.EqualTo(3));
            Assert.That(state[0].Count(), Is.EqualTo(2));
            Assert.That(state[0][0], Is.EqualTo(1));
            Assert.That(state[0][1], Is.EqualTo(2));
            Assert.That(state[1].Count(), Is.EqualTo(0));
            Assert.That(state[2].Count(), Is.EqualTo(0));

            var anotherDigraph = new Digraph();
            anotherDigraph.SetMemento(memento);
            Assert.That(digraph.V, Is.EqualTo(3));
            Assert.That(digraph.E, Is.EqualTo(2));
            Assert.That(digraph.GetAdjList(0).Count, Is.EqualTo(2));
        }

        [Test]
        public void Digraph_Dups()
        {
            var digraph = new Digraph();
            digraph.AddVertex(0);
            digraph.AddVertex(1);
            digraph.AddEdge(0, 1);

            digraph.AddVertex(0);
            Assert.That(digraph.V, Is.EqualTo(2));

            digraph.AddEdge(0, 2);
            Assert.That(digraph.E, Is.EqualTo(2));
            Assert.That(digraph.GetAdjList(0).Count, Is.EqualTo(2));
        }
    }        
}
