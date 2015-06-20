using System.IO;
using Lucene.Net.Index;

namespace CodeVision
{
    public abstract class FileIndexer
    {
        public FileIndexer Successor { get; set; }
        protected ILogger Logger;

        public FileIndexer() : this (null, null)
        {
        }

        public FileIndexer(FileIndexer successor, ILogger logger)
        {
            this.Successor = successor;
            this.Logger = logger;
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
