using Azure;
using Humanizer;
using LoCoMPro.Data;
using LoCoMPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.ObjectModelRemoting;
using Microsoft.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.IntervalRTree;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace LoCoMPro.Pages
{
    /// <summary>
    /// Model Page for AddProductPage, handles the functionality related to handling database operations, HTTP requests, and checking for user authorization.
    /// </summary>
    [Authorize]
    public class AddProductPageModel : LoCoMProPageModel
    {  
        private readonly UserManager<User> _userManager;

        /// <summary>
        /// Creates a new AddProductPageModel instance.
        /// </summary>
        /// <param name="context">Database context to utilize.</param>
        /// <param name="configuration">Routing configuration for WebPage.</param>
        /// <param name="userManager">User manager to handle user permissions.</param>
        public AddProductPageModel(LoCoMProContext context, IConfiguration configuration, UserManager<User> userManager)
           : base(context, configuration)
        {
            _userManager = userManager;
        }
        private static int imageIdCounter = 1;

        /// <summary>
        /// Available categories for products.
        /// </summary>
        public List<SelectListItem>? CategoryList { get; set; }

        /// <summary>
        /// List of product images.
        /// </summary>

        [BindProperty]
        public List<IFormFile>? ProductImages { get; set; }


        /// <summary>
        /// Loads categories from the DB.
        /// </summary>
        private void LoadCategories()
        {
            // Retrieves all categories from the database and stores them
            var categories = _context.Categories.ToList();

            // Assigns the 'CategoryList' property a list of 'SelectListItem' items created from 'categories'
            CategoryList = categories
                .Select(category => new SelectListItem
                {
                    Value = category.CategoryName, // data that is sent to the server when the user selects an item from the list
                    Text = category.CategoryName  // text that is displayed to the user in a dropdown list
                })
                .ToList();
        }

        /// <summary>
        /// Method called in response to an HTTP GET request.
        /// </summary>
        public void OnGet()
        {
            LoadCategories();
        }

        /// <summary>
        /// Adds the product to the DB, and redirects to Main Page.
        /// </summary>
        /// <returns>Redirect to Same page if the product is not valid, and to /Index was added successfully.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Get the information of the form
            string provinciaName = Request.Form["province"]!;
            string cantonName = Request.Form["canton"]!;
            string storeName = Request.Form["store"]!;
            string productName = Request.Form["productName"]!;
            float price = Convert.ToSingle(Request.Form["price"]);
            string? chosenCategory = Request.Form["category"];
            string? brandName = CheckNull(Request.Form["brand"]);
            string? modelName = CheckNull(Request.Form["model"]);
            string? comment = CheckNull(Request.Form["comment"]);
            double latitude = Convert.ToDouble(Request.Form["latitude"]);
            double longitude = Convert.ToDouble(Request.Form["longitude"]);
            var coordinates = new Coordinate(longitude, latitude);
            var geolocation = new Point(coordinates.X, coordinates.Y) { SRID = 4326 };

            cantonName = ValidateCanton(provinciaName, cantonName);
            // Get the product if exists in the context
            var productToAdd = _context.Products
                .Include(p => p.Registers)
                .Include(p => p.Stores)
                .Include(p => p.Categories)
                .FirstOrDefault(p => p.Name == productName);

            // Get the user by their ID
            var user = _context.Users.First(u => u.Id == _userManager.GetUserId(User));
            // Check and create a new store if not exists
            var store = AddStoreRelation(storeName, cantonName, provinciaName, geolocation);
            // Get category can be null
            var category = _context.Categories.FirstOrDefault(c => c.CategoryName == chosenCategory);

            // If the product doesn't exists
            if (productToAdd == null)
            {
                // Create new product
                productToAdd = CreateProduct(productName, brandName, modelName, category);

                // Add the product to the context
                _context.Products.Add(productToAdd);
            }
            // Checks if the category-store (AsociatedWith) relationship already exists, if not, adds it
            else if (category != null && !productToAdd.Categories!.Contains(category))
            {
                productToAdd.Categories.Add(category);
            }

            // Checks if the produc-store (Sells) relationship already exists, if not, adds it
            if (!productToAdd.Stores!.Contains(store)) productToAdd.Stores.Add(store);

            // Create new Register
            var newRegister = CreateRegister(productToAdd, store, price, comment, user);

            // Add the product to the context
            _context.Registers.Add(newRegister);

            // Save all changes in the contextDB
            await _context.SaveChangesAsync();

            TempData["FeedbackMessage"] = "Su registro fue agregado correctamente!";

            return RedirectToPage("/Index");
        }

        /// <summary>
        /// Adds a store to the context if it does not exist already.
        /// </summary>
        /// <param name="storeName">Name of the store to add.</param>
        /// <param name="cantonName">Canton where the store is located.</param>
        /// <param name="provinceName">Province where the store is located.</param>
        /// <param name="geolocation">Geolocation of the store to add.</param>
        /// <returns>A new store if it was not added, or found store.</returns>
        internal Store AddStoreRelation(string storeName, string cantonName, string provinceName, Point? geolocation = null)
        {
            var store = _context.Stores.Find(storeName, cantonName, provinceName);
            if (store == null) // If the store doesn't exist
            {
                // Create new store
                store = new Store()
                {
                    Name = storeName,
                    Location = _context.Cantones.First(c => c.CantonName == cantonName),
                    Geolocation = geolocation,

                };
                _context.Stores.Add(store);
            } else if (store.Geolocation == null)
            {
                store.Geolocation = geolocation;
            }
            return store;
        }

        /// <summary>
        /// Checks wether of not the given string is empty.
        /// </summary>
        /// <param name="attribute">String that needs checking.</param>
        /// <returns>If null if <paramref name="attribute"/> is empty, otherwise returns its value.</returns>
        // Method that returns null for omitted attributes
        static internal string? CheckNull(string? attribute)
        {
            if (attribute == "")
            {
                attribute = null;
            }
            return attribute;
        }

        /// <summary>
        /// Creates a product with given data.
        /// </summary>
        /// <param name="productName">Name of the product to create.</param>
        /// <param name="brandName">Brand of the product to create.</param>
        /// <param name="modelName">Model of the product to create.</param>
        /// <param name="category">Category asociated with the product.</param>
        /// <returns>New product that was created.</returns>
        internal Product CreateProduct(string productName, string? brandName, string? modelName, Category? category)
        {
            // Create new product
            var productToAdd = new Product()
            {
                Name = productName,
                Brand = brandName,
                Model = modelName,
            };

            if (category != null)
            {   // Add category-product relation
                productToAdd.Categories!.Add(category);
            }
            return productToAdd;
        }

        /// <summary>
        /// Creates a register with given data.
        /// </summary>
        /// <param name="productToAdd">Product that the register refers to.</param>
        /// <param name="store">Store where the product is sold.</param>
        /// <param name="price">Price of the product.</param>
        /// <param name="comment">Aditional comment left by the user.</param>
        /// <param name="user">User that submitted the register.</param>
        /// <returns>New register that was created.</returns>
        internal Register CreateRegister(Product productToAdd, Store store, float price, string? comment, User user)
        {
            DateTime dateTime = DateTime.Now;
            dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day
                , dateTime.Hour, dateTime.Minute, dateTime.Second, 0);

            Register newRegister = new()
            {
                SubmitionDate = dateTime,
                Contributor = user,
                Product = productToAdd,
                Store = store,
                Price = price,
                Comment = comment,
                Images = CreateFormattedImagesList(user, dateTime, productToAdd, store)
            };          
            return newRegister;
        }

        /// <summary>
        /// Creates a list of images with the data converted in bytes.
        /// </summary>
        /// <param name="user">User that submitted the register.</param>
        /// <param name="dateTime">Time when the images were submitted.</param>
        /// <param name="productToAdd">Product that the register refers to.</param>
        /// <param name="store">Store where the product is sold.</param>
        /// <returns>New list of images with the data converted in bytes.</returns>
        internal List<Image> CreateFormattedImagesList(User user, DateTime dateTime, Product productToAdd, Store store)
        {
            List<Image> newImagesList = new List<Image>();
            if (ProductImages != null && ProductImages.Any())
            {
                foreach (var image in ProductImages)
                {
                    if (image != null && image.Length > 0)
                    {
                        using (var stream = image.OpenReadStream())
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                stream.CopyTo(memoryStream);
                                byte[] imageBytes = memoryStream.ToArray();

                                string imageType = image.ContentType;
                                Image newImage = new()
                                {
                                    ImageId = imageIdCounter,
                                    SubmitionDate = dateTime,
                                    Contributor = user,
                                    ContributorId = user.Id,
                                    ProductName = productToAdd.Name,
                                    StoreName = store.Name,
                                    CantonName = store.CantonName!,
                                    ProvinceName = store.ProvinciaName!,
                                    ImageData = imageBytes,
                                    ImageType = imageType
                                };
                                newImagesList.Add(newImage);
                                ++imageIdCounter;
                            }
                        }
                    }
                }
            }
            return newImagesList;
        }

        /// <summary>
        /// Suggests data for autocomplete on required inputs of AddProduct form.
        /// </summary>
        /// <param name="field"> Type of data to recommend.</param>
        /// <param name="term"> Term to look up and recommend data for.</param>
        /// <param name="provinceName"> Name of the Province asociated with the store.</param>
        /// <param name="cantonName"> Name of the Canton asociated with the store.</param>
        /// <returns>List of suggestions for the autocomplete</returns>
        public IActionResult OnGetAutocompleteSuggestions(string field, string term, string provinceName, string cantonName)
        {
            List<String> availableSuggestions = new List<string>() { "" };
            if (field == "#store")
            {
                // Look for saved Stores in current location
                availableSuggestions = _context.Stores
                    .Where(s => s.ProvinciaName == provinceName && s.CantonName == cantonName)
                    .Select(s => s.Name)
                    .ToList();
            }
            else if (field == "#productName")
            {
                // Take the available sources form the store inventory or from the total number of produts.
                availableSuggestions = _context.Products.Select(p => p.Name)
                                                        .ToList();
            }

            // Filter suggestions based on the user's input
            var filteredSuggestions = availableSuggestions
                .Where(suggestion => suggestion.Contains(term, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return new JsonResult(filteredSuggestions);
        }
        
        /// <summary>
        /// Returs a Dictionary of the autofill data for a known product that the user selects.
        /// </summary>
        /// <param name="productName"> Name of the product to get the autofill data for.</param>
        /// <returns>Dictionay of with the autofill data for requested product.</returns>
        public IActionResult OnGetProductAutofillData(string productName)
        {
            var data = new Dictionary<string, string>();

            // Get first Pro uct Match
            Product? productMatch = _context.Products
                .Include(p => p.Categories)
                .FirstOrDefault(p => p.Name == productName);

            // If no product was found with the given input return empty dictionary
            if (productMatch == null) return new JsonResult(data);

            // Get product Brand and model into the dictionary.
            data["#brand"] = productMatch.Brand ?? "";
            data["#model"] = productMatch.Model ?? "";

            // Get first Category result or null
            data["#category"] = (productMatch.Categories != null && productMatch.Categories.Any()) ? productMatch.Categories.First().CategoryName : "";

            return new JsonResult(data);
        }

        /// <summary>
        /// Ensures that canton is a valid canton.
        /// </summary>
        /// <param name="province">Province to take into consideration</param>
        /// <param name="canton">Canton to validate.</param>
        /// <returns>Valid canton to save</returns>
        private string ValidateCanton(string province, string canton)
        {
            var response = "";
            if (canton == "")
            {
                response = (province.Equals("Guanacaste")) ? "Liberia" : province;
            } else
            {
                response = canton;
            }
            return response;
        }

        public IActionResult OnPostUploadImage([FromForm] List<IFormFile> ProductImages, string store, string productName)
        {
            this.ProductImages = ProductImages;
            string storex = store;
            string productx = productName;
            if (storex != null && storex != "")
            {
                Test(storex);

            }
            return new JsonResult("OK");
        }

        public string Test(string store)
        {
            string Testing = store;
            return "Hi";
        }
    }
}