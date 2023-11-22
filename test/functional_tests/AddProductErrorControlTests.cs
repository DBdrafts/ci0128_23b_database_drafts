using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using functional_tests.Shared;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace functional_tests
{
    public class AddProductErrorControlTests
    {
        // Test by Geancarlo Rivera Hernández C06516 | Sprint 3
        [Test]
        public void AddProductWithoutEnteringLocation()
        {
            // Arrange
            var driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://localhost:7119/Identity/Account/Login");
            var testPage = new Login(driver);

            var email = "geanca567@hotmail.com";
            var password = "Geanca567!";

            // Act
            testPage.SingIn(email, password);
            testPage.ChangeUrl("https://localhost:7119/AddProductPage");

            driver.FindElement(By.Id("form-submit-button")).Click();
            string reportFeedbackMessage = driver.FindElement(By.Id("feedbackMessage")).Text;

            // Assert
            Assert.That(reportFeedbackMessage.Contains("Por favor, seleccione una ubicación."));

        }

        // Test by Geancarlo Rivera Hernández C06516 | Sprint 3
        [Test]
        public void AddProductWithoutEnteringStoreName()
        {
            // Arrange
            var driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://localhost:7119/Identity/Account/Login");
            var testPage = new Login(driver);

            var email = "geanca567@hotmail.com";
            var password = "Geanca567!";

            // Act
            testPage.SingIn(email, password);
            testPage.ChangeUrl("https://localhost:7119/AddProductPage");
            
            driver.FindElement(By.Id("showPopupButton")).Click();

            IWebElement selectProvince= driver.FindElement(By.Id("province"));
            selectProvince.Click();
            selectProvince.FindElement(By.XPath("//option[@value='Alajuela']")).Click();

            IWebElement selectCanton = driver.FindElement(By.Id("canton"));
            selectCanton.Click();
            selectCanton.FindElement(By.XPath("//option[@value='Atenas']")).Click();

            driver.FindElement(By.Id("saveLocationMap-button")).Click();

            driver.FindElement(By.Id("form-submit-button")).Click();
            string reportFeedbackMessage = driver.FindElement(By.Id("feedbackMessage")).Text;

            // Assert
            Assert.That(reportFeedbackMessage.Contains("Por favor, ingrese el nombre del establecimiento."));

        }
    }
}