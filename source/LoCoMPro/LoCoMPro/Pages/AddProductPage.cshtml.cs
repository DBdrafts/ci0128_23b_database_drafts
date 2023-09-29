using LoCoMPro.Data;
using LoCoMPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.ObjectModelRemoting;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public void OnGet()
        {
            var categories = _context.Categories.ToList();
            CategoryList = categories
                .Select(category => new SelectListItem
                {
                    Value = category.CategoryName,
                    Text = category.CategoryName
                })
                .ToList();


            var provincias = _context.Provincias.ToList();
            ProvinciaList = provincias
                .Select(provincia => new SelectListItem
                {
                    Value = provincia.Name,
                    Text = provincia.Name
                })
                .ToList();

            if (provincias.Any())
            {
                var primerProvincia = provincias.First().Name;
                LoadCantones(primerProvincia);
            }
        }

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

        public JsonResult OnGetCantones(string provincia)
        {
            LoadCantones(provincia);
            return new JsonResult(CantonList);
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

            var product = new Product
            {
                Name = productName,
                Brand = brandName,
                Model = modelName
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToPage("/Index");
        }
    }
}