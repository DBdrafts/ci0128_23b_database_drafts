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
        /// Wether or not the Page is Checked.
        /// </summary>
        [BindProperty]
        public bool IsChecked { get; set; }

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
        /// List of registers that match the Search string.
        /// </summary>
        public PaginatedList<Register> Register { get; set; } = default!;

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
        /// Current type of sort.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? CurrentSort { get; set; }

        /// <summary>
        /// Type of sort by price.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? PriceSort { get; set; }

        /// <summary>
        /// OnGet method that handles the GET request.
        /// </summary>
        /// <param name="pageIndex">Paginated page index to see results of.</param>
        /// <param name="sortOrder">Order to use when showing search results.</param>
        /// <returns></returns>
        public async Task OnGetAsync(int? pageIndex, string sortOrder)
        {

            CurrentSort = GetSortOrder(sortOrder);

            // Prepare the query to retrieve data from the database
            var categories = from c in _context.Categories
                             select c;

            var registers = from r in _context.Registers
                            select r;

           if (Province is not null and not "")
            {
                registers = registers.Where(r => r.ProvinciaName == Province);
                if (Canton is not null and not "")
                {
                    registers = registers.Where(r => r.CantonName == Canton);
                }
            }
            /* Get the number of pages required for all records */
            var pageSize = Configuration.GetValue("PageSize", 5);

            /* Check if SelectedCategories is null, if not, creates a list of the categories separated by ',' in the string */
            List<string> SelectedCategoriesList = !String.IsNullOrEmpty(SelectedCategories) ? SelectedCategories.Split(',').ToList() : null!;
            /* Check if SelectedProvinces is null, if not, creates a list of the provinces separated by ',' in the string */
            List<string> selectedProvincesList = !String.IsNullOrEmpty(SelectedProvinces) ? SelectedProvinces.Split(',').ToList() : null!;
            /* Check if SelectedCantons is null, if not, creates a list of the categories separated by ',' in the string */
            List<string> selectedCantonsList = !String.IsNullOrEmpty(SelectedCantons) ? SelectedCantons.Split(',').ToList() : null!;

            IQueryable<Register> registersMatched = GetRegistersByType(registers);
            /* Filter by categories*/
            if (SelectedCategoriesList != null && SelectedCategoriesList.Count > 0 && SelectedCategoriesList[0] != null)
            {
                /* A list is obtained with the names of all the products associated with any category on the SelectedCategoriesList.*/
                var filteredProducts = _context.Products
                    .Where(p => p.Categories!.Any(c => SelectedCategoriesList.Contains(c.CategoryName)))
                    .Select(p => p.Name)
                    .ToList();

                /* The registers associated with the selected categories are obtained */
                registers = registers.Where(r => filteredProducts.Contains(r.ProductName!));
            }
            registers = FilterByLocation(ref registers, selectedProvincesList, selectedCantonsList);

            /* Get registers based on the selected search type */
            IQueryable<Register> registersQuery = GetRegistersByType(registers);

            /* Retrieve data from the database */
            // Query to get all categories associated with at least one product in the register list
            Category = await categories
                            .Where(category => category.Products!.Any(product =>
                                registersMatched.Any(register => register.ProductName == product.Name)))
                            .ToListAsync();

            // Query to get all provinces associated with at least one register in the register list
            var provincias = _context.Provincias
                            .Where(province => registersMatched.Any(register => register.ProvinciaName == province.Name))
                            .ToList();
            Provinces = provincias;

            // Query to get all cantons associated with at least one register in the register list
            var cantons = _context.Cantones
                            .Where(canton => registersMatched.Any(register => register.CantonName == canton.CantonName))
                            .ToList();
            Cantons = cantons;

            /* Get an unordered list of registers */
            List<Register> unorderedList = OrderRegisters(registersQuery.ToList(), sortOrder);

            /* Create the paginated list of registers from the list */
            Register = (await PaginatedList<Register>.CreateAsync(unorderedList,
                pageIndex ?? 1, pageSize));
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
    }
}
