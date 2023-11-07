using LoCoMPro.Pages;
using Newtonsoft.Json.Linq;
using NuGet.Protocol;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace LocationTests
{
    [TestClass]
    // Declaration of the test class
    public class RazorPageTests : BaseTest
    {
        // Test by Dwayne Taylor Monterrosa C17827 | Sprint 2
        [Test]
        public void GetProvincesForSelector()
        {
            // Arrange for the mock configuration
            // Create an instance of the page model (pageModel) using the CreatePageModel()
            var pageModel = (IndexModel)CreatePageModel("index_page");

            // Get the provinces, in this case just GenericProvince.
            var provinces = pageModel.OnGetProvinces().ToJson();

            // Assert to check if GenericProvince is in the result.
            Assert.IsTrue(provinces.Contains("GenericProvince"));
        }

        // Test by Dwayne Taylor Monterrosa C17827 | Sprint 2
        [Test]
        public void GetCantonsForSelector()
        {
            // Arrange for the mock configuration
            // Create an instance of the page model (pageModel) using the CreatePageModel()
            var pageModel = (IndexModel)CreatePageModel("index_page");

            // Get the Cantons, for GenericProvince.
            var cantons = pageModel.OnGetCantones("GenericProvince").ToJson();

            // Assert to check that GenericProvince and Paraiso are in the result.
            Assert.IsTrue(cantons.Contains("GenericCanton1") && cantons.Contains("GenericCanton2"));
        }

        // Test by Dwayne Taylor Monterrosa C17827 | Sprint 2
        [Test]
        public void GetCantonsInvalidProvince()
        {
            // Arrange for the mock configuration
            // Create an instance of the page model (pageModel) using the CreatePageModel()
            var pageModel = (IndexModel)CreatePageModel("index_page");
            var expected = "{\"Value\":[{\"Disabled\":false,\"Selected\":false,\"Text\":\"Elegir Cantón\",\"Value\":\"\"}]}";

            // Get the Cantons, for Cartago.
            var cantons = pageModel.OnGetCantones("Invalid").ToJson();

            // Ensure that the return value contains just the empty value.
            Assert.IsTrue(expected.Equals(cantons));
        }

        // Test by Dwayne Taylor Monterrosa C17827 | Sprint 2
        [Test]
        public void GetCoordinatesFromValidCanton()
        {
            // Arrange for the mock configuration
            // Create an instance of the page model (pageModel) using the CreatePageModel()
            var pageModel = (IndexModel)CreatePageModel("index_page");

            // Get the geolocation for Paraiso canton.
            var jsonResult = pageModel.OnGetLocationFromCanton("GenericProvince", "GenericCanton2").ToJson(); // Deserialize the JsonResult to get the inner JSON string

            var result = JObject.Parse(jsonResult); // Get the actual data of the JSONResult

            // Extract the inner JSON string
            string innerJsonString = result["Value"]!["point"]!.ToString();

            // Deserialize the inner JSON string to get coordinates as floats
            var innerData = JObject.Parse(innerJsonString);
            var latitude = (double)innerData["coordinates"]![0]!;
            var longitude = (double)innerData["coordinates"]![1]!;

            Assert.IsTrue(latitude == 6.9 && longitude == 4.2);
        }

        // Test by Dwayne Taylor Monterrosa C17827 | Sprint 2
        [Test]
        public void GetCoordinatesFromValidCantonWithoutLocation()
        {
            // Arrange for the mock configuration
            // Create an instance of the page model (pageModel) using the CreatePageModel()
            var pageModel = (IndexModel)CreatePageModel("index_page");

            // Get the geolocation for Paraiso canton.
            var result = pageModel.OnGetLocationFromCanton("GenericProvince", "GenericCanton1");

            // Ensure that since Cartago does not have a Geolocation, to return the geolocation for said canton.
            Assert.IsTrue(result.StatusCode == 500);
        }

        // Test by Dwayne Taylor Monterrosa C17827 | Sprint 2
        [Test]
        public void GetCoordinatesFromInValidCanton()
        {
            // Arrange for the mock configuration
            // Create an instance of the page model (pageModel) using the CreatePageModel()
            var pageModel = (IndexModel)CreatePageModel("index_page");

            // Get the geolocation for Paraiso canton.
            var result = pageModel.OnGetLocationFromCanton("GenericProvince", "Invalid");

            // Ensure that since Cartago does not have a Geolocation, to return the geolocation for said canton.
            Assert.IsTrue(result.StatusCode == 500);
        }


    }
}
