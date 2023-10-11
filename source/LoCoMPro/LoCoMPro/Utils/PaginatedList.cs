using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoCoMPro.Models;
using LoCoMPro.Pages;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.EntityFrameworkCore;

namespace LoCoMPro.Utils
{
    /// <summary>
    /// Class to manipulate paginated list.
    /// </summary>
    /// <typeparam name="T">Type for the paginatied list.</typeparam>
    public class PaginatedList<T> : List<T>
    {
        /// <summary>
        /// Index of the actual page.
        /// </summary>
        public int PageIndex { get; private set; }

        /// <summary>
        /// Total amount of pages.
        /// </summary>
        public int TotalPages { get; private set; }

        /// <summary>
        /// Constructor of the paginated list
        /// <param name="items">Items that will be saved in the paginated list.</param>
        /// <param name="count">Number of items that will be saved in the paginated list.</param>
        /// <param name="pageIndex">Index to initialized the paginated list with.</param>
        /// <param name="pageSize">Maximum number of items that will be saved in each page.</param>
        /// </summary> 
        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            AddRange(items);
        }

        ///<summary>
        /// Checks if the paginated list has a previous page.
        /// </summary> 
        public bool HasPreviousPage => PageIndex > 1;

        /// <summary>
        /// Checks if the paginated list has a next page.
        /// </summary>
        public bool HasNextPage => PageIndex < TotalPages;

        /// <summary>
        /// Gets the page index of the paginated list.
        /// </summary>
        /// <param name="pageIndex">Index that of the query.</param>
        /// <returns>Number of current page.</returns>
        public static int GetPageIndex(int? pageIndex)
        {
            // If the page index is lower that 1 or null 
            return !pageIndex.HasValue || pageIndex < 1 ? 1 : pageIndex.Value;
        }

        /// <summary>
        /// Gets and stores the content of the pages from a query.
        /// </summary>
        /// <param name="source">Source of the data to populate PaginatedList with.</param>
        /// <param name="pageIndex">Index to use for page.</param>
        /// <param name="pageSize">Maximum number of items allowed in each page.</param>
        /// <returns>Paginated list result of the creation.</returns>
        public static async Task<PaginatedList<T>> CreateAsync(
            IQueryable<T> source, int pageIndex, int pageSize)
        {
            pageIndex = GetPageIndex(pageIndex);
            var count = await source.CountAsync();
            var items = await source.Skip(
                (pageIndex - 1) * pageSize)
                .Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }

        /// <summary>
        /// Gets and stores the content of the pages from a list.
        /// </summary>
        /// <param name="source">Source of the data to populate PaginatedList with.</param>
        /// <param name="pageIndex">Index to use for page.</param>
        /// <param name="pageSize">Maximum number of items allowed in each page.</param>
        /// <returns>Paginated list result of the creation.</returns>
        public static async Task<PaginatedList<T>> CreateAsync(
            List<T> source, int pageIndex, int pageSize)
        {
            pageIndex = GetPageIndex(pageIndex);
            var count = source.Count;
            var items = source.Skip(
                (pageIndex - 1) * pageSize)
                .Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}