using LoCoMPro.Areas.Identity.Pages.Account.Manage;
using LoCoMPro.Data;
using LoCoMPro.Models;
using LoCoMPro.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace LoCoMPro.Pages
{
    /// <summary>
    /// Page model for Product Page, handles requests, and access to database to show Web Page.
    /// </summary>
    public class ProductPageModel : LoCoMProPageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Reference to the user product list
        /// </summary>
        public UserProductList _userProductList { get; set; }

        /// <summary>
        /// Creates a new ProductPageModel.
        /// </summary>
        /// <param name="context">DB Context to pull data from.</param>
        /// <param name="configuration">Configuration for page.</param>
        /// <param name="userManager">User manager to handle user permissions.</param>
        /// <param name="httpContextAccessor">Allow access to the http context
        // Product Page constructor 
        public ProductPageModel(LoCoMProContext context, IConfiguration configuration
            , UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
            : base(context, configuration)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _userProductList = new UserProductList(_httpContextAccessor);
        }

        /// <summary>
        /// List of the product that exist in the database.
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
        /// List of the review made by the user that exist in the database.
        /// </summary>
        public IList<Review> UserReviews = new List<Review>();

        /// <summary>
        /// List of the product that are in the list of the user
        /// </summary>
        public IList<Register> UserProductList = new List<Register>();

        /// <summary>
        /// List of the reports made by the user that exist in the database.
        /// </summary>
        public IList<Report> UserReports = new List<Report>();

        /// <summary>
        /// List of the registers that exist in the database.
        /// </summary>
        public IEnumerable<Register>? Registers { get; set; } = new List<Register>();

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
        /// Avg calculated price for product.
        /// </summary>
        public decimal AvgPrice { get; set; }

        /// <summary>
        /// Flag to know if the product is in the list already
        /// </summary>
        public bool AlreadyInProductList { get; set; }

        /// <summary>
        /// Number of results.
        /// </summary>
        public int ResultsNumber;

        /// <summary>
        /// GET HTTP request, initializes page values.
        /// </summary>
        /// <param name="searchProductName">Product to display data of.</param>
        /// <param name="searchStoreName">Store where the product is sold.</param>
        /// <param name="searchProvinceName">Province where the store is located.</param>
        /// <param name="searchCantonName">Canton where the store is located.</param>
        /// <returns></returns>
        public async Task OnGetAsync(string searchProductName, string searchStoreName, string searchProvinceName, 
            string searchCantonName)
        {

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

            // add the images from every register
            registers = registers.Include(r => r.Images);

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
                registers = registers.Include(r => r.Images);

            }      

            // Get the average of the registers within last month.
            AvgPrice = GetAveragePrice(registers, DateTime.Now.AddYears(-1).Date, DateTime.Now) ;

            List<string> userIds = registers.Select(r => r.ContributorId).Distinct().ToList()!;
            Users = await _context.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();

            ResultsNumber = registers.Count();

            // Gets the Data From data base 
            Registers = await registers.ToListAsync();

            // Obtains the review made by the user
            ObtainUserReviews();

            // Obtains the reports made by the user
            ObtainUserReports();

            // Prepare the list data needed
            PrepareProductListData();
        }

        /// <summary>
        /// Orders <paramref name="registers"/> by <paramref name="orderName"/> price.
        /// </summary>
        /// <param name="orderName">Type of order to use.</param>
        /// <param name="registers">Registers to order.</param>
        /// <returns>Registers ordered by <paramref name="orderName"/> price.</returns>
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
        /// <returns>Registers ordered by <paramref name="orderName"/> date.</returns>
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
        /// Gets and sets the review made by the User
        /// </summary>
        public async void ObtainUserReviews()
        {
            // Gets the user that is registered
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            // If there´s is a registered user
            if (user != null)
            {
                // Gets all the reviews
                var reviews = from p in _context.Reviews select p;

                // Filter the reviews to gets the made by the user in this product and store
                reviews = reviews.Where(x => x.ReviewerId == user.Id);
                reviews = reviews.Where(x => x.ProductName != null && x.ProductName.Contains(SearchProductName));
                reviews = reviews.Where(x => x.StoreName != null && x.StoreName.Contains(SearchStoreName));

                // Make a list with the review
                UserReviews = reviews.ToList();
            }
        }

        /// <summary>
        /// Prepares the data needed to work with the list
        /// </summary>
        public void PrepareProductListData()
        {
            // Gets the product and store
            Product firstNonNullProduct = Product.FirstOrDefault(r => r.Name != null);
            Store firstNonNullStore = Store.FirstOrDefault(r => r.Name != null);

            // Creates the element of the list 
            UserProductListElement ProductAsElement = new UserProductListElement(
                firstNonNullProduct.Name, firstNonNullProduct.Brand
                , firstNonNullProduct.Model, firstNonNullStore.Name
                , firstNonNullStore.ProvinciaName, firstNonNullStore.CantonName
                , AvgPrice.ToString("N0"));

            ProductAsElement.ProductBrand = ProductAsElement.ProductBrand ?? "";
            ProductAsElement.ProductModel = ProductAsElement.ProductModel ?? "";

            // Checks if the product is already in the user list
            if (_userProductList.ExistElementInList(ProductAsElement))
            {
                AlreadyInProductList = true;
            }
        }

        /// <summary>
        /// Add the product to the user list
        /// <param name="productData">The data of the product</param>
        /// </summary>
        public IActionResult OnPostAddToProductList(string productData)
        {
            // Gets and split the data
            string[] values = SplitString(productData, '\x1F');

            var newElement = new UserProductListElement(values[0], values[1], values[2]
                , values[3], values[4], values[5], values[6]);

            // If the element is not in the list
            if (!_userProductList.ExistElementInList(newElement))
            {
                // Adds the element to the list
                _userProductList.AddProductToList(newElement);
            }

            return new JsonResult("OK");
        }

        /// <summary>
        /// Delete the product from the user list
        /// </summary>
        public IActionResult OnPostRemoveFromProductList(string productData)
        {
            // Gets and split the data
            string[] values = SplitString(productData, '\x1F');

            var removeElement = new UserProductListElement(values[0], values[1], values[2]
                , values[3], values[4], values[5], values[6]);

            // If the element is not in the list
            if (_userProductList.ExistElementInList(removeElement))
            {
                // Adds the element to the list
                _userProductList.RemoveProductFromList(removeElement);
            }

            return new JsonResult("OK");
        }

        /// <summary>
        /// Handle report interactions
        /// </summary>
        public async void ObtainUserReports()
        {
            // Gets the user that is registered
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            // If there´s is a registered user
            if (user != null)
            {
                // Gets all the reviews
                var reports = from p in _context.Reports select p;

                // Filter the reviews to gets the made by the user in this product and store
                reports = reports.Where(x => x.ReporterId == user.Id);
                reports = reports.Where(x => x.ProductName != null && x.ProductName.Contains(SearchProductName));
                reports = reports.Where(x => x.StoreName != null && x.StoreName.Contains(SearchStoreName));

                // Make a list with the review
                UserReports = reports.ToList();
            }
        }

        /// <summary>
        /// Handle report interactions (review, report or both).
        /// </summary>
        /// <param name="registerKeys">Foreign keys for identification the specific register.</param>
        /// <param name="reportChanged">Bool to check if a report changed.</param>
        /// <param name="reviewedValue">Float with register review.</param>
        /// <returns>Success message to clients side.</returns>
        public IActionResult OnPostHandleInteraction(string registerKeys, bool reportChanged, float reviewedValue)
        {
            string[] values = SplitString(registerKeys, '\x1F');
            string submitionDate = values[0], contributorId = values[1], productName = values[2], storeName = values[3];
            
            DateTime registSubmitDate = DateTime.Parse(submitionDate);
            var user = _context.Users.FirstOrDefault(u => u.Id == _userManager.GetUserId(User));
            DateTime interactionDate = FormatDateTimeNow();

            var registerToUpdate = GetRegisterToUpdate(contributorId, productName, storeName, registSubmitDate);

            if (reportChanged)
            {
                HandleReport(user!, registerToUpdate, interactionDate, contributorId,
                    productName, storeName, registSubmitDate);
            }

            if (reviewedValue > 0)
            {
                HandleReview(user!, registerToUpdate, interactionDate, contributorId,
                    productName, storeName, registSubmitDate, reviewedValue);
            }

            _context.SaveChanges();

            return new JsonResult("OK");
        }


        /// <summary>
        /// Gets the register on which an interaction was performed.
        /// </summary>
        /// <param name="contributorId">ID of the user associated with the register.</param>
        /// <param name="productName">Product associated with the register.</param>
        /// <param name="storeName">Store associated with the register.</param>
        /// <param name="registSubmitDate">Date and time the registration was made.</param>
        /// <returns>Register to update.</returns>
        private Register GetRegisterToUpdate(string contributorId, string productName,
            string storeName, DateTime registSubmitDate)
        {
            return _context.Registers.Include(r => r.Contributor).First(r => r.ContributorId == contributorId
                && r.ProductName == productName && r.StoreName == storeName && r.SubmitionDate == registSubmitDate);
        }

        /// <summary>
        /// Returns current dateTime without mili, micro and nanoseconds.
        /// </summary>
        /// <returns>Current dateTime formatted.</returns>
        static private DateTime FormatDateTimeNow()
        {
            DateTime dateTimeNow = DateTime.Now;
            dateTimeNow = new DateTime(dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day,
                dateTimeNow.Hour, dateTimeNow.Minute, dateTimeNow.Second, 0);
            return dateTimeNow;
        }

        /// <summary>
        /// Splits a string into an array of substrings based on the specified delimiter character.
        /// </summary>
        /// <param name="input">The input string to split.</param>
        /// <param name="delimiter">The character used as the delimiter.</param>
        /// <returns>An array of substrings created by splitting the input string.</returns>
        static string[] SplitString(string input, char delimiter)
        {
            return input.Split(delimiter);
        }

        /// <summary>
        /// Handle a report made by a user about a register.
        /// </summary>
        /// <param name="user">User that made the report.</param>
        /// <param name="registerToUpdate">Register to which the report was made.</param>
        /// <param name="registSubmitDate"><See cref="GetRegisterToUpdate"/>).</param>
        /// <param name="interactionDate">Date and time the report is made.</param>
        /// <param name="contributorId"><See cref="GetRegisterToUpdate"/>).</param>
        /// <param name="productName"><See cref="GetRegisterToUpdate"/>).</param>
        /// <param name="storeName"><See cref="GetRegisterToUpdate"/>).</param>
        private void HandleReport(User user, Register registerToUpdate, DateTime interactionDate,
           string contributorId, string productName, string storeName, DateTime registSubmitDate)
        {
            var lastReport = _context.Reports.FirstOrDefault(r => r.ReporterId == user!.Id
                && r.ProductName == productName
                && r.StoreName == storeName
                && r.SubmitionDate == registSubmitDate
                && r.ContributorId == contributorId);

            if (lastReport == null)
            {
                _context.Reports.Add(new Report
                {
                    ReportedRegister = registerToUpdate,
                    Reporter = user!,
                    ReportDate = interactionDate,
                    CantonName = registerToUpdate.CantonName!,
                    ProvinceName = registerToUpdate.ProvinciaName!,
                    ReportState = User.IsInRole("Moderator") ? 2 : 1
                });
            }
            else
            {
                _context.Reports.Remove(lastReport);
            }
        }

        /// <summary>
        /// Handle a review made by a user about a register.
        /// </summary>
        /// <param name="user">User that made the review.</param>
        /// <param name="registerToUpdate">Register to which the review was made.</param>
        /// <param name="registSubmitDate"><See cref="GetRegisterToUpdate"/>).</param>
        /// <param name="interactionDate">Date and time the review is made.</param>
        /// <param name="contributorId"><See cref="GetRegisterToUpdate"/>).</param>
        /// <param name="productName"><See cref="GetRegisterToUpdate"/>).</param>
        /// <param name="storeName"><See cref="GetRegisterToUpdate"/>).</param>
        /// <param name="reviewedValue"><See cref="OnPostHandleInteraction"/>).</param>
        private void HandleReview(User user, Register registerToUpdate, DateTime interactionDate, 
            string contributorId, string productName, string storeName, DateTime registSubmitDate, float reviewedValue)
        {
            var lastReview = _context.Reviews.FirstOrDefault(r => r.ReviewerId == user.Id
                && r.ProductName == productName
                && r.StoreName == storeName
                && r.SubmitionDate == registSubmitDate
                && r.ContributorId == contributorId);

            if (lastReview == null)
            {
                _context.Reviews.Add(new Review() { ReviewedRegister = registerToUpdate,
                    Reviewer = user!, ReviewValue = reviewedValue, ReviewDate = interactionDate,
                    CantonName = registerToUpdate.CantonName!, ProvinceName = registerToUpdate.ProvinciaName!
                });
            }
            else
            {
                lastReview.ReviewValue = reviewedValue;
                lastReview.ReviewDate = interactionDate;
            }
        }
    }   
}