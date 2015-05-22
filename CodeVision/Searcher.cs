using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeVision.Model;
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
        public int MaxNumberOfHits { get { return 10000; } }

        public ReadOnlyHitCollection Search(string searchExpression, int page = 1, int hitsPerPage = 10)
        {
            string defaultFieldName = Fields.Content;
            var query = new QueryParser(Version.LUCENE_30, defaultFieldName, new CSharpAnalyzer()).Parse(searchExpression.ToLower());
            
            var indexDirectory = new SimpleFSDirectory(new DirectoryInfo("Index"));
            var hits = new List<Hit>();
            int totalHits;
            using (var reader = IndexReader.Open(indexDirectory, true))
            {
                var searcher = new IndexSearcher(reader);
                ScoreDoc[] scoreDocs = searcher.Search(query, MaxNumberOfHits).ScoreDocs;
                totalHits = scoreDocs.Length;
                foreach (var scoreDoc in scoreDocs)
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

            var hitsOnOnePage = hits.GetPage(page, hitsPerPage).ToList();
            return new ReadOnlyHitCollection(hitsOnOnePage, totalHits);
        }
    }
}
