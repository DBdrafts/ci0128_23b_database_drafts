using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using functional_tests.Shared;
using OpenQA.Selenium.Support.UI;

namespace functional_tests
{
    public class ModerateOldRegistersTest
    {

        // Test by Alonso León Rodríguez B94247 | Sprint 3
        [Test]
        public void numberOldProductsWithOldRegisters()
        {
            // Arrange
            var driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://localhost:7119/Identity/Account/Login");
            var login = new Login(driver);

            var email = "locomoderador@gmail.com";
            var password = "Mod123!";

            login.SingIn(email, password);

            // Act
            driver.FindElement(By.Id("moderator-button")).Click();
            driver.FindElement(By.Id("moderate-old-button")).Click();

            // Assert
            IWebElement productsWithOldRegistersCount = driver.FindElement(By.Id("report-count"));
            int numProductsWithOldRegisters = int.Parse(productsWithOldRegistersCount.Text);

            if (numProductsWithOldRegisters > 0)
            {
                Assert.IsTrue(driver.FindElements(By.ClassName("open-interactions-button")).Count > 0, "Expected to find the interactions button");
            }
            else if (numProductsWithOldRegisters == 0)
            {
                Assert.IsFalse(driver.FindElements(By.ClassName("open-interactions-button")).Count > 0, "Not expecting to find the interactions button");
            }
            driver.Quit();
        }


        // Test by Alonso León Rodríguez B94247 | Sprint 3
        [Test]
        public void numberOfProductsWithOldRegistersAfterHide()
        {
            // Arrange
            var chromeOptions = new ChromeOptions();

            chromeOptions.AddArgument("start-maximized");
            chromeOptions.AddArgument("window-size=1920,1080");

            var driver = new ChromeDriver(chromeOptions);
            driver.Navigate().GoToUrl("https://localhost:7119/Identity/Account/Login");
            var login = new Login(driver);

            var email = "locomoderador@gmail.com";
            var password = "Mod123!";

            login.SingIn(email, password);

            // Act
            driver.FindElement(By.Id("moderator-button")).Click();
            driver.FindElement(By.Id("moderate-old-button")).Click();

            IWebElement productsWithOldRegistersCount = driver.FindElement(By.Id("report-count"));
            int numProductsWithOldRegisters = int.Parse(productsWithOldRegistersCount.Text);

            if (numProductsWithOldRegisters > 0)
            {

                driver.FindElement(By.Id("button-open-moderate-popup")).Click();
                driver.FindElement(By.Id("hideRegisters")).Click();

                IWebElement updatedProductsWithOldRegistersCount = driver.FindElement(By.Id("report-count"));
                int numUpdated = int.Parse(updatedProductsWithOldRegistersCount.Text);

                var feedbackMessageElement = driver.FindElement(By.Id("feedbackMessage"));

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                wait.Until(driver => feedbackMessageElement.Displayed);

                string reportFeedbackMessage = feedbackMessageElement.Text;

                // Assert
                Assert.That(reportFeedbackMessage.Contains("Los registros han sido ocultados exitosamente"));
            }
            else if (numProductsWithOldRegisters == 0)
            {
                // Assert
                Assert.IsFalse(driver.FindElements(By.ClassName("open-interactions-button")).Count > 0, "Not expecting to find the interactions button");
            }
            driver.Quit();
        }

        // Test by Alonso León Rodríguez B94247 | Sprint 3
        [Test]
        public void numberOfProductsWithOldRegistersAfterKeep()
        {
            // Arrange
            var chromeOptions = new ChromeOptions();

            chromeOptions.AddArgument("start-maximized");
            chromeOptions.AddArgument("window-size=1920,1080");

            var driver = new ChromeDriver(chromeOptions);
            driver.Navigate().GoToUrl("https://localhost:7119/Identity/Account/Login");
            var login = new Login(driver);

            var email = "locomoderador@gmail.com";
            var password = "Mod123!";

            login.SingIn(email, password);

            // Act
            driver.FindElement(By.Id("moderator-button")).Click();
            driver.FindElement(By.Id("moderate-old-button")).Click();

            IWebElement productsWithOldRegistersCount = driver.FindElement(By.Id("report-count"));
            int numProductsWithOldRegisters = int.Parse(productsWithOldRegistersCount.Text);

            if (numProductsWithOldRegisters > 0)
            {

                driver.FindElement(By.Id("button-open-moderate-popup")).Click();
                driver.FindElement(By.Id("keepRegisters")).Click();

                IWebElement updatedProductsWithOldRegistersCount = driver.FindElement(By.Id("report-count"));
                int numUpdated = int.Parse(updatedProductsWithOldRegistersCount.Text);

                var feedbackMessageElement = driver.FindElement(By.Id("feedbackMessage"));

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                wait.Until(driver => feedbackMessageElement.Displayed);

                string reportFeedbackMessage = feedbackMessageElement.Text;

                // Assert
                Assert.That(reportFeedbackMessage.Contains("Se han conservado los registros"));
            }
            else if (numProductsWithOldRegisters == 0)
            {
                // Assert
                Assert.IsFalse(driver.FindElements(By.ClassName("open-interactions-button")).Count > 0, "Not expecting to find the interactions button");
            }
            driver.Quit();
        }
    }
}