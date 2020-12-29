using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class PagedList<T> : List<T>
    {
        public PagedList(IEnumerable<T> items, int count, int pageNumber, int itemsPerPage)
        {
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)itemsPerPage);
            PageSize = itemsPerPage;
            TotalCount = count;
            // set the range of the items inside the ctor
            AddRange(items);
            // I added this 
            ListOfItems = items;
        }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }
        // I added this property, just to have the items accessible in a property
        public IEnumerable<T> ListOfItems { get; set; }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int itemsPerPage)
        {
            // we have no choice but to execute two queries against the dbs
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * itemsPerPage)
                                    .Take(itemsPerPage)
                                    .ToListAsync();

            return new PagedList<T>(items, count, pageNumber, itemsPerPage);
        }
    }
}
