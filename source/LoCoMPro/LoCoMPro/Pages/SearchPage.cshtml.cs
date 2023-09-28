using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LoCoMPro.Pages
{
    public class SearchPageModel : PageModel
    {
        [BindProperty]
        public bool IsChecked { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            return Page();
        }
    }
}
