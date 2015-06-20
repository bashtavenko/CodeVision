using System;
using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Store;

namespace CodeVision
{
    public class Indexer
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly FileIndexer _fileIndexer;

        private int _fileCount;
        
        public Indexer(ILogger logger)
            : this(logger, CodeVisionConfigurationSection.Load())
        {
        }

        public Indexer(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _fileCount = 0;
            var jsFileIndexer = new JavaScriptFileIndexer(null, _logger);
            _fileIndexer = new CSharpFileIndexer(jsFileIndexer);
        }

        public void Index()
        {
            Index((_configuration.ContentRootPath));
        }

        public void Index(string contentPath)
        {
            var indexDirectory = new SimpleFSDirectory(new DirectoryInfo(_configuration.IndexPath));
            Log(string.Format("Begining to index {0}. Index location: {1}", contentPath, indexDirectory.Directory.FullName));
            var analyzer = AnalyzerBuilder.CreateAnalyzer();
            using (var writer = new IndexWriter(indexDirectory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                IndexDirectory(writer, new DirectoryInfo(contentPath));
            }
            Log(string.Format("Indexed {0:N0} files.", _fileCount));
        }
    
        private void IndexDirectory(IndexWriter writer, DirectoryInfo dir)
        {
            foreach (var file in dir.GetFiles())
            {
                try
                {
                    bool indexed = _fileIndexer.Index(writer, file);
                    if (indexed)
                    {
                        _fileCount++;
                    }
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
