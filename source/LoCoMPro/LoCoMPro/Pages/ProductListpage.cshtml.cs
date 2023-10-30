using LoCoMPro.Data;
using LoCoMPro.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
        }

        /// <summary>
        /// GET HTTP request, initializes page values.
        /// </summary>
        /// <returns></returns>
        public async Task OnGetAsync()
        {
            // Gets the user product list
            UserProductList = _userProductList.GetUserProductList();

            // Calculates the total amout between all the products
            calculateTotalPrice();
        }

        /// <summary>
        /// Calculates the total amount between all the products in the list
        /// </summary>
        /// <returns></returns>
        public void calculateTotalPrice()
        {
            // Gets the average price of the product an added it to the total amount
            foreach (var product in UserProductList) {
                string avgPriceString = product.AvgPrice;
                TotalPrice += int.Parse(avgPriceString.Replace(",", ""));
            }
        }
    }
}
