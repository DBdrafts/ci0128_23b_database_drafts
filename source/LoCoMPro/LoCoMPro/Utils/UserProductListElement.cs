namespace LoCoMPro.Utils
{
    /// <summary>
    /// Element of the user product list that store the information
    /// </summary>
    public class UserProductListElement
    {
        public string ProductName { get; set; }

        public string ProductBrand { get; set; }

        public string ProductModel { get; set; }

        public string StoreName { get; set; }

        public string Province { get; set; }

        public string Canton { get; set; }

        public string AvgPrice { get; set; }

        /// <summary>
        /// Create the element for the user product list
        /// </summary>
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
