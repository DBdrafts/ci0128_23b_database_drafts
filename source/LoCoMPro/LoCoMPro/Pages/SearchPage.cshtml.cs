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

        [BindProperty(SupportsGet = true)]
        public string? SelectedCategories{ get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SelectedProvinces { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SelectedCantons { get; set; }

        /* List of the categories that exist in the database */
        public IList<Category> Category { get; set; } = default!;

        /* List of the Provinces that exist in the database */
        public IList<Provincia> Provinces { get; set; } = default!;
        /* List of the Provinces that exist in the database */
        public IList<Canton> Cantons { get; set; } = default!;

        // List of the registers that match with the search string 
        public PaginatedList<Register> Register { get; set; } = default!;

        // Text enters as the search attribute 
        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        // Text enters as the search type attribute 
        [BindProperty(SupportsGet = true)]
        public string? SearchType { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? Province { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? Canton { get; set; }

        // Current type of sort 
        [BindProperty(SupportsGet = true)]
        public string? CurrentSort { get; set; }

        // Type of sort by price 
        [BindProperty(SupportsGet = true)]
        public string? PriceSort { get; set; }

        /* OnGet method that handles the GET request */
        public async Task OnGetAsync(int? pageIndex, string sortOrder)
        {

            PriceSort = GetSortOrder(sortOrder);

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
            PaginatedList<Register> unorderedList = (await PaginatedList<Register>.CreateAsync(
                registersQuery, pageIndex ?? 1, pageSize));

            /* Copy the information of ordered registers */
            Register = new PaginatedList<Register>(OrderRegisters(unorderedList.ToList(), sortOrder),
                unorderedList.PageIndex, unorderedList.TotalPages);
        }


        public ref IQueryable<Register> FilterByLocation(ref IQueryable<Register> registers, List<string> selectedProvinces, List<string> selectedCantons)
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

        /* OnPost method that sent request */
        public IActionResult OnPost()
        {
            return Page();
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
