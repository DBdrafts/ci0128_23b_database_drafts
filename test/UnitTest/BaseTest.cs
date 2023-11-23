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
using NetTopologySuite.IO;
using NetTopologySuite.Geometries;

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

        // Mock logger.
        var mockLogger = new Mock<ILogger<IndexModel>>();

        // Create a context instance of the DB using the configured options
        var dbContext = new LoCoMProContext(options);
        InitContext(ref dbContext);

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
            , mockUserStore, mockHttpContextAccessor, mockLogger, pageType);
    }

    // Create LoCoMPro page model depending of the string received
    protected LoCoMProPageModel CreateLoCoMProPageModel(LoCoMProContext dbContext
        , Mock<IConfiguration> mockConfiguration, Mock<UserManager<User>> mockUserManager
        , Mock<SignInManager<User>> mockSignInManager, Mock<IUserStore<User>> mockUserStore
        , Mock<IHttpContextAccessor> mockHttpContextAccessor, Mock<ILogger<IndexModel>> mockLogger
        , string pageType)
    {
        // Return the type of the LoCoMPro 
        switch (pageType)
        {
            // Have to return a Search page
            case "search_page":
                return new SearchPageModel(dbContext, mockConfiguration.Object, mockUserManager.Object);

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
            case "index_page":
                return new IndexModel(mockLogger.Object, dbContext, mockConfiguration.Object, mockUserManager.Object, mockSignInManager.Object);

            // Have to return a Product List page
            case "product_list":
                return new ProductListPageModel(dbContext, mockConfiguration.Object, null
                    , mockUserManager.Object);

            // Have to return a Report List page
            case "report_list_page":
                return new ListReportPageModel(dbContext, mockConfiguration.Object, null
                    , mockUserManager.Object);

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
        {
            Id = "3af5899a-3957-415a-9675-be20966ba6d7",
            UserName = "Prueba123",
            NormalizedUserName = "PRUEBA123",
            Email = "prueba123@gmail.com",
            NormalizedEmail = "PRUEBA123@GMAIL.COM",
            PasswordHash = "AQAAAAIAAYagAAAAEJfi9TUT6VewLFHdzos2qZ29eaoRr4s0YjS60YhkekCR0Mzbe5LMp3sYgj+elkblVA==",
            Location = canton
        };

        // Create an instance of class for random values
        Random random = new Random();
        // Create an instance of products
        var product = new Product() { Name = "Laptop" };
        // Initialize an empty list of objects registers
        var result = new List<Register>();

        byte[] imageBytes = new byte[]
        {
            0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01, 0x01, 0x00, 0x00, 0x01,
            0x00, 0x01, 0x00, 0x00, 0xFF, 0xDB, 0x00, 0x43, 0x00, 0x03, 0x02, 0x02, 0x03, 0x02, 0x02, 0x03,
            0x03, 0x03, 0x03, 0x04, 0x03, 0x03, 0x04, 0x05, 0x08, 0x05, 0x05, 0x04, 0x04, 0x05, 0x0A, 0x07,
            0x07, 0x06, 0x08, 0x0C, 0x0A, 0x0C, 0x0C, 0x0B, 0x0A, 0x0B, 0x0B, 0x0D, 0x0E, 0x12, 0x10, 0x0D
        };

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

            // Create a new Datetime with the random values of the seed
            DateTime randomDate = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);

            // Create a new Register
            var newRegister = new Register()
            {
                Product = product,
                Contributor = user,
                Store = store,
                Price = random.Next(743, 1369), // Add a random number for the price
                SubmitionDate = randomDate,
                Comment = "comment",
                CantonName = "Cartago",
                ProvinciaName = "Cartago",
            };

            // Create a new Report with a reference to the Register
            var report = new Report
            {
                Reporter = user,
                ReportedRegister = newRegister, // Assign the corresponding Register
                ReporterId = user.Id,
                ContributorId = user.Id,
                ProductName = product.Name,
                StoreName = store.Name,
                SubmitionDate = randomDate,
                CantonName = "Cartago",
                ProvinceName = "Cartago",
                ReportDate = DateTime.Now,
                ReportState = random.Next(0,3)
            };

            // Create a new Review with a reference to the Register
            var review = new Review
            {
                Reviewer = user,
                ReviewedRegister = newRegister,
                ReviewerId = user.Id,
                ContributorId = user.Id,
                ProductName = product.Name,
                StoreName = store.Name,
                SubmitionDate = randomDate,
                CantonName = "Cartago",
                ProvinceName = "Cartago",
                ReviewDate = randomDate,
                ReviewValue = random.Next(1, 6) // Set the review value in 1 to 5
            };

            // Create a new Image with a reference to the Register
            var image = new Image
            {
                Contributor = user,
                Register = newRegister,
                ImageId = random.Next(1, 1000),
                SubmitionDate = randomDate,
                ImageData = imageBytes,
                ImageType = "image/jpeg",
                ContributorId = user.Id,
                ProductName = product.Name,
                StoreName = store.Name,
                CantonName = "Cartago",
                ProvinceName = "Cartago"
            };

            // Assign the Report to the Register's Reports collection
            newRegister.Reports = new List<Report> { report };

            // Assign the Review to the Register's Reports collection
            newRegister.Reviews = new List<Review> { review };

            // Add the Images to the Register's Reports collection
            newRegister.Images = new List<Image> { image };

            // Add the Register to the result list
            result.Add(newRegister);
        }
        // Return the list of initialized registers
        return result;
    }

    protected LoCoMProContext createLocoproContext ()
    {
        var options = new DbContextOptionsBuilder<LoCoMProContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
            .Options;
        return new LoCoMProContext(options);
    }

    // Test to create elements of the list
    public static UserProductListElement CreateElementTest()
    {
        return new UserProductListElement("ProductTest", "BrandTest"
           , "ModelTest", "StoreTest", "ProvinceTest", "CantonTest", "10");
    }
    public static UserProductListElement CreateDifferentElementTest()
    {
        return new UserProductListElement("ProductTest1", "BrandTest1"
           , "ModelTest1", "StoreTest1", "ProvinceTest1", "CantonTest1", "100");
    }

    public static UserProductListElement CreateSimilarElementTest()
    {
        return new UserProductListElement("ProductTest1", "BrandTest"
           , "ModelTest", "StoreTest", "ProvinceTest", "CantonTest", "25");
    }

    private void InitContext(ref LoCoMProContext context)
    {
        var province = new Provincia { Name = "GenericProvince" };

        var coordinates = new Coordinate(6.9, 4.2);
        var geolocation = new Point(coordinates.X, coordinates.Y) { SRID = 4326 };
        
        var canton1 = new Canton { CantonName = "GenericCanton1", Provincia = province };
        var canton2 = new Canton { CantonName = "GenericCanton2", Provincia = province, Geolocation = geolocation};

        var category = new Category { CategoryName = "Category" };
        var product = new Product { Name = "GenericProduct", Model = "GenericModel", Brand = "GenericBrand" };
        product.Categories!.Add(category);
        var store1 = new Store
        {
            Name = "GenericStore1",
            Location = canton1,
            ProvinciaName = "GenericProvince",
            CantonName = "GenericCanton1",
        };

        var store2 = new Store
        { 
            Name = "GenericStore2",
            Location = canton2,
            ProvinciaName = "GenericProvince",
            CantonName = "GenericCanton2",
            Geolocation = geolocation
        };
        if (!context.Provincias.Any())
        {
            context.Provincias.Add(province);
            context.Provincias.Add(province);
            context.Cantones.Add(canton1);
            context.Cantones.Add(canton2);
            context.Stores.Add(store1);
            context.Stores.Add(store2);
            context.Products.Add(product);
            context.SaveChanges();
        }
    }
}
