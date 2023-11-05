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
using Microsoft.AspNetCore.Identity;
using Assert = NUnit.Framework.Assert;
using System;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using LoCoMPro.Areas.Identity.Pages.Account;
using Microsoft.Extensions.Logging;
using Castle.Core.Smtp;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;

[TestClass]
// Declaration of the test class
public class BaseTest
{
    // Variable to save a mock user
    protected UserManager<User> UserManager { get; }

    // Constructor that creates the mock user
    public BaseTest()
    {
        var userStore = new Mock<IUserStore<User>>();
        UserManager = new UserManager<User>(userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);
    }

    // Method to create the mock of the Razor Page Model.
    protected LoCoMProPageModel CreatePageModel(string pageType)
    {
        // Create a simulation object (mock) IConfiguration
        // Simulate the real behavior of the real object with the configuration
        var mockConfiguration = new Mock<IConfiguration>();

        // Set the mock's behavior and then return "TestSettingValue" when setup done
        mockConfiguration.Setup(c => c["SomeSetting"]).Returns("TestSettingValue");

        // Create a instance for the options for the context of DB
        // Configure the context of DB to not use a real DB
        // Get the configuration options of the LoCoMProContext DB 
        var options = new DbContextOptionsBuilder<LoCoMProContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
            .Options;

        // Create a context instance of the DB using the configured options
        var dbContext = new LoCoMProContext(options);

        // Create a UserStore to be used as a required argument in UserManager
        var mockUserStore = new Mock<IUserStore<User>>();

        // Create a ContextAccessor to be used as a required argument in SignInManager
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

        // Create a UserManager
        var mockUserManager = new Mock<UserManager<User>>(mockUserStore.Object, null, null
            , null, null, null, null, null, null);

        // Create a UserClaimsPrincipalFactory to be used as a required argument in SigninManager
        var mockUserClaimsPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<User>>();

        // Create a SignInManager
        var mockSignInManager = new Mock<SignInManager<User>>(
            mockUserManager.Object,
            mockHttpContextAccessor.Object,
            mockUserClaimsPrincipalFactory.Object, null, null, null, null
        );

        // return the new instance of new ProductPageModel instance
        // created with the database context and mock configuration
        // real instance
        return CreateLoCoMProPageModel(dbContext, mockConfiguration, mockUserManager, mockSignInManager
            , mockUserStore, mockHttpContextAccessor, pageType);
    }

    // Create LoCoMPro page model depending of the string received
    protected LoCoMProPageModel CreateLoCoMProPageModel(LoCoMProContext dbContext
        , Mock<IConfiguration> mockConfiguration, Mock<UserManager<User>> mockUserManager
        , Mock<SignInManager<User>> mockSignInManager, Mock<IUserStore<User>> mockUserStore
        , Mock<IHttpContextAccessor> mockHttpContextAccessor
        , string pageType)
    {
        // Return the type of the LoCoMPro 
        switch (pageType)
        {
            // Have to return a Search page
            case "search_page":
                return new SearchPageModel(dbContext, mockConfiguration.Object);

            // Have to return a Product page
            case "product_page":
                return new ProductPageModel(dbContext, mockConfiguration.Object, mockUserManager.Object, mockHttpContextAccessor.Object);

            // Have to return a Add Register page
            case "add_register_page":
                return new AddProductPageModel(dbContext, mockConfiguration.Object, mockUserManager.Object);

            // Have to return a Login page
            case "login_page":
                return new LoginModel(dbContext, mockConfiguration.Object, mockSignInManager.Object, null);

            // Have to return a Register page
            case "register_page":
                return new RegisterModel(dbContext, mockConfiguration.Object, mockUserManager.Object
                    , mockUserStore.Object, mockSignInManager.Object, null, null);

            // Return a base page model
            default:
                return new LoCoMProPageModel(dbContext, mockConfiguration.Object);
        }
    }

    // Method to Initialize the registers
    protected List<Register> InitRegisters()
    {
        // Create an instance of Province 
        var province = new Provincia { Name = "Cartago" };
        // Create an instance of Canton
        var canton = new Canton { CantonName = "Cartago", Provincia = province };
        // Create an instance of Store
        var store = new Store() 
        { Name = "Tienda", CantonName = "Cartago", ProvinciaName = "Cartago", Location = canton };
        // Create an instance of User
        var user = new User
        { Id = "3af5899a-3957-415a-9675-be20966ba6d7", UserName = "Prueba123", 
            NormalizedUserName = "PRUEBA123", Email = "prueba123@gmail.com", 
            NormalizedEmail = "PRUEBA123@GMAIL.COM", 
            PasswordHash = "AQAAAAIAAYagAAAAEJfi9TUT6VewLFHdzos2qZ29eaoRr4s0YjS60YhkekCR0Mzbe5LMp3sYgj+elkblVA==", 
            Location = canton };

        // Create an instance of class for random values
        Random random = new Random();
        // Create an instance of products
        var product = new Product() { Name = "Laptop" };

        // Initiliaze an empty list of objects registers
        var result = new List<Register>();
        // For to create 10 objects 
        for (int i = 0; i < 10; ++i)
        {   
            // Seed of values for random datetime
            int year = random.Next(2022, 2024); 
            int month = random.Next(1, 13); 
            int day = random.Next(1, 29); 
            int hour = random.Next(0, 24); 
            int minute = random.Next(0, 60); 
            int second = random.Next(0, 60);

            // Create a new Datetime with the tandom values of the seed
            DateTime randomDate = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);

            // Add registers to list
            result.Add(new Register()
            {   
                Product = product,
                Contributor = user,
                Store = store,
                // Add a random number for the price
                Price = random.Next(743, 1369),
                SubmitionDate = randomDate,
                Comment = "comment",
                CantonName = "Cartago",
                ProvinciaName = "Cartago",
            });
        }
        // returns the list of initialized registers
        return result;
    }
}
