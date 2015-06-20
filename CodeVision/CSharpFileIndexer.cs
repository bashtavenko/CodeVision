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
                var doc = new Document();
                doc.Add(new Field(Fields.Content, file.OpenText(), Field.TermVector.WITH_OFFSETS));
                var parser = new CSharpParser();
                var syntax = parser.Parse(file.FullName);
                AddComments(doc, syntax);
                AddUsings(doc, syntax);
                AddClasses(doc, syntax);
                doc.Add(new Field(Fields.Path, Path.Combine(file.DirectoryName, file.Name), Field.Store.YES, Field.Index.NO));
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