using functional_tests.Shared;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

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

        [Test]
        public void LocationMantainsData()
        {
            // Arrange
            driver.Navigate().GoToUrl("https://localhost:7119");

            driver.FindElement(By.Id("showPopupButton"));


            // Act
            driver.FindElement(By.Id("add-to-list")).Click();


            // Assert
            Assert.IsTrue(driver.FindElement(By.Id("add-to-list")).GetCssValue("display") == "none");
            Assert.IsTrue(driver.FindElement(By.Id("remove-from-list")).GetCssValue("display") == "inline-block");
        }

        [TearDown]
        public void TearDown()
        {
            //driver.Quit();
        }
    }
}

