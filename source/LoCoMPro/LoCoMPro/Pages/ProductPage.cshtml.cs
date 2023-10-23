using LoCoMPro.Data;
using LoCoMPro.Models;
using LoCoMPro.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace LoCoMPro.Pages
{
    /// <summary>
    /// Page model for Product Page, handles requests, and access to database to show Web Page.
    /// </summary>
    public class ProductPageModel : LoCoMProPageModel
    {
        
        /// <summary>
        /// Creates a new ProductPageModel.
        /// </summary>
        /// <param name="context">DB Context to pull data from.</param>
        /// <param name="configuration">Configuration for page.</param>
        // Product Page constructor 
        public ProductPageModel(LoCoMProContext context, IConfiguration configuration)
            : base(context, configuration) { }

        /// <summary>
        /// List of the product that exist in the databas.
        /// </summary>
        public IList<Product> Product { get; set; } = default!;

        /// <summary>
        /// List of the stores that exist in the database.
        /// </summary>
        public IList<Store> Store { get; set; } = default!;

        /// <summary>
        /// List of the users that exist in the database.
        /// </summary>
        public IList<User> Users { get; set; } = default!;

        /// <summary>
        /// List of the registers that exist in the database.
        /// </summary>
        public PaginatedList<Register> Register { get; set; } = default!;

        /// <summary>
        /// Requested product name.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? SearchProductName { get; set; }

        /// <summary>
        /// Requested store name.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? SearchStoreName { get; set; }

        /// <summary>
        /// Requested province name.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? SearchProvinceName { get; set; }

        /// <summary>
        /// Requested canton name.
        /// </summary> 
        [BindProperty(SupportsGet = true)]
        public string? SearchCantonName { get; set; }

        /// <summary>
        /// Current sort being used by sort.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? CurrentSort { get; set; }

        /// <summary>
        /// Price sort.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? PriceSort { get; set; }

        /// <summary>
        /// Attr for sort the date register.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? DateSort { get; set; }

        /// <summary>
        /// Avg calculated price for product.
        /// </summary>
        public decimal AvgPrice { get; set; }

        /// <summary>
        /// GET HTTP request, initializes page values.
        /// </summary>
        /// <param name="searchProductName">Product to desplay data of.</param>
        /// <param name="searchStoreName">Store where the product is sold.</param>
        /// <param name="searchProvinceName">Province where the store is located.</param>
        /// <param name="searchCantonName">Canton where the store is located.</param>
        /// <param name="pageIndex">Page index of registers to visualize.</param>
        /// <param name="sortOrder">Order to use when showing values.</param>
        /// <returns></returns>
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

            

            // Get the average of the registers within last month.
            AvgPrice = GetAveragePrice(registers, DateTime.Now.AddYears(-1).Date, DateTime.Now) ;

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

            List<string> userIds = registers.Select(r => r.ContributorId).Distinct().ToList()!;
            Users = await _context.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();

            // Gets the Data From data base 
            Register = await PaginatedList<Register>.CreateAsync(
                registers.AsNoTracking(), pageIndex ?? 1, pageSize);
        }

        /// <summary>
        /// Orders <paramref name="registers"/> by <paramref name="orderName"/> price.
        /// </summary>
        /// <param name="orderName">Type of order to use.</param>
        /// <param name="registers">Registers to order.</param>
        /// <returns>Reqgisters ordered by <paramref name="orderName"/> price.</returns>
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
        /// Orders <paramref name="registers"/> by <paramref name="orderName"/> date.
        /// </summary>
        /// <param name="orderName">Type of order to use.</param>
        /// <param name="registers">Registers to order.</param>
        /// <returns>Reqgisters ordered by <paramref name="orderName"/> date.</returns>
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

        /// <summary>
        /// Gets the average price of the registers within the given time frame.
        /// </summary>
        /// <param name="registers">Registers to use for calculation.</param>
        /// <param name="from">Starting date to take registers from for calculation.</param>
        /// <param name="to">Ending date to take the registers from for calculation.</param>
        /// <returns>Average price of the registers within the given time frame.</returns>
        public decimal GetAveragePrice(IQueryable<Register> registers, DateTime? from, DateTime? to)
        {
            if (from != null && to != null)
            {
                registers = registers.Where(r => (r.SubmitionDate >= from) && (r.SubmitionDate <= to));
            }
            double avgPrice = (registers is not null && registers.Count() > 1) ? registers.Average(r => r.Price) : 0.0;
            return Convert.ToDecimal(avgPrice);
        }

        /// <summary>
        /// Hanldle report interactions
        /// </summary>
        /// <param registerKeys="from"> foreign keys for identification the specific register.</param>
        /// 
        public IActionResult OnPostHandleInteraction(string registerKeys)
        {
            string[] values = SplitString(registerKeys, '\x1F'); // Splits the string with the char31 as a delimitator
            string submitionDate = values[0], contributorId = values[1], productName = values[2], storeName = values[3];
            DateTime dateTime = DateTime.Parse(submitionDate);

            var registerToUpdate = _context.Registers.Include(r => r.Contributor).First(r => r.ContributorId == contributorId
                && r.ProductName == productName && r.StoreName == storeName && r.SubmitionDate == dateTime);

            uint reportValue = 1;
            
            // TODO: Get Rol

            /* This is just an example!
            userRol = getUserRol(); 
            if (userRol == mod)
            {
                reportValue = 2;
            }
            */

            registerToUpdate.NumCorrections = reportValue;
            _context.SaveChanges();
            return new JsonResult("OK");
        }

        static string[] SplitString(string input, char delimiter)
        {
            return input.Split(delimiter);
        }

    }
}