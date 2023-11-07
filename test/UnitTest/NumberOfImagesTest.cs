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

namespace NumberOfImagesTest
{
    [TestClass]
    // Declaration of the test class
    public class RazorPageTests : BaseTest
    {
        // Test by Alonso León Rodríguez B94247 | Sprint 2
        [Test]
        public void getNumberOfImagesInRegister()
        {
            // Arrange
            // Mock configuration
            // Create an instance of the page model (pageModel) using the CreatePageModel().
            var pageModel = (ProductPageModel)CreatePageModel("product_page");

            // Initialize a the register whitout images
            // Create an instance of Province 
            var province = new Provincia { Name = "Cartago" };
            // Create an instance of Canton
            var canton = new Canton { CantonName = "Cartago", Provincia = province };
            // Create an instance of Store
            var store = new Store()
            { Name = "Tienda", CantonName = "Cartago", ProvinciaName = "Cartago", Location = canton };
            // Create an instance of User
            var user1 = new User() { UserName = "Alr", Location = canton, Id = "12345" };
            // Create a new Product
            var product = new Product() { Name = "Laptop" };

            // Create an instance of class for random values
            Random random = new Random();

            // Seed of values for random datetime
            int year = random.Next(2022, 2024);
            int month = random.Next(1, 13);
            int day = random.Next(1, 29);
            int hour = random.Next(0, 24);
            int minute = random.Next(0, 60);
            int second = random.Next(0, 60);

            // Create a new Datetime with the random values of the seed
            DateTime randomDate = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);

            // Add a new register whit that information
            var newRegister = new Register()
            {
                Product = product,
                Contributor = user1,
                Store = store,
                Price = random.Next(743, 1369), // Add a random number for the price
                SubmitionDate = randomDate,
                Comment = "comment",
                CantonName = "Cartago",
                ProvinciaName = "Cartago",
            };

            // Add a new Image with a reference to the Register
            var image = new Image
            {
                Contributor = user1,
                Register = newRegister,
                ImageId = random.Next(1, 1000),
                SubmitionDate = randomDate,
                ImageData = null, // nothing in the space of imageData
                ImageType = "image/jpeg",
                ContributorId = user1.Id,
                ProductName = product.Name,
                StoreName = store.Name,
                CantonName = "Cartago",
                ProvinceName = "Cartago"
            };

            // Add the Images to the Register's Reports collection
            newRegister.Images = new List<Image> { image };

            // Act
            // Get the bool in case register has images
            int numImages = pageModel.GetNumberOfImagesForRegister(newRegister);

            // Assert
            // Check if the register has images, so the result should be true
            Assert.AreEqual(1, numImages);

        }
    }
