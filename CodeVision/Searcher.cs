using System.Collections.Generic;
using System.IO;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;

namespace CodeVision
{
    public class Searcher
    {
        public List<Hit> Search(string searchExpression)
        {
            var analyzer = new StandardAnalyzer(Version.LUCENE_30);
            var query = new QueryParser(Version.LUCENE_30, "contents", analyzer).Parse(searchExpression);
            
            const int hitsPerPage = 10;
            var indexDirectory = new SimpleFSDirectory(new DirectoryInfo("Index"));
            var hits = new List<Hit>();
            using (var reader = IndexReader.Open(indexDirectory, true))
            {
                var searcher = new IndexSearcher(reader);
                var collector = TopScoreDocCollector.Create(hitsPerPage, true);
                searcher.Search(query, collector);
                foreach (var scoreDoc in collector.TopDocs().ScoreDocs)
                {
                    int docId = scoreDoc.Doc;
                    var hit = new Hit {FilePath = searcher.Doc(docId).Get("path") , Score = scoreDoc.Score};
                    var tv = reader.GetTermFreqVector(docId, "contents");
                    var tpv = tv as TermPositionVector;

                    int termIndex = tv.IndexOf(searchExpression);
                    if (termIndex != -1 && tpv != null)
                    {
                        hit.Offsets = new List<Offset>();
                        foreach (var offset in tpv.GetOffsets(termIndex))
                        {
                            hit.Offsets.Add(new Offset {StartOffset = offset.StartOffset, EndOffset = offset.EndOffset});
                        }
                    }
                    hits.Add(hit);
                }
            }
            return hits;
        }
    }
}
