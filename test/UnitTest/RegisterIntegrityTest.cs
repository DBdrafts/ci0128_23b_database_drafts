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
using NUnit.Framework.Internal;
using Microsoft.Win32;

namespace RegisterIntegrityTest
{
    [TestClass]
    public class RazorPageTests : BaseTest
    {
        // Test by Omar Fabian Camacho Calvo C11476 | Sprint 2
        [Test]
        public void MaxReportTest()
        {
            // Arrange for the mock configuration
            // Create an instance of the page model (pageModel) using the CreatePageModel().
            var pageModel = (ProductPageModel)CreatePageModel("product_page");

            // Initialize a collection of registers
            ICollection<Register> registers = InitRegisters();

            foreach (var register in registers)
            {
                // Act
                // Stores the number of the status report
                var result = pageModel.GetHighestReportState(register);

                // Define the expected highest report state value for this specific register
                int expectedHighestReportState = 2;

                // Assert
                // If the number is upper to value 2, the test and logic fails
                Assert.True(result <= expectedHighestReportState);
            }
        }

        // Test by Omar Fabian Camacho Calvo C11476 | Sprint 2
        [Test]
        public void ReportStatusAreNotNegative()
        {
            // Arrange
            // Mock configuration
            // Create an instance of the page model (pageModel) using the CreatePageModel().
            var pageModel = (ProductPageModel)CreatePageModel("product_page");

            // Initialize a collection of registers
            ICollection<Register> registers = InitRegisters();

            // Act and Assert
            foreach (var register in registers)
            {
                // Act
                // Stores the number of the status report
                var result = pageModel.GetHighestReportState(register);

                // Assert
                // If the number is lower to value 0
                Assert.False(result < 0);
            }
        }

        // Test by Omar Fabian Camacho Calvo C11476 | Sprint 2
        [Test]
        public void GetHighestReportState_ReturnsValidValue()
        {
            // Arrange
            var pageModel = (ProductPageModel)CreatePageModel("product_page");

            // Initialize a collection of registers
            ICollection<Register> registers = InitRegisters();

            // Act and Assert
            foreach (var register in registers)
            {
                // Act
                // Stores the number of the status report
                var result = pageModel.GetHighestReportState(register);

                // Assert
                // If in range [0,2] cause the number of Status for
                Assert.That(result, Is.InRange(0, 2));
            }
        }

        // Test by Omar Fabian Camacho Calvo C11476 | Sprint 2
        [Test]
        public void GetNumberOfRegisters_Empty()
        {
            // Arrange
            var pageModel = (ProductPageModel)CreatePageModel("product_page");

            // Initialize a collection of registers as an IQueryable
            // empty register list
            var emptyRegisters = new List<Register>().AsQueryable();

            // Act
            // Get the amount of registers 
            var result = pageModel.GetNumberOfRegisters(emptyRegisters);

            // Assert
            // ckeck a return as number 0, to avoid 'null' o 'false' return in a
            // expected decimal value  
            Assert.AreEqual(0, result);
        }

        // Test by Omar Fabian Camacho Calvo C11476 | Sprint 2
        [Test]
        public void ImagesAreNotNullVerify()
        {
            // Arrange
            // Mock configuration
            // Create an instance of the page model (pageModel) using the CreatePageModel().
            var pageModel = (ProductPageModel)CreatePageModel("product_page");

            // Initialize a collection of registers
            ICollection<Register> registers = InitRegisters();

            // Act and Assert
            foreach (var register in registers)
            {
                // Act
                // Get the bool in case register has images
                bool hasImages = pageModel.RegisterHasImages(register);

                // Assert
                // Check if the register has images, so the result should be true
                Assert.IsTrue(hasImages);
            }
        }

        // Test by Omar Fabian Camacho Calvo C11476 | Sprint 2
        [Test]
        public void ImagesAreNullVerify()
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
            var user = new User
            {
                Id = "3af5899a-3957-415a-9675-be20966ba6d7",
                UserName = "Prueba123",
                NormalizedUserName = "PRUEBA123",
                Email = "prueba123@gmail.com",
                NormalizedEmail = "PRUEBA123@GMAIL.COM",
                PasswordHash = "AQAAAAIAAYagAAAAEJfi9TUT6VewLFHdzos2qZ29eaoRr4s0YjS60YhkekCR0Mzbe5LMp3sYgj+elkblVA==",
                Location = canton
            };
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
                Contributor = user,
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
                Contributor = user,
                Register = newRegister,
                ImageId = random.Next(1, 1000),
                SubmitionDate = randomDate,
                ImageData = null, // nothing in the space of imageData
                ImageType = "image/jpeg",
                ContributorId = user.Id,
                ProductName = product.Name,
                StoreName = store.Name,
                CantonName = "Cartago",
                ProvinceName = "Cartago"
            };

            // Add the Images to the Register's Reports collection
            newRegister.Images = new List<Image> { image };

            // Act
            // Get the bool in case register has images
            bool hasImages = pageModel.RegisterHasImages(newRegister);

            // Assert
            // Check if the register has images, so the result should be true
            Assert.IsFalse(hasImages);
            
        }

    }
}
