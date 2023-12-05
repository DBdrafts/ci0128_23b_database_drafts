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


namespace InteractionsTest
{
    [TestClass]
    // Declaration of the test class
    public class RazorPageTests : BaseTest
    {
        // Test by Geancarlo Rivera Hernández C06516 | Sprint 2
        [Test]
        public void SplitStringWithChar31AsSeparator()
        {
            // Arrange
            string values = "123\u001fLeche\u001fPali\u001fDos Pinos";
            char delimiter = '\u001f';
            // Act
            string[] result = ProductPageModel.SplitString(values, delimiter);

            // Asserts
            Assert.AreEqual("123", result[0]);
            Assert.AreEqual("Leche", result[1]);
            Assert.AreEqual("Pali", result[2]);
            Assert.AreEqual("Dos Pinos", result[3]);
        }

        // Test by Geancarlo Rivera Hernández C06516 | Sprint 2
        [Test]
        public void SplitStringWithoutDelimiterInTheString()
        {
            // Arrange
            string value = "Leche Semidescremada Dos Pinos de 1 Litro en bolsa plastica";
            char delimiter = '\u001f';

            // Act
            string[] result = ProductPageModel.SplitString(value, delimiter);

            // Asserts
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(value, result[0]);
        }

        // Test by Geancarlo Rivera Hernández C06516 | Sprint 2
        [Test]
        public void SplitStringWithInvalidString()
        {
            // Arrange
            string? value = null;
            char delimiter = '\u001f';

            try
            {
                // Act
                ProductPageModel.SplitString(value, delimiter);
                // Assert
                Assert.Fail("Expected ArgumentException was not thrown.");
            }
            catch (ArgumentException ex)
            {
                // Assert
                Assert.AreEqual("Input string cannot be empty or null.", ex.Message);
            }
        }

        // Test by Geancarlo Rivera Hernández C06516 | Sprint 2
        [Test]
        public void TruncateSubSeconds_ValidInput()
        {
            // Arrange
            DateTime inputDateTime = new DateTime(2023, 11, 4, 15, 30, 45, 123, 5); // 2023-11-04 15:30:45.123.5

            // Act
            DateTime result = ProductPageModel.TruncateSubSeconds(inputDateTime);
            
            DateTime expectedDateTime = new DateTime(2023, 11, 4, 15, 30, 45, 0); //  2023-11-04 15:30:45.0.0

            // Assert
            Assert.AreEqual(expectedDateTime, result);
        }

        // Test by Geancarlo Rivera Hernández C06516 | Sprint 2
        [Test]
        public void TruncateSubSecondsExceptionForInvalidInput()
        {
            // Arrange
            DateTime inputDateTime = DateTime.MinValue; // Invalid input for the method
            try
            {
                //Act
                ProductPageModel.TruncateSubSeconds(inputDateTime);
                //Assert
                Assert.Fail("Expected ArgumentException was not thrown.");
            }
            catch (ArgumentException ex)
            {
                // Assert
                Assert.AreEqual("Invalid dateTimeValue", ex.Message);
            }
        }

    }
}