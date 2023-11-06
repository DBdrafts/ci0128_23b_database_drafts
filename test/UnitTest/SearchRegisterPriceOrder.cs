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

namespace SearchRegistersPriceOrderTest
{
    /*[TestClass]
    // Declaration of the test class
    public class RazorPageTests : BaseTest
    {
        //  Test by Julio Alejandro Rodríguez Salguera C16717
        [Test]
        public void SearchProductPageOrdersByPriceAscendant()
        {
            // Arrange for the mock configuration
            // Create an instance of the page model (pageModel) using the CreatePageModel().
            var pageModel = (SearchPageModel)CreatePageModel("search_page");

            // Initialize a list of registers
            List<Register> registers = InitRegisters();

            // Method OrderRegisters to test from SearchPage
            // Name of sort is the kind of sort, and registers the list of order
            var response = pageModel.OrderRegisters(registers, "price_desc").ToArray();

            // Assert to check if first element is higher that second
            for (int i = 0; i < response.Length - 1; i++)
            {
                Assert.IsTrue(response[i].Price >= response[i + 1].Price);
            }

        }

        //  Test by Julio Alejandro Rodríguez Salguera C16717
        [Test]
        public void SearchProductPageOrdersByPriceDescend()
        {
            // Arrange for the mock configuration
            // Create an instance of the page model (pageModel) using the CreatePageModel().
            var pageModel = (SearchPageModel)CreatePageModel("search_page");

            // Initialize a list of registers
            List<Register> registers = InitRegisters();


            // Method OrderRegisters to test from SearchPage
            // Name of sort is the kind of sort, and registers the list of order
            var response = pageModel.OrderRegisters(registers, "price_asc").ToArray();

            // Assert to check if first element is lower that second
            for (int i = 0; i < response.Length - 1; i++)
            {
                Assert.IsTrue(response[i].Price <= response[i + 1].Price);
            }

        }

        //  Test by Julio Alejandro Rodríguez Salguera C16717
        [Test]
        public void SearchProductPageOrdersByPriceNull()
        {
            // Arrange for the mock configuration
            // Create an instance of the page model (pageModel) using the CreatePageModel().
            var pageModel = (SearchPageModel)CreatePageModel("search_page");

            // Initialize a list of registers as null
            List<Register>? registers = null;

            // Method OrderRegisters to test from SearchPage
            // Name of sort is the kind of sort, and registers the list of order
            // Having the register list as null
            var response = pageModel.OrderRegisters(registers, "price_asc").ToArray();

            // Assert to check if first element is lower that second
            for (int i = 0; i < response.Length - 1; i++)
            {
                Assert.IsTrue(response[i].Price <= response[i + 1].Price);
            }
        }

        //  Test by Julio Alejandro Rodríguez Salguera C16717
        [Test]
        public void SearchProductPageOrdersByPriceEmpty()
        {
            // Arrange for the mock configuration
            // Create an instance of the page model (pageModel) using the CreatePageModel().
            var pageModel = (SearchPageModel)CreatePageModel("search_page");

            // Initialize a list of registers as null
            List<Register> registers = new List<Register>();

            // Method OrderRegisters to test from SearchPage
            // Name of sort is the kind of sort, and registers the list of order
            // Having the register list as null
            var response = pageModel.OrderRegisters(registers, "price_desc").ToArray();

            // Assert to check if first element is lower that second
            for (int i = 0; i < response.Length - 1; i++)
            {
                Assert.IsTrue(response[i].Price <= response[i + 1].Price);
            }
        }

        //  Test by Julio Alejandro Rodríguez Salguera C16717
        [Test]
        public void SearchProductPageGetSortOrderAscDesc()
        {
            // Arrange for the mock configuration
            // Create an instance of the page model (pageModel) using the CreatePageModel().
            var pageModel = (SearchPageModel)CreatePageModel("search_page");

            // String to sort by price ascendant
            string sortOrderAsc = "price_asc";

            // Checks if the sort order didn´t change 
            Assert.IsTrue(pageModel.GetSortOrder(sortOrderAsc).Equals(sortOrderAsc));

            // String to sort by price descendant
            string sortOrderDesc = "price_desc";

            // Checks if the sort order didn´t change 
            Assert.IsTrue(pageModel.GetSortOrder(sortOrderDesc).Equals(sortOrderDesc));
        }

        //  Test by Julio Alejandro Rodríguez Salguera C16717
        [Test]
        public void SearchProductPageGetSortOrderNullEmpty()
        {
            // Arrange for the mock configuration
            // Create an instance of the page model (pageModel) using the CreatePageModel().
            var pageModel = (SearchPageModel)CreatePageModel("search_page");

            // String null to sort
            string? sortOrderNull = null;

            // Checks if the sort order change to the default 
            Assert.IsTrue(pageModel.GetSortOrder(sortOrderNull).Equals("price_asc"));

            // String empty to sort
            string sortOrderEmpty= "";

            // Checks if the sort order change to the default 
            Assert.IsTrue(pageModel.GetSortOrder(sortOrderEmpty).Equals("price_asc"));
        }
    }*/
}