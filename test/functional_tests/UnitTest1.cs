using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace functional_tests
{
    public class AddToUserProductListTest
    {
        private IWebDriver driver;

        [SetUp]
        public void Setup()
        {
            var options = new EdgeOptions();
            //driver = new EdgeDriver(options);
            driver = new EdgeDriver();
        }

        [Test]
        public void AddToUserProductList()
        {
            driver.Navigate().GoToUrl("https://localhost:7119/");

            //var title = driver.Title;

            //driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(500);

            //var textBox = driver.FindElement(By.Name("my-text"));
            //var submitButton = driver.FindElement(By.TagName("button"));

            //textBox.SendKeys("Selenium");
            //submitButton.Click();

            //var message = driver.FindElement(By.Id("message"));
            //var value = message.Text;

            //driver.Quit();
        }
    }
}