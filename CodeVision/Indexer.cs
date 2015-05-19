using System;
using System.IO;
using System.IO.Pipes;
using CodeVision.CSharp;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Util;

namespace CodeVision
{
    public class Indexer
    {
        private readonly ILogger _logger;
        
        public Indexer(ILogger logger)
        {
            _logger = logger;
        }

        public void Index(string contentPath)
        {
            var indexDirectory = new SimpleFSDirectory(new DirectoryInfo("Index"));
            Log(string.Format("Begining to index {0}. Index location: {1}", contentPath, indexDirectory.Directory.FullName));
            using (var writer = new IndexWriter(indexDirectory, new CSharpAnalyzer(), true, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                IndexDirectory(writer, new DirectoryInfo(contentPath));
            }
            Log("Done.");
        }

        private void IndexDirectory(IndexWriter writer, DirectoryInfo dir)
        {
            foreach (var file in dir.GetFiles())
            {
                try
                {
                    IndexFile(writer, file);
                }
                catch (Exception ex)
                {
                    Log("Failed to index file", ex);       
                }
            }
            
            foreach (var subDir in dir.GetDirectories())
            {
                IndexDirectory(writer, subDir);
            }
        }

        private void IndexFile(IndexWriter writer, FileInfo file)
        {
            if (writer == null || file == null || Path.GetExtension(file.Name) != ".cs" 
                || !file.Exists || (file.Attributes & FileAttributes.Hidden) != 0)
            {
                return;
            }
            Log(file.FullName);
            var doc = new Document();
            doc.Add(new Field(Fields.Content, file.OpenText(), Field.TermVector.WITH_OFFSETS));
            var parser = new CSharpParser();
            var syntax = parser.Parse(file.FullName);
            AddUsings(doc, syntax);
            AddClasses(doc, syntax);
            doc.Add(new Field(Fields.Path, Path.Combine(file.DirectoryName, file.Name), Field.Store.YES, Field.Index.NO));
            writer.AddDocument(doc);
        }

        private void AddUsings(Document doc, CSharpFileSyntax syntax)
        {
            foreach (var @using in syntax.Usings)
            {
                doc.Add(new Field(Fields.Using, @using, Field.Store.YES, Field.Index.NOT_ANALYZED, Field.TermVector.WITH_OFFSETS));
            }
        }

        private void AddClasses(Document doc, CSharpFileSyntax syntax)
        {
            foreach (var @class in syntax.Classes)
            {
                doc.Add(new Field(Fields.Class, @class.ClassName, Field.Store.YES, Field.Index.NOT_ANALYZED, Field.TermVector.WITH_OFFSETS));
                foreach (var @interface in @class.Interfaces)
                {
                    doc.Add(new Field(Fields.Interface, @interface, Field.Store.YES, Field.Index.NOT_ANALYZED, Field.TermVector.WITH_OFFSETS));    
                }
                AddMethods(doc, @class);
            }
        }

        private void AddMethods(Document doc, CSharpClass @class)
        {
            foreach (var method in @class.Methods)
            {
                doc.Add(new Field(Fields.Method, method.MethodName, Field.Store.YES, Field.Index.NOT_ANALYZED, Field.TermVector.WITH_OFFSETS));
                doc.Add(new Field(Fields.Return, method.ReturnType, Field.Store.YES, Field.Index.NOT_ANALYZED, Field.TermVector.WITH_OFFSETS));
                foreach (var parameter in method.Parameters)
                {
                    doc.Add(new Field(Fields.Parameter, parameter, Field.Store.YES, Field.Index.NOT_ANALYZED, Field.TermVector.WITH_OFFSETS));
                }
                doc.Add(new Field(Fields.Code, method.Body, Field.Store.NO, Field.Index.ANALYZED, Field.TermVector.WITH_OFFSETS));
            }
        }

        private void Log(string message, Exception ex = null)
        {
            if (_logger != null)
            {
                if (ex != null)
                {
                    _logger.Log(message, ex);
                }
                else
                {
                    _logger.Log(message);    
                }
            }
        }
    }
}
