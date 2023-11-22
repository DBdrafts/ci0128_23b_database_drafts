using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using functional_tests.Shared;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using OpenQA.Selenium.Support.UI;

namespace functional_tests
{
    public class ReportRegisterTest
    {
        // Test by Geancarlo Rivera Hernández C06516 | Sprint 3
        [Test]
        public void ReportRegister()
        {
            // Arrange
            var driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://localhost:7119/Identity/Account/Login");
            var testPage = new Login(driver);

            var email = "geanca567@hotmail.com";
            var password = "Geanca567!";

            // Act
            testPage.SingIn(email, password);
            testPage.GoToProductPage("Celular");

            driver.FindElement(By.Id("open-popup-button-2")).Click();
            driver.FindElement(By.Id("reportIcon")).Click();
            driver.FindElement(By.Id("saveButton")).Click();

            var feedbackMessageElement = driver.FindElement(By.Id("feedbackMessage"));

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            wait.Until(driver => feedbackMessageElement.Displayed);

            string reportFeedbackMessage = feedbackMessageElement.Text;

            // Assert
            Assert.That(reportFeedbackMessage.Contains("Su reporte se ha realizado correctamente!") ||
                        reportFeedbackMessage.Contains("Su reporte ha sido revertido correctamente!"));

        }
    }
}