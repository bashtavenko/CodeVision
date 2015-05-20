using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using CodeVision.Model;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Search.Highlight;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;

namespace CodeVision
{
    public class Searcher
    {
        public List<Hit> Search(string searchExpression)
        {
            string defaultFieldName = Fields.Content;
            var query = new QueryParser(Version.LUCENE_30, defaultFieldName, new CSharpAnalyzer()).Parse(searchExpression.ToLower());
            
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
                    var hit = new Hit {FilePath = searcher.Doc(docId).Get(Fields.Path) , Score = scoreDoc.Score};
                    
                    // Get offsets
                    var primitiveQuery = query.Rewrite(reader);
                    var terms = new HashSet<Term>();
                    primitiveQuery.ExtractTerms(terms);

                    var termFreqVector = reader.GetTermFreqVector(docId, Fields.Content);
                    var termPositionVector = termFreqVector as TermPositionVector;
                    if (termFreqVector == null || termPositionVector == null)
                    {
                        throw new ArgumentException("Must have term frequencies and positions vectors");
                    }

                    foreach (var term in terms)
                    {
                        int termIndex = termFreqVector.IndexOf(term.Text);
                        if (termIndex != -1)
                        {
                            foreach (var offset in termPositionVector.GetOffsets(termIndex))
                            {
                                hit.Offsets.Add(new Offset
                                {
                                    StartOffset = offset.StartOffset,
                                    EndOffset = offset.EndOffset
                                });
                            }
                        }
                    }

                    // Highlighter from contrib package
                    var tokenStream = TokenSources.GetTokenStream(termPositionVector);

                    string field = terms.First().Field; // TODO: There can be multiple term fields, like code: and method:
                    var scorer = new QueryScorer(primitiveQuery, field);
                    var fragmenter = new SimpleSpanFragmenter(scorer);
                    var highlighter = new Highlighter(scorer) { TextFragmenter = fragmenter };

                    string text;
                    using (var sr = new StreamReader(hit.FilePath))
                    {
                        text = sr.ReadToEnd();
                    }
                    hit.BestFragment = highlighter.GetBestFragment(tokenStream, text);
                    
                    hits.Add(hit);
                }
            }
            return hits;
        }
    }
}
