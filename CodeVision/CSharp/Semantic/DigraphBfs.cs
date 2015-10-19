using System.Collections.Generic;
using System.Linq;

namespace CodeVision.CSharp.Semantic
{
    public class DigraphBfs
    {
        public IEnumerable<int> Preorder { get { return _preorder; }}

        private readonly bool[] _marked;
        private readonly int _s;
        private readonly Queue<int> _preorder;

        public DigraphBfs(Digraph g, int s)
        {
            _s = s;
            _marked = new bool[g.V];
            _preorder = new Queue<int>();
            Bfs(g, s);
        }
        
        private void Bfs(Digraph g, int s)
        {
            var q = new Queue<int>();
            _marked[s] = true;
            q.Enqueue(s);
            while (q.Any())
            {
                int v = q.Dequeue();
                foreach (int w in g.GetAdjList(v))
                {
                    if (!_marked[w])
                    {
                        _marked[w] = true;
                        q.Enqueue(w);
                        _preorder.Enqueue(w);
                    }
                }
            }
        }
    }
}
