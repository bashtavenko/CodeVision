using System.Collections.Generic;

namespace CodeVision.Web.ViewModels
{
    public class SearchResult
    {
        public string SearchExpression { get; set; }
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