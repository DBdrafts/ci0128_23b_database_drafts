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
            ObtainStoresThatSells();
        }


        internal void ObtainStoresThatSells()
        {
            IList<Product> WantedProducts = ObtainProductsFromList();
            
            foreach(var product in WantedProducts)
            {
                var stores = from s in _context.Stores select s;

                AddToProductStores(product, stores.Where(x => x.Products.Contains(product)).ToList());
            }
        }
        
        internal IList<Product> ObtainProductsFromList()
        {
            var productNames = UserProductList.Select(element => element.ProductName).ToList();

            return _context.Products.Where(p => productNames.Contains(p.Name)).ToList();
        }

        internal void AddToProductStores(Product product, List<Store> Stores)
        {
            foreach(var store in Stores)
            {

                var register = from r in _context.Registers select r;

                // TODO(Traerse el de mejor nota, mmmmm)?
                Register registerFromStore = register.Where(x => x.Product == product
                                                            && x.Store == store).FirstOrDefault()!;

                if(StoreProducts.ContainsKey(store))
                {
                    StoreProducts[store].Add(registerFromStore);
                }
                else
                {
                    StoreProducts.Add(store, new List<Register> { registerFromStore });
                }
            }
        }
    }
}
