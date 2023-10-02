using Elfie.Serialization;
using LoCoMPro.Data;
using LoCoMPro.Models;
using LoCoMPro.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        /* OnGet method that manage the GET request */
        public async Task OnGetAsync(string searchType, string searchString, int? pageIndex)
        {
            /* If the page index is lower that 1 */
            pageIndex = pageIndex < 1 ? 1: pageIndex;

            SearchString = searchString;

            SearchType = searchType;

            /* Prepares the query to get the data from the database */
            var categories = from c in _context.Categories
                             select c;

            var registers = from r in _context.Registers
                            select r;

            GetRegistersByType(ref registers);


            /* Get th amount of pages that will be needed for all the registers */
            var pageSize = Configuration.GetValue("PageSize", 5);

            /* Gets the data from the database */
            Category = await categories.ToListAsync();
            Register = await PaginatedList<Register>.CreateAsync(
                registers.AsNoTracking(), pageIndex ?? 1, pageSize);
        }

        public void GetRegistersByType(ref IQueryable<Register>? registers)
        {
            switch (SearchType)
            {
                case "Nombre":
                default:
                    registers = from r in _context.Registers
                                where r.ProductName.Contains(SearchString)
                                group r by new { r.ProductName, r.StoreName } into grouped
                                select grouped.OrderByDescending(r => r.SubmitionDate).First();
                    break;
            }
        }

        /* OnPost method that sent request */
        public IActionResult OnPost()
        {
            return Page();
        }
    }
}
