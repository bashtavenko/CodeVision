using System;
using System.Collections;
using System.Collections.Generic;

namespace CodeVision.Model
{
    public class ReadOnlyHitCollection : IList<Hit>, IList
    {
        private readonly IList<Hit> _hits;
        private readonly int _totalHits;

        public ReadOnlyHitCollection(IList<Hit> hits, int totalHits)
        {
            if (hits == null)
            {
                throw new NullReferenceException("Must have hits");
            }
            _hits = hits;
            _totalHits = totalHits;
        }

        public int TotalHits { get { return _totalHits; }  }

        public IEnumerator<Hit> GetEnumerator()
        {
            return _hits.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(Hit item)
        {
            throw new NotSupportedException();
        }

        public int Add(object value)
        {
            throw new NotSupportedException();
        }

        public bool Contains(object value)
        {
            return _hits.Contains(value as Hit);
        }

        void IList.Clear()
        {
            throw new NotSupportedException();
        }

        public int IndexOf(object value)
        {
            return _hits.IndexOf(value as Hit);
        }

        public void Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        public void Remove(object value)
        {
            throw new NotSupportedException();
        }

        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        object IList.this[int index]
        {
            get { return _hits[index]; }
            set { throw new NotSupportedException(); }
        }

        public bool IsReadOnly { get { return true; } }
        public bool IsFixedSize { get { return true; } }

        void ICollection<Hit>.Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(Hit item)
        {
            return _hits.Contains(item);
        }

        public void CopyTo(Hit[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public bool Remove(Hit item)
        {
            throw new NotSupportedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotSupportedException();
        }

        public int Count { get { return _hits.Count; } }

        public object SyncRoot { get; private set; }

        public bool IsSynchronized { get; private set; }

        public int IndexOf(Hit item)
        {
            return _hits.IndexOf(item);
        }

        public void Insert(int index, Hit item)
        {
            throw new NotSupportedException();
        }

        void IList<Hit>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public Hit this[int index]
        {
            get { return _hits[index]; }
            set { throw new NotSupportedException(); }
        }
    }
}
