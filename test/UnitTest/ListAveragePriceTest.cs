using LoCoMPro.Pages;
using LoCoMPro.Utils;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assert = NUnit.Framework.Assert;

namespace ListAveragePriceTest
{
    [TestClass]
    // Declaration of the test class
    public class RazorPageTests : BaseTest
    {
        // Test by Julio Alejandro Rodríguez Salguera C16717
        [Test]
        public void ConvertIntFromStringWithComma()
        {
            // Arrange

            // Create an instance of the page model (pageModel) using the CreatePageModel()
            var pageModel = (ProductListPageModel)CreatePageModel("product_list");

            string stringOfIntWithComma = "1,000";
            CultureInfo culture = CultureInfo.InvariantCulture;

            // Act

            // Converts the string with comma to a int
            int intFromString = pageModel.ConvertIntFromString(culture, stringOfIntWithComma);

            // Assert

            Assert.IsTrue(intFromString == 1000);
        }

        // Test by Julio Alejandro Rodríguez Salguera C16717
        [Test]
        public void ConvertIntFromStringWithDot()
        {
            // Arrange

            // Create an instance of the page model (pageModel) using the CreatePageModel()
            var pageModel = (ProductListPageModel)CreatePageModel("product_list");

            string stringOfIntWithComma = "1.000";
            CultureInfo culture = CultureInfo.InvariantCulture;

            // Act

            // Converts the string with dot to a int
            int intFromString = pageModel.ConvertIntFromString(culture, stringOfIntWithComma);

            // Assert

            Assert.IsTrue(intFromString == 1000);
        }

        // Test by Julio Alejandro Rodríguez Salguera C16717
        [Test]
        public void ConvertIntFromStringWithSpace()
        {
            // Arrange

            // Create an instance of the page model (pageModel) using the CreatePageModel()
            var pageModel = (ProductListPageModel)CreatePageModel("product_list");

            string stringOfIntWithComma = "1 000";
            CultureInfo culture = CultureInfo.InvariantCulture;

            // Act

            // Converts the string with space to a int
            int intFromString = pageModel.ConvertIntFromString(culture, stringOfIntWithComma);

            // Assert

            Assert.IsTrue(intFromString == 1000);
        }

        // Test by Julio Alejandro Rodríguez Salguera C16717
        [Test]
        public void ConvertInvalidIntFromString()
        {
            // Arrange

            // Create an instance of the page model (pageModel) using the CreatePageModel()
            var pageModel = (ProductListPageModel)CreatePageModel("product_list");

            string stringWithInvalidInt = "Invalid int";
            CultureInfo culture = CultureInfo.InvariantCulture;

            // Act

            // Tries to convert an invalid int from a string
            int intFromString = pageModel.ConvertIntFromString(culture, stringWithInvalidInt);

            // Assert

            Assert.IsTrue(intFromString == 0);
        }

        // Test by Julio Alejandro Rodríguez Salguera C16717
        [Test]
        public void CalculateTotalPriceStandard()
        {
            // Arrange

            // Create an instance of the page model (pageModel) using the CreatePageModel()
            var pageModel = (ProductListPageModel)CreatePageModel("product_list");

            // Adds element to the list
            pageModel.UserProductList.Add(CreateElementTest());
            pageModel.UserProductList.Add(CreateDifferentElementTest());
            pageModel.UserProductList.Add(CreateSimilarElementTest());

            // Act

            // Calculate the total price of the elements in the list
            pageModel.CalculateTotalPrice();

            // Assert

            Assert.IsTrue(pageModel.TotalPrice == 135);
        }

        // Test by Julio Alejandro Rodríguez Salguera C16717
        [Test]
        public void CalculateTotalPriceDifferentString()
        {
            // Arrange

            // Create an instance of the page model (pageModel) using the CreatePageModel()
            var pageModel = (ProductListPageModel)CreatePageModel("product_list");

            // Creates the elements of the list
            UserProductListElement newElement1 = CreateElementTest();
            UserProductListElement newElement2 = CreateElementTest();
            UserProductListElement newElement3 = CreateElementTest();

            // Set new data
            newElement1.AvgPrice = "1,000";
            newElement2.AvgPrice = "1.000";
            newElement3.AvgPrice = "1 000";


            // Adds element to the list
            pageModel.UserProductList.Add(newElement1);
            pageModel.UserProductList.Add(newElement2);
            pageModel.UserProductList.Add(newElement3);

            // Act

            // Calculate the total price of the elements in the list
            pageModel.CalculateTotalPrice();

            // Assert

            Assert.IsTrue(pageModel.TotalPrice == 3000);
        }

        // Test by Julio Alejandro Rodríguez Salguera C16717
        [Test]
        public void CalculateTotalPriceInvalidString()
        {
            // Arrange

            // Create an instance of the page model (pageModel) using the CreatePageModel()
            var pageModel = (ProductListPageModel)CreatePageModel("product_list");

            // Creates the elements of the list
            UserProductListElement newElement1 = CreateElementTest();
            UserProductListElement newElement2 = CreateElementTest();
            UserProductListElement newElement3 = CreateElementTest();

            // Set new data
            newElement1.AvgPrice = "Invalid Int";
            newElement2.AvgPrice = "1,000 Test";
            newElement3.AvgPrice = "-1";


            // Adds element to the list
            pageModel.UserProductList.Add(newElement1);
            pageModel.UserProductList.Add(newElement2);
            pageModel.UserProductList.Add(newElement3);

            // Act

            // Calculate the total price of the elements in the list
            pageModel.CalculateTotalPrice();

            // Assert

            Assert.IsTrue(pageModel.TotalPrice == 1001);
        }
    }
}
