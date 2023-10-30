namespace LoCoMPro.Utils
{
    /// <summary>
    /// Element of the user product list that store the information
    /// </summary>
    public class UserProductListElement
    {
        /// <summary>
        /// Name of the product.
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Brand of the product.
        /// </summary>
        public string ProductBrand { get; set; }

        /// <summary>
        /// Model of the product.
        /// </summary>
        public string ProductModel { get; set; }

        /// <summary>
        /// Name of the store.
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// Name of the province.
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// Name of the canton.
        /// </summary>
        public string Canton { get; set; }

        /// <summary>
        /// Average price of the product.
        /// </summary>
        public string AvgPrice { get; set; }

        /// <summary>
        /// Create the element for the user product list
        /// </summary>
        /// <param name="productName">Name of the product</param>
        /// <param name="productBrand">Brand of the product</param>
        /// <param name="productModel">Model of the product</param>
        /// <param name="storeName">Name of the store</param>
        /// <param name="province">Name of the province</param>
        /// <param name="canton">Name of the canton</param>
        /// <param name="avgPrice">Average price of the product</param>
        public UserProductListElement(string productName
            , string productBrand, string productModel, string storeName
            , string province, string canton, string avgPrice)
        {
            ProductName = productName;
            ProductBrand = productBrand;
            ProductModel = productModel;
            StoreName = storeName;
            Province = province;
            Canton = canton;
            AvgPrice = avgPrice;
        }
    }
}
