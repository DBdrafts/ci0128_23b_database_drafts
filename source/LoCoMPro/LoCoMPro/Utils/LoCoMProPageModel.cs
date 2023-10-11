using LoCoMPro.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LoCoMPro.Pages
{
    /// <summary>
    /// General PageModel for LoComPro system.
    /// </summary>
    public class LoCoMProPageModel : PageModel
    {
        /// <summary>
        /// Context to use for page.
        /// </summary>
        protected readonly LoCoMPro.Data.LoCoMProContext _context;

        /// <summary>
        /// Configuration for the page.
        /// </summary>
        protected readonly IConfiguration Configuration;

        /// <summary>
        /// LoComPro Page constructor.
        /// </summary>
        /// <param name="context">DB context to use for page.</param>
        /// <param name="configuration">Configuration for page.</param>
        public LoCoMProPageModel(LoCoMProContext context, IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;
        }
    }
}