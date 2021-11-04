using System;
using System.Collections.Generic;

namespace Domain
{
    public class PaginatedList<T>
    {
        public PaginatedList(List<T> items, int currentPage, int pageSize, int totalItems)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            TotalItems = totalItems;
            Items = items;
        }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public List<T> Items { get; set; }
    }

    public class PaginationDto
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
}