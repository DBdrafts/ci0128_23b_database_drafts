using Azure;
using Humanizer;
using LoCoMPro.Data;
using LoCoMPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.ObjectModelRemoting;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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

            string productName = Request.Form["productName"]!;
            string brandName = Request.Form["brand"]!;
            string modelName = Request.Form["model"]!;
            float price = Convert.ToSingle(Request.Form["price"]);
            string categoria = Request.Form["category"]!;
            string provinciaName = Request.Form["selectedProvince"]!;
            string cantonName = Request.Form["selectedCanton"]!;
            var product = new Product
            {
                Name = productName,
                Brand = brandName,
                Model = modelName
            };

            Debug.WriteLine($"El resultado de la provincia es: {provinciaName}");
            Debug.WriteLine($"El resultado del canton es: {cantonName}");
            Debug.WriteLine($"El resultado de la categoría es: {categoria}");
            // if (string.Compare (provinciaName, "San Jose") == 0)
            //{
            //    var provinciaNueva = new Provincia { Name = "EstaPruebaFunciona" };
            //    _context.Provincias.Add(provinciaNueva);

            //}


            //if (string.Compare (cantonName, "Montes de Oca") == 0)
            //{
            //    var cantonProv = new Provincia { Name = "CantonNice" };
            //    _context.Provincias.Add(cantonProv);

            //}

            //_context.Products.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToPage("/Index");
        }
    }
}