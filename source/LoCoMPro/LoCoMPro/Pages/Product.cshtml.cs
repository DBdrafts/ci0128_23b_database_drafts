using LoCoMPro.Data;
using LoCoMPro.Models;
using LoCoMPro.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LoCoMPro.Pages
{
    public class ProductPageModel : PageModel
    {
        /* Context of the data base */
        private readonly LoCoMPro.Data.LoCoMProContext _context;

        /* Configuration for the page */
        private readonly IConfiguration Configuration;
        /* Product Page constructor */
        public ProductPageModel(LoCoMProContext context, IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;
        }

        /* List of the product that exist in the database */
        public IList<Product> Product { get; set; } = default!;

        /* List of the stores that exist in the database */
        public IList<Store> Store { get; set; } = default!;

        /* List of the registers that exist in the database */
        public PaginatedList<Register> Register { get; set; } = default!;

        /* Product resquested name */
        [BindProperty(SupportsGet = true)]
        public string? SearchProductName { get; set; }

        /* Store resquested name */
        [BindProperty(SupportsGet = true)]
        public string? SearchStoreName { get; set; }

        /* Province resquested name */
        [BindProperty(SupportsGet = true)]
        public string? SearchProvinceName { get; set; }

        /* Canton resquested name */
        [BindProperty(SupportsGet = true)]
        public string? SearchCantonName { get; set; }

        public async Task OnGetAsync(string searchProductName, string searchStoreName, string searchProvinceName, 
            string searchCantonName, int? pageIndex)
        {
            /* If the page registers is lower that 1 */
            pageIndex = pageIndex < 1 ? 1 : pageIndex;

            // Search Name to search
            SearchProductName = searchProductName;
            SearchStoreName = searchStoreName;
            SearchProvinceName = searchProvinceName;
            SearchCantonName = searchCantonName;

            // Initial request for all the products in the database
            var products = from p in _context.Products select p;

            // Initial request for all the stores in the database
            var stores = from s in _context.Stores select s;


            // If the name of the productResquested is not null
            if (!string.IsNullOrEmpty(SearchProductName))
            {
                // Delimits products with a where the properties are same 
                products = products.Where(x => x.Name != null && x.Name.Contains(SearchProductName));
            }

            // If the name of the storeResquested is not null
            if (!string.IsNullOrEmpty(SearchStoreName))
            {
                // Delimits stores with a where the properties are same 
                stores = stores.Where(x => x.Name != null && x.Name.Contains(SearchStoreName));
                stores = stores.Where(x => x.CantonName != null && x.CantonName.Contains(SearchCantonName));
                stores = stores.Where(x => x.ProvinciaName != null && x.ProvinciaName.Contains(SearchProvinceName));
            }

            // Gets the Data From Databasse 
            Product = await products.ToListAsync();
            Store = await stores.ToListAsync();

            // Initial request for all the registers in the database
            var registers = from r in _context.Registers select r;

            // If the name of the propertiesSearch is not null 
            if (!string.IsNullOrEmpty(SearchProductName) &&
                !string.IsNullOrEmpty(SearchStoreName) && 
                !string.IsNullOrEmpty(SearchCantonName) &&
                !string.IsNullOrEmpty(SearchProvinceName)) {

                // Delimits registers with the same properties
                registers = registers.Where(x => x.ProductName != null && x.ProductName.Contains(SearchProductName));
                registers = registers.Where(x => x.StoreName != null && x.StoreName.Contains(SearchStoreName));
                registers = registers.Where(x => x.CantonName != null && x.CantonName.Contains(SearchCantonName));
                registers = registers.Where(x => x.ProvinciaName != null && x.ProvinciaName.Contains(SearchProvinceName));

            }

            // Get th amount of pages that will be needed for all the registers 
            var pageSize = Configuration.GetValue("PageSize", 5);

            // Gets the Data From Databasse 
            Register = await PaginatedList<Register>.CreateAsync(
                registers.AsNoTracking(), pageIndex ?? 1, pageSize);

        }



    } 
}