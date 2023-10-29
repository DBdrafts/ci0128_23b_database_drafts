using LoCoMPro.Models;
using System.Text.Json;

namespace LoCoMPro.Utils
{
    public class UserProductList
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly string productListName = "UserProductList";

        public UserProductList(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void AddProductToList(UserProductListElement newElement)
        {

            IList<UserProductListElement> UserProductList = GetUserProductList();

            UserProductList.Add(newElement);

            saveListState(UserProductList);
        }

        public bool ExistElementInList(UserProductListElement newElement)
        {
            IList<UserProductListElement> UserProductList = GetUserProductList();

            var existingElement = UserProductList.FirstOrDefault(item =>
                item.ProductName == newElement.ProductName &&
                item.ProductBrand == newElement.ProductBrand &&
                item.ProductModel == newElement.ProductModel &&
                item.StoreName == newElement.StoreName &&
                item.Province == newElement.Province &&
                item.Canton == newElement.Canton &&
                item.AvgPrice == newElement.AvgPrice);

            return existingElement != null;
        }

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

        private string? getSerializedList()
        {
            return _httpContextAccessor.HttpContext!.Session.GetString(productListName);
        }

        private IList<UserProductListElement> getDeserializedList(string serializedList)
        {
            return JsonSerializer.Deserialize<List<UserProductListElement>>(serializedList)!;
        }

        private void saveListState(IList<UserProductListElement> UserProductList)
        {
            var serializedList = JsonSerializer.Serialize(UserProductList);
            _httpContextAccessor.HttpContext.Session.SetString(productListName, serializedList);
        }
    }
}
