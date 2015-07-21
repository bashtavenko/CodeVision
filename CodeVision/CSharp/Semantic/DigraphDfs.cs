using System.Collections.Generic;
using System.Linq;

namespace CodeVision.CSharp.Semantic
{
    public class DigraphDfs
    {
        private bool[] _marked;
        private readonly int _s;

        public IEnumerable<int> ReachableVertices
        {
            get
            {
                return _marked                    
                    .Select((value, index) => new { Value = value, Index = index })
                    .Where (w => w.Value == true && w.Index != _s)
                    .Select(s => s.Index)
                    .ToList();
            }
        }

        public DigraphDfs(Digraph g, int s)
        {
            _s = s;
            _marked = new bool[g.V];
            Dfs(g, s);
        }
        
        private void Dfs(Digraph g, int v)
        {
            _marked[v] = true;
            foreach (var w in g.GetAdjList(v))
            {
                if (!_marked[w])
                {
                    Dfs(g, w);
                }
            }
        }
    }
}
