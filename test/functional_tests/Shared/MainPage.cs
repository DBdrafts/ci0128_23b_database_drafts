using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace functional_tests.Shared
{
    internal class MainPage : GeneralPageElements
    {
        public MainPage(WebDriver driver) : base(driver)
        {
            url = "https://localhost:7119";
        }

        public void SearchProduct(string productToSearch)
        {
            SearchBar().SendKeys(productToSearch);
            SearchButton().Click();
        }
    }
}
