using LoCoMPro.Data;
using LoCoMPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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

        /* OnGet method that manage the GET request */
        public async Task OnGetAsync()
        {
            var categories = from c in _context.Categories
                             select c;

            var registros = from r in _context.Registers
                            select r;

            registros = registros.Where(x => x.ProductName.Contains(SearchString));

            Category = await categories.ToListAsync();
            Register = await registros.ToListAsync();
        }

        /* OnPost method that sent request */
        public IActionResult OnPost()
        {
            return Page();
        }
    }
}
