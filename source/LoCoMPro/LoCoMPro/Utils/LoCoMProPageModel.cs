using LoCoMPro.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LoCoMPro.Pages
{
    //  General PageModel for LoComPro system
    public class LoCoMProPageModel : PageModel
    {
        // Context of the data base 
        protected readonly LoCoMPro.Data.LoCoMProContext _context;
        // Configuration for the page 
        protected readonly IConfiguration Configuration;

        // LoComPro Page constructor 
        public LoCoMProPageModel(LoCoMProContext context, IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;
        }

        // OnPost method that sent request 
        //public IActionResult OnPost()
        //{
        //    return Page();
        //}
    }
}