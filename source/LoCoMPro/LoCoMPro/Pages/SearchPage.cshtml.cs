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
using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;

namespace LoCoMPro.Pages
{
    /// <summary>
    /// Page model for SearchPage, handels requests, database access, and preparing data for the page.
    /// </summary>
    public class SearchPageModel : LoCoMProPageModel
    {
        /// <summary>
        /// Creates a new SearchPageModel, requires a context and configuration.
        /// </summary>
        /// <param name="context">DB context to use for page.</param>
        /// <param name="configuration">Configuration for page.</param>
        public SearchPageModel(LoCoMProContext context, IConfiguration configuration)
            : base(context, configuration) { }

        /// <summary>
        /// Categories that the user wants to filter by.
        /// <p>Its string with category names separated by a comma.</p>
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? SelectedCategories{ get; set; }

        /// <summary>
        /// Provinces that the user wants to filter by.
        /// <p>Its string with province names separated by a comma.</p>
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? SelectedProvinces { get; set; }

        /// <summary>
        /// Cantons that the user wants to filter by.
        /// <p>Its string with canton names separated by a comma.</p>
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? SelectedCantons { get; set; }

        /// <summary>
        /// List of the categories that the user can filter by.
        /// </summary>
        public IList<Category> Category { get; set; } = default!;

        /// <summary>
        /// List of the provinces that the user can filter by.
        /// </summary>
        public IList<Provincia> Provinces { get; set; } = default!;

        /// <summary>
        /// List of the cantons that the user can filter by.
        /// </summary>
        public IList<Canton> Cantons { get; set; } = default!;

        /// <summary>
        /// Search String introduced by the user.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        /// <summary>
        /// Type of search the user is performing.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? SearchType { get; set; }

        /// <summary>
        /// Province the user wants to base the search on.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? Province { get; set; }

        /// <summary>
        /// Canton the user wants to base the search on.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? Canton { get; set; }

        /// <summary>
        /// Maps product name to list of categories.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public Dictionary<string, string> CategoryMap { get; set; } = default!;

        /// <summary>
        /// Result of the query.
        /// </summary>
        public IEnumerable<Register>? Registers { get; set; } = new List<Register>();
        /// <summary>
        /// OnGet method that handles the GET request.
        /// </summary>
        /// <param name="pageIndex">Paginated page index to see results of.</param>
        /// <param name="sortOrder">Order to use when showing search results.</param>
        /// <returns></returns>
        public async Task OnGetAsync(int? pageIndex, string sortOrder)
        {

            // Prepare the query to retrieve data from the database
            var categories = from c in _context.Categories
                             select c;

            var registers = from r in _context.Registers
                            where r.Reports.All(report => report.ReportState != 2)
                            select r;

            if (Province is not null and not "")
            {
                registers = registers.Where(r => r.ProvinciaName == Province);
                if (Canton is not null and not "")
                {
                    registers = registers.Where(r => r.CantonName == Canton);
                }
            }

            var match = GetRegistersByType(registers);

            /* Retrieve data from the database */
            // Query to get all categories associated with at least one product in the register list
            Category = await categories
                            .Where(category => category.Products!.Any(product =>
                                match.Any(register => register.ProductName == product.Name)))
                            .ToListAsync();

            // Query to get all provinces associated with at least one register in the register list
            Provinces = _context.Provincias
                            .Where(province => match.Any(register => register.ProvinciaName == province.Name))
                            .ToList();

            // Query to get all cantons associated with at least one register in the register list
            Cantons = _context.Cantones
                            .Where(canton => match.Any(register => register.CantonName == canton.CantonName))
                            .ToList();

            if (match != null)
            {
                // Fetch data from the database
                var productsInRegisters = _context.Products
                    .Where(product => match.Any(register => register.ProductName == product.Name))
                    .Include(product => product.Categories)
                    .ToList();

                //  Gets the registers that match with the categories
                if (productsInRegisters != null)
                {
                    var groupedProductsInRegisters = productsInRegisters
                        .GroupBy(product => product.Name)
                        .Where(group => group.Any(item => item.Categories != null)) // Filter out groups with null Categories
                        .ToDictionary(
                            group => group.Key,  // ProductName as the key
                            group => string.Join(";", group.SelectMany(item => item.Categories!.Select(category => category.CategoryName)))
                        );
                    CategoryMap = groupedProductsInRegisters;
                }

                Registers = match;
            }
        }

        /// <summary>
        /// Filters the <paramref name="registers"/> by the <paramref name="selectedProvinces"/> and the <paramref name="selectedCantons"/>.
        /// </summary>
        /// <param name="registers">Registers to filter.</param>
        /// <param name="selectedProvinces">Provinces to filter the registers by.</param>
        /// <param name="selectedCantons">Cantons to filter the registers by.</param>
        /// <returns>Filtered registers with the given selections.</returns>
        public ref IQueryable<Register> FilterByLocation(ref IQueryable<Register> registers, List<string>? selectedProvinces = null, List<string>? selectedCantons = null)
        {
            // Filter by Province
            if (selectedProvinces != null && selectedProvinces.Count > 0 && selectedProvinces[0] != null)
            {
                /* The registers associated with the Province are obtained */
                registers = registers.Where(r => selectedProvinces.Contains(r.ProvinciaName!));
            }
            // Filter by Canton
            if (selectedCantons != null && selectedCantons.Count > 0 && selectedCantons[0] != null)
            {
                registers = registers.Where(r => selectedCantons.Contains(r.CantonName!));
            }
            return ref registers;
        }

        /// <summary>
        /// OnPost method that sent request.
        /// </summary>
        /// <returns>Redirect to search results page.</returns>
        public IActionResult OnPost()
        {
            return Page();
        }

        /// <summary>
        /// Gets the registers by using the type of search choose.
        /// </summary>
        /// <param name="registersQuery">Registers to base the search on.</param>
        /// <returns></returns>
        public IQueryable<Register> GetRegistersByType(IQueryable<Register>? registersQuery)
        {
            IQueryable<Register> resultQuery;

            // Filter the register by the type of search choose 
            switch (SearchType)
            {
                case "Nombre":
                default:
                    resultQuery = registersQuery!
                        .Where(r => r.ProductName!.Contains(SearchString!));
                    break;
                case "Marca":
                    resultQuery = registersQuery!
                        .Where(r => _context.Products.Any(p => p.Name == r.ProductName && p.Brand!.Contains(SearchString!)));
                    break;
                case "Modelo":
                    resultQuery = registersQuery!
                        .Where(r => _context.Products.Any(p => p.Name == r.ProductName && p.Model!.Contains(SearchString!)));
                    break;
            }

            resultQuery = resultQuery.GroupBy(r => new { r.ProductName, r.StoreName })
                        .Select(grouped => grouped.OrderByDescending(r => r.SubmitionDate).First());

            return resultQuery;
        }

        /// <summary>
        /// Order the registers by the sort order choose.
        /// </summary>
        /// <param name="unorderedList">List of registers to order.</param>
        /// <param name="sortOrder">Type of order to use.</param>
        /// <returns>Ordered list of registers.</returns>
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

        /// <summary>
        /// Gets the sort order of the registers.
        /// </summary>
        /// <param name="sortOrder">Order selected by the user.</param>
        /// <returns><paramref name="sortOrder"/> value if its is not empty or null, 'price_asc' otherwise.</returns>
        public string GetSortOrder(string? sortOrder)
        {
            // If null, the order by price as default 
             return String.IsNullOrEmpty(sortOrder) ? "price_asc" : sortOrder;
        }

        /// <summary>
        /// Gets search results.
        /// </summary>
        /// <returns>Search Results.</returns>
        public JsonResult OnGetRegisters()
        {
            Assert.IsNotNull(Registers);
            return new JsonResult(Registers.ToArray());

        }
    }
}
