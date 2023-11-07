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
using NetTopologySuite.Geometries;

namespace UserLocationTests
{
    [TestClass]
    public class RazorPageTests : BaseTest
    {
        // Test by Omar Fabian Camacho Calvo C11476 | Sprint 2
        [Test]
        public void CheckUserWithLocation()
        {
            // Arrange for the mock configuration
            // Create an instance of the page model (pageModel) using the CreatePageModel().
            var pageModel = (SearchPageModel)CreatePageModel("search_page");

            // Create a dot for geolocation.
            double longitude = -83.585642333984381;
            double latitude = 9.6130706857624624;
            var coordinates = new Coordinate(longitude, latitude);
            var geolocationSelected = new Point(coordinates.X, coordinates.Y) { SRID = 4326 };

            // Create an instance of Province 
            var province = new Provincia { Name = "Cartago" };
            // Create an instance of Canton
            var canton = new Canton { CantonName = "Cartago", Provincia = province };

            // Initialize a user
            var user = new User
            {
                Id = "3af5899a-3957-415a-9675-be20966ba6d7",
                UserName = "Prueba123",
                NormalizedUserName = "PRUEBA123",
                Email = "prueba123@gmail.com",
                NormalizedEmail = "PRUEBA123@GMAIL.COM",
                PasswordHash = "AQAAAAIAAYagAAAAEJfi9TUT6VewLFHdzos2qZ29eaoRr4s0YjS60YhkekCR0Mzbe5LMp3sYgj+elkblVA==",
                Location = canton,
                Geolocation = geolocationSelected
            };
                       
            // Act
            // Stores the result if the user stores geolocation
            var result = pageModel.UserhasLocation(user);

            // Assert
            // Check if the user has geolocation, so the result should be true
            Assert.IsTrue(result);
        }

        // Test by Omar Fabian Camacho Calvo C11476 | Sprint 2
        [Test]
        public void CheckUserWithoutLocation()
        {
            // Arrange for the mock configuration
            // Create an instance of the page model (pageModel) using the CreatePageModel().
            var pageModel = (SearchPageModel)CreatePageModel("search_page");

            // Create an instance of Province 
            var province = new Provincia { Name = "Cartago" };
            // Create an instance of Canton
            var canton = new Canton { CantonName = "Cartago", Provincia = province };

            // Initialize a user
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

            // Act
            // Stores the result if the user stores geolocation
            var result = pageModel.UserhasLocation(user);

            // Assert
            // Check if the user has geolocation, so the result should be true
            Assert.IsFalse(result);
        }
    }
}
