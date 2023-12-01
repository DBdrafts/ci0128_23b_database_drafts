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
    public class AnomaliesPageTests
    {
        // Test by Omar Camacho Calvo C11476 | Sprint 3
        [Test]
        public void TestButtonToAnormalRegistersPage()
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
            driver.FindElement(By.Id("moderate-anomalies-button")).Click();

            // Assert
            IWebElement userLogged = driver.FindElement(By.Id("cant-reportes"));
        }

        // Test by Omar Camacho Calvo C11476 | Sprint 3
        [Test]
        public void ChecktheNumberOfAnormalRegisters()
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
            driver.FindElement(By.Id("moderate-anomalies-button")).Click();

            // Assert
            IWebElement cantReportesElement = driver.FindElement(By.Id("report-count"));
            int anormalRegisterAmount = int.Parse(cantReportesElement.Text);

            if (anormalRegisterAmount > 0)
            {
                Assert.IsTrue(driver.FindElements(By.ClassName("open-interactions-button")).Count > 0, "Expected to find the interactions button");
            }
            else if(anormalRegisterAmount == 0)
            {
                Assert.IsFalse(driver.FindElements(By.ClassName("open-interactions-button")).Count > 0, "Not expecting to find the interactions button");
            }
        }

        
        // Test by Omar Camacho Calvo C11476 | Sprint 3
        [Test]
        public void UpdatetheNumberOfAnormalRegistersAfterAccept()
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
            driver.FindElement(By.Id("moderate-anomalies-button")).Click();

            IWebElement cantReportesElement = driver.FindElement(By.Id("report-count"));
            int FirstAnormalRegisterAmount = int.Parse(cantReportesElement.Text);

            if (FirstAnormalRegisterAmount > 0)
            {

                driver.FindElement(By.Id("button-open-moderate-popup")).Click();
                driver.FindElement(By.Id("acceptReport")).Click();

                IWebElement cantReportesElementUpdate = driver.FindElement(By.Id("report-count"));
                int SecondAnormalRegisterAmount = int.Parse(cantReportesElementUpdate.Text);

                var feedbackMessageElement = driver.FindElement(By.Id("feedbackMessage"));

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                wait.Until(driver => feedbackMessageElement.Displayed);

                string reportFeedbackMessage = feedbackMessageElement.Text;

                // Assert
                Assert.That(reportFeedbackMessage.Contains("El reporte anómalo ha sido aprobado exitosamente"));



            }
            else if (FirstAnormalRegisterAmount == 0)
            {
                // Assert
                Assert.IsFalse(driver.FindElements(By.ClassName("open-interactions-button")).Count > 0, "Not expecting to find the interactions button");
            }
        }

        // Test by Omar Camacho Calvo C11476 | Sprint 3
        [Test]
        public void UpdatetheNumberOfAnormalRegistersAfterReject()
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
            driver.FindElement(By.Id("moderate-anomalies-button")).Click();

            IWebElement cantReportesElement = driver.FindElement(By.Id("report-count"));
            int FirstAnormalRegisterAmount = int.Parse(cantReportesElement.Text);

            if (FirstAnormalRegisterAmount > 0)
            {

                driver.FindElement(By.Id("button-open-moderate-popup")).Click();
                driver.FindElement(By.Id("rejectReport")).Click();

                var feedbackMessageElement = driver.FindElement(By.Id("feedbackMessage"));

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                wait.Until(driver => feedbackMessageElement.Displayed);

                string reportFeedbackMessage = feedbackMessageElement.Text;

                // Assert
                Assert.That(reportFeedbackMessage.Contains("El reporte anómalo ha sido rechazado exitosamente"));


            }
            else if (FirstAnormalRegisterAmount == 0)
            {
                // Assert
                Assert.IsFalse(driver.FindElements(By.ClassName("open-interactions-button")).Count > 0, "Not expecting to find the interactions button");
            }
        }
        
        
    }
}