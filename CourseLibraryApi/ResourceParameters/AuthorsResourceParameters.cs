using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibraryApi.ResourceParameters
{
    public class AuthorsResourceParameters
    {
        private int _pageSize = 10;
        private const int maxPageSize = 20;
        public string MainCategory { get; set; }
        public string SearchQuery { get; set; }

        public int PageSize
        {
            get => _pageSize;
            set
            {
                _pageSize = (value <= maxPageSize) ? value : maxPageSize;
            }
        }

        public string OrderBy { get; set; } = "Name";

        public int PageNumber { get; set; } = 1;
    }
}