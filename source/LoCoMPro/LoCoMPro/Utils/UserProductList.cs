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
        /// Remove a element from the user list
        /// </summary>
        /// <param name="removeElement">Element of the list to be removed
        public void RemoveProductFromList(UserProductListElement removeElement)
        {
            IList<UserProductListElement> UserProductList = GetUserProductList();

            // Remove the element of the list
            UserProductList.Remove(GetListElement(UserProductList, removeElement)!);

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
            var existingElement = GetListElement(UserProductList, newElement);

            // Returns if the element already exist
            return existingElement != null;
        }

        /// <summary>
        /// Gets the first element that coincide with the element sent
        /// </summary>
        /// <param name="UserProductList">User list of product
        /// <param name="compareElement">Element that is going to be search
        public UserProductListElement? GetListElement(IList<UserProductListElement> UserProductList
            , UserProductListElement compareElement)
        {
            // Get the first element that coincide
            var existingElement = UserProductList.FirstOrDefault(item =>
                item.ProductName == compareElement.ProductName &&
                item.ProductBrand == compareElement.ProductBrand &&
                item.ProductModel == compareElement.ProductModel &&
                item.StoreName == compareElement.StoreName &&
                item.Province == compareElement.Province &&
                item.Canton == compareElement.Canton);

            return existingElement;
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
