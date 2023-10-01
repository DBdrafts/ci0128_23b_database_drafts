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
        public IList<Product> Product { get; set; } = default!;

        /* List of the stores that exist in the database */
        public IList<Store> Store { get; set; } = default!;

        /* List of the registers that exist in the database */
        public IList<Register> Register { get; set; } = default!;

        /* Amount Register Displayed */
        public int amountRegisterDisplayed { get; set; } = 3;

        /* Product resquested name */
        [BindProperty(SupportsGet = true)]
        public string? requestedProductName { get; set; }
        public string? requestedStoreName { get; set; }

        public async Task OnGetAsync(string searchProductName, string searchStoreName)
        {
            // Requested Name to search
            requestedProductName = searchProductName;
            requestedStoreName = searchStoreName;

            // Initial request for all the products in the database
            var products = from p in _context.Products select p;

            // Initial request for all the stores in the database
            var stores = from s in _context.Stores select s;


            // If the name of the productResquested is not null
            if (!string.IsNullOrEmpty(requestedProductName))
            {
                // Delimits products with a where the properties are same 
                products = products.Where(x => x.Name != null && x.Name.Contains(requestedProductName));
            }

            // If the name of the storeResquested is not null
            if (!string.IsNullOrEmpty(requestedStoreName))
            {
                // Delimits stores with a where the properties are same 
                stores = stores.Where(x => x.Name != null && x.Name.Contains(requestedStoreName));
            }
            
            // Gets the Data From Databasse 
            Product = await products.ToListAsync();
            Store = await stores.ToListAsync();

            // Initial request for all the registers in the database
            var registers = from r in _context.Registers select r;

            // If the name of the propertiesRequested is not null 
            if (!string.IsNullOrEmpty(requestedProductName) &&
                !string.IsNullOrEmpty(requestedStoreName)) {

                // Delimits registers with the same properties
                registers = registers.Where(x => x.ProductName != null && x.ProductName.Contains(requestedProductName));
                registers = registers.Where(x => x.StoreName != null && x.StoreName.Contains(requestedStoreName));
            }

            // Gets the Data From Databasse 
            Register = await registers.ToListAsync();

        }

    } 
}
