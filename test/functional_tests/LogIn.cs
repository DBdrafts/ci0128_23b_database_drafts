using functional_tests.Shared;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace functional_tests
{
    public class Login : GeneralPageElement
    {
        protected WebDriver _driver;
        public string url => "https://localhost:7119/Identity/Account/Login";

        public Login(WebDriver driver) : base(driver)
        {
            _driver = driver;
        }

        public void SingIn(string email, string password)
        {
            EmailInput().SendKeys(email);
            PasswordInput().SendKeys(password);
            SignInButton().Click();
        }
    }
}