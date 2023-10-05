using Azure;
using Humanizer;
using LoCoMPro.Data;
using LoCoMPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.ObjectModelRemoting;
using Microsoft.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace LoCoMPro.Pages
{
    [Authorize]
    public class AddProductPageModel : PageModel
    {
        private readonly LoCoMPro.Data.LoCoMProContext _context;

        public AddProductPageModel(LoCoMProContext context)
        {
            _context = context;
        }

        public List<SelectListItem>? CategoryList { get; set; }
        public List<SelectListItem>? ProvinciaList { get; set; }
        public List<SelectListItem>? CantonList { get; set; }


        // NOTE: The following 3 methods can be modularized, but to improve understanding, they were left as is
        // maybe we can do it later


        // Method for loading the list of categories from the database
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

        // Method for loading the list of provinces from the database
        private void LoadProvincias()
        {
            var provincias = _context.Provincias.ToList();
            ProvinciaList = provincias
                .Select(provincia => new SelectListItem
                {
                    Value = provincia.Name,
                    Text = provincia.Name
                })
                .ToList();

            // Checks if there is at least one province
            if (provincias.Any())
            {
                LoadCantones(provincias.First().Name);
            }
        }

        // Method for loading the list of cantones from the database
        private void LoadCantones(string provincia)
        {
            var cantones = _context.Cantones
                .Where(c => c.ProvinciaName == provincia)
                .ToList();

            CantonList = cantones
                .Select(canton => new SelectListItem
                {
                    Value = canton.CantonName,
                    Text = canton.CantonName
                })
            .ToList();
        }

        // Method called in response to an HTTP GET request to retrieve the list of cantones associated with a specific province
        public JsonResult OnGetCantones(string provincia)
        {
            LoadCantones(provincia);
            return new JsonResult(CantonList);
        }

        // Method called in response to an HTTP GET request
        public void OnGet()
        {
            LoadCategories();
            LoadProvincias();
        }

        // Get the data of the form and stores it in the DB
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Get the information of the form
            string provinciaName = Request.Form["selectedProvince"]!;
            string cantonName = Request.Form["selectedCanton"]!;
            string storeName= Request.Form["store"]!;
            string productName = Request.Form["productName"]!;
            float price = Convert.ToSingle(Request.Form["price"]);
            string? chosenCategory = Request.Form["category"];
            string? brandName = CheckNull(Request.Form["brand"]);
            string? modelName = CheckNull(Request.Form["model"]);
            string? comment = CheckNull(Request.Form["comment"]);

            string userName = "Jose Miguel Garcia Lopez";  // STATIC USER


            var productToAdd = _context.Products.Find(productName);  // Get the product if exists in the context
            var store = AddStoreRelation(storeName, cantonName, provinciaName);  // Check and create a new store if not exists
            var category = _context.Categories.Find(chosenCategory);  // Get category/ can be null

            if (productToAdd == null)  // If the product doesn't exists
            {
                // Create new product
                productToAdd = new()
                {
                    Name = productName,
                    Brand = brandName,
                    Model = modelName,
                    Categories = new List<Category>()
                };
                
                if (category != null)
                {   // Add category-product relation
                    productToAdd.Categories.Add(category);
                }
                // Add the product to the context
                _context.Products.Add(productToAdd);

            } else // if the product already exists
            {   
                if (category != null)
                {   // Checks if the category-store (AsociatedWith) relationship already exists, if not, adds it
                    string sqlCategoryQuery =
                    "IF NOT EXISTS (SELECT * FROM AsociatedWith WHERE CategoryName = {0} AND ProductName = {1})\n" +
                    "BEGIN\n" +
                    "    INSERT INTO AsociatedWith (CategoryName, ProductName) VALUES ({0}, {1})\n" +
                    "END";
                    try
                    {   // Apply SqlQuery
                        _ = _context.Database.ExecuteSqlRaw(sqlCategoryQuery, chosenCategory!, productName);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }
                }
            }
            _context.SaveChanges();

            // Checks if the produc-store (Sells) relationship already exists, if not, adds it
            string sqlQuery =
                "IF NOT EXISTS (SELECT * FROM Sells WHERE ProductName = {0} AND StoreName = {1} AND ProvinceName = {2} AND CantonName = {3})\n" +
                "BEGIN\n" +
                "    INSERT INTO Sells (ProductName, StoreName, ProvinceName, CantonName) VALUES ({0}, {1}, {2}, {3})\n" +
                "END";
            try
            {
                _ = _context.Database.ExecuteSqlRaw(sqlQuery, productName, storeName, provinciaName, cantonName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            // Create new Register
            Register newRegister = new()
            {
                SubmitionDate = DateTime.Now,
                Contributor = _context.Users.First(u => u.UserName == userName), // TODO: CHANGE STATIC USER!
                Product = productToAdd,
                Store = store,
                Price = price,
                Comment = comment
            };
            _context.Registers.Add(newRegister);  // Add the product to the context

            await _context.SaveChangesAsync();  // Save all changes in the contextDB

            return RedirectToPage("/Index");
        }

        private Store AddStoreRelation(string storeName, string cantonName, string provinceName) {
            var store = _context.Stores.Find(storeName, cantonName, provinceName);
            if (store == null) // If the store doesn't exist
            {
                // Create new store
                store = new()
                {
                    Name = storeName,
                    Location = _context.Cantones.First(c => c.CantonName == cantonName),
                    Products = new List<Product>()
                };
                _context.Stores.Add(store);
            } else
            {
                // Initialize product list
                store.Products = new List<Product> ();
            }
            return store;
        }

        // Method that returns null for omitted attributes
        static private string? CheckNull(string? attribute)
        {
            if (attribute == "")
            {
                attribute = null;
            }
            return attribute;
        }
        public IActionResult OnGetAutocompleteSuggestions(string field, string term, string provinceName, string cantonName, string storeName)
        {
            // Create a list withe the available suggestions, given the current inputs
            var availableSuggestions = new List<string>() { "Hola" };
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
                // Look for products sold in current store.
                string sqlQuery =
                    "SELECT ProductName\n" +
                    "FROM Sells\n" +
                    "WHERE StoreName = @p0 AND\n" +
                    "      ProvinceName = @p1 AND\n" +
                    "      CantonName = @p2;";
                availableSuggestions = _context.Database.SqlQueryRaw<string>(sqlQuery, storeName, provinceName, cantonName).ToList();
            }

            // Filter suggestions based on the user's input
            var filteredSuggestions = availableSuggestions
                .Where(suggestion => suggestion.Contains(term, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return new JsonResult(filteredSuggestions);
        }
        public IActionResult OnGetProductAutofillData(string productName)
        {
            var data = new Dictionary<string, string>();

            // Get first Pro uct Match
            Product? productMatch = _context.Products.FirstOrDefault(p => p.Name == productName);
            
            // If no product was found with the given input return empty dictionary
            if (productMatch == null) return new JsonResult(data);

            // Get product Brand and model into the dictionary.
            data["#brand"] = productMatch.Brand ?? "";
            data["#model"] = productMatch.Model ?? "";
            // Get First Category into the dictionary.
            string sqlQuery =
                    "SELECT TOP 1 CategoryName as Value\n" +
                    "FROM AsociatedWith a \n" +
                    "WHERE ProductName = @productName";
            // Parameters
            var parameters = new SqlParameter("@productName", SqlDbType.VarChar) { Value = productName };

            // Get first Category result or null
            var categoryName = _context.Database.SqlQueryRaw<string>(sqlQuery, parameters).FirstOrDefault();//.FirstOrDefault();
            //FirstOrDefault();
            data["#category"] = categoryName ?? "";
            // Return Result
            return new JsonResult(data);
        }
    }

}
