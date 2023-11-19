using LoCoMPro.Data;
using LoCoMPro.Models;
using LoCoMPro.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LoCoMPro.Pages
{
    /// <summary>
    /// Page model for List Report Page, that shows the result report based in the user list
    /// </summary>
    public class ListReportPageModel : LoCoMProPageModel
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
        /// List with products in the list of the user
        /// </summary>
        public IList<Product> WantedProducts { get; set; }

        /// <summary>
        /// Structure that bind the stores with the products that sells
        /// </summary>
        public Dictionary<Store, List<Register>> StoreProducts { get; set; } = new Dictionary<Store, List<Register>>();

        /// <summary>
        /// Creates a new ListReportPageModel.
        /// </summary>
        /// <param name="context">DB Context to pull data from.</param>
        /// <param name="configuration">Configuration for page.</param>
        /// <param name="httpContextAccessor">Allow access to the http context
        public ListReportPageModel(LoCoMProContext context, IConfiguration configuration
            , IHttpContextAccessor? httpContextAccessor)
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
        }

        /// <summary>
        /// GET HTTP request, initializes page values.
        /// </summary>
        /// <returns></returns>
        public async void OnGetAsync()
        {
            // Obtains the list of stores that sells the products the list has
            ObtainStoresListWithProducts();
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
                var register = from r in _context.Registers select r;

                // Gets a register from the product
                Register registerFromStore = register.Where(x => x.Product == product
                                                            && x.Store == store).FirstOrDefault()!;

                // If the store was already stored
                if(StoreProducts.ContainsKey(store))
                {
                    // Only add the product
                    StoreProducts[store].Add(registerFromStore);
                }
                // If wasn´t stored already
                else
                {
                    // Add the store and add the product to the list
                    StoreProducts.Add(store, new List<Register> { registerFromStore });
                }
            }
        }

    }

}
