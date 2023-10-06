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
        public async Task OnGetAsync(string searchType, string searchString, int? pageIndex, string sortOrder)
        {
            /* Si el índice de página es menor que 1 */
            pageIndex = pageIndex < 1 ? 1 : pageIndex;

            /* Obtén el tipo de orden por precio */
            PriceSort = String.IsNullOrEmpty(sortOrder) ? "price_desc" : "";

            SearchString = searchString;
            SearchType = searchType;
            CurrentSort = sortOrder;

            /* Prepara la consulta para obtener los datos de la base de datos */
            var categories = from c in _context.Categories
                             select c;

            var registers = from r in _context.Registers
                            select r;

            /* Obtén la cantidad de páginas que se necesitarán para todos los registros */
            var pageSize = Configuration.GetValue("PageSize", 3);

            /* Obtiene los datos de la base de datos */
            Category = await categories.ToListAsync();

            if (SelectedCategories != null && SelectedCategories.Count > 0 && SelectedCategories[0] != null)
            {
                var filteredProducts = _context.Products
                    .Where(p => p.Categories.Any(c => SelectedCategories.Contains(c.CategoryName)))
                    .Select(p => p.Name)
                    .ToList();

                registers = registers.Where(r => filteredProducts.Contains(r.ProductName));
            }

            /* Obtiene los registros según el tipo de búsqueda seleccionado */
            IQueryable<Register> registersQuery = GetRegistersByType(registers);

            /* Obtiene una lista desordenada de registros */
            PaginatedList<Register> unorderedList = (await PaginatedList<Register>.CreateAsync(
                registersQuery, pageIndex ?? 1, pageSize));

            /* Copia la información de los registros ordenados */
            Register = new PaginatedList<Register>(OrderRegisters(unorderedList.ToList(), sortOrder),
                unorderedList.PageIndex, unorderedList.TotalPages);
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
