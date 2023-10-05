using Elfie.Serialization;
using LoCoMPro.Data;
using LoCoMPro.Models;
using LoCoMPro.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Drawing.Printing;

namespace LoCoMPro.Pages
{
    public class SearchPageModel : PageModel
    {
        /* Context of the data base */
        private readonly LoCoMPro.Data.LoCoMProContext _context;
        /* Configuration for the page */
        private readonly IConfiguration Configuration;

        /* Search Page constructor */
        public SearchPageModel(LoCoMProContext context, IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;
        }

        /* Determinate if the check-box was activated */
        [BindProperty]
        public bool IsChecked { get; set; }

        [BindProperty(SupportsGet = true)]
        public List<string> SelectedCategories { get; set; }

        /* List of the categories that exist in the database */
        public IList<Category> Category { get; set; } = default!;

        /* List of the registers that match with the search string */
        public PaginatedList<Register> Register { get; set; } = default!;

        /* Text enters as the search attribute */
        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        /* Text enters as the search type attribute */
        [BindProperty(SupportsGet = true)]
        public string? SearchType { get; set; }

        /* Current type of sort */
        [BindProperty(SupportsGet = true)]
        public string? CurrentSort { get; set; }

        /* Type of sort by price */
        [BindProperty(SupportsGet = true)]
        public string PriceSort { get; set; }

        /* OnGet method that manage the GET request */
        public async Task OnGetAsync(string searchType, string searchString, int? pageIndex, string sortOrder, string selectedCategories)
        {
            

            /* If the page index is lower that 1 */
            pageIndex = pageIndex < 1 ? 1 : pageIndex;

            /* Get the type of sort by price */
            PriceSort = String.IsNullOrEmpty(sortOrder) ? "price_desc" : "";

            SearchString = searchString;

            SearchType = searchType;

            CurrentSort = sortOrder;

            /* Prepares the query to get the data from the database */
            var categories = from c in _context.Categories
                             select c;

            var registers = from r in _context.Registers
                            select r;


            /* Get th amount of pages that will be needed for all the registers */
            var pageSize = Configuration.GetValue("PageSize", 5);

            /* Gets the data from the database */
            Category = await categories.ToListAsync();

            if (selectedCategories != null)
            {
                var split = selectedCategories.Split(',');
                SelectedCategories = (split != null) ? split!.ToList() : null;
            }

            if (SelectedCategories != null && SelectedCategories.Count() > 0)
            {
                var filteredProducts = _context.Products
                    .Where(p => p.Categories.Any(c => SelectedCategories.Contains(c.CategoryName)))
                    .Select(p => p.Name)
                    .ToList();
                //registers = registers.Where(r => filteredProducts.Contains(r.ProductName));
            
                registers = registers.Where(r => filteredProducts.Contains(r.ProductName));
            }

            /* Gets the registers by using the type of search choose */
            IQueryable<Register> registersQuery = GetRegistersByType(registers);


            /* Gets a unordered list of registers */
            PaginatedList<Register> UnorderedList = (await PaginatedList<Register>.CreateAsync(
                registersQuery, pageIndex ?? 1, pageSize));

            /* Copy the information of the registers ordered */
            Register = new PaginatedList<Register>(OrderRegisters(UnorderedList.ToList(), sortOrder)
                , UnorderedList.PageIndex, UnorderedList.TotalPages);

        }

        /* OnPost method that sent request */
        public IActionResult OnPost()
        {
            return Page();
        }

        /* Gets the registers by using the type of search choose */
        public IQueryable<Register> GetRegistersByType(IQueryable<Register>? registersQuery)
        {
            IQueryable<Register> resultQuery;

            /* Filter the the register by the type of search choose */
            switch (SearchType)
            {
                case "Nombre":
                default:
                    resultQuery = registersQuery
                        .Where(r => r.ProductName.Contains(SearchString));
                    break;
                case "Marca":
                    resultQuery = registersQuery
                        .Where(r => _context.Products.Any(p => p.Name == r.ProductName && p.Brand.Contains(SearchString)));
                    break;
                case "Modelo":
                    resultQuery = registersQuery
                        .Where(r => _context.Products.Any(p => p.Name == r.ProductName && p.Model.Contains(SearchString)));
                    break;
            }

            resultQuery = resultQuery.GroupBy(r => new { r.ProductName, r.StoreName })
                        .Select(grouped => grouped.OrderByDescending(r => r.SubmitionDate).First());

            return resultQuery;
        }

        /* Order the registers by the sort order choose */
        public List<Register> OrderRegisters(List<Register>? unorderedList, string sortOrder)
        {
            List<Register> orderedList;

            /* Sort the list depending of the parameter */
            switch (sortOrder)
            {
                /* Order in case of price_descending*/
                case "price_desc":
                    orderedList = unorderedList.OrderByDescending(r => r.Price).ToList();
                    break;

                /* Normal order for the price */
                case "price_asc":
                default:
                    orderedList = unorderedList.OrderBy(r => r.Price).ToList();
                    break;
            }

            return orderedList;
        }
    }
}
