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

namespace AddRegisterTest
{
    [TestClass]
    // Declaration of the test class
    public class RazorPageTests : BaseTest
    {
        // Test by Geancarlo Rivera Hernández C06516
        [Test]
        public void CheckNullForEmptyStrings()
        {
            // Set the empty string to the variable
            string emptyString = "";

            // Pass the string to the method and get de result in response variable
            var response = AddProductPageModel.CheckNull(emptyString);

            // Assert thar checks if the string is null
            Assert.IsNull(response);
        }

        // Test by Geancarlo Rivera Hernández C06516
        [Test]
        public void CheckNullForStringsWithData()
        {
            // Set the string to the variable
            string attributeString = "Walmart";

            // Pass the string to the method and get de result in response variable
            var response = AddProductPageModel.CheckNull(attributeString);

            // Assert that checks if the string is not null
            Assert.IsNotNull(response);
        }

        // Test by Geancarlo Rivera Hernández C06516
        [Test]
        public void CreateProductWhenCategoryIsNull()
        {
            // Arrange for the mock configuration
            // Create an instance of the page model (pageModel) using the CreatePageModel()
            var pageModel = (AddProductPageModel)CreatePageModel("add_register_page");

            // Initializes the attributes that the new product needs to be created with a null category
            Category? category = null;
            string productName = "Arroz";
            string brandName = "TIO PELON";
            string modelName = "1Kg";

            // Call the create method with the attributes and get the new product
            var newProduct = pageModel.CreateProduct(productName, brandName, modelName, category);

            // Asserts that checks if the attributes of the new product are equal to
            // the initialized attributes sended as parameters in the create method
            Assert.AreEqual(productName, newProduct.Name);
            Assert.AreEqual(brandName, newProduct.Brand);
            Assert.AreEqual(modelName, newProduct.Model);

            // Assert that checks if the number of elements in the list of categories of the new product is 0 
            Assert.Zero(newProduct.Categories!.Count);
        }

        // Test by Geancarlo Rivera Hernández C06516
        [Test]
        public void CreateProductWhenCategoryIsNotNull()
        {
            // Arrange for the mock configuration
            // Create an instance of the page model (pageModel) using the CreatePageModel()
            var pageModel = (AddProductPageModel)CreatePageModel("add_register_page");

            // Initializes the attributes that the new product needs to be created with a non null category
            Category? category = new Category { CategoryName = "Comida" };
            string productName = "Arroz";
            string brandName = "TIO PELON";
            string modelName = "1Kg";

            // Call the create method with the attributes and get the new product
            var newProduct = pageModel.CreateProduct(productName, brandName, modelName, category);

            // Asserts that checks if the attributes of the new product are equal to
            // the initialized attributes sended as parameters in the create method
            Assert.AreEqual(productName, newProduct.Name);
            Assert.AreEqual(brandName, newProduct.Brand);
            Assert.AreEqual(modelName, newProduct.Model);

            // Assert that checks if the number of elements in the list of categories of the new product is greater than 0 
            Assert.Greater(newProduct.Categories!.Count, 0);
        }

        // Test by Geancarlo Rivera Hernández C06516
        [Test]
        public void CreateRegister()
        {
            // Arrange for the mock configuration
            // Create an instance of the page model (pageModel) using the CreatePageModel()
            var pageModel = (AddProductPageModel)CreatePageModel("add_register_page");

            // Initializes the attributes that the new register needs to be created
            var product = pageModel.CreateProduct("Arroz", "TIO PELON", "1Kg", null);
            var provincia = new Provincia() { Name = "Heredia" };
            var canton = new Canton() { CantonName = "Santo Domingo", Provincia = provincia };
            var store = new Store() { Name = "Pali", Location = canton };
            var comment = "Estaba en oferta";
            var user = new User() { UserName = "Gean", Location = canton, Id = "c06516" };

            // Call the create method with the attributes and get the new register
            var newRegister = pageModel.CreateRegister(product, store, 500, comment, user);

            // Asserts that checks if the attributes of the new register are equal to
            // the initialized attributes sended as parameters in the create method
            Assert.AreEqual(product.Name, newRegister.Product.Name);
            Assert.AreEqual(store.Name, newRegister.Store.Name);
            Assert.AreEqual(500, newRegister.Price);
            Assert.AreEqual(comment, newRegister.Comment);
            Assert.AreEqual(user.Id, newRegister.Contributor.Id);
        }

        // Test by Geancarlo Rivera Hernández C06516
        [Test]
        public void CreateFormatedImagesListWithoutImages()
        {
            var pageModel = (AddProductPageModel)CreatePageModel("add_register_page");

            // Initializes the attributes that the new register needs to be created
            DateTime inputDateTime = new DateTime(2023, 11, 4, 15, 30, 45, 0);
            var product = pageModel.CreateProduct("Arroz", "TIO PELON", "1Kg", null);
            var provincia = new Provincia() { Name = "Heredia" };
            var canton = new Canton() { CantonName = "Santo Domingo", Provincia = provincia };
            var store = new Store() { Name = "Pali", Location = canton };
            var user = new User() { UserName = "Gean", Location = canton, Id = "c06516" };

            pageModel.ProductImages = null;
            var result = pageModel.CreateFormattedImagesList(user, inputDateTime, product, store);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        // Test by Geancarlo Rivera Hernández C06516
        [Test]
        public void CreateFormattedImagesListWithNonNullImages() {
            // Arrange for the mock configuration
            // Create an instance of the page model (pageModel) using the CreatePageModel()
            var pageModel = (AddProductPageModel)CreatePageModel("add_register_page");

            // Initializes the attributes that the new register needs to be created
            DateTime inputDateTime = new DateTime(2023, 11, 4, 15, 30, 45, 0);
            var product = pageModel.CreateProduct("Arroz", "TIO PELON", "1Kg", null);
            var provincia = new Provincia() { Name = "Heredia" };
            var canton = new Canton() { CantonName = "Santo Domingo", Provincia = provincia };
            var store = new Store() { Name = "Pali", Location = canton };
            var user = new User() { UserName = "Gean", Location = canton, Id = "c06516" };

            byte[] imageBytes = new byte[]
            {
                0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01, 0x01, 0x00, 0x00, 0x01,
                0x00, 0x01, 0x00, 0x00, 0xFF, 0xDB, 0x00, 0x43, 0x00, 0x03, 0x02, 0x02, 0x03, 0x02, 0x02, 0x03,
                0x03, 0x03, 0x03, 0x04, 0x03, 0x03, 0x04, 0x05, 0x08, 0x05, 0x05, 0x04, 0x04, 0x05, 0x0A, 0x07,
                0x07, 0x06, 0x08, 0x0C, 0x0A, 0x0C, 0x0C, 0x0B, 0x0A, 0x0B, 0x0B, 0x0D, 0x0E, 0x12, 0x10, 0x0D
            };

            var formFiles = new List<IFormFile>
            {
               new FormFile(new MemoryStream(imageBytes), 0, imageBytes.Length, "ProductImages", "image1.jpg")
               {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/jpeg"
               },
               new FormFile(new MemoryStream(imageBytes), 0, imageBytes.Length, "ProductImages", "image2.jpg")
               {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/jpeg"
               }
            };

            pageModel.ProductImages = formFiles;
            var result = pageModel.CreateFormattedImagesList(user, inputDateTime, product, store);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        // Test by Geancarlo Rivera Hernández C06516
        [Test]
        public void CreateFormattedImagesListWithZeroBytesImages()
        {
            // Arrange for the mock configuration
            // Create an instance of the page model (pageModel) using the CreatePageModel()
            var pageModel = (AddProductPageModel)CreatePageModel("add_register_page");

            // Initializes the attributes that the new register needs to be created
            DateTime inputDateTime = new DateTime(2023, 11, 4, 15, 30, 45, 0);
            var product = pageModel.CreateProduct("Arroz", "TIO PELON", "1Kg", null);
            var provincia = new Provincia() { Name = "Heredia" };
            var canton = new Canton() { CantonName = "Santo Domingo", Provincia = provincia };
            var store = new Store() { Name = "Pali", Location = canton };
            var user = new User() { UserName = "Gean", Location = canton, Id = "c06516" };

            byte[] imageBytes = new byte[] { };

            var formFiles = new List<IFormFile>
            {
               new FormFile(new MemoryStream(imageBytes), 0, imageBytes.Length, "ProductImages", "image1.jpg")
               {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/jpeg"
               },
               new FormFile(new MemoryStream(imageBytes), 0, imageBytes.Length, "ProductImages", "image2.jpg")
               {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/jpeg"
               }
            };

            pageModel.ProductImages = formFiles;
            var result = pageModel.CreateFormattedImagesList(user, inputDateTime, product, store);

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }
    }
}

