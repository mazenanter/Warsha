using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Common
{
    public class PagedResult<T>
    {
        public List<T> Items { get; private set; }
        public int PageSize { get; private set; }
        public int PageNumber { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasNextPage => PageNumber < TotalPages;
        public bool HasPreviousPage => PageNumber > 1;

        public static PagedResult<T> Create(List<T> items, int totalCount, int pageNumber, int pageSize)
        {
            return new PagedResult<T>()
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
            };
        }
    }
}
