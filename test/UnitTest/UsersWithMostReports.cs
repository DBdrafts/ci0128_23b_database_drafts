using LoCoMPro.Data;
using LoCoMPro.Models;
using LoCoMPro.Pages;
using NUnit.Framework;
using OpenQA.Selenium.DevTools.V117.IndexedDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assert = NUnit.Framework.Assert;

namespace UsersWithMostReports
{
    [TestClass]
    public class RazorPageTests : BaseTest
    {
        // Test by Alonso León Rodríguez B94247  | Sprint 3
        [Test]
        public void CalculatePercentageWithNormalCasualParameters()
        {
            // Arrange
            var pageModel = (UsersWhoCreateMoreReportsModel)CreatePageModel("users_who_create_more_reports");
            int totalReports = 23;
            int moderatedReports = 5;

            // Act
            var result = pageModel.calculatePercentage(totalReports, moderatedReports);

            // Assert
            Assert.AreEqual(22, result);
        }

        // Test by Alonso León Rodríguez B94247  | Sprint 3
        [Test]
        public void CalculatePercentageWithInvertedParameters()
        {
            // Arrange
            var pageModel = (UsersWhoCreateMoreReportsModel)CreatePageModel("users_who_create_more_reports");
            int totalReports = 23;
            int moderatedReports = 5;

            // Act
            var result = pageModel.calculatePercentage(moderatedReports, totalReports);

            // Assert
            Assert.AreNotEqual(22, result);
        }

        // Test by Alonso León Rodríguez B94247  | Sprint 3
        [Test]
        public void getReporterSuccess()
        {
            // Arrange
            var pageModel = (UsersWhoCreateMoreReportsModel)CreatePageModel("users_who_create_more_reports");

            var product = new Product()
            {
                Name = "Galletas",
                Brand = "Oreo",
                Model = "Chocolate",
            };
            var provincia = new Provincia() { Name = "San José" };
            var canton = new Canton() { CantonName = "Tibás", Provincia = provincia };
            var store = new Store() { Name = "Walmart", Location = canton };
            var user1 = new User() { UserName = "Alr", Location = canton, Id = "12345" };
            var user2 = new User() { UserName = "Lar", Location = canton, Id = "54321" };
            var date = DateTime.Now;

            var register = new Register()
            {
                Contributor = user1,
                Product = product,
                Store = store,
                SubmitionDate = date,
                Price = 500
            };

            var report = new Report()
            {
                Reporter = user1,
                ReportedRegister = register,
                CantonName = canton.CantonName,
                ProvinceName = provincia.Name,
                ReportDate = date,
            };
            pageModel.Reports.Add(report);

            // Act
            var user = pageModel.getReporter(report);

            // Assert
            string expected = "Alr";
            Assert.AreEqual(expected, user.UserName);
        }

        // Test by Alonso León Rodríguez B94247  | Sprint 3
        [Test]
        public void getNumberOfReportsSuccess()
        {
            // Arrange
            var pageModel = (UsersWhoCreateMoreReportsModel)CreatePageModel("users_who_create_more_reports");

            var product = new Product()
            {
                Name = "Galletas",
                Brand = "Oreo",
                Model = "Chocolate",
            };
            var provincia = new Provincia() { Name = "San José" };
            var canton = new Canton() { CantonName = "Tibás", Provincia = provincia };
            var store = new Store() { Name = "Walmart", Location = canton };
            var user1 = new User() { UserName = "Alr", Location = canton, Id = "12345" };
            var user2 = new User() { UserName = "Lar", Location = canton, Id = "54321" };
            var date = DateTime.Now;

            var register = new Register()
            {
                Contributor = user1,
                Product = product,
                Store = store,
                SubmitionDate = date,
                Price = 500
            };
            var register2 = new Register()
            {
                Contributor = user2,
                Product = product,
                Store = store,
                SubmitionDate = date,
                Price = 500
            };

            var report = new Report()
            {
                Reporter = user1,
                ReportedRegister = register,
                CantonName = canton.CantonName,
                ProvinceName = provincia.Name,
                ReportDate = date,
            };

            var report2 = new Report()
            {
                Reporter = user1,
                ReportedRegister = register2,
                CantonName = canton.CantonName,
                ProvinceName = provincia.Name,
                ReportDate = date,
            };

            pageModel.Reports.Add(report);
            pageModel.Reports.Add(report2);

            // Act
            int numReports = pageModel.numberOfReports(user1);

            // Assert
            Assert.AreEqual(2, numReports);
        }

        // Test by Alonso León Rodríguez B94247  | Sprint 3
        [Test]
        public void getNumberOfApprovedReportsSuccess()
        {
            // Arrange
            var pageModel = (UsersWhoCreateMoreReportsModel)CreatePageModel("users_who_create_more_reports");

            var product = new Product()
            {
                Name = "Galletas",
                Brand = "Oreo",
                Model = "Chocolate",
            };
            var provincia = new Provincia() { Name = "San José" };
            var canton = new Canton() { CantonName = "Tibás", Provincia = provincia };
            var store = new Store() { Name = "Walmart", Location = canton };
            var user1 = new User() { UserName = "Alr", Location = canton, Id = "12345" };
            var user2 = new User() { UserName = "Lar", Location = canton, Id = "54321" };
            var date = DateTime.Now;

            var register = new Register()
            {
                Contributor = user1,
                Product = product,
                Store = store,
                SubmitionDate = date,
                Price = 500
            };
            var register2 = new Register()
            {
                Contributor = user2,
                Product = product,
                Store = store,
                SubmitionDate = date,
                Price = 500
            };

            var report = new Report()
            {
                Reporter = user1,
                ReportedRegister = register,
                CantonName = canton.CantonName,
                ProvinceName = provincia.Name,
                ReportDate = date,
                ReportState = 2
            };

            var report2 = new Report()
            {
                Reporter = user1,
                ReportedRegister = register2,
                CantonName = canton.CantonName,
                ProvinceName = provincia.Name,
                ReportDate = date,
                ReportState = 0
            };

            pageModel.Reports.Add(report);
            pageModel.Reports.Add(report2);

            // Act
            int numApprovedReports = pageModel.numberOfApprovedReports(user1);

            // Assert
            Assert.AreEqual(1, numApprovedReports);
        }

        // Test by Alonso León Rodríguez B94247  | Sprint 3
        [Test]
        public void getNumberOfRejectedReportsSuccess()
        {
            // Arrange
            var pageModel = (UsersWhoCreateMoreReportsModel)CreatePageModel("users_who_create_more_reports");

            var product = new Product()
            {
                Name = "Galletas",
                Brand = "Oreo",
                Model = "Chocolate",
            };
            var provincia = new Provincia() { Name = "San José" };
            var canton = new Canton() { CantonName = "Tibás", Provincia = provincia };
            var store = new Store() { Name = "Walmart", Location = canton };
            var user1 = new User() { UserName = "Alr", Location = canton, Id = "12345" };
            var user2 = new User() { UserName = "Lar", Location = canton, Id = "54321" };
            var date = DateTime.Now;

            var register = new Register()
            {
                Contributor = user1,
                Product = product,
                Store = store,
                SubmitionDate = date,
                Price = 500
            };
            var register2 = new Register()
            {
                Contributor = user2,
                Product = product,
                Store = store,
                SubmitionDate = date,
                Price = 500
            };

            var report = new Report()
            {
                Reporter = user1,
                ReportedRegister = register,
                CantonName = canton.CantonName,
                ProvinceName = provincia.Name,
                ReportDate = date,
                ContributorId = user1.Id,
                ReportState = 0
            };

            var report2 = new Report()
            {
                Reporter = user1,
                ReportedRegister = register2,
                CantonName = canton.CantonName,
                ProvinceName = provincia.Name,
                ReportDate = date,
                ContributorId = user1.Id,
                ReportState = 0
            };

            pageModel.Reports.Add(report);
            pageModel.Reports.Add(report2);

            // Act
            int numRejectedReports = pageModel.numberOfReports(user1);

            // Assert
            Assert.AreEqual(2, numRejectedReports);
        }
    }
}
