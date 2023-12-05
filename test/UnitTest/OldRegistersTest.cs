using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OldRegistersTest
{
    [TestClass]
    public class RazorPageTests : BaseTest
    {
        // Test by Alonso León Rodríguez B94247  | Sprint 3
        [Test]
        public void calculate80PercentIndexSuccess()
        {
            // Arrange
            var pageModel = (ModerateOldRegistersModel)CreatePageModel("moderate_old_registers");
            int totalRegisters = 11;

            // Act
            int _80PercentIndex = pageModel.calculate80PercentIndex(totalRegisters);

            // Assert
            int expected = 9;
            Assert.AreEqual(expected, _80PercentIndex);
        }

        // Test by Alonso León Rodríguez B94247  | Sprint 3
        [Test]
        public void calculate80PercentIndexOneRegister()
        {
            // Arrange
            var pageModel = (ModerateOldRegistersModel)CreatePageModel("moderate_old_registers");
            int totalRegisters = 1;

            // Act
            int _80PercentIndex = pageModel.calculate80PercentIndex(totalRegisters);

            // Assert
            int expected = 1;
            Assert.AreEqual(expected, _80PercentIndex);
        }
        // Test by Alonso León Rodríguez B94247  | Sprint 3
        [Test]
        public void existentOldRegisters()
        {
            // Arrange
            var pageModel = (ModerateOldRegistersModel)CreatePageModel("moderate_old_registers");
            var product = new Product()
            {
                Name = "Galletas",
                Brand = "Oreo",
                Model = "Chocolate",
                Stores = new List<Store>()
            };
            var provincia = new Provincia() { Name = "San José" };
            var canton = new Canton() { CantonName = "Tibás", Provincia = provincia };

            var store = new Store() { Name = "Walmart", Location = canton };
            product.Stores.Add(store);

            var user1 = new User() { UserName = "Alr", Location = canton, Id = "12345" };
            var user2 = new User() { UserName = "Lar", Location = canton, Id = "54321" };

            var register1 = new Register()
            {
                Contributor = user1,
                Product = product,
                ProductName = product.Name,
                Store = store,
                StoreName = store.Name,
                SubmitionDate = new DateTime(2023, 10, 2, 9, 0, 0, DateTimeKind.Utc),
                Price = 500
            }; 
            var register2 = new Register()
            {
                Contributor = user2,
                Product = product,
                ProductName = product.Name,
                Store = store,
                StoreName = store.Name,
                SubmitionDate = new DateTime(2023, 9, 2, 9, 0, 0, DateTimeKind.Utc),
                Price = 500
            };
            var register3 = new Register()
            {
                Contributor = user2,
                Product = product,
                ProductName = product.Name,
                Store = store,
                StoreName = store.Name,
                SubmitionDate = new DateTime(2023, 1, 2, 9, 0, 0, DateTimeKind.Utc),
                Price = 500
            };

            pageModel.Products.Add(product);
            pageModel.Registers.Add(register1);
            pageModel.Registers.Add(register2);
            pageModel.Registers.Add(register3);

            // Act
            pageModel.lookForOldRegisters(2);

            // Assert
            int expected = 1;
            Assert.AreEqual(expected, pageModel.OldRegisters.Count);
        }

        // Test by Alonso León Rodríguez B94247  | Sprint 3
        [Test]
        public void nonExistenOldRegisters()
        {
            // Arrange
            var pageModel = (ModerateOldRegistersModel)CreatePageModel("moderate_old_registers");
            var product = new Product()
            {
                Name = "Galletas",
                Brand = "Oreo",
                Model = "Chocolate",
                Stores = new List<Store>()
            };
            var provincia = new Provincia() { Name = "San José" };
            var canton = new Canton() { CantonName = "Tibás", Provincia = provincia };

            var store = new Store() { Name = "Walmart", Location = canton };
            product.Stores.Add(store);

            var user1 = new User() { UserName = "Alr", Location = canton, Id = "12345" };
            var user2 = new User() { UserName = "Lar", Location = canton, Id = "54321" };

            var register1 = new Register()
            {
                Contributor = user1,
                Product = product,
                ProductName = product.Name,
                Store = store,
                StoreName = store.Name,
                SubmitionDate = new DateTime(2023, 10, 2, 9, 0, 0, DateTimeKind.Utc),
                Price = 500
            };
            var register2 = new Register()
            {
                Contributor = user2,
                Product = product,
                ProductName = product.Name,
                Store = store,
                StoreName = store.Name,
                SubmitionDate = new DateTime(2023, 9, 2, 9, 0, 0, DateTimeKind.Utc),
                Price = 500
            };
            var register3 = new Register()
            {
                Contributor = user2,
                Product = product,
                ProductName = product.Name,
                Store = store,
                StoreName = store.Name,
                SubmitionDate = new DateTime(2023, 8, 15, 9, 0, 0, DateTimeKind.Utc),
                Price = 500
            };

            pageModel.Products.Add(product);
            pageModel.Registers.Add(register1);
            pageModel.Registers.Add(register2);
            pageModel.Registers.Add(register3);

            // Act
            pageModel.lookForOldRegisters(2);

            // Assert
            int expected = 0;
            Assert.AreEqual(expected, pageModel.OldRegisters.Count);

        }
    }
}