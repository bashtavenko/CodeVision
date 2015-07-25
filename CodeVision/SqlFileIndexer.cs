using System.IO;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Index;

namespace CodeVision
{
    public class SqlFileIndexer : FileIndexer
    {
        public SqlFileIndexer(FileIndexer successor, ILogger logger) : base(successor, logger)
        {
        }
        
        protected override bool CanIndex(FileInfo file)
        {
            return file != null && Path.GetExtension(file.Name) == ".sql" && file.Exists &&
                   ((file.Attributes & FileAttributes.Hidden) == 0);
        }

        public override bool Index(IndexWriter writer, FileInfo file)
        {
            if (CanIndex(file))
            {
                var doc = new Document();
                doc.Add(new Field(Fields.Content, file.OpenText(), Field.TermVector.WITH_OFFSETS));

                string path = Path.Combine(file.DirectoryName ?? string.Empty, file.Name);
                doc.Add(new Field(Fields.Path, path, Field.Store.YES, Field.Index.NO));
                doc.Add(new Field(Fields.Key, path, Field.Store.NO, Field.Index.NOT_ANALYZED_NO_NORMS));
                doc.Add(new Field(Fields.Language, Languages.Sql, Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));
                writer.AddDocument(doc); 
                return true;
            }
            else
            {
                return base.Index(writer, file);
            }
        }
    }
}