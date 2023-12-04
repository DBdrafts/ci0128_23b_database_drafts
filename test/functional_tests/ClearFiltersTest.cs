using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using functional_tests.Shared;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace functional_tests
{
    public class ClearFiltersTest
    {
        // Omar Camacho Calvo C11476 | Sprint 2
        [Test]
        public void ClearFiltersInSearchPage()
        {
            var driver = new ChromeDriver();
            // Arrange
            driver.Navigate().GoToUrl("https://localhost:7119/");
            var MainPage = new MainPage(driver);

            MainPage.SearchProduct("le");

            // Act
            driver.FindElement(By.CssSelector("input[type='checkbox'][name='SelectedCategories'][value='Ropa']")).Click();
            driver.FindElement(By.CssSelector("input[type='checkbox'][name='SelectedCantons'][value='Escazú']")).Click();
            driver.FindElement(By.Id("clear-filters")).Click();
            driver.FindElement(By.CssSelector("input[type='checkbox'][name='SelectedCategories'][value='Comida']")).Click();


            //Assert
            Assert.IsFalse(driver.FindElement(By.CssSelector("input[type='checkbox'][name='SelectedCategories'][value='Ropa']")).Selected);
            Assert.IsFalse(driver.FindElement(By.CssSelector("input[type='checkbox'][name='SelectedCantons'][value='Escazú']")).Selected);
            Assert.IsTrue(driver.FindElement(By.CssSelector("input[type='checkbox'][name='SelectedCategories'][value='Comida']")).Selected);
            driver.Quit();
        }
    }
}
