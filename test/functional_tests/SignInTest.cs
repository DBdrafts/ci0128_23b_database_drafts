using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace functional_tests
{
    public class SignInTest
    {
        [Test]
        public void SignInWithValidUserAccount()
        {
            // Arrange
            var driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://localhost:7119/Identity/Account/Login");
            var login = new Login(driver);

            var email = "geanca567@hotmail.com";
            var password = "Geanca567!";

            // Act

            login.SingIn(email, password);

            // Assert
            //Assert.That(login.IsLoggedIn(), Is.True);
        }
    }
}
