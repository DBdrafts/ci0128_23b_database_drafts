using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace functional_tests.Shared
{
    public class GeneralPageElements
    {
        protected IWebDriver driver { get; }

        protected string url { get; set; } = "https://localhost:7119";

        public GeneralPageElements(IWebDriver newDriver)
        {
            driver = newDriver;
        }

        protected IWebElement UserNotLoggedButton()
        {
            return driver.FindElement(By.Id("usr-not-logged"));
        }

        protected IWebElement UserLoggedButton()
        {
            return driver.FindElement(By.Id("usr-logged"));
        }

        protected IWebElement RegisterButton()
        {
            return driver.FindElement(By.Id("register-button"));
        }

        protected IWebElement LoginButton()
        {
            return driver.FindElement(By.Id("login-button"));
        }

        protected IWebElement LogoutButton()
        {
            return driver.FindElement(By.Id("logout-button"));
        }

        protected IWebElement ProfileButton()
        {
            return driver.FindElement(By.Id("perfil-button"));
        }

        protected IWebElement SearchBar()
        {
            return driver.FindElement(By.Id("search-input"));
        }

        protected IWebElement SearchButton()
        {
            return driver.FindElement(By.Id("search-button"));
        }


        // For Log In
        protected IWebElement EmailInput()
        {
            return driver.FindElement(By.Id("Input_Email"));
        }

        protected IWebElement PasswordInput()
        {
            return driver.FindElement(By.Id("psw"));
        }

        protected IWebElement SignInButton()
        {
            return driver.FindElement(By.Id("iniSesion"));
        }


    }
}