using LoCoMPro.Utils;
using Microsoft.AspNetCore.Http;
using Moq;
using NuGet.ContentModel;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assert = NUnit.Framework.Assert;

namespace UserProductListTest
{
    [TestClass]
    // Declaration of the test class
    public class RazorPageTests : BaseTest
    {

        // Test by Julio Alejandro Rodríguez Salguera C16717 | Sprint 2
        [Test]
        public void GetSameElementsInList() {
            // Arrange
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            var userProductList = new UserProductList(httpContextAccessor.Object);

            IList<UserProductListElement> elementsList = new List<UserProductListElement>();

            var newTestElement = CreateElementTest();

            elementsList.Add(newTestElement);

            // Act
            UserProductListElement? testElement = userProductList.GetListElement(elementsList, CreateElementTest());

            // Assert
            Assert.IsNotNull(testElement);
            Assert.AreEqual(newTestElement, testElement);
        }

        // Test by Julio Alejandro Rodríguez Salguera C16717 | Sprint 2
        [Test]
        public void GetDiferentElementsInList()
        {
            // Arrange
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            var userProductList = new UserProductList(httpContextAccessor.Object);

            IList<UserProductListElement> elementsList = new List<UserProductListElement>();

            var newTestElement = CreateElementTest();

            elementsList.Add(newTestElement);

            // Act
            UserProductListElement? testElement = userProductList.GetListElement(elementsList, CreateDifferentElementTest());

            // Assert
            Assert.IsNull(testElement);
        }

        // Test by Julio Alejandro Rodríguez Salguera C16717 | Sprint 2
        [Test]
        public void GetSimilarElementsInList()
        {
            // Arrange
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            var userProductList = new UserProductList(httpContextAccessor.Object);

            IList<UserProductListElement> elementsList = new List<UserProductListElement>();

            var newTestElement = CreateElementTest();

            elementsList.Add(newTestElement);

            // Act
            UserProductListElement? testElement = userProductList.GetListElement(elementsList, CreateSimilarElementTest());

            // Assert
            Assert.IsNull(testElement);
        }


        

    }
}
