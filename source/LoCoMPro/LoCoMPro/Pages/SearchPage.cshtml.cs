using Elfie.Serialization;
using LoCoMPro.Data;
using LoCoMPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;

namespace LoCoMPro.Pages
{
    public class SearchPageModel : PageModel
    {
        /* Determinate if the check-box was activated */
        [BindProperty]
        public bool IsChecked { get; set; }

        /* Context of the data base */
        private readonly LoCoMPro.Data.LoCoMProContext _context;

        /* Search Page constructor */
        public SearchPageModel(LoCoMProContext context)
        {
            _context = context;
        }

        /* List of the categories that exist in the database */
        public IList<Category> Category { get; set; } = default!;

        /* List of the registers that match with the search string */
        public IList<Register> Register { get; set; } = default!;

        /* Text enters as the search attribute */
        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        /* Max amount of elements that can be at the page */
        public int AmountElements { get; set; } = 5;

        /* Index of the actual page */
        public int ActualPage { get; set; }

        /* Total amount of pages */
        public int TotalPages { get; set; }

        /* OnGet method that manage the GET request */
        public async Task OnGetAsync(int pageIndex = 1)
        {
            /* Prepares the query to get the data from the database */
            var categories = from c in _context.Categories
                             select c;

            var registers = from r in _context.Registers
                            select r;

            registers = registers.Where(x => x.ProductName.Contains(SearchString));

            /* Counts the total amount of pages */
            TotalPages = (int)Math.Ceiling(await registers.CountAsync() / (double)AmountElements);

            /* Gets the data from the database */
            Category = await categories.ToListAsync();
            Register = await registers.Skip((pageIndex - 1) * AmountElements).Take(AmountElements).ToListAsync();

            ActualPage = pageIndex;
        }


        /* OnPost method that sent request */
        public IActionResult OnPost()
        {
            return Page();
        }
    }
}
