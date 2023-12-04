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

namespace MostReportedTests
{
    [TestClass]
    // Declaration of the test class
    public class RazorPageTests : BaseTest
    {
        // Test by Omar Camacho Calvo | Sprint 3
        [Test]
        public void GetRejectedPercentage_WithValidValues_ReturnsCorrectPercentage()
        {// work done
            // Arrange
            var pageModel = (MostReportedPageModel)CreatePageModel("most_reported_users");
            int totalAports = 100;
            int hiddenAports = 20;

            // Act
            var result = pageModel.GetRejectedPercentage(totalAports, hiddenAports);

            // Assert
            Assert.AreEqual(20, result);
        }

        // Test by Omar Camacho Calvo | Sprint 3
        [Test]
        public void GetRejectedPercentage_WithAllRejected_Returns100Percentage()
        { // reject totally
            // Arrange
            var pageModel = (MostReportedPageModel)CreatePageModel("most_reported_users");
            int totalAports = 50;
            int hiddenAports = 50;

            // Act
            var result = pageModel.GetRejectedPercentage(totalAports, hiddenAports);

            // Assert
            Assert.AreEqual(100, result);
        }

        // Test by Omar Camacho Calvo | Sprint 3
        [Test]
        public void GetRejectedPercentage_WithZeroTotalAports_ReturnsZeroPercentage()
        { // 0 Apports but some hidden Apports
            // Arrange
            var pageModel = (MostReportedPageModel)CreatePageModel("most_reported_users");
            int totalAports = 0;
            int hiddenAports = 10;

            // Act
            var result = pageModel.GetRejectedPercentage(totalAports, hiddenAports);

            // Assert
            Assert.AreEqual(0, result);
        }

        // Test by Omar Camacho Calvo | Sprint 3
        [Test]
        public void GetRejectedPercentage_WithNegativeValues_ReturnsZeroPercentage()
        { // negative values
            // Arrange
            var pageModel = (MostReportedPageModel)CreatePageModel("most_reported_users");
            int totalAports = -100;
            int hiddenAports = -20;

            // Act
            var result = pageModel.GetRejectedPercentage(totalAports, hiddenAports);

            // Assert
            Assert.AreEqual(0, result);
        }

        // Test by Omar Camacho Calvo | Sprint 3
        [Test]
        public void GetRejectedPercentage_WithDecimalValues_ReturnsRoundedPercentage()
        { // round test
            // Arrange
            var pageModel = (MostReportedPageModel)CreatePageModel("most_reported_users");
            int totalAports = 30;
            int hiddenAports = 10;

            // Act
            var result = pageModel.GetRejectedPercentage(totalAports, hiddenAports);

            // Assert
            Assert.AreEqual(33, result); // 10/30 * 100 = 33.33, rounded to 33
        }
    }
}
