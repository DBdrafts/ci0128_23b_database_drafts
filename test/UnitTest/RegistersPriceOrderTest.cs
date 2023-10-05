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
//using Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using Assert = NUnit.Framework.Assert;
using System;
using Microsoft.AspNetCore.Http.HttpResults;

namespace RegistersPriceOrderTest
{
    [TestClass]
    // Declaration of the test class
    public class RazorPageTests
    {
        // Method to create the mock of the Razor Page Model.
        public ProductPageModel CreatePageModel()
        {
            // Create a simulation object (mock) de IConfiguration
            // Simule the real behavior of the real object with the configuration
            var mockConfiguration = new Mock<IConfiguration>();

            // Set the mock's behavior and then return "TestSettingValue" when setup done
            mockConfiguration.Setup(c => c["SomeSetting"]).Returns("TestSettingValue");

            // Create a instance for the options for the context of DB
            // Configure the context of DB to not use a real DB
            // Get the configuration options of the LoCoMProContext DB 
            var options = new DbContextOptionsBuilder<LoCoMProContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            // Create a context instance of the DB using the configured options
            var dbContext = new LoCoMProContext(options);

            // return the new instance of new ProductPageModel instance
            // created with the database context and mock configuration
            // real intance
            return new ProductPageModel(dbContext, mockConfiguration.Object);
        }

        [Test]
        public void ProductPageOrdersByPriceAscendent()
        {
            // Arrange for the mock configuration
            // Create an instance of the page model (pageModel) using the CreatePageModel().
            var pageModel = CreatePageModel();

            // Initialize a collection of registers
            ICollection<Register> registers = InitRegisters();

            // Method OrderRegistersByPrice to test from ProductPage
            // Name of sort is the kind of sort, and registers the Icollection list of order
            var response = pageModel.OrderRegistersByPrice("price_desc", ref registers).ToArray();

            // Assert to check if first element is higher to second
            for (int i = 0; i < response.Length - 1; i++)
            {
                Assert.IsTrue(response[i].Price >= response[i + 1].Price);
            }

        }

        [Test]
        public void ProductPageOrdersByPriceDescend()
        {
            // Arrange for the mock configuration
            // Create an instance of the page model (pageModel) using the CreatePageModel().
            var pageModel = CreatePageModel();

            // Initialize a collection of registers
            ICollection<Register> registers = InitRegisters();


            // Method OrderRegistersByPrice to test from ProductPage
            // Name of sort is the kind of sort, and registers the Icollection list of order
            var response = pageModel.OrderRegistersByPrice(" ", ref registers).ToArray();

            // Assert to check if first element is lower to second
            for (int i = 0; i < response.Length - 1; i++)
            {
                Assert.IsTrue(response[i].Price <= response[i + 1].Price);
            }

        }

        // Method to Initialize the registers
        public List<Register> InitRegisters()
        {
            // Create an instance of Province 
            var province = new Provincia { Name = "Cartago" };
            // Create an instance of Canton
            var canton = new Canton { CantonName = "Cartago", Provincia = province };
            // Create an instance of Store
            var store = new Store() 
            { Name = "Tienda", CantonName = "Cartago", ProvinciaName = "Cartago", Location = canton };
            // Create an instance of User
            var user = new User()
            { Id = "03", UserName = "Persona",Password = "123456", Email = "email@gmail.com",Location = canton };

            // Create an instance of class for random values
            Random random = new Random();
            // Create an instance of products
            var product = new Product() { Name = "Laptop" };

            // Initiliaze an empty list of objects registers
            var result = new List<Register>();
            // For to create 10 objects 
            for (int i = 0; i < 10; ++i)
            {   
                // Seed of values for random datetime
                int year = random.Next(2022, 2024); 
                int month = random.Next(1, 13); 
                int day = random.Next(1, 29); 
                int hour = random.Next(0, 24); 
                int minute = random.Next(0, 60); 
                int second = random.Next(0, 60);

                // Create a new Datetime with the tandom values of the seed
                DateTime randomDate = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);

                // Add registers to list
                result.Add(new Register()
                {   
                    Product = product,
                    Contributor = user,
                    Store = store,
                    // Add a random number for the price
                    Price = random.Next(743, 1369),
                    SubmitionDate = randomDate,
                    Comment = "comment"
                });
            }
            // returns the list of initialized registers
            return result;
        }
    }
}