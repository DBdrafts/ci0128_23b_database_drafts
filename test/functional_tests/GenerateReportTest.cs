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
    internal class GenerateReportTest
    {
        // Test by Julio Alejandro Rodríguez Salguera
        // Test from the Sprint 3
        [Test]
        public void GenerateReportFromListTest()
        {
            var driver = new ChromeDriver();
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
            
            // Add a product to the list
            driver.FindElement(By.Id("add-to-list")).Click();

            // Go to the list page
            testPage.ChangeUrl("https://localhost:7119");

            // Go to the user list page
            driver.FindElement(By.Id("user-list-button")).Click();

            // Act
            driver.FindElement(By.Id("generate-report-button")).Click();

            // Assert
            Assert.IsTrue(driver.Url == "https://localhost:7119/ListReportPage");
            driver.Quit();
        }

        // Test by Julio Alejandro Rodríguez Salguera
        // Test from the Sprint 3
        [Test]
        public void SortReportListByPriceTest()
        {
            var driver = new ChromeDriver();
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

            // Add a product to the list
            driver.FindElement(By.Id("add-to-list")).Click();

            // Go to the list page
            testPage.ChangeUrl("https://localhost:7119");
            
            // Go to the user list page
            driver.FindElement(By.Id("user-list-button")).Click();

            // Generate the report
            driver.FindElement(By.Id("generate-report-button")).Click();

            // Act
            driver.FindElement(By.Id("sort-button-price")).Click();

            // Assert

            // Gets the sort elements
            IReadOnlyCollection<IWebElement> reportResult = driver.FindElements(By.ClassName("report-list-block"));

            for(int resultIndex = 0; resultIndex < reportResult.Count-1; resultIndex++)
            {
                Assert.LessOrEqual(reportResult.ElementAt(resultIndex).FindElement(By.Id("report-price")).GetAttribute("Value")
                    , reportResult.ElementAt(resultIndex+1).FindElement(By.Id("report-price")).GetAttribute("Value"));
            }
            driver.Quit();
        }

        // Test by Julio Alejandro Rodríguez Salguera
        // Test from the Sprint 3
        [Test]
        public void SortReportListByDistanceTest()
        {
            var driver = new ChromeDriver();
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

            // Add a product to the list
            driver.FindElement(By.Id("add-to-list")).Click();

            // Go to the list page
            testPage.ChangeUrl("https://localhost:7119");

            // Go to the user list page
            driver.FindElement(By.Id("user-list-button")).Click();

            // Generate the report
            driver.FindElement(By.Id("generate-report-button")).Click();

            // Act
            driver.FindElement(By.Id("sort-button-distance")).Click();

            // Assert

            // Gets the sort elements
            IReadOnlyCollection<IWebElement> reportResult = driver.FindElements(By.ClassName("report-list-block"));

            for (int resultIndex = 0; resultIndex < reportResult.Count - 1; resultIndex++)
            {
                string firstDistanceString = reportResult.ElementAt(resultIndex).FindElement(By.Id("report-location")).GetAttribute("Value");
                string secondDistanceString = reportResult.ElementAt(resultIndex + 1).FindElement(By.Id("report-location")).GetAttribute("Value");

                float firstDistance = 0;
                float secondDistance = 0;

                try
                {
                    firstDistance = float.Parse(firstDistanceString);
                    secondDistance = float.Parse(secondDistanceString);
                } catch (FormatException) {
                    Console.WriteLine("Input string is not in the correct format");
                }

                Assert.LessOrEqual(firstDistance, secondDistance);
            }
            driver.Quit();
        }
    }
}
