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
using System.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text;

namespace RoundToCloserRealTest
{
    [TestClass]
    // Declaration of the test class
    public class BaseRazorPageTests : BaseTest
    {
        // Test by Alonso León Rodríguez | Sprint 2
        [Test]
        public void roundToCloserPasses()
        {
            var context = createLocoproContext();
            float numberToRound = 3.2F;
            float expectedResult = 3F;

            var result = context.RoundToCloserReal(numberToRound);

            Assert.AreEqual(expectedResult, result);
        }

        // Test by Alonso León Rodríguez | Sprint 2
        [Test]
        public void roundToCloserFails()
        {
            var context = createLocoproContext();
            float numberToRound = 3.3F;
            float expectedResult = 3F;

            var result = context.RoundToCloserReal(numberToRound);

            Assert.AreNotEqual(expectedResult, result);
        }

        // Test by Alonso León Rodríguez | Sprint 2
        [Test]
        public void roundToCloserNegativeInput()
        {
            var context = createLocoproContext();
            float numberToRound = -1.0F;
            float expectedResult = 0F;

            var result = context.RoundToCloserReal(numberToRound);

            Assert.AreEqual(expectedResult, result);
        }

        // Test by Alonso León Rodríguez | Sprint 2
        [Test]
        public void roundToCloserNullInput()
        {
            var context = createLocoproContext();
            float expectedResult = 0F;

            var result = context.RoundToCloserReal(null);

            Assert.AreEqual(expectedResult, result);
        }
    }
}
