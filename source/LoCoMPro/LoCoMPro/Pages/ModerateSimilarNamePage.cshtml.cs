using LoCoMPro.Data;
using LoCoMPro.Models;
using LoCoMPro.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Fastenshtein;
using System.Text.RegularExpressions;
using System.Data.Entity;
using Microsoft.IdentityModel.Tokens;
using NuGet.RuntimeModel;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

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
        public async Task OnGetAsync()
        {
            var currentUserId = _userManager.GetUserId(User);

            // Define a threshold for similarity
            int threshold = 3;

            // Retrieve all products from the database
            var allProducts = _context.Products.ToList();

            // Group products based on Levenshtein distance
            SimilarNamesList = GroupProductsByCloseness(allProducts, threshold);

            // Now productGroups contains groups of products based on closeness
            // You can further process or display these groups as needed


        }

        public IActionResult OnPostChangeSelectedProductsName(string productName, Dictionary<string, bool> groupProductNames)
        {
            if (productName.IsNullOrEmpty() || groupProductNames.IsNullOrEmpty())
            {
                return new JsonResult(new { Message = "Invalid Parameters", StatusCode = 500 });
            }

            ChangeProductNames(productName, groupProductNames);
            
            return new JsonResult("Ok");
        }

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

        static List<List<Product>> GroupProductsByCloseness(List<Product> products, int threshold)
        {
            var productGroups = new List<List<Product>>();

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
