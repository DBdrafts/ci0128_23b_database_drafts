using Elfie.Serialization;
using LoCoMPro.Data;
using LoCoMPro.Models;
using LoCoMPro.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Drawing.Printing;

namespace LoCoMPro.Pages
{
    public class SearchPageModel : LoCoMProPageModel
    {
        // Search Page constructor 
        public SearchPageModel(LoCoMProContext context, IConfiguration configuration)
            : base(context, configuration) { }

        // Determinate if the check-box was activated 
        [BindProperty]
        public bool IsChecked { get; set; }

        // List of the categories that exist in the database 
        public IList<Category> Category { get; set; } = default!;

        // List of the registers that match with the search string 
        public PaginatedList<Register> Register { get; set; } = default!;

        // Text enters as the search attribute 
        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        // Text enters as the search type attribute 
        [BindProperty(SupportsGet = true)]
        public string? SearchType { get; set; }

        // Current type of sort 
        [BindProperty(SupportsGet = true)]
        public string? CurrentSort { get; set; }

        // Type of sort by price 
        [BindProperty(SupportsGet = true)]
        public string? PriceSort { get; set; }

        // OnGet method that manage the GET request 
        public async Task OnGetAsync(string searchType, string searchString
            , int? pageIndex, string sortOrder)
        {

            PriceSort = GetSortOrder(sortOrder);

            SearchString = searchString;

            SearchType = searchType;

            CurrentSort = sortOrder;

            // Prepares the query to get the data from the database 
            var categories = from c in _context.Categories
                             select c;

            var registers = from r in _context.Registers
                            select r;

            // Gets the registers by using the type of search choose 
            IQueryable<Register> registersQuery = GetRegistersByType(registers);


            // Get th amount of pages that will be needed for all the registers 
            var pageSize = Configuration.GetValue("PageSize", 5);

            // Gets the data from the database 
            Category = await categories.ToListAsync();

            // Gets a unordered list of registers 
            PaginatedList<Register> UnorderedList = (await PaginatedList<Register>.CreateAsync(
                registersQuery, pageIndex ?? 1, pageSize));

            // Copy the information of the registers ordered 
            Register = new PaginatedList<Register>(OrderRegisters(UnorderedList.ToList(), sortOrder)
                , UnorderedList.PageIndex, UnorderedList.TotalPages);

        }

        // Gets the registers by using the type of search choose 
        public IQueryable<Register> GetRegistersByType(IQueryable<Register>? registersQuery)
        {
            IQueryable<Register> resultQuery;

            // Filter the register by the type of search choose 
            switch (SearchType)
            {
                case "Nombre":
                default:
                    resultQuery = registersQuery
                        .Where(r => r.ProductName.Contains(SearchString!));
                    break;
                case "Marca":
                    resultQuery = registersQuery
                        .Where(r => _context.Products.Any(p => p.Name == r.ProductName && p.Brand.Contains(SearchString!)));
                    break;
                case "Modelo":
                    resultQuery = registersQuery
                        .Where(r => _context.Products.Any(p => p.Name == r.ProductName && p.Model.Contains(SearchString!)));
                    break;
            }

            resultQuery = resultQuery.GroupBy(r => new { r.ProductName, r.StoreName })
                        .Select(grouped => grouped.OrderByDescending(r => r.SubmitionDate).First());

            return resultQuery;
        }

        // Order the registers by the sort order choose 
        public List<Register> OrderRegisters(List<Register>? unorderedList, string sortOrder)
        {
            List<Register> orderedList = new List<Register>();

            if (!unorderedList.IsNullOrEmpty())
            {
                // Sort the list depending of the parameter 
                switch (sortOrder)
                {
                    // Order in case of price_descending
                    case "price_desc":
                        orderedList = unorderedList!.OrderByDescending(r => r.Price).ToList();
                        break;

                    // Normal order for the price 
                    case "price_asc":
                    default:
                        orderedList = unorderedList!.OrderBy(r => r.Price).ToList();
                        break;
                }
            }

            return orderedList;
        }

        // Gets the sort order of the registers 
        public string GetSortOrder(string? sortOrder)
        {
            // If null, the order by price as default 
             return String.IsNullOrEmpty(sortOrder) ? "price_asc" : sortOrder;
        }
    }
}
