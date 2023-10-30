using LoCoMPro.Models;
using System.Text.Json;

namespace LoCoMPro.Utils
{
    /// <summary>
    /// List of the product that the user is interested in
    /// </summary>
    public class UserProductList
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Name of the User Product List
        /// </summary>
        private readonly string productListName = "UserProductList";

        /// <summary>
        /// Creates the list with the Http Context accessor
        /// </summary>
        /// <param name="httpContextAccessor">Allow access to the http context
        public UserProductList(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Add a new element to the user list
        /// </summary>
        /// <param name="newElement">New element of the list
        public void AddProductToList(UserProductListElement newElement)
        {
            IList<UserProductListElement> UserProductList = GetUserProductList();

            // Adds the new element to the list
            UserProductList.Add(newElement);

            // Saves the actual state of the list
            saveListState(UserProductList);
        }

        /// <summary>
        /// Checks if the product exist in the actual user list
        /// </summary>
        /// <param name="newElement">Element that is going to be search
        public bool ExistElementInList(UserProductListElement newElement)
        {
            IList<UserProductListElement> UserProductList = GetUserProductList();

            // Get the first element that coincide
            var existingElement = UserProductList.FirstOrDefault(item =>
                item.ProductName == newElement.ProductName &&
                item.ProductBrand == newElement.ProductBrand &&
                item.ProductModel == newElement.ProductModel &&
                item.StoreName == newElement.StoreName &&
                item.Province == newElement.Province &&
                item.Canton == newElement.Canton &&
                item.AvgPrice == newElement.AvgPrice);

            // Returns if the element already exist
            return existingElement != null;
        }

        /// <summary>
        /// Gets the user list of products
        /// </summary>
        /// <returns>List of element of the list</returns>
        public IList<UserProductListElement> GetUserProductList()
        {
            // Get the serialized form of the list
            var serializedList = getSerializedList();

            IList<UserProductListElement> UserProductList = new List<UserProductListElement>();

            // Gets the list if exist, or create it if not
            if (serializedList != null)
            {
                UserProductList = getDeserializedList(serializedList);
            }

            return UserProductList;
        }

        /// <summary>
        /// Gets the serialized string of the list
        /// </summary>
        /// <returns>String that contains the serialized list</returns>
        private string? getSerializedList()
        {
            return _httpContextAccessor.HttpContext!.Session.GetString(productListName);
        }

        /// <summary>
        /// Gets the deserialized user list
        /// </summary>
        /// <returns>List of product of the list</returns>
        private IList<UserProductListElement> getDeserializedList(string serializedList)
        {
            return JsonSerializer.Deserialize<List<UserProductListElement>>(serializedList)!;
        }

        /// <summary>
        /// Saves the actual state of the list
        /// </summary>
        /// <param name="UserProductList">The user list to be store
        private void saveListState(IList<UserProductListElement> UserProductList)
        {
            var serializedList = JsonSerializer.Serialize(UserProductList);
            _httpContextAccessor.HttpContext.Session.SetString(productListName, serializedList);
        }
    }
}
