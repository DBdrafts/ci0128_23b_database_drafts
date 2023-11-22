using Microsoft.VisualBasic.FileIO;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace functional_tests.Shared
{
    internal class Search : MainPage
    {
        public Search(WebDriver driver) : base(driver)
        {
            url = "https://localhost:7119";
            driver.Navigate().GoToUrl(url);
        }

        public void SearchProduct(string searchString, string searchType)
        {
            var options = SearchOptions();
            SearchTypeSelect().Click();

            options.FirstOrDefault(o => o.Text == searchType)!.Click();
            SearchBar().SendKeys(searchString);
            SearchButton().Click();
        }
  
    }
}
