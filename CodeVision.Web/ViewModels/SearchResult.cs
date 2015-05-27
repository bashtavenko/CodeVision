using System;
using System.Collections.Generic;
using System.Text;

namespace CodeVision.Web.ViewModels
{
    public class SearchResult
    {
        public string SearchExpression { get; set; }

        public string SearchExpressionEncoded
        {
            get
            {
                byte[] bytes = Encoding.UTF8.GetBytes(SearchExpression);
                return Convert.ToBase64String(bytes);
            }
        }
        public int TotalHits { get; private set; }
        public SearchResultsPagedList Hits { get; set; }

        public SearchResult(string searchExpression, List<SearchHit> hits, int pageNumber, int pageSize, int totalHits)
        {
            Hits = new SearchResultsPagedList(hits, pageNumber, pageSize, totalHits);
            this.TotalHits = totalHits;
            this.SearchExpression = searchExpression;
        }
    }
}