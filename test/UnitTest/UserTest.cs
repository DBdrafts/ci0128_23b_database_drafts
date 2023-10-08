using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LoCoMPro.Pages;
using LoCoMPro.Data;
using LoCoMPro.Models;
using LoCoMPro.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Assert = NUnit.Framework.Assert;
using System;
using Microsoft.AspNetCore.Http.HttpResults;
using LoCoMPro.Areas.Identity.Pages.Account;
using System.ComponentModel.DataAnnotations;

namespace UserTest
{
    [TestClass]
    public class RazorPageTests : BaseTest
    {
        // Test by Alonso León Rodríguez B94247
        [Test]
        public void LoginModelEmailInvalidFormat()
        {
            var inputModel = new LoginModel.InputModel
            {
                Email = "usuario@",
                Password = "Usuario123!"
            };

            var validationContext = new ValidationContext(inputModel, null, null);
            var validationResults = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(inputModel, validationContext, validationResults, true);

            Assert.IsFalse(isValid);
        }

        // Test by Alonso León Rodríguez B94247
        [Test]
        public void LoginModelEmailValidFormat()
        {
            var inputModel = new LoginModel.InputModel
            {
                Email = "usuario@example.com",
                Password = "Usuario123!"
            };

            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(inputModel, new ValidationContext(inputModel), validationResults, true);

            Assert.IsTrue(isValid);
        }

        // Test by Alonso León Rodríguez B94247
        [Test]
        public void RegisterUserEmailInvalidFormat()
        {
            var inputModel = new RegisterModel.InputModel
            {
                UserName = "Usuario",
                Email = "usuario@com",
                Password = "Usuario123!",
                ConfirmPassword = "Usuario123!"
            };

            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(inputModel, new ValidationContext(inputModel), validationResults, true);

            Assert.IsFalse(isValid);
        }

        // Test by Alonso León Rodríguez B94247
        [Test]
        public void RegisterUserEmailValidFormat()
        {
            var inputModel = new RegisterModel.InputModel
            {
                UserName = "Usuario",
                Email = "usuario@ucr.ac.cr",
                Password = "Usuario123!",
                ConfirmPassword = "Usuario123!"
            };

            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(inputModel, new ValidationContext(inputModel), validationResults, true);

            Assert.IsTrue(isValid);
        }

        // Test by Alonso León Rodríguez B94247
        [Test]
        public void RegisterUserPasswordDoesNotMatch()
        {
            var inputModel = new RegisterModel.InputModel
            {
                UserName = "Usuario",
                Email = "usuario@ucr.ac.cr",
                Password = "Usuario123!",
                ConfirmPassword = "Usuario567!"
            };

            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(inputModel, new ValidationContext(inputModel), validationResults, true);

            Assert.IsFalse(isValid);
        }
    }
}