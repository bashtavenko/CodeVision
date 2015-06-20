using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace CodeVision.Web.ViewModels
{
    public class SearchResult
    {
        public string SearchExpression { get; set; }

        public List<SelectListItem> LanguageList
        {
            get
            {
                return new List<SelectListItem>()
                {
                    new SelectListItem {Text = "All", Value = "-1", Selected = true},
                    new SelectListItem {Text = "C#", Value = "cs"},
                    new SelectListItem {Text = "JavaScript", Value = "js"},
                    new SelectListItem {Text = "SQL", Value = "sql"}
                };
            }
        }

        public List<SelectListItem> SortList
        {
            get
            {
                return new List<SelectListItem>()
                {
                    new SelectListItem {Text = "Relevance", Value = "-1", Selected = true},
                    new SelectListItem {Text = "Language", Value = "language"}
                };
            }
        }

        public string Language { get; set; }
        public string Sort{ get; set; }

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
        public string Error { get; set; }
        public bool HasError { get { return !string.IsNullOrEmpty(Error) && !Hits.Any(); } }

        public SearchResult()
            : this(string.Empty, new List<SearchHit>(), null, null, 1, 10, 0)
        {
        }

        public SearchResult(string searchExpression, string language, string sort, int pageNumber, int pageSize,
            string error) : this (searchExpression, new List<SearchHit>(), language, sort, pageNumber, pageSize, 0)
        {
            this.Error = error;
        }

        public SearchResult(string searchExpression, List<SearchHit> hits, string language, string sort, int pageNumber, int pageSize, int totalHits)
        {
            Hits = new SearchResultsPagedList(hits, pageNumber, pageSize, totalHits);
            this.TotalHits = totalHits;
            this.SearchExpression = searchExpression;
            this.Language = language;
            this.Sort = sort;
        }
    }
}