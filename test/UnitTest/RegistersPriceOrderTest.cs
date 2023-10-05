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

namespace RegistersPriceOrderTest
{
    [TestClass]
    // Declaration of the test class
    public class RazorPageTests : BaseTest
    {
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
    }
}