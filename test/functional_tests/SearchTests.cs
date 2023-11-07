using functional_tests.Shared;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Support.UI;
using System.Windows;

namespace functional_tests
{
    public class SearchTests
    {
        WebDriver? driver = null;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
        }

        // Test by Dwayne Taylor Monterrosa C17827 | Sprint 2
        [Test]
        public void SearchByBrand()
        {
            // Arrange
            Search search = new Search(driver!);

            // Act
            search.SearchProduct("apple", "Marca");

            // Assert that the searchType and the searchString where selected correctly
            Assert.IsTrue(driver!.Url.Contains("searchType=Marca&searchString=apple"));
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }
    }
}

