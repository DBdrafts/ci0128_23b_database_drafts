using LoCoMPro.Data;
using LoCoMPro.Models;
using LoCoMPro.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LoCoMPro.Pages
{
    /// <summary>
    /// Page model for Product Page, handles requests and business logic.
    /// </summary>
    public class ProductPageModel : LoCoMProPageModel
    {

        /// <summary>
        /// Product Page constructor.
        /// </summary>
        /// <param name="context">Db context to use for page.</param>
        /// <param name="configuration">Configuration for Product Page.</param>
        public ProductPageModel(LoCoMProContext context, IConfiguration configuration)
            : base(context, configuration) { }

        /// <summary>
        /// List of the product that exist in the database.
        /// </summary>
        public IList<Product> Product { get; set; } = default!;

        /// <summary>
        /// List of the stores that exist in the database.
        /// </summary>
        public IList<Store> Store { get; set; } = default!;

        /// <summary>
        /// List of the registers that exist in the database.
        /// </summary> 
        public PaginatedList<Register> Register { get; set; } = default!;

        /// <summary>
        /// Product requested name.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? SearchProductName { get; set; }

        /// <summary>
        /// Store requested name.
        /// </summary> 
        [BindProperty(SupportsGet = true)]
        public string? SearchStoreName { get; set; }

        /// <summary>
        /// Province requested name.
        /// </summary> 
        [BindProperty(SupportsGet = true)]
        public string? SearchProvinceName { get; set; }

        /// <summary>
        /// Canton requested name.
        /// </summary> 
        [BindProperty(SupportsGet = true)]
        public string? SearchCantonName { get; set; }

        /// <summary>
        /// Current sort type.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? CurrentSort { get; set; }

        /// <summary>
        /// Current type of price sort.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? PriceSort { get; set; }

        /// <summary>
        /// Current type of date sort.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? DateSort { get; set; }

        /// <summary>
        /// Handles get request for product page.
        /// </summary>
        /// <param name="searchProductName">Product that needs to be displayed.</param>
        /// <param name="searchStoreName">Name of store that sells the product.</param>
        /// <param name="searchProvinceName">Province where the product is sold.</param>
        /// <param name="searchCantonName">Canton where the product is sold.</param>
        /// <param name="pageIndex">Index of page being displayed.</param>
        /// <param name="sortOrder">Sort order to user when displaying product registers.</param>
        /// <returns>It's a task, threfore no value if returned.</returns>
        public async Task OnGetAsync(string searchProductName, string searchStoreName, string searchProvinceName, 
            string searchCantonName, int? pageIndex, string sortOrder)
        {
            CurrentSort = sortOrder;
            PriceSort = sortOrder == "price" ? "price_desc" : "price";
            
            // if sortOrder is Date, match date else date_desc
            DateSort = sortOrder == "date" ? "date_desc" : "date";

            // Attr of the product from the params of method
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

            // Gets the Data From Data base 
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

            // Code to order
            switch (sortOrder)
            {
                // Order in case of price_descending
                case "price_desc":   
                    registers = registers.OrderByDescending(r => r.Price);
                    break;
                //  Order in case of price
                case "price":     
                    registers = registers.OrderBy(r => r.Price);
                    break;
                // Oldest order for Submition Date
                case "date_desc":
                    registers = registers.OrderBy(r => r.SubmitionDate);
                    break;
                // Normal order for the price
                default:
                    registers = registers.OrderByDescending(r => r.SubmitionDate);
                    break;
            }

            // Get th amount of pages that will be needed for all the registers
            var pageSize = Configuration.GetValue("PageSize", 5);

            // Gets the Data From data base 
            Register = await PaginatedList<Register>.CreateAsync(
                registers.AsNoTracking(), pageIndex ?? 1, pageSize);
        }

        /// <summary>
        /// Orders <paramref name="registers"/> by <paramref name="orderName"/> according to price.
        /// </summary>
        /// <param name="orderName">Order to use for registers.</param>
        /// <param name="registers">Registers that need ordering.</param>
        /// <returns>Ordered registers.</returns>
        public IOrderedEnumerable<Register> OrderRegistersByPrice(string orderName, ref ICollection<Register> registers)
        {
            switch (orderName)
            {
                // Order in case of price_descending 
                case "price_desc":
                    return registers.OrderByDescending(r => r.Price);
                // Normal order for the price
                default:
                    return registers.OrderBy(r => r.Price);
                    
            }
        }

        /// <summary>
        /// Orders <paramref name="registers"/> by <paramref name="orderName"/> according to date.
        /// </summary>
        /// <param name="orderName">Order to use for registers.</param>
        /// <param name="registers">Registers that need ordering.</param>
        /// <returns>Ordered registers.</returns>
        public IOrderedEnumerable<Register> OrderRegistersByDate(string orderName, ref ICollection<Register> registers)
        {
            switch (orderName)
            {
                // Order in case of date_descending 
                case "date_desc":
                    return registers.OrderByDescending(r => r.SubmitionDate);
                // Normal order for the date
                case "date":
                    return registers.OrderBy(r => r.SubmitionDate);
                // Normal order in case date null
                default:
                    return registers.OrderBy(r => r.Price);

            }
        }
    } 
}