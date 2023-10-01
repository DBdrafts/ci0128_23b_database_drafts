using LoCoMPro.Data;
using LoCoMPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LoCoMPro.Pages
{
    public class ProductPageModel : PageModel
    {
        /* Context of the data base */
        private readonly LoCoMPro.Data.LoCoMProContext _context;

        /* Product Page constructor */
        public ProductPageModel(LoCoMProContext context)
        {
            _context = context;
        }

        /* List of the product that exist in the database */
        public IList<Register> Register { get; set; } = default!;

        /* Amount Register Displayed */
        public int amountRegisterDisplayed { get; set; } = 3;

        /* Product resquested name */
        [BindProperty(SupportsGet = true)]
        public string? requestedProductName { get; set; }
        public string? requestedStoreName { get; set; }
        public string? ProductBrand { get; set; }
        public string? ProductModel { get; set; }
        public string? ProductName { get; set; }
        public string? CantonName { get; set; }
        public string? ProvinceName { get; set; }



        public async Task OnGetAsync(string searchProductName, string searchStoreName)
        {
            // Requested Name to search
            requestedProductName = searchProductName;
            requestedStoreName = searchStoreName;
            // Initial request that request all the registers in the database
            var registers = from r in _context.Registers select r;

            // If the name of th productResquested is not null
            if (!string.IsNullOrEmpty(requestedProductName))
            {
                // Delimits registers with a where the properties are same 
                registers = registers.Where(x => x.ProductName != null && x.ProductName.Contains(requestedProductName));
            }

            if (!string.IsNullOrEmpty(requestedStoreName))
            {
                // Delimits registers with a where the properties are same 
                registers = registers.Where(x => x.StoreName != null && x.StoreName.Contains(requestedStoreName));
            }

            Register = await registers.ToListAsync();
        }

    } 
}
