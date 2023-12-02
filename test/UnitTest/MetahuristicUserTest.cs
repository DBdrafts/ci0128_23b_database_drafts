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

namespace MetahuristicUserTest
{
    [TestClass]
    // Declaration of the test class
    public class RazorPageTests : BaseTest
    {
        // Test by Omar Camacho Calvo | Sprint 3
        [Test]
        public void CheckUserAutomaticMatchingId()
        {
            // Arrange
            // Mock configuration or set up necessary dependencies
            var pageModel = (ModerateAnomaliesPageModel)CreatePageModel("moderate_anormal_registers");

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
                Id = "7d5b4e6b-28eb-4a70-8ee6-e7378e024aa4",
                UserName = "Automatic",
                NormalizedUserName = "AUTOMATIC",
                Email = "automatic123@gmail.com",
                NormalizedEmail = "AUTOMATIC123@GMAIL.COM",
                PasswordHash = "AQAAAAIAAYagAAAAEJfi9TUT6VewLFHdzos2qZ29eaoRr4s0YjS60YhkekCR0Mzbe5LMp3sYgj+elkblVA==",
                Location = canton,
                Geolocation = geolocationSelected
            };

            // Create a user for testing
            var testUserId = "7d5b4e6b-28eb-4a70-8ee6-e7378e024aa4";


            // Act
            // Call the checkUserAutomatic method with the test user and its ID
            var result = pageModel.checkUserAutomatic(user, testUserId);

            // Assert
            // Check if the result is true, indicating a match
            Assert.IsTrue(result);
        }

        // Test by Omar Camacho Calvo | Sprint 3
        [Test]
        public void CheckUserAutomaticNoMatchingId()
        {
            // Arrange
            // Mock configuration or set up necessary dependencies
            var pageModel = (ModerateAnomaliesPageModel)CreatePageModel("moderate_anormal_registers");

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
                Id = "73af5899a-3957-415a-9675-be20966ba6d7",
                UserName = "Automatic",
                NormalizedUserName = "AUTOMATIC",
                Email = "automatic123@gmail.com",
                NormalizedEmail = "AUTOMATIC123@GMAIL.COM",
                PasswordHash = "AQAAAAIAAYagAAAAEJfi9TUT6VewLFHdzos2qZ29eaoRr4s0YjS60YhkekCR0Mzbe5LMp3sYgj+elkblVA==",
                Location = canton,
                Geolocation = geolocationSelected
            };

            // Create a user for testing
            var testUserId = "7d5b4e6b-28eb-4a70-8ee6-e7378e024aa4";


            // Act
            // Call the checkUserAutomatic method with the test user and its ID
            var result = pageModel.checkUserAutomatic(user, testUserId);

            // Assert
            // Check if the result is true, indicating a match
            Assert.IsFalse(result);
        }

        // Test by Omar Camacho Calvo | Sprint 3
        [Test]
        public void CheckUserAutomaticForNullUser()
        {
            // Arrange
            // Mock configuration or set up necessary dependencies
            var pageModel = (ModerateAnomaliesPageModel)CreatePageModel("moderate_anormal_registers");

            // Create a null user
            User user = null;

            // Create a test user ID (it can be any value since the user is null)
            var testUserId = "7d5b4e6b-28eb-4a70-8ee6-e7378e024aa4";

            // Act
            // Call the checkUserAutomatic method with a null user and a test ID
            var result = pageModel.checkUserAutomatic(user, testUserId);

            // Assert
            // Check if the result is false, indicating no match (since the user is null)
            Assert.IsFalse(result);
        }

        // Test by Omar Camacho Calvo | Sprint 3
        [Test]
        public void CheckUserAutomatiSensitiveId()
        {
            // Arrange
            // Mock configuration or set up necessary dependencies
            var pageModel = (ModerateAnomaliesPageModel)CreatePageModel("moderate_anormal_registers");

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
                Id = "7d5b4e6b-28eb-4a70-8ee6-e7378e024aa4",
                UserName = "Automatic",
                NormalizedUserName = "AUTOMATIC",
                Email = "automatic123@gmail.com",
                NormalizedEmail = "AUTOMATIC123@GMAIL.COM",
                PasswordHash = "AQAAAAIAAYagAAAAEJfi9TUT6VewLFHdzos2qZ29eaoRr4s0YjS60YhkekCR0Mzbe5LMp3sYgj+elkblVA==",
                Location = canton,
                Geolocation = geolocationSelected
            };

            // Almost same ID but uppercase for not match
            var testUserId = "7D5B4e6B-28eb-4a70-8ee6-e7378e024aa4";

            // Act
            // Call the checkUserAutomatic method with the test user and its ID
            var result = pageModel.checkUserAutomatic(user, testUserId);

            // Assert
            // Check if the result is False, indicating a expected not match
            Assert.IsFalse(result);
        }

        // Test by Omar Camacho Calvo | Sprint 3
        [Test]
        public void CheckUserAutomaticWithNoID()
        {
            // Arrange
            // Mock configuration or set up necessary dependencies
            var pageModel = (ModerateAnomaliesPageModel)CreatePageModel("moderate_anormal_registers");

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
                UserName = "Automatic",
                NormalizedUserName = "AUTOMATIC",
                Email = "automatic123@gmail.com",
                NormalizedEmail = "AUTOMATIC123@GMAIL.COM",
                PasswordHash = "AQAAAAIAAYagAAAAEJfi9TUT6VewLFHdzos2qZ29eaoRr4s0YjS60YhkekCR0Mzbe5LMp3sYgj+elkblVA==",
                Location = canton,
                Geolocation = geolocationSelected
            };

            // Almosta ID to check
            var testUserId = "7d5b4e6b-28eb-4a70-8ee6-e7378e024aa4";

            // Act
            // Call the checkUserAutomatic method with the test user and its ID
            var result = pageModel.checkUserAutomatic(user, testUserId);

            // Assert
            // Check if the result is False, indicating a expected not match
            Assert.IsFalse(result);
        }
    }
}
