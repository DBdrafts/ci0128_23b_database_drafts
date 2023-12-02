using OpenQA.Selenium;
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
    public class ChangeUsernameTest
    {
        // Test by Alonso León Rodríguez B94247 | Sprint 2
        [Test]
        public void UserNameChanged()
        {
            // Arrange
            var driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://localhost:7119/Identity/Account/Login");
            var test = new Login(driver);

            var email = "prueba123@gmail.com";
            var password = "Prueba123!";
            test.SingIn(email, password);

            driver.FindElement(By.Id("usr-logged")).Click();
            driver.FindElement(By.Id("perfil-button")).Click();
            IWebElement previousUserName = driver.FindElement(By.Id("username"));
            string prevUserName = previousUserName.Text;
            string newUserName = prevUserName + '1';


            // Act
            driver.FindElement(By.Id("changeUsername")).Click();
            driver.FindElement(By.Id("newUsername")).Clear();
            driver.FindElement(By.Id("newUsername")).SendKeys(newUserName);
            driver.FindElement(By.Id("changePswButton")).Click();

            driver.FindElement(By.Id("usr-logged")).Click();
            driver.FindElement(By.Id("perfil-button")).Click();
            IWebElement changedUserName = driver.FindElement(By.Id("username"));

            // Assert
            Assert.IsTrue(changedUserName.Text != prevUserName);
        }
    }
}