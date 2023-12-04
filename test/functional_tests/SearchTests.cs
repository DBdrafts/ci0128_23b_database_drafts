using functional_tests.Shared;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace functional_tests
{
    public class SearchTests
    {
        ChromeDriver driver;

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

        // Test by Dwayne Taylor Monterrosa C17827 | Sprint 3
        [Test]
        public void SearchByLocation()
        {
            driver.Navigate().GoToUrl("https://localhost:7119/Identity/Account/Login");
            var testPage = new Login(driver);

            var email = "geanca567@hotmail.com";
            var password = "Geanca567!";

            // Act
            testPage.SingIn(email, password);
            //testPage.ChangeUrl("https://localhost:7119/");
            var search = new Search(driver);

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            driver.FindElement(By.Id("showPopupButton")).Click();

            IWebElement selectProvince = driver.FindElement(By.Id("province"));
            selectProvince.Click();
            selectProvince.FindElement(By.XPath("//option[@value='Cartago']")).Click();

            IWebElement selectCanton = driver.FindElement(By.Id("canton"));
            selectCanton.Click();
            wait.Until(driver => selectCanton.Displayed);

            var selectCantonValue = selectCanton.FindElement(By.XPath("//option[@value='Oreamuno']"));
            wait.Until(driver => selectCantonValue.Displayed);
            selectCantonValue.Click();

            driver.FindElement(By.Id("saveLocationMap-button")).Click();

            search.SearchProduct("Celular", "Nombre");

            var result = driver.FindElements(By.ClassName("result-block")).First();
            var distance = double.Parse(result.FindElement(By.Id("register-distance")).GetAttribute("value"));

            // Assert
            Assert.That(distance > 0.0);
        }

        // Test by Dwayne Taylor Monterrosa C17827 | Sprint 3
        [Test]
        public void LocationStays()
        {
            driver.Navigate().GoToUrl("https://localhost:7119/Identity/Account/Login");
            var testPage = new Login(driver);

            var email = "geanca567@hotmail.com";
            var password = "Geanca567!";

            // Act
            testPage.SingIn(email, password);
            //testPage.ChangeUrl("https://localhost:7119/");
            var search = new Search(driver);

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            driver.FindElement(By.Id("showPopupButton")).Click();

            IWebElement selectProvince = driver.FindElement(By.Id("province"));
            selectProvince.Click();
            selectProvince.FindElement(By.XPath("//option[@value='Cartago']")).Click();

            IWebElement selectCanton = driver.FindElement(By.Id("canton"));
            selectCanton.Click();
            wait.Until(driver => selectCanton.Displayed);

            var selectCantonValue = selectCanton.FindElement(By.XPath("//option[@value='Oreamuno']"));
            wait.Until(driver => selectCantonValue.Displayed);
            selectCantonValue.Click();

            driver.FindElement(By.Id("saveLocationMap-button")).Click();

            search.SearchProduct("Celular", "Nombre");
            driver.Navigate().GoToUrl("https://localhost:7119/");

            var locationButton = driver.FindElement(By.Id("showPopupButton"));
            var locationButtonText = locationButton.FindElement(By.Id("buttonSpan")).Text;

            // Assert
            Assert.That(locationButtonText.Equals("Cartago, Oreamuno"));
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }
    }
}

