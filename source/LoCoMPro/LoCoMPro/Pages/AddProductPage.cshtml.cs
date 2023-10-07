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
    [Authorize]
    public class AddProductPageModel : LoCoMProPageModel
    {
        private readonly UserManager<User> _userManager;

        public AddProductPageModel(LoCoMProContext context, IConfiguration configuration, UserManager<User> userManager)
           : base(context, configuration) {
            _userManager = userManager;
        }

        /*
        public AddProductPageModel(LoCoMProContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }*/

        /*
        public AddProductPageModel(LoCoMProContext context, IConfiguration configuration)
            : base(context, configuration) { } */

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
            CantonList!.Insert(0, new SelectListItem { Value = "", Text = "ElegirCanton" });
            return new JsonResult(CantonList);
        }

        public JsonResult OnGetProvinces()
        {
            LoadProvincias();
            ProvinciaList!.Insert(0, new SelectListItem { Value = "", Text = "ElegirProvincia" });
            return new JsonResult(ProvinciaList);

        }

        // Method called in response to an HTTP GET request
        public void OnGet()
        {
            LoadCategories();
            
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

            string Id = _userManager.GetUserId(User);

            // Get the product if exists in the context
            var productToAdd = _context.Products
                .Include(p => p.Registers)
                .Include(p => p.Stores)
                .Include(p => p.Categories)
                .FirstOrDefault(p => p.Name == productName);
            // Check and create a new store if not exists
            var store = AddStoreRelation(storeName, cantonName, provinciaName);
            // Get category can be null
            var category = _context.Categories.FirstOrDefault(c => c.CategoryName == chosenCategory);

            // If the product doesn't exists
            if (productToAdd == null)  
            {
                // Create new product
                productToAdd = new()
                {
                    Name = productName,
                    Brand = brandName,
                    Model = modelName,
                };
                
                if (category != null)
                {   // Add category-product relation
                    productToAdd.Categories!.Add(category);
                }
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
            Register newRegister = new()
            {
                SubmitionDate = DateTime.Now,
                Contributor = _context.Users.First(u => u.Id == Id),
                Product = productToAdd,
                Store = store,
                Price = price,
                Comment = comment
            };
            // Add the product to the context
            _context.Registers.Add(newRegister);

            // Save all changes in the contextDB
            await _context.SaveChangesAsync();

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
                };
                _context.Stores.Add(store);
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

        // Suggests data for autocomplete on required inputs of page.
        public IActionResult OnGetAutocompleteSuggestions(string field, string term, string provinceName, string cantonName, string storeName)
        {
            // Create a list with the available suggestions, given the current inputs
            List<String> availableSuggestions = new List<string>() { "" };
            // When aked for the autofill for store
            if (field == "#store")
            {
                // Look for saved Stores in current location
                availableSuggestions = _context.Stores
                    .Where(s => s.ProvinciaName == provinceName && s.CantonName == cantonName)
                    .Select(s => s.Name)
                    .ToList();
            }
            // When asked for the autofill for product
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
        // Autofills model, brand and category based on product name
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
            //data["#category"] = (productMatch.Categories != null) ? productMatch.Categories!.First().CategoryName : "";
            data["#category"] = (productMatch.Categories != null && productMatch.Categories.Any()) ? productMatch.Categories.First().CategoryName : "";

            return new JsonResult(data);
        }
    }

}
