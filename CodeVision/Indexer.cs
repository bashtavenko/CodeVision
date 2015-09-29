using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;

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
            var sqlFileIndexer = new SqlFileIndexer(jsFileIndexer, _logger);
            _fileIndexer = new CSharpFileIndexer(sqlFileIndexer);
        }

        public void Index()
        {
            Index(_configuration.ContentRootPath);
        }

        public void Index(string contentPath)
        {
            Index(_configuration.ContentRootPath, new List<string>());
        }

        public void Index(string contentPath, List<string> foldersToExclude)
        {
            if (foldersToExclude == null)
            {
                throw new ArgumentException ("Must not have null collection of folders", "foldersToExclude");
            }
            var indexDirectory = new SimpleFSDirectory(new DirectoryInfo(_configuration.IndexPath));
            Log(string.Format("Begining to index {0}. Index location: {1}", contentPath, indexDirectory.Directory.FullName));
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var analyzer = AnalyzerBuilder.CreateAnalyzer();
            using (var writer = new IndexWriter(indexDirectory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                IndexDirectory(writer, new DirectoryInfo(contentPath), foldersToExclude);
            }
            stopWatch.Stop();
            Log(string.Format("Indexed {0:N0} files in {1:00}:{2:00}.{3:00}", _fileCount, stopWatch.Elapsed.Hours, stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds));
        }
    
        private void IndexDirectory(IndexWriter writer, DirectoryInfo dir, List<string> foldersToExclude)
        {
            // Assuming that root folder cannot be excluded
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
                if (foldersToExclude.FirstOrDefault(s => s.Equals(subDir.Name, StringComparison.InvariantCultureIgnoreCase)) != null)
                {
                    Log(string.Format("Excluded {0} folder", subDir.Name));
                    continue;
                }               

                IndexDirectory(writer, subDir, foldersToExclude);
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
