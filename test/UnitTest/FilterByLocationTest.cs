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
namespace SearchPageFilterTests
{
    [TestClass]
    internal class RazorPageTest : BaseTest
    {
        // Test by Dwayne Taylor Monterrosa C17827
        [Test]
        public void SearchPageFilterByProvince()
        {
            // Arrange for the mock configuration
            // Create an instance of the page model (pageModel) using the CreatePageModel().
            var pageModel = (SearchPageModel)CreatePageModel("search_page");
            // Create filter list.
            List<string> selectedProvinces = new() { "Cartago" };

            // Initialize a collection of registers
            var registers = InitRegisters().AsQueryable();

            // Method FilterByLocation to test from SearchPage
            // Passes the registers and the filter parameters.
            var response = pageModel.FilterByLocation(registers: ref registers, selectedProvinces: selectedProvinces);

            // Assert to ensure the output is not empty.
            Assert.IsTrue(response is not null && response.Any());
            // Assert to check if the output was filtered.
            foreach (var register in response!) {
                Assert.IsTrue(register.ProvinciaName == "Cartago");
            }
        }

        // Test by Dwayne Taylor Monterrosa C17827
        [Test]
        public void SearchPageFilterByUnknownProvince()
        {
            // Arrange for the mock configuration
            // Create an instance of the page model (pageModel) using the CreatePageModel().
            var pageModel = (SearchPageModel)CreatePageModel("search_page");
            // Create filter list.
            List<string> selectedProvinces = new() { "Unknown" };

            // Initialize a collection of registers
            var registers = InitRegisters().AsQueryable();

            // Method FilterByLocation to test from SearchPage
            // Passes the registers and the filter parameters.
            var response = pageModel.FilterByLocation(registers: ref registers, selectedProvinces: selectedProvinces);

            // Assert to ensure that the otuptu is empty.
            Assert.IsTrue(response is not null && !response.Any());
        }

        // Test by Dwayne Taylor Monterrosa C17827
        [Test]
        public void SearchPageFilterByCanton()
        {
            // Arrange for the mock configuration
            // Create an instance of the page model (pageModel) using the CreatePageModel().
            var pageModel = (SearchPageModel)CreatePageModel("search_page");
            // Create filter list.
            List<string> selectedCantons = new() { "Cartago" };

            // Initialize a collection of registers
            var registers = InitRegisters().AsQueryable();

            // Method FilterByLocation to test from SearchPage
            // Passes the registers and the filter parameters.
            var response = pageModel.FilterByLocation(registers: ref registers, selectedCantons: selectedCantons);

            // Assert to ensure the output is not empty.
            Assert.IsTrue(response is not null && response.Any());
            // Assert to check if the output was filtered.
            foreach (var register in response!)
            {
                Assert.IsTrue(register.CantonName == "Cartago");
            }
        }

        // Test by Dwayne Taylor Monterrosa C17827
        [Test]
        public void SearchPageFilterByUnknownCanton()
        {
            // Arrange for the mock configuration
            // Create an instance of the page model (pageModel) using the CreatePageModel().
            var pageModel = (SearchPageModel)CreatePageModel("search_page");
            // Create filter list.
            List<string> selectedCantons = new() { "Unknown" };

            // Initialize a collection of registers
            var registers = InitRegisters().AsQueryable();

            // Method FilterByLocation to test from SearchPage
            // Passes the registers and the filter parameters.
            var response = pageModel.FilterByLocation(registers: ref registers, selectedCantons: selectedCantons);

            // Assert to ensure that the otuptu is empty.
            Assert.IsTrue(response is not null && !response.Any());
        }

        // Test by Dwayne Taylor Monterrosa C17827
        [Test]
        public void SearchPageFilterByProvinceAndCanton()
        {
            // Arrange for the mock configuration
            // Create an instance of the page model (pageModel) using the CreatePageModel().
            var pageModel = (SearchPageModel)CreatePageModel("search_page");
            // Create filter lists.
            List<string> selectedProvinces = new() { "Cartago" };
            List<string> selectedCantons = new() { "Cartago" };

            // Initialize a collection of registers
            var registers = InitRegisters().AsQueryable();

            // Method FilterByLocation to test from SearchPage
            // Passes the registers and the filter parameters.
            var response = pageModel.FilterByLocation(registers: ref registers, selectedProvinces: selectedProvinces, selectedCantons: selectedCantons);

            // Assert to ensure the output is not empty.
            Assert.IsTrue(response is not null && response.Any());
            // Assert to check if the output was filtered.
            foreach (var register in response!)
            {
                Assert.IsTrue(register.ProvinciaName == "Cartago" && register.CantonName == "Cartago");
            }
        }


    }
}
