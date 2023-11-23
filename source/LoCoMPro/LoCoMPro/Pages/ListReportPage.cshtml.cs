using LoCoMPro.Data;
using LoCoMPro.Models;
using LoCoMPro.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LoCoMPro.Pages
{
    /// <summary>
    /// Page model for List Report Page, that shows the result report based in the user list
    /// </summary>
    public class ListReportPageModel : LoCoMProPageModel
    {
        private readonly UserManager<User> _userManager;

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
        /// List with products in the list of the user
        /// </summary>
        public IList<Product> WantedProducts { get; set; }

        /// <summary>
        /// Structure that bind the stores with the products that sells
        /// </summary>
        public Dictionary<Store, List<Register>> StoreProducts { get; set; } = new Dictionary<Store, List<Register>>();

        /// <summary>
        /// Structure that bind the stores with the distance to the user
        /// </summary>
        public Dictionary<Store, double> StoreDistances { get; set; } = new Dictionary<Store, double>();

        /// <summary>
        /// User in the page
        /// </summary>
        public User UserInPage;

        /// <summary>
        /// Creates a new ListReportPageModel.
        /// </summary>
        /// <param name="context">DB Context to pull data from.</param>
        /// <param name="configuration">Configuration for page.</param>
        /// <param name="httpContextAccessor">Allow access to the http context
        public ListReportPageModel(LoCoMProContext context, IConfiguration configuration
            , IHttpContextAccessor? httpContextAccessor, UserManager<User> userManager)
            : base(context, configuration)
        {
            _httpContextAccessor = httpContextAccessor;

            if (httpContextAccessor != null)
            {
                _userProductList = new UserProductList(_httpContextAccessor);

                // Gets the user product list
                UserProductList = _userProductList.GetUserProductList();
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
            // Get the user in the page
            UserInPage = await _userManager.GetUserAsync(User);

            // Obtains the list of stores that sells the products the list has
            ObtainStoresListWithProducts();

            // Gets the distance between the user and the stores
            ObtainDistances();

            // Filters the stores from the report list following several criteria
            FilterStoreFromReport();
        }

        /// <summary>
        /// Obtains the list of stores that sells the products the list has with the list of products the store has
        /// </summary>
        /// <returns></returns>
        internal void ObtainStoresListWithProducts()
        {
            // Obtains the product that are in the list
            WantedProducts = ObtainProductsFromList();
            
            // For each product the add it to the list of the store
            foreach(var product in WantedProducts)
            {
                var stores = from s in _context.Stores select s;

                // Adds a product to the list of the stores that sells it
                AddToProductStores(product, stores.Where(x => x.Products.Contains(product)).ToList());
            }
        }

        /// <summary>
        /// Obtains the product that are in the list of the user
        /// </summary>
        /// <returns></returns>
        internal IList<Product> ObtainProductsFromList()
        {
            var productNames = UserProductList.Select(element => element.ProductName).ToList();

            // Returns the list of products that are in the list
            return _context.Products.Where(p => productNames.Contains(p.Name)).ToList();
        }

        /// <summary>
        /// Adds a product to the list of the stores that sells it
        /// </summary>
        /// <param name="product"> Product to add to the list </param>
        /// <param name="Stores"> Stores that sells the product </param>
        /// <returns></returns>
        internal void AddToProductStores(Product product, List<Store> Stores)
        {
            // For each stores that sells the product
            foreach(var store in Stores)
            {
                var register = from r in _context.Registers
                               where r.Reports.All(report => report.ReportState != 2)
                               select r;

                // Gets a register from the product
                Register registerFromStore = register.Where(x => x.Product == product
                                                            && x.Store == store).FirstOrDefault()!;

                if (registerFromStore != null)
                {
                    // If the store was already stored
                    if(StoreProducts.ContainsKey(store))
                    {
                        // Only add the product
                        StoreProducts[store].Add(registerFromStore);
                    }
                    // If wasn't stored already
                    else
                    {
                        // Add the store and add the product to the list
                        StoreProducts.Add(store, new List<Register> { registerFromStore });
                    }
                }
            }
        }

        /// <summary>
        /// Filters the stores from the report list following several criteria
        /// </summary>
        /// <returns></returns>
        internal void FilterStoreFromReport()
        {
            // Filters based on the amount of products
            FilterStoreByProductAmount();

            // Filters based on the distance of the store
            FilterStoreByDistance();
        }

        /// <summary>
        /// Filters the stores from the report list based on the amount of product the store have
        /// </summary>
        /// <returns></returns>
        internal void FilterStoreByProductAmount()
        {
            // Gets the minimal amount of products
            int minimunProductAmount = CalculateMinimalProductAmount(WantedProducts.Count);

            // For each store in the report
            foreach (var store in StoreProducts)
            {
                // Remove the store from the report if does not have the enough amount of products
                if (store.Value.Count < minimunProductAmount || store.Value.Count == 0 || store.Value.Count > WantedProducts.Count)
                {
                    StoreProducts.Remove(store.Key);
                }
            }
        }

        /// <summary>
        /// Returns the minimal amount of products the list of the report can have, rounding to the lower
        /// </summary>
        /// <param name="amountOfProduct"> Total amount of product of the list
        /// <returns></returns>
        internal int CalculateMinimalProductAmount(int amountOfProduct)
        {
            // Checks that the number is not negative
            amountOfProduct = amountOfProduct < 0 ? 0 : amountOfProduct;
            // Returns the minimal amount of products the list of the report can have, rounding to the lower
            // In this case, is the 30% of the total of products of the list
            return (int)(amountOfProduct * 0.3);
        }

        /// <summary>
        /// Filters the stores from the report list based on the distances between the user and the store
        /// </summary>
        /// <returns></returns>
        internal void FilterStoreByDistance()
        {
            // For each store in the report
            foreach (var store in StoreProducts)
            {
                // Remove the store from the report if the distance between the user and the store
                // is bigger than approximately 50 kilometers
                if (StoreDistances[store.Key] > 0.45)
                {
                    StoreProducts.Remove(store.Key);
                    StoreDistances.Remove(store.Key);
                }
            }
        }

        /// <summary>
        /// Obtains the distance between the user and the stores
        /// </summary>
        /// <returns></returns>
        internal void ObtainDistances()
        {
            // If the user has choose a location
            if(UserHasLocation(UserInPage))
            {
                SetRealDistances();
            }
            else
            // If the user does not has a location
            {
                SetStandardDistances();
            }
        }

        /// <summary>
        /// Sets the distances as the real distances between the user and the store
        /// </summary>
        /// <returns></returns>
        internal void SetRealDistances()
        {
            // Gets the distance between the user and each store
            foreach (var store in StoreProducts)
            {
                // If the store has geolocation, add the real distance
                if (store.Key.Geolocation != null)
                {
                    StoreDistances.Add(store.Key, UserInPage.Geolocation!.Distance(store.Key.Geolocation));
                }
                else
                {
                    StoreDistances.Add(store.Key, 0);
                }
            }
        }

        /// <summary>
        /// Sets all distances as a default value
        /// </summary>
        /// <returns></returns>
        internal void SetStandardDistances()
        {
            // Sets all the distance to 0
            foreach (var store in StoreProducts)
            {
                StoreDistances.Add(store.Key, 0);
            }
        }
    }

}
