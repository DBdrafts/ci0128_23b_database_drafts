using LoCoMPro.Data;
using LoCoMPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Fastenshtein;
using Microsoft.IdentityModel.Tokens;

namespace LoCoMPro.Pages
{
    /// <summary>
    /// Page model for Moderate Similar Names Pages
    /// </summary>
    [Authorize(Roles = "Moderator")]
    public class ModerateSimilarNamePageModel : LoCoMProPageModel
    {
        private readonly UserManager<User> _userManager;

        private readonly IHttpContextAccessor? _httpContextAccessor;

        /// <summary>
        /// Structure that contains list of product with similar names
        /// </summary>
        public List<List<Product>> SimilarNamesList { get; set; } = new List<List<Product>>();

        /// <summary>
        /// Creates a new ModerateSimilarNamePageModel.
        /// </summary>
        /// <param name="context">DB Context to pull data from.</param>
        /// <param name="configuration">Configuration for page.</param>
        /// <param name="httpContextAccessor">Allow access to the http context
        /// <param name="userManager">User manager to handle user permissions.</param>
        public ModerateSimilarNamePageModel(LoCoMProContext context, IConfiguration configuration
            , IHttpContextAccessor? httpContextAccessor, UserManager<User> userManager)
            : base(context, configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        /// <summary>
        /// GET HTTP request, initializes page values.
        /// </summary>
        public void OnGetAsync()
        {
            var currentUserId = _userManager.GetUserId(User);

            // Define a threshold for similarity
            double threshold = 2;

            // Retrieve all products from the database
            var allProducts = _context.Products.ToList();

            // Group products based on Levenshtein distance
            SimilarNamesList = GroupProductsByCloseness(allProducts, threshold);

        }

        /// <summary>
        /// Updates the name of every product marked as true in <paramref name="groupProductNames"/> with the name <paramref name="productName"/>.
        /// </summary>
        /// <param name="productName">New name that products must have.</param>
        /// <param name="groupProductNames">Dictionary where the keys are the name of the products to change and the value is set to true if said product must be changed.</param>
        /// <returns>Response of calling this function.</returns>
        public IActionResult OnPostChangeSelectedProductsName(string productName, Dictionary<string, bool> groupProductNames)
        {
            if (productName.IsNullOrEmpty() || groupProductNames.IsNullOrEmpty())
            {
                return new JsonResult(new { Message = "Invalid Parameters", StatusCode = 500 });
            }

            ChangeProductNames(productName, groupProductNames);
            
            return new JsonResult("Ok");
        }

        /// <summary>
        /// Changes the name of each product in the database
        /// </summary>
        /// <param name="productName">New name that products must have.</param>
        /// <param name="groupProductNames">Dictionary where the keys are the name of the products to change and the value is set to true if said product must be changed.</param>
        private void ChangeProductNames(string productName, Dictionary<string, bool> groupProductNames)
        {
            using (var transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.RepeatableRead))
            {
                try
                {
                    foreach (var product in groupProductNames)
                    {
                        if (product.Value == false || product.Key == productName) { continue; }
                        _context.UpdateProductName(productName, product.Key);

                    }

                    // If everything is successful, commit the transaction
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // Handle exceptions and optionally roll back the transaction
                    Console.WriteLine($"Error: {ex.Message}");
                    transaction.Rollback();
                }
            }
        }

        /// <summary>
        /// Groups Products by their coseness to each other using Lavenshtein distance.
        /// </summary>
        /// <param name="products">Products to group.</param>
        /// <param name="threshold">Maximum distance the Product Names must be from each other.</param>
        /// <returns>Array of groupped products.</returns>
        static List<List<Product>> GroupProductsByCloseness(List<Product> products, double threshold)
        {
            var productGroups = new List<List<Product>>();
            if (products.IsNullOrEmpty()) return productGroups;

            foreach (var product in products)
            {
                // Find the group with the closest product names
                var group = productGroups
                    .OrderBy(g => g.Min(p => Levenshtein.Distance(product.Name, p.Name)))
                    .FirstOrDefault(p => p.Any(p => Levenshtein.Distance(product.Name, p.Name) <= threshold));

                if (group != null)
                {
                    // Add the product to the existing group
                    group.Add(product);
                }
                else
                {
                    // Create a new group with the current product
                    productGroups.Add(new List<Product> { product });
                }
            }

            return productGroups.Where(g => g.Count() > 1).ToList();
        }

    }
}
