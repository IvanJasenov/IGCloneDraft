using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class PaginationParms
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10; // our default page size

        public int PageNumber { get; set; } = 1;

        public int ItemsPerPage
        {
            get => _pageSize;

            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}
