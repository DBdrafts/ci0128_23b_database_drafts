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

        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Reference to the user product list
        /// </summary>
        public UserProductList _userProductList { get; set; }

        /// <summary>
        /// List with products of the user product list
        /// </summary>
        public IList<UserProductListElement> UserProductList { get; set; }

        /// <summary>
        /// Total amount of products
        /// </summary>
        public int TotalAmountProduct { get; set; }

        /// <summary>
        /// Total amount that all products cost
        /// </summary>
        public int TotalPrice { get; set; }

        /// <summary>
        /// Creates a new ProductListPageModel.
        /// </summary>
        /// <param name="context">DB Context to pull data from.</param>
        /// <param name="configuration">Configuration for page.</param>
        /// <param name="httpContextAccessor">Allow access to the http context
        public ProductListPageModel(LoCoMProContext context, IConfiguration configuration
            , IHttpContextAccessor httpContextAccessor)
            : base(context, configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _userProductList = new UserProductList(_httpContextAccessor);

            // Gets the user product list
            UserProductList = _userProductList.GetUserProductList();
        }

        /// <summary>
        /// GET HTTP request, initializes page values.
        /// </summary>
        /// <returns></returns>
        public async Task OnGetAsync()
        {
            // Calculates the total amout between all the products
            calculateTotalPrice();
        }

        /// <summary>
        /// Calculates the total amount between all the products in the list
        /// </summary>
        /// <returns></returns>
        public void calculateTotalPrice()
        {
            CultureInfo culture = CultureInfo.InvariantCulture; // Usar la configuración regional invariable para evitar problemas

            // Gets the average price of the product and add it to the total amount
            foreach (var product in UserProductList)
            {
                string avgPriceString = product.AvgPrice;
                avgPriceString = Regex.Replace(avgPriceString, @"[^\d]", "");

                if (int.TryParse(avgPriceString, NumberStyles.Number, culture, out int avgPrice))
                {
                    TotalPrice += avgPrice;
                }
            }
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
