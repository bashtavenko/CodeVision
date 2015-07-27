using System.IO;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Index;

namespace CodeVision
{
    public class JavaScriptFileIndexer : FileIndexer
    {
        private static readonly string[] ThirdPartyFileNamesOrFolders =
        {
            "jquery", "microsoft", "telerik", "angular", ".min", "tiny_mce", "modernizr",
            "bootstrap", "jasmine", "opencover", "bower_components"
        };
        
        public JavaScriptFileIndexer(FileIndexer successor, ILogger logger) : base(successor, logger)
        {
        }

        public bool IsThirdParty(string filePath)
        {
            return ThirdPartyFileNamesOrFolders.Any(f => filePath.ToLower().Contains(f));
        }

        protected override bool CanIndex(FileInfo file)
        {
            return file != null && Path.GetExtension(file.Name) == ".js" && file.Exists &&
                   ((file.Attributes & FileAttributes.Hidden) == 0)
                   && !IsThirdParty(file.FullName);
        }

        public override bool Index(IndexWriter writer, FileInfo file)
        {
            if (CanIndex(file))
            {
                var doc = new Document();
                doc.Add(new Field(Fields.Content, file.OpenText(), Field.TermVector.WITH_OFFSETS));

                string path = Path.Combine(file.DirectoryName ?? string.Empty, file.Name);
                doc.Add(new Field(Fields.Path, path, Field.Store.YES, Field.Index.NO));
                doc.Add(new Field(Fields.Language, Languages.JavaScript, Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));
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