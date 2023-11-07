using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace functional_tests.Shared
{
    internal class Login : MainPage
    {
        public Login(WebDriver driver) : base(driver)
        {
            url = "https://localhost:7119/Identity/Account/Login";
        }

        public void SingIn(string email, string password)
        {
            EmailInput().SendKeys(email);
            PasswordInput().SendKeys(password);
            SignInButton().Click();
        }
    }
}