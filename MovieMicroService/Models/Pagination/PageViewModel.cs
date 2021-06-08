using System;

namespace MovieMicroService.Models.Pagination
{
    public class PageViewModel
    {
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public long Count { get; set; }

        public PageViewModel(long count, int pageNumber, int pageSize)
        {
            Count = count;
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }

        public bool HasPreviousPage
        {
            get
            {
                return PageNumber > 1;
            }
        }

        public bool HasNextPage
        {
            get
            {
                return PageNumber < TotalPages;
            }
        }
    }
}

