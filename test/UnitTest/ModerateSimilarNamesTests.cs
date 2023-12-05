using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LoCoMPro.Pages;
using LoCoMPro.Data;
using LoCoMPro.Models;
using LoCoMPro.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Assert = NUnit.Framework.Assert;
using System;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text;
using NetTopologySuite.Geometries;
using Microsoft.IdentityModel.Tokens;
using static NetTopologySuite.Geometries.Utilities.GeometryMapper;
using NuGet.Protocol;

namespace ModerateSimilarNamesTests
{
    [TestClass]
    // Declaration of the test class
    public class RazorPageTests : BaseTest
    {
        // Test by Dwayne Taylor Monterrosa C17827 | Sprint 3
        [Test]
        public void GroupProductsByNameCloseness()
        {
            // Arrange
            var products = new List<Product>();
            products.Add(new Product { Name = "Papa" });
            products.Add(new Product { Name = "Ropa de vestir" });
            products.Add(new Product { Name = "Perro" });
            products.Add(new Product { Name = "Gorra nike gris" });
            products.Add(new Product { Name = "Popa" });

            // Act
            var response = ModerateSimilarNamePageModel.GroupProductsByCloseness(products, 3.0);

            // Assert that the group
            Assert.IsTrue(response[0][0].Name.Equals("Papa") && response[0][1].Name.Equals("Popa"));
        }

        // Test by Dwayne Taylor Monterrosa C17827 | Sprint 3
        [Test]
        public void GroupProductsEmptyList() {
            // Arrange
            var products = new List<Product>();

            // Act
            var response = ModerateSimilarNamePageModel.GroupProductsByCloseness(products, 3.0);

            // Assert that the group
            Assert.IsTrue(response.IsNullOrEmpty());
        }

        // Test by Dwayne Taylor Monterrosa C17827 | Sprint 3
        [Test]
        public void GroupProductsByNameInvalidTreshold()
        {
            // Arrange
            var products = new List<Product>();
            products.Add(new Product { Name = "Papa" });
            products.Add(new Product { Name = "Ropa de vestir" });
            products.Add(new Product { Name = "Perro" });
            products.Add(new Product { Name = "Gorra nike gris" });
            products.Add(new Product { Name = "Popa" });


            // Act and Assert
            Assert.Throws<ArgumentException>(() => ModerateSimilarNamePageModel.GroupProductsByCloseness(products, -10000));
        }

        // Test by Dwayne Taylor Monterrosa C17827 | Sprint 3
        [Test]
        public void ChangeProductNamesInvalidNewName() {
            // Arrange
            //public IActionResult OnPostChangeSelectedProductsName(string productName, Dictionary<string, bool> groupProductNames)
            var pageModel = (ModerateSimilarNamePageModel)CreatePageModel("moderate_similar_names_page");
            var dictionary = new Dictionary<string, bool>() { { "GenericProduct", true } };
            
            // Act
            var result = pageModel.ChangeProductNames("Not a valid name", dictionary);

            // Assert
            Assert.AreEqual(result, 0);
        }

        // Test by Dwayne Taylor Monterrosa C17827 | Sprint 3
        [Test]
        public void ChangeProductNamesSameName()
        {
            // Arrange
            //public IActionResult OnPostChangeSelectedProductsName(string productName, Dictionary<string, bool> groupProductNames)
            var pageModel = (ModerateSimilarNamePageModel)CreatePageModel("moderate_similar_names_page");
            var dictionary = new Dictionary<string, bool>() { { "GenericProduct", true } };

            // Act
            var result = pageModel.ChangeProductNames("GenericProduct", dictionary);

            // Assert
            Assert.AreEqual(result, 0);
        }

        // Test by Dwayne Taylor Monterrosa C17827 | Sprint 3
        [Test]
        public void ChangeProductNamesEmptyNewName()
        {
            // Arrange
            //public IActionResult OnPostChangeSelectedProductsName(string productName, Dictionary<string, bool> groupProductNames)
            var pageModel = (ModerateSimilarNamePageModel)CreatePageModel("moderate_similar_names_page");
            var dictionary = new Dictionary<string, bool>() { { "GenericProduct", true } };

            // Act
            var result = pageModel.ChangeProductNames("", dictionary);

            // Assert
            Assert.AreEqual(result, 0);
        }

        // Test by Dwayne Taylor Monterrosa C17827 | Sprint 3
        [Test]
        public void ChangeProductNamesEmptyGroupProductNames()
        {
            // Arrange
            //public IActionResult OnPostChangeSelectedProductsName(string productName, Dictionary<string, bool> groupProductNames)
            var pageModel = (ModerateSimilarNamePageModel)CreatePageModel("moderate_similar_names_page");
            var dictionary = new Dictionary<string, bool>() { };

            // Act
            var result = pageModel.ChangeProductNames("Laptop", dictionary);

            // Assert
            Assert.AreEqual(result, 0);
        }

        // Test by Dwayne Taylor Monterrosa C17827 | Sprint 3
        [Test]
        public void OnPostChangeSelectedProductsNameInvalidNewProductName()
        {
            // Arrange
            //public IActionResult OnPostChangeSelectedProductsName(string productName, Dictionary<string, bool> groupProductNames)
            var pageModel = (ModerateSimilarNamePageModel)CreatePageModel("moderate_similar_names_page");
            var dictionary = new Dictionary<string, bool>() { { "GenericProduct", true } };

            // Act
            var result = pageModel.OnPostChangeSelectedProductsName("Actually an invalid name", dictionary) as JsonResult;
            // Access the Message and StatusCode properties
            var message = result!.Value!.GetType().GetProperty("Message")?.GetValue(result.Value);
            var statusCode = result.Value.GetType().GetProperty("StatusCode")?.GetValue(result.Value);


            // Assert
            Assert.IsTrue(message!.Equals("Didn't changed products") && statusCode!.Equals(500));
        }

        // Test by Dwayne Taylor Monterrosa C17827 | Sprint 3
        [Test]
        public void OnPostChangeSelectedProductsNameEmptyNewName()
        {
            // Arrange
            //public IActionResult OnPostChangeSelectedProductsName(string productName, Dictionary<string, bool> groupProductNames)
            var pageModel = (ModerateSimilarNamePageModel)CreatePageModel("moderate_similar_names_page");
            var dictionary = new Dictionary<string, bool>() { { "GenericProduct", true } };

            // Act
            var result = pageModel.OnPostChangeSelectedProductsName("", dictionary) as JsonResult;
            // Access the Message and StatusCode properties
            var message = result!.Value!.GetType().GetProperty("Message")?.GetValue(result.Value);
            var statusCode = result.Value.GetType().GetProperty("StatusCode")?.GetValue(result.Value);

            // Assert
            Assert.IsTrue(message!.Equals("Invalid Parameters") && statusCode!.Equals(500));
        }

        // Test by Dwayne Taylor Monterrosa C17827 | Sprint 3
        [Test]
        public void OnPostChangeSelectedProductsNameEmptyGroupProductNames()
        {
            // Arrange
            //public IActionResult OnPostChangeSelectedProductsName(string productName, Dictionary<string, bool> groupProductNames)
            var pageModel = (ModerateSimilarNamePageModel)CreatePageModel("moderate_similar_names_page");
            var dictionary = new Dictionary<string, bool>() { };

            // Act
            var result = pageModel.OnPostChangeSelectedProductsName("Laptop", dictionary) as JsonResult;
            // Access the Message and StatusCode properties
            var message = result!.Value!.GetType().GetProperty("Message")?.GetValue(result.Value);
            var statusCode = result.Value.GetType().GetProperty("StatusCode")?.GetValue(result.Value);

            // Assert
            Assert.IsTrue(message!.Equals("Invalid Parameters") && statusCode!.Equals(500));
        }

    }
}

