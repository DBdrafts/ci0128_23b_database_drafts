using functional_tests.Shared;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace functional_tests
{
    public class SearchTests
    {
        // Test by Dwayne Taylor Monterrosa C17827 | Sprint 2
        [Test]
        public void SearchByBrand()
        {
            var driver = new ChromeDriver();
            // Arrange
            Search search = new Search(driver!);

            // Act
            search.SearchProduct("apple", "Marca");

            // Assert that the searchType and the searchString where selected correctly
            Assert.IsTrue(driver!.Url.Contains("searchType=Marca&searchString=apple"));
            driver.Quit();
        }

    //    [TearDown]
    //    public void TearDown()
    //    {
    //        driver.Quit();
    //    }
    }
}

