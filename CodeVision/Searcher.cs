using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        private readonly IConfiguration _configuration;

        public Searcher()
            : this(CodeVisionConfigurationSection.Load())
        {
        }

        public Searcher(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ReadOnlyHitCollection Search(string searchExpression, int page = 1, int hitsPerPage = 10)
        {
            if (string.IsNullOrEmpty(searchExpression))
            {
                throw new NullReferenceException("Must have searchExpression");
            }
            string defaultFieldName = Fields.Content;
            var query = new QueryParser(Version.LUCENE_30, defaultFieldName, new CSharpAnalyzer()).Parse(searchExpression.ToLower());
            
            var indexDirectory = new SimpleFSDirectory(new DirectoryInfo(_configuration.IndexPath));
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
                    var hit = new Hit(_configuration.ContentRootPath, searcher.Doc(docId).Get(Fields.Path)) { Score = scoreDoc.Score};
                    
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
                    var formatter = new SimpleHTMLFormatter("<kbd>", "</kbd>");
                    var highlighter = new Highlighter(formatter, scorer) { TextFragmenter = fragmenter };

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

        public string GetFileContent(Hit hit)
        {
            string sourceString;
            using (var sr = new StreamReader(hit.FilePath))
            {
                sourceString =  sr.ReadToEnd();
            }

            var result = new StringBuilder();
            int currentIndex = 0;
            foreach (var offset in hit.Offsets)
            {
                if (offset.StartOffset > sourceString.Length - 1 || offset.EndOffset > sourceString.Length - 1)
                {
                    throw new ArgumentException("Invalid offsets");
                }
                result.Append(sourceString.Substring(currentIndex, offset.StartOffset - currentIndex));
                result.Append("<kbd>");
                result.Append(sourceString.Substring(offset.StartOffset, offset.EndOffset - offset.StartOffset));
                result.Append("</kbd>");
                currentIndex = offset.EndOffset;
            }

            result.Append(sourceString.Substring(currentIndex, sourceString.Length - currentIndex));
            return result.ToString();
        }
    }
}
