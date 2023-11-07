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

namespace ReportedRegisterTest
{
    [TestClass]
    // Declaration of the test class
    public class RazorPageTests : BaseTest
    {
        // Test by Alonso León Rodríguez B94247 | Sprint 2
        [Test]
        public void checkReporterIsOwnerTrue()
        {
            var pageModel = (ProductPageModel)CreatePageModel("product_page");
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
                ContributorId = user1.Id
            };

            // Act
            int result = pageModel.checkReporterIsOwner(report, user1);

            Assert.AreEqual(1, result);
        }

        // Test by Alonso León Rodríguez B94247 | Sprint 2
        [Test]
        public void checkReporterIsNotOwner()
        {
            var pageModel = (ProductPageModel)CreatePageModel("product_page");
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
                ContributorId = user2.Id
            };

            // Act
            int result = pageModel.checkReporterIsOwner(report, user1);

            Assert.AreEqual(0, result);
        }

        // Test by Alonso León Rodríguez B94247 | Sprint 2
        [Test]
        public void checkReporterIsOwnerNull()
        {
            var pageModel = (ProductPageModel)CreatePageModel("product_page");
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
                Contributor = user2,
                Product = product,
                Store = store,
                SubmitionDate = date,
                Price = 500
            };

            // Act
            int result = pageModel.checkReporterIsOwner( null, user1);

            Assert.AreEqual(-1, result);
        }
    }
}

