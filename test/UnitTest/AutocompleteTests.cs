using LoCoMPro.Pages;
using NuGet.Protocol;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace AutocompleteTests
{
    // Declaration of the test class
    [TestClass]
    public class RazorPageTests : BaseTest
    {
        // Test by Dwayne Taylor Monterrosa C17827
        [Test]
        public void GetAutofillData()
        {
            // Arrange for the mock configuration
            // Create an instance of the page model (pageModel) using the CreatePageModel()
            var pageModel = (AddProductPageModel)CreatePageModel("add_register_page");
            var expected = "{\"Value\":{\"#brand\":\"GenericBrand\",\"#model\":\"GenericModel\",\"#category\":\"Category\"}}";
            // Initializes data for store to add.
            string productName = "GenericProduct";

          
            // Call the create method with the attributes and get the new product
            var data = pageModel.OnGetProductAutofillData(productName).ToJson();

            // Assert that the Geolocation for store was not updated.
            Assert.IsTrue(data.Equals(expected));
        }
    }
}
