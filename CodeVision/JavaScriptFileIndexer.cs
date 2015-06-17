using System.IO;
using Lucene.Net.Documents;
using Lucene.Net.Index;

namespace CodeVision
{
    public class JavaScriptFileIndexer : FileIndexer
    {
        protected override bool CanIndex(FileInfo file)
        {
            return file != null && Path.GetExtension(file.Name) == ".js" && file.Exists && ((file.Attributes & FileAttributes.Hidden) == 0);
        }

        public override bool Index(IndexWriter writer, FileInfo file)
        {
            if (CanIndex(file))
            {
                var doc = new Document();
                doc.Add(new Field(Fields.Content, file.OpenText(), Field.TermVector.WITH_OFFSETS));
                doc.Add(new Field(Fields.Path, Path.Combine(file.DirectoryName, file.Name), Field.Store.YES, Field.Index.NO));
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