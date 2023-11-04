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

namespace RegisterReportTest
{
    [TestClass]
    public class RazorPageTests : BaseTest
    {
        // Test by Omar Fabian Camacho Calvo C11476
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

        // Test by Omar Fabian Camacho Calvo C11476
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

        // Test by Omar Fabian Camacho Calvo C11476
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

        // Test by Omar Fabian Camacho Calvo C11476
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

    }
}
