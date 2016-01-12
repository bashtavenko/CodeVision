using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeVision.Dependencies
{
    /// <summary>
    /// Basically it is persistable digraph that has objects (symbols) as opposed to just integers. 
    /// </summary>
    /// <typeparam name="T">Anything that has Id property because Id is required for persistance.</typeparam>
    public class SymbolGraph<T> where T : Symbol
    {
        public IEnumerable<T> Symbols => SymbolTable.Keys.AsEnumerable();
        public Dictionary<T, int> SymbolTable { get; }
        public Dictionary<int, T> Keys { get; }
        public Digraph Digraph { get; }

        public SymbolGraph() : this (null, new Digraph())
        {            
        }

        public SymbolGraph(Memento<T[]> memento) : this(memento, new Digraph())
        {
        }

        public SymbolGraph(Memento<T[]> memento, Digraph g)
        {
            SymbolTable = new Dictionary<T, int>();
            Keys = new Dictionary<int, T>();

            if (memento != null)
            {
                SetMemento(memento);
            }
            Digraph = g;
        }

        public int AddSymbol(T symbol)
        {
            int vertexIndex = AddVertexInternal(symbol);
            Digraph.AddVertex(vertexIndex);
            return vertexIndex;
        }

        public void AddEdge(T fromSymbol, T toSymbol)
        {
            int vi = AddVertexInternal(fromSymbol);
            int wi = AddVertexInternal(toSymbol);
            Digraph.AddEdge(vi, wi);
        } 

        public List<T> GetDependencies (T symbol, DependencyDirection direction, DependencyLevel level)
        {
            if (!SymbolTable.ContainsKey(symbol))
            {
                throw new ArgumentException(nameof(symbol));
            }
            return GetDependencies(SymbolTable[symbol], direction, level);
        }
 
        public List<T> GetDependencies (int symbolId, DependencyDirection direction, DependencyLevel level)
        {
            if (symbolId < 0 || symbolId >= SymbolTable.Count)
            {
                throw new ArgumentException(nameof(symbolId));
            }
            Digraph g = direction == DependencyDirection.Downstream ? Digraph : Digraph.Reverse();
            IEnumerable<int> symbolIndexes;
            if (level == DependencyLevel.DirectOnly)
            {
                // That's easy
                symbolIndexes = g.GetAdjList(symbolId);
            }
            else
            {
                // Run BFS to find out
                var bfs = new DigraphBfs(g, symbolId);
                symbolIndexes = bfs.Preorder;
            }

            List<T> symbols = symbolIndexes
                .Select(i => Keys[i])
                .ToList();

            return symbols;
        }

        public Memento<T[]> CreateMemento()
        {            
            return new Memento<T[]>(SymbolTable.Keys.ToArray());
        }

        public void SetMemento(Memento<T[]> memento)
        {   
            if (memento == null)
            {
                throw new ArgumentNullException(nameof(memento));
            }
                     
            foreach (T symbol in memento.State)
            {
                AddKey(symbol);
            }
        }

        private int AddVertexInternal (T v)
        {
            int vertexIndex;
            if (!SymbolTable.ContainsKey(v))
            {
                vertexIndex = AddKey(v);
            }
            else
            {
                vertexIndex = SymbolTable[v];
            }
            return vertexIndex;
        }

        private int AddKey(T v)
        {
            int vertexIndex = SymbolTable.Count;
            v.Id = vertexIndex;
            SymbolTable.Add(v, vertexIndex);
            Keys.Add(vertexIndex, v);
            return vertexIndex;
        }
    }
}