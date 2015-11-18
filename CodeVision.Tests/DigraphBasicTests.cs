using System.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using CodeVision.Dependencies;

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
        public void Digraph_OutOfRange()
        {
            var digraph = new Digraph();
            digraph.AddVertex(0);
            digraph.AddEdge(0, 1);
            digraph.AddEdge(0, 2);

            Assert.Throws<ArgumentException>(() => digraph.GetAdjList(-1));
            Assert.Throws<ArgumentException>(() => digraph.GetAdjList(5));
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
        public void Digraph_VertexWithGap()
        {
            var adj = new HashSet<int>[5];
            
            var digraph = new Digraph();
            digraph.AddVertex(101);

            // That is rather strange, we have 101 phantom vertices
            Assert.That(digraph.V, Is.EqualTo(102)); 
            var list = digraph.GetAdjList(0);
            Assert.IsEmpty(list);
        }

        [Test]
        public void Digraph_Memento_FromGraphWithGaps()
        {
            var digraph = new Digraph();
            digraph.AddVertex(1);
            int[][] adjacencyList = digraph.CreateMemento().State;

            Assert.That(adjacencyList.Length, Is.EqualTo(2));
            Assert.IsEmpty(adjacencyList[0]);
            Assert.IsEmpty(adjacencyList[1]);
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

        [Test]
        public void Digraph_RemoveEdge()
        {
            var digraph = new Digraph();
            digraph.AddVertex(0);
            digraph.AddVertex(1);
            digraph.AddVertex(2);
            digraph.AddEdge(0, 1);
            digraph.AddEdge(0, 2);

            digraph.RemoveEdge(0, 1);

            Assert.That(digraph.V, Is.EqualTo(3));
            Assert.That(digraph.E, Is.EqualTo(1));
            Assert.That(digraph.GetAdjList(0).Count(), Is.EqualTo(1));
        }
    }        
}
