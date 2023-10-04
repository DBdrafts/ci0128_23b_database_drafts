using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LoCoMPro.Utils
{
    /* Class to manipulate paginated list */
    public class PaginatedList<T> : List<T>
    {
        /* Index of the actual page */
        public int PageIndex { get; private set; }

        /* Total amount of pages */
        public int TotalPages { get; private set; }

        /* Constructor of the paginated list */
        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            AddRange(items);
        }

        /* Check if have a previous page */
        public bool HasPreviousPage => PageIndex > 1;

        /* Check if have a next page */
        public bool HasNextPage => PageIndex < TotalPages;

        /* Gets and stores the content of the pages */
        public static async Task<PaginatedList<T>> CreateAsync(
            IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip(
                (pageIndex - 1) * pageSize)
                .Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}