using System.IO;
using System.Linq;
using System.Text;
using CodeVision.CSharp;
using Lucene.Net.Documents;
using Lucene.Net.Index;

namespace CodeVision
{
    public class CSharpFileIndexer : FileIndexer
    {
        public CSharpFileIndexer(FileIndexer successor) : base(successor, null)
        {
        }

        protected override bool CanIndex(FileInfo file)
        {
            return file != null && Path.GetExtension(file.Name) == ".cs" && file.Exists && ((file.Attributes & FileAttributes.Hidden) == 0);
        }

        public override bool Index(IndexWriter writer, FileInfo file)
        {
            if (CanIndex(file))
            {
                if (writer == null)
                {
                    return false;
                }
                string path = Path.Combine(file.DirectoryName ?? string.Empty, file.Name);
                var parser = new CSharpParser();
                var syntax = parser.Parse(file.FullName);

                // Build a key based on the file syntax 
                // TODO: Need semantic model in order to get full class name, i.e. namespace.className
                // The original reason for having key field was to use DuplicateFilter. It however cannot be used
                // if index has multiple segments, not at least in version 3.0.1. Once this bug is fixed, key field
                // can be added to other indexers to be used by DuplicateFilter, which requires all documents to have it.
                string key;
                if (syntax.Classes.Any() && syntax.Usings.Any())
                {
                    key = string.Format("{0}.{1}", syntax.Usings.First(), syntax.Classes.First().ClassName);
                    using (var reader = writer.GetReader()) // We want to get a new reader once per document
                    {
                        var term = new Term(Fields.Key, key);
                        var docs = reader.TermDocs(term);
                        if (docs.Next())
                        {
                            return false; // We have already indexed this file.
                        }
                    }
                }
                else
                {
                    key = path;
                }
                var doc = new Document();
                doc.Add(new Field(Fields.Key, key, Field.Store.NO, Field.Index.NOT_ANALYZED_NO_NORMS));
                doc.Add(new Field(Fields.Content, file.OpenText(), Field.TermVector.WITH_OFFSETS));
                
                AddComments(doc, syntax);
                AddUsings(doc, syntax);
                AddClasses(doc, syntax);
                
                doc.Add(new Field(Fields.Path, path, Field.Store.YES, Field.Index.NO));
                doc.Add(new Field(Fields.Language, Languages.CSharp, Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));
                writer.AddDocument(doc); // here we can specify an analyzer
                return true;
            }
            else
            {
                return base.Index(writer, file);
            }
        }

        private void AddComments(Document doc, CSharpFileSyntax syntax)
        {
            if (syntax.Comments.Any())
            {
                var sb = new StringBuilder();
                syntax.Comments.ForEach(c => sb.Append(c));
                doc.Add(new Field(Fields.Comment, sb.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            }
        }

        private void AddUsings(Document doc, CSharpFileSyntax syntax)
        {
            foreach (var @using in syntax.Usings)
            {
                doc.Add(new Field(Fields.Using, @using, Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));
            }
        }

        private void AddClasses(Document doc, CSharpFileSyntax syntax)
        {
            foreach (var @class in syntax.Classes)
            {
                doc.Add(new Field(Fields.Class, @class.ClassName, Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));
                foreach (var @interface in @class.Interfaces)
                {
                    doc.Add(new Field(Fields.Interface, @interface, Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));
                }
                if (!string.IsNullOrEmpty(@class.BaseClassName))
                {
                    doc.Add(new Field(Fields.Base, @class.BaseClassName, Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));
                }
                AddMethods(doc, @class);
            }
        }

        private void AddMethods(Document doc, CSharpClass @class)
        {
            foreach (var method in @class.Methods)
            {
                doc.Add(new Field(Fields.Method, method.MethodName, Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));
                doc.Add(new Field(Fields.Return, method.ReturnType, Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));
                foreach (var parameter in method.Parameters)
                {
                    doc.Add(new Field(Fields.Parameter, parameter, Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));
                }
                if (method.Body != null)
                {
                    doc.Add(new Field(Fields.Code, method.Body, Field.Store.NO, Field.Index.ANALYZED));
                }
            }
        }

    }
}