using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using functional_tests.Shared;

namespace functional_tests
{
    internal class AddProductToListTest
    {
        WebDriver driver = new ChromeDriver();

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
        }

        // Test by Julio Alejandro Rodríguez Salguera
        // Test from the Sprint 2
        [Test]
        public void AddProductToUserListTest()
        {
            // Arrange
            driver.Navigate().GoToUrl("https://localhost:7119/Identity/Account/Login");

            Login testPage = new Login(driver);

            // Make the Login with a test user
            var email = "julio444@ucr.ac.cr";
            var password = "Julio444!";

            testPage.SingIn(email, password);

            // Go to the main page
            testPage.ChangeUrl("https://localhost:7119");

            // Search the product that is going to be add to the list
            testPage.GoToProductPage("Leche");

            // Act
            driver.FindElement(By.Id("add-to-list")).Click();


            // Assert
            Assert.IsTrue(driver.FindElement(By.Id("add-to-list")).GetCssValue("display") == "none");
            Assert.IsTrue(driver.FindElement(By.Id("remove-from-list")).GetCssValue("display") == "inline-block");

        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }
    }
}
