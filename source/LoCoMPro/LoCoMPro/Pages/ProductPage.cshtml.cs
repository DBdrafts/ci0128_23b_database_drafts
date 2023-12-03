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
using Microsoft.Win32;
using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using static System.Runtime.InteropServices.JavaScript.JSType;


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
        /// <param name="userManager">User manager to handle user permissions.</param>
        /// <param name="httpContextAccessor">Allow access to the http context
        public ProductPageModel(LoCoMProContext context, IConfiguration configuration
            , UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
            : base(context, configuration)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _userProductList = new UserProductList(_httpContextAccessor);
        }
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Reference to the user product list
        /// </summary>
        public UserProductList _userProductList { get; set; }

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
        /// Average review value of the registers
        /// </summary>
        public IList<float> registerAverageReview { get; set; }

        /// <summary>
        /// Number of reviews a register has
        /// </summary>
        public IList<int> registerReviewCount {  get; set; }

        /// <summary>
        /// Number of results.
        /// </summary>
        public User? UserInPage;

        /// <summary>
        /// GET HTTP request, initializes page values.
        /// </summary>
        /// <param name="searchProductName">Product to display data of.</param>
        /// <param name="searchStoreName">Store where the product is sold.</param>
        /// <param name="searchProvinceName">Province where the store is located.</param>
        /// <param name="searchCantonName">Canton where the store is located.</param>
        public async Task OnGetAsync(string searchProductName, string searchStoreName, string searchProvinceName, 
            string searchCantonName)
        {
            if (User != null && _userManager != null)
            {
                // Get the user in the page
                UserInPage = await _userManager.GetUserAsync(User);
                if (UserInPage != null) {
                    _userProductList.SetListName(UserInPage.Id);
                }
            }

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

            // Initial request for all the registers in the database if the reportState is not 2
            var registers = from r in _context.Registers
                            where r.Reports.All(report => report.ReportState != 2)
                            select r;

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
            // If just one, set the average price as that

            if (registers.Any())
            {
                AvgPrice = GetNumberOfRegisters(registers) > 1
                ? GetAveragePrice(registers, DateTime.Now.AddYears(-1).Date, DateTime.Now)
                : Convert.ToDecimal(registers.First().Price);
            }
            else
            {
                AvgPrice = 0;
            }


            List<string> userIds = registers.Select(r => r.ContributorId).Distinct().ToList()!;
            Users = await _context.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();

            // Gets the Data From data base 
            Registers = await registers.ToListAsync();

            // Obtains the review made by the user
            ObtainUserReviews(UserInPage);

            // Obtains the reports made by the user
            ObtainUserReports(UserInPage);

            // Prepare the list data needed
            PrepareProductListData();

            // Gets the average review value of the registers
            ObtainAverageReviewValues();
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
            double avgPrice = (registers is not null) ? registers.Average(r => r.Price) : 0.0;
            return Convert.ToDecimal(avgPrice);
        }

        /// <summary>
        /// Obtains the averages review values of the registers
        /// </summary>
        public void ObtainAverageReviewValues()
        {
            registerAverageReview = new List<float>();
            registerReviewCount = new List<int>();
            // Gets the average value for each register
            foreach (Register register in Registers!)
            {
                registerAverageReview.Add(_context.GetAverageReviewValue(register.ContributorId
                    , register.ProductName, register.StoreName, register.SubmitionDate));
                registerReviewCount.Add(_context.GetReviewCount(register));
            }
        }

        /// <summary>
        /// Gets the amount of registers of the product.
        /// </summary>
        /// <param name="registers">Registers to use for calculation.</param>
        public int GetNumberOfRegisters(IQueryable<Register> registers)
        {
            return registers.Count();
        }

        /// <summary>
        /// Gets and sets the review made by the User
        /// </summary>
        public void ObtainUserReviews(User user)
        {
            // If there�s is a registered user
            if (user != null)
            {
                // Gets all the reviews
                var reviews = from p in _context.Reviews select p;

                // Filter the reviews to gets the made by the user in this product and store
                reviews = reviews.Where(x => x.ReviewerId == user.Id);
                reviews = reviews.Where(x => x.ProductName != null && x.ProductName.Contains(SearchProductName));
                reviews = reviews.Where(x => x.StoreName != null && x.StoreName.Contains(SearchStoreName));
                reviews = reviews.Where(x => x.ProvinceName != null && x.ProvinceName.Contains(SearchProvinceName));
                reviews = reviews.Where(x => x.CantonName != null && x.CantonName.Contains(SearchCantonName));

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

            ProductAsElement.ProductBrand = ProductAsElement.ProductBrand ?? "N/A";
            ProductAsElement.ProductModel = ProductAsElement.ProductModel ?? "N/A";

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
            
            _userProductList.SetListName(_userManager.GetUserId(User));

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
        public void ObtainUserReports(User user)
        {
            // If there�s is a registered user
            if (user != null)
            {
                // Gets all the reviews
                var reports = from p in _context.Reports select p;

                // Filter the reviews to gets the made by the user in this product and store
                reports = reports.Where(x => x.ReporterId == user.Id);
                reports = reports.Where(x => x.ProductName != null && x.ProductName.Contains(SearchProductName));
                reports = reports.Where(x => x.StoreName != null && x.StoreName.Contains(SearchStoreName));
                reports = reports.Where(x => x.ProvinceName != null && x.ProvinceName.Contains(SearchProvinceName));
                reports = reports.Where(x => x.CantonName != null && x.CantonName.Contains(SearchCantonName));

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
        /// <param name="reportComment">Reason of the report</param>
        /// <returns>Success message to clients side.</returns>
        public IActionResult OnPostHandleInteraction(string registerKeys, bool reportChanged, string reviewedValue, string reportComment)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;
            float.TryParse(reviewedValue, NumberStyles.Float, culture, out float reviewedValueF);
            var (user, registerToUpdate, interactionDate) = GetInteractionValues(registerKeys);

            if (reportChanged)
            {
                HandleReport(user!, registerToUpdate, interactionDate, reportComment);
            }

            if (reviewedValueF > 0)
            {
                HandleReview(user!, registerToUpdate, interactionDate, reviewedValueF);
            }

            _context.SaveChanges();

            return new JsonResult("OK");
        }

        /// <summary>
        /// Process the keys and gets the values to handle the interaction.
        /// </summary>
        /// <param name="registerKeys">Foreign keys for identification the specific register.</param>
        /// <returns>The register to update, the DateTime interaction and the user that made the interaction.</returns>
        private (User?, Register, DateTime) GetInteractionValues(string registerKeys)
        {
            // Get the keys to indeficade the register by splitting the keys string
            string[] values = SplitString(registerKeys, '\x1F');
            string submitionDate = values[0], contributorId = values[1], productName = values[2], storeName = values[3];

            // Convert the string submitionDate type to a dateTime type
            DateTime registSubmitDate = DateTime.Parse(submitionDate);
            // Get the user in the actual sesion
            var user = _context.Users.FirstOrDefault(u => u.Id == _userManager.GetUserId(User));

            // Get the actual interaction time
            DateTime interactionDate = TruncateSubSeconds(DateTime.Now);

            var registerToUpdate = GetRegisterToUpdate(contributorId, productName, storeName, registSubmitDate);
            return (user, registerToUpdate, interactionDate);
        }

        /// <summary>
        /// Splits a string into an array of substrings based on the specified delimiter character.
        /// </summary>
        /// <param name="input">The input string to split.</param>
        /// <param name="delimiter">The character used as the delimiter.</param>
        /// <returns>An array of substrings created by splitting the input string.</returns>
        internal static string[] SplitString(string input, char delimiter)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("Input string cannot be empty or null.");
            }

            return input.Split(delimiter);
        }

        /// <summary>
        /// Returns a dateTime variable without mili, micro and nanoseconds.
        /// </summary>
        /// <returns>Current dateTime formatted.</returns>
        internal static DateTime TruncateSubSeconds(DateTime dateTime)
        {
            if (dateTime <= DateTime.MinValue)
            {
                throw new ArgumentException("Invalid dateTimeValue");
            }

            dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day,
                dateTime.Hour, dateTime.Minute, dateTime.Second, 0);
            return dateTime;
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
        /// Handle a report made by a user about a register.
        /// </summary>
        /// <param name="user">User that made the report.</param>
        /// <param name="registerToUpdate">Register to which the report was made.</param>
        /// <param name="reportComment">Reason of the report</param>
        /// <param name="interactionDate">Date and time the report is made.</param>
        private void HandleReport(User user, Register registerToUpdate, DateTime interactionDate, string reportComment)
        {
            var lastReport = PreviousReport(user, registerToUpdate);
            if (lastReport == null)
            {
                _context.Reports.Add(new Report
                {
                    ReportedRegister = registerToUpdate,
                    Reporter = user!,
                    ReportDate = interactionDate,
                    CantonName = registerToUpdate.CantonName!,
                    ProvinceName = registerToUpdate.ProvinciaName!,
                    ReportState = 1,
                    Reason = reportComment
                });
            }
            else
            {
                _context.Reports.Remove(lastReport);
            }
        }

        /// <summary>
        /// returns the previous report if the user had already made one for that register
        /// <param name="register">Register to  check if the user has already made a report </param>
        /// <param name="user">User to check if they have already made a report to the register</param>
        /// <returns>The report of the register or null if the user has not already made a report for the register.</returns>
        /// </summary>
        private Report? PreviousReport(User user, Register register)
        {
            return _context.Reports.FirstOrDefault(r => r.ReporterId == user!.Id
                && r.ProductName == register.ProductName
                && r.StoreName == register.StoreName
                && r.SubmitionDate == register.SubmitionDate
                && r.ContributorId == register.ContributorId);
        }

        /// <summary>
        /// Returns the previous review if the user has already made one for that register.
        /// </summary>
        /// <param name="user">User to check if they have already made a report to the register.</param>
        /// <param name="register">Register to check if the user has already made a report.</param>
        /// <returns>The review that the user made or null if the user has not made one.</returns>
        private Review? PreviousReview(User user, Register register)
        {
            return _context.Reviews.FirstOrDefault(r => r.ReviewerId == user!.Id
                && r.ProductName == register.ProductName
                && r.StoreName == register.StoreName
                && r.ReviewedRegister.SubmitionDate == register.SubmitionDate
                && r.ContributorId == register.ContributorId);
        }

        /// <summary>
        /// Handle a review made by a user about a register.
        /// </summary>
        /// <param name="user">User that made the review.</param>
        /// <param name="registerToUpdate">Register to which the review was made.</param>
        /// <param name="interactionDate">Date and time the review is made.</param>
        /// <param name="reviewedValue"><See cref="OnPostHandleInteraction"/>).</param>
        private void HandleReview(User user, Register registerToUpdate, DateTime interactionDate, float reviewedValue)
        {
            var lastReview = _context.Reviews.FirstOrDefault(r => r.ReviewerId == user.Id
                && r.ProductName == registerToUpdate.ProductName
                && r.StoreName == registerToUpdate.StoreName
                && r.SubmitionDate == registerToUpdate.SubmitionDate
                && r.ContributorId == registerToUpdate.ContributorId);

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

        /// <summary>
        /// Get the amount of images of one registers
        /// </summary>
        /// <param name="registerToCheck">Register to check the images count.</param>
        public int GetNumberOfImagesForRegister(Register registerToCheck)
        {
            // Initialize a var int to store the number of images
            int imagesAmount = 0;

            // Check if the input register is not null
            if (registerToCheck != null && registerToCheck.Images != null)
            {
                // Set imagesAmount to the count of images in the register
                imagesAmount = registerToCheck.Images.Count;
            }

            // Return the number of images
            return imagesAmount;
        }

        /// <summary>
        /// Get the amount of images of one registers
        /// </summary>
        /// <param name="registerToCheck">Register to directly check if register has images</param>
        public bool RegisterHasImages(Register registerToCheck)
        {
            // Initialize a bool var to indicate whether the register has images.
            bool hasImages = false;

            // Check if the input register is not null
            if (registerToCheck != null && registerToCheck.Images != null &&
                registerToCheck.Images.Any(image => image.ImageData != null))
            {
                // Set hasImages to true 
                hasImages = true;
            }

            // Return the boolean indicating whether the register has images.
            return hasImages;
        }

        /// <summary>
        /// Get the Max status of report of a register
        /// </summary>
        /// <param name="registerToCheck">Register to directly check if register has images</param>
        public int GetHighestReportState(Register registerToCheck)
        {
            // Initialize a variable to store the highest report state
            int highestReportState = 0;

            // Check if the input register has Reports and there's at least one report
            if (registerToCheck.Reports != null && registerToCheck.Reports.Any())
            {
                // Find the maximum ReportState value among the reports
                highestReportState = registerToCheck.Reports.Max(report => report.ReportState);
            }

            // Return the highest report state
            return highestReportState;
        }

        /// <summary>
        /// Checks if the reporter is the owner of the register
        /// </summary>
        public int checkReporterIsOwner(Report report, User reporter)
        {
            int result = -1;
            if (report != null && reporter != null)
            {
                if (report.ContributorId == reporter.Id)
                {
                    result = 1;
                }
                else
                {
                    result = 0;
                }
            }
            return result;
        }

        /// <summary>
        /// Check if the user had already made a report for the register
        /// <param name="registerKeys">Keys to identificate the register to check </param>
        /// <returns> a JsonResult with the boolean result.</returns>
        /// </summary>
        public JsonResult OnGetCheckReportStatus(string registerKeys)
        {
            var (user, registerToUpdate, _) = GetInteractionValues(registerKeys);
            bool hasReported = false;
            string? previousReportComment = null;
            if (user != null)
            {
                var report = PreviousReport(user!, registerToUpdate);
                if (report != null)
                {
                    hasReported = true;
                    previousReportComment = report.Reason;
                }
            }
            return new JsonResult(new { hasReported, previousReportComment });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registerKeys"></param>
        /// <returns></returns>
        public JsonResult OnGetCheckLastRaiting(string registerKeys)
        {
            var (user, registerToUpdate, _) = GetInteractionValues(registerKeys);
            bool hasReviewed = false;
            float? previousReview = null;
            if (user != null)
            {
                var review = PreviousReview(user!, registerToUpdate);
                if (review != null)
                {
                    hasReviewed = true;
                    previousReview = review.ReviewValue;
                }
            }
            return new JsonResult(new { hasReviewed, previousReview });
        }

        /// <summary>
        /// DD
        /// </summary>
        /// <param name="registerKeys"></param>
        /// <returns></returns>
        public JsonResult OnGetAverageRegisterRating(string registerKeys)
        {
            var (_, register, _) = GetInteractionValues(registerKeys);
            float rating = 0.0f;
            int reviewCount = 0;
            if (register != null)
            {
                rating = _context.GetAverageReviewValue(register.ContributorId!, register.ProductName!, register.StoreName!, register.SubmitionDate);
                reviewCount = _context.GetReviewCount(register);
            }
            return new JsonResult(new { rating, reviewCount });
        }
    }
}