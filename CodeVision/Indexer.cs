using System.IO;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Util;

namespace CodeVision
{
    public class Indexer
    {
        public void Index(string contentPath)
        {
            var analyzer = new StandardAnalyzer(Version.LUCENE_30);

            var indexDirectory = new SimpleFSDirectory(new DirectoryInfo("Index"));
            using (var writer = new IndexWriter(indexDirectory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                IndexDirectory(writer, new DirectoryInfo(contentPath));
            }
        }

        private void IndexDirectory(IndexWriter writer, DirectoryInfo dir)
        {
            foreach (var file in dir.GetFiles())
            {
                IndexFile(writer, file);    
            }
            
            foreach (var subDir in dir.GetDirectories())
            {
                IndexDirectory(writer, subDir);
            }
        }

        private void IndexFile(IndexWriter writer, FileInfo file)
        {
            if (writer == null || file == null  || Path.GetExtension(file.Name) != ".cs")
            {
                return;
            }
            var doc = new Document();
            doc.Add(new Field("path", Path.Combine(file.DirectoryName, file.Name), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("filename", file.Name, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("modified", file.LastWriteTime.ToShortDateString(), Field.Store.NO, Field.Index.ANALYZED));
            doc.Add(new Field("contents", file.OpenText(), Field.TermVector.WITH_OFFSETS));

            writer.AddDocument(doc);
        }
    }
}
