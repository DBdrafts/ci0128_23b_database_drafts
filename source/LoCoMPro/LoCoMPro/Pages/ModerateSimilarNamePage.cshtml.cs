using LoCoMPro.Data;
using LoCoMPro.Models;
using LoCoMPro.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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

            Product p1 = new Product() { Name = "Test1"};
            Product p2 = new Product() { Name = "Test2"};
            Product p3 = new Product() { Name = "Test3"};

            // Testing
            // From where on, be free
            SimilarNamesList.Add(new List<Product> { });
            SimilarNamesList[0].Add(p1);
            SimilarNamesList[0].Add(p2);
            SimilarNamesList[0].Add(p3);

            SimilarNamesList.Add(new List<Product> { });
            SimilarNamesList[1].Add(p2);
            SimilarNamesList[1].Add(p3);

            SimilarNamesList.Add(new List<Product> { });
            SimilarNamesList[2].Add(p3);

        }
    }
}
