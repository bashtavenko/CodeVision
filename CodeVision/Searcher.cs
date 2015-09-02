using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public ReadOnlyHitCollection Search(string searchExpression, Model.Filter filter = null, string sortField = null, int page = 1, int hitsPerPage = 10)
        {
            if (string.IsNullOrEmpty(searchExpression))
            {
                throw new SearchException("Must have searchExpression");
            }
            string defaultFieldName = Fields.Content;
            var analyzer = AnalyzerBuilder.CreateAnalyzer();
            Query query;
            try
            {
                query = new QueryParser(Version.LUCENE_30, defaultFieldName, analyzer).Parse(searchExpression.ToLower());
            }
            catch (ParseException ex)
            {
                throw new SearchException(string.Format("Sorry, '{0}' isn't something we can search for so far.", searchExpression), ex);
            }
            
            var indexDirectory = new SimpleFSDirectory(new DirectoryInfo(_configuration.IndexPath));
            
            List<Hit> onePageOfHits;
            int totalHits;
            using (var reader = IndexReader.Open(indexDirectory, true))
            {
                //  Get one page of hits
                var hits = new List<Hit>();
                var searcher = new IndexSearcher(reader);
                
                var termsFilter = filter != null && !string.IsNullOrEmpty(filter.Field) 
                    ? new FieldCacheTermsFilter(filter.Field, filter.Terms.ToArray())
                    : null;
                var sort = !string.IsNullOrEmpty(sortField)
                    ? new Sort(new SortField(sortField, SortField.STRING))
                    : Sort.RELEVANCE;

                ScoreDoc[] scoreDocs = searcher.Search(query, termsFilter, MaxNumberOfHits, sort).ScoreDocs;
                totalHits = scoreDocs.Length;

                foreach (var scoreDoc in scoreDocs)
                {
                    int docId = scoreDoc.Doc;
                    string filePath = searcher.Doc(docId).Get(Fields.Path);
                    string language = searcher.Doc(docId).Get(Fields.Language);
                    var hit = new Hit(docId, _configuration.ContentRootPath, filePath, scoreDoc.Score, language);
                    hits.Add(hit);
                }

                onePageOfHits = hits.GetPage(page, hitsPerPage).ToList();

                // Get offsets and higlights on the page we are going to return
                foreach (var hit in onePageOfHits)
                {
                    var primitiveQuery = query.Rewrite(reader);
                    var terms = new HashSet<Term>();
                    primitiveQuery.ExtractTerms(terms);
                    string searchField = string.Empty;
                    if (terms.Count == 0)
                    {
                        // There can be all kinds of queires
                        var prefixQuery = query as PrefixQuery;
                        if (prefixQuery != null)
                        {
                            searchField = prefixQuery.Prefix.Field;
                            primitiveQuery = prefixQuery;
                        }
                    }
                    else
                    {
                        // TODO: There can be multiple term fields, like code: and method:
                        searchField = terms.First().Field;
                    }

                    var termFreqVector = reader.GetTermFreqVector(hit.DocId, Fields.Content);
                    var termPositionVector = termFreqVector as TermPositionVector;
                    if (termFreqVector == null || termPositionVector == null)
                    {
                        throw new ArgumentException("Must have term frequencies and positions vectors");
                    }

                    // No offsets for prefix and other non-term based queries
                    const int maxOffsetNumber = 10;
                    foreach (var term in terms)
                    {
                        int termIndex = termFreqVector.IndexOf(term.Text); // Meaning get me this term, not text yet.
                        if (termIndex != -1)
                        {
                            foreach (var offset in termPositionVector.GetOffsets(termIndex))
                            {
                                if (hit.Offsets.Count < maxOffsetNumber)
                                {
                                    hit.Offsets.Add(new Offset
                                    {
                                        StartOffset = offset.StartOffset,
                                        EndOffset = offset.EndOffset
                                    });
                                }
                            }
                        }
                    }

                    // Highlighter from contrib package
                    var tokenStream = TokenSources.GetTokenStream(termPositionVector);
                    var scorer = new QueryScorer(primitiveQuery, searchField);
                    var fragmenter = new SimpleSpanFragmenter(scorer);
                    var formatter = new SimpleHTMLFormatter("<kbd>", "</kbd>");
                    var highlighter = new Highlighter(formatter, scorer) {TextFragmenter = fragmenter};

                    string text;
                    using (var sr = new StreamReader(hit.FilePath))
                    {
                        text = sr.ReadToEnd();
                    }
                    string bestFragment = highlighter.GetBestFragment(tokenStream, text);
                    hit.BestFragment = EscapeHtmlMarkup(bestFragment);
                }
            }
      
            return new ReadOnlyHitCollection(onePageOfHits, totalHits);
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
            foreach (var offset in hit.Offsets.OrderBy(s => s.StartOffset))
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

            var resultString = EscapeHtmlMarkup(result.ToString());
            return resultString;
        }

        // Cases like <summary> and also generics
        // List<<kbd>Account</kbd>> => List&lt;<kbd>Account</kbd>&gt;
        public string EscapeHtmlMarkup(string source)
        {
            string resultString = Regex.Replace(source, "<(?!kbd|/kbd)", "&lt;");
            resultString = Regex.Replace(resultString, "(?<!kbd)>", "&gt;");
            return resultString;
        }
    }
}
