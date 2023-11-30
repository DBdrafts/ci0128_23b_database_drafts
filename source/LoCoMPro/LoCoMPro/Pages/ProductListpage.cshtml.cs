using LoCoMPro.Data;
using LoCoMPro.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LoCoMPro.Pages
{
    /// <summary>
    /// Page model for User Product List Page, that shows the product of the list
    /// </summary>
    public class ProductListPageModel : LoCoMProPageModel
    {
        private readonly UserManager<User> _userManager;

        private readonly IHttpContextAccessor? _httpContextAccessor;

        /// <summary>
        /// Reference to the user product list
        /// </summary>
        public UserProductList _userProductList { get; set; }

        /// <summary>
        /// List with products of the user product list
        /// </summary>
        public IList<UserProductListElement> UserProductList { get; set; }


        /// <summary>
        /// Flag if the user has choose a location
        /// </summary>
        public bool HasUserLocation { get; set; } = false;

        /// <summary>
        /// Creates a new ProductListPageModel.
        /// </summary>
        /// <param name="context">DB Context to pull data from.</param>
        /// <param name="configuration">Configuration for page.</param>
        /// <param name="httpContextAccessor">Allow access to the http context
        /// <param name="userManager">User manager to handle user permissions.</param>
        public ProductListPageModel(LoCoMProContext context, IConfiguration configuration
            , IHttpContextAccessor? httpContextAccessor, UserManager<User> userManager)
            : base(context, configuration)
        {
            _httpContextAccessor = httpContextAccessor;

            if (_httpContextAccessor != null)
            {
                _userProductList = new UserProductList(_httpContextAccessor);
            }
            else
            {
                UserProductList = new List<UserProductListElement>();
            }
            _userManager = userManager;
        }

        /// <summary>
        /// GET HTTP request, initializes page values.
        /// </summary>
        /// <returns></returns>
        public async Task OnGetAsync()
        {
            User? actualUser = null;
            if (User != null && _userManager != null)
            {
                actualUser = (await _userManager.GetUserAsync(User))!;
            }
            if (actualUser != null)
            {
                _userProductList.SetListName(actualUser.Id);

                // Gets the user product list
                UserProductList = _userProductList.GetUserProductList();

                HasUserLocation = actualUser.Geolocation != null;
            }
        }

        /// <summary>
        /// Delete the product from the user list
        /// </summary>
        public IActionResult OnPostRemoveFromProductList(string productData)
        {
            if (User != null && _userManager != null)
            {
                _userProductList.SetListName(_userManager.GetUserId(User)!);
            }

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
        /// Splits a string into an array of substrings based on the specified delimiter character.
        /// </summary>
        /// <param name="input">The input string to split.</param>
        /// <param name="delimiter">The character used as the delimiter.</param>
        /// <returns>An array of substrings created by splitting the input string.</returns>
        static string[] SplitString(string input, char delimiter)
        {
            return input.Split(delimiter);
        }
    }
}
