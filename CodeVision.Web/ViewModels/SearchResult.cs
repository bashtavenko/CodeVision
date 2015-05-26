using System.Collections.Generic;

namespace CodeVision.Web.ViewModels
{
    public class SearchResult
    {
        public int TotalHits { get; set; }
        public List<SearchHit> Hits { get; set; }

        public SearchResult()
        {
            Hits = new List<SearchHit>();
        }
    }
}