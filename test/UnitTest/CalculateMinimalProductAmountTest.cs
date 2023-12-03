using LoCoMPro.Pages;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assert = NUnit.Framework.Assert;

namespace CalculateMinimalProductAmountTest
{
    [TestClass]
    // Declaration of the test class
    public class RazorPageTests : BaseTest
    {
        // Test by Julio Alejandro Rodríguez Salguera C16717 | Sprint 3
        [Test]
        public void CalculateMinimalProductAmountZero()
        {
            // Arrange

            // Create an instance of the page model (pageModel) using the CreatePageModel()
            var pageModel = (ListReportPageModel)CreatePageModel("report_list_page");

            // Set the amount to test
            int productAmount = 0;

            // Act

            // Calculate the minimal product amount if the product amount is 0
            int minimalProductAmount = pageModel.CalculateMinimalProductAmount(productAmount);

            // Assert

            Assert.IsTrue(minimalProductAmount == 0);
        }

        // Test by Julio Alejandro Rodríguez Salguera C16717 | Sprint 3
        [Test]
        public void CalculateMinimalProductAmountPositiveInteger()
        {
            // Arrange

            // Create an instance of the page model (pageModel) using the CreatePageModel()
            var pageModel = (ListReportPageModel)CreatePageModel("report_list_page");

            // Set the amount to test
            int productAmount = 100;

            // Act

            // Calculate the minimal product amount if the product amount is positive and the result an integer
            int minimalProductAmount = pageModel.CalculateMinimalProductAmount(productAmount);

            // Assert

            Assert.IsTrue(minimalProductAmount == 30);
        }

        // Test by Julio Alejandro Rodríguez Salguera C16717 | Sprint 3
        [Test]
        public void CalculateMinimalProductAmountPositiveFraction()
        {
            // Arrange

            // Create an instance of the page model (pageModel) using the CreatePageModel()
            var pageModel = (ListReportPageModel)CreatePageModel("report_list_page");

            // Set the amount to test
            int productAmount = 8;

            // Act

            // Calculate the minimal product amount if the product amount is positive and the result a fraction
            int minimalProductAmount = pageModel.CalculateMinimalProductAmount(productAmount);

            // Assert

            Assert.IsTrue(minimalProductAmount == 2);
        }

        // Test by Julio Alejandro Rodríguez Salguera C16717 | Sprint 3
        [Test]
        public void CalculateMinimalProductAmountNegative()
        {
            // Arrange

            // Create an instance of the page model (pageModel) using the CreatePageModel()
            var pageModel = (ListReportPageModel)CreatePageModel("report_list_page");

            // Set the amount to test
            int productAmount = -100;

            // Act

            // Calculate the minimal product amount if the product amount is negative
            int minimalProductAmount = pageModel.CalculateMinimalProductAmount(productAmount);

            // Assert

            Assert.IsTrue(minimalProductAmount == 0);
        }
    }
}
