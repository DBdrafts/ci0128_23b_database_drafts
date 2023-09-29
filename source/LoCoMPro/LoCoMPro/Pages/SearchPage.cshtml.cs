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
        [BindProperty]
        public bool IsChecked { get; set; }

        private readonly LoCoMPro.Data.LoCoMProContext _context;

        public SearchPageModel(LoCoMProContext context)
        {
            _context = context;
        }

        public IList<Category> Category { get; set; } = default!;

        public IList<Register> Register { get; set; } = default!;


        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

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

        public IActionResult OnPost()
        {
            return Page();
        }
    }
}
