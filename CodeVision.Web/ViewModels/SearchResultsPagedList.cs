using System;
using System.Collections.Generic;
using PagedList;

namespace CodeVision.Web.ViewModels
{
    // The reason for subclassing PageList class as opposed to simply using it, is to avoid
    // dependency in CodeVision which handles paging on its own. Plus, it wouldn't work with large
    // datasets anyway. 
    public class SearchResultsPagedList : BasePagedList<SearchHit>
    {
        public SearchResultsPagedList(List<SearchHit> hits,  int pageNumber, int pageSize, int totalItemCount) :
            base(pageNumber, pageSize, totalItemCount)
        {
            this.Subset.AddRange(hits);

            PageCount = TotalItemCount > 0 
 						? (int)Math.Ceiling(TotalItemCount / (double)PageSize) 
						: 0; 
			HasPreviousPage = PageNumber > 1; 
			HasNextPage = PageNumber < PageCount; 
			IsFirstPage = PageNumber == 1; 
			IsLastPage = PageNumber >= PageCount; 
			FirstItemOnPage = (PageNumber - 1) * PageSize + 1; 
			var numberOfLastItemOnPage = FirstItemOnPage + PageSize - 1; 
			LastItemOnPage = numberOfLastItemOnPage > TotalItemCount 
							? TotalItemCount 
							: numberOfLastItemOnPage; 

        }
    }
}