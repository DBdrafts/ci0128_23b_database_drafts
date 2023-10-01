using Azure;
using Humanizer;
using LoCoMPro.Data;
using LoCoMPro.Models;
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
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace LoCoMPro.Pages
{
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


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string provinciaName = Request.Form["selectedProvince"]!;
            string cantonName = Request.Form["selectedCanton"]!;
            string storeName= Request.Form["location"]!;
            string productName = Request.Form["productName"]!;
            float price = Convert.ToSingle(Request.Form["price"]);
            string chosenCategory = Request.Form["category"]!;
            string brandName = Request.Form["brand"]!;
            string modelName = Request.Form["model"]!;
            string comment = Request.Form["comment"]!;

            string userName = "Jose Miguel Garcia Lopez";  // STATIC USER

            Debug.WriteLine($"provincia: {provinciaName}");
            Debug.WriteLine($"canton: {cantonName}");
            Debug.WriteLine($"Tienda: {storeName}");
            Debug.WriteLine($"Producto: {productName}");
            Debug.WriteLine($"Precio: {price}");
            Debug.WriteLine($"Categor�a: {chosenCategory}");
            Debug.WriteLine($"Marca: {brandName}");
            Debug.WriteLine($"Modelo: {modelName}");
            Debug.WriteLine($"Comentario: {comment}");


            
            if (!_context.Products.Any(p => p.Name == productName))  // If the product doesn't exists
            {
                // Create new product
                Product newProduct = new()
                {
                    Name = productName,
                    Brand = brandName,
                    Model = modelName,
                    Categories = new List<Category>() { _context.Categories.FirstOrDefault(c => c.CategoryName == chosenCategory) }
                };
                _context.Products.Add(newProduct);
                _context.SaveChanges();

                AddStoreRelation(storeName, cantonName, provinciaName);
                //var store = _context.Stores.First(s => s.Name == storeName && s.CantonName == cantonName && s.ProvinciaName == provinciaName);
                // Use raw SQL to insert data into the table
                string sql = "INSERT INTO Sells (ProductName, StoreName, ProvinceName, CantonName) VALUES ({0}, {1}, {2}, {3})";
                _context.Database.ExecuteSqlRaw(sql, productName, storeName, provinciaName, cantonName);
                //_context.Add("Sells").
                //store.Products?.Add(newProduct);
                //_context.Database.
            } else
            {
                AddStoreRelation(storeName, cantonName, provinciaName);
                string sql = "SELECT 1 FROM Sells WHERE ProductName = {0} AND StoreName = {1}";
                var result = _context.Database.ExecuteSqlRaw(sql, productName, storeName);
                if (result < 0)
                {
                    sql = "INSERT INTO Sells (ProductName, StoreName, ProvinceName, CantonName) VALUES ({0}, {1}, {2}, {3})";
                    _context.Database.ExecuteSqlRaw(sql, productName, storeName, provinciaName, cantonName);
                }
                //Product product = _context.Products.First(p => p.Name == productName);
                //if (!_context.Stores.First(s => s.Name == storeName).Products.Contains(product))
                //{
                //    _context.Stores.First(s => s.Name == storeName).Products.Add(product);
                //}

            }
            _context.SaveChanges();

            // Create new Register
            Register newRegister = new()
            {
                SubmitionDate = DateTime.Now,
                Contributor = _context.Users.First(u => u.UserName == userName), // TODO: CHANGE STATIC USER!
                Product = _context.Products.First(p => p.Name == productName),
                Store = _context.Stores.First(s => s.Name == storeName),
                Price = price,
                Comment = comment
            };
            _context.Registers.Add(newRegister);

            await _context.SaveChangesAsync();
            return RedirectToPage("/Index");
        }

        private void AddStoreRelation(string storeName, string cantonName, string provinceName) {
            //var store = _context.Stores.First(p => p.Name == storeName && p.CantonName == cantonName && p.ProvinciaName == provinceName);
            if (!_context.Stores.Any(s => s.Name == storeName /*&& s.CantonName == cantonName && s.ProvinciaName == provinceName*/)) // If the store doesn't exist
            {
                // Create new store
                Store newStore = new()
                {
                    Name = storeName,
                    Location = _context.Cantones.First(c => c.CantonName == cantonName),
                    Products = new List<Product>()
                };
                _context.Stores.Add(newStore);
                _context.SaveChanges();
            }


        }
    }
}
