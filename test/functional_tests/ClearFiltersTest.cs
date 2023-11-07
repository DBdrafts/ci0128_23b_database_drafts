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
        // Omar Camacho Calvo C11476
        [Test]
        public void ClearFiltersInSearchPage()
        {
            // Arrange
            var driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://localhost:7119/");
            var MainPage = new MainPage(driver);

            MainPage.SearchProduct("le");

            // Act
            driver.FindElement(By.CssSelector("input[type='checkbox'][name='SelectedCategories'][value='Ropa']")).Click();
            driver.FindElement(By.CssSelector("input[type='checkbox'][name='SelectedCantons'][value='Escazú']")).Click();
            driver.FindElement(By.Id("clear-filters")).Click();
            

            //Assert
            //Assert.;
        }

    }
}
