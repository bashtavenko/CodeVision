using System.IO;
using Lucene.Net.Index;

namespace CodeVision
{
    public abstract class FileIndexer
    {
        public FileIndexer Successor { get; set; }

        public FileIndexer() : this (null)
        {
        }

        public FileIndexer(FileIndexer successor)
        {
            Successor = successor;
        }

        protected virtual bool CanIndex(FileInfo file)
        {
            return false;
        }

        public virtual bool Index(IndexWriter writer, FileInfo file)
        {
            if (Successor != null)
            {
                return Successor.Index(writer, file);
            }
            else
            {
                return false;
            }
        }
    }
}
