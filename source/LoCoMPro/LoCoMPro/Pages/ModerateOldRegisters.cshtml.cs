using LoCoMPro.Data;
using LoCoMPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using NetTopologySuite.Algorithm;
using System.Diagnostics;
using System.Globalization;

namespace LoCoMPro.Pages
{
    [Authorize(Roles = "Moderator")]
    public class ModerateOldRegistersModel : LoCoMProPageModel
    {
        private readonly UserManager<User> _userManager;

        public IList<Product>? Products { get; set; } = new List<Product>(); 

        public IList<Register>? Registers { get; set; } = new List<Register>();

        public IList<Product> ProductsWithOldRegisters { get; set; } = new List<Product>();

        public IList<Store> StoresWithOldRegisters { get; set; } = new List<Store>();

        public IList<int> NumberOfOldRegisters {  get; set; }

        public IList<DateTime> dateLimits {  get; set; } = new List<DateTime>();

        public ModerateOldRegistersModel(LoCoMProContext context, IConfiguration configuration,
            UserManager<User> userManager) : base(context, configuration) {
            _userManager = userManager;
        }

        public async Task OnGetAsync()
        {
            int numProducts = await getProducts();
            int numRegisters = await getRegisters();

            if (numProducts == 0)
            {
                System.Diagnostics.Debug.WriteLine("There are no products");
            }
            if (numRegisters == 0)
            {
                System.Diagnostics.Debug.WriteLine("There are no registers");
            }
            lookForOldRegisters();
        }

        public async Task<int> getProducts()
        {
            var products = from p in _context.Products
                           select p;
            if (products != null)
            {
                Products = await products.ToListAsync();
            }
            return Products.Count;
        }
        public async Task<int> getRegisters()
        {
            var registers = from r in _context.Registers
                         select r;
            if (registers != null)
            {
                Registers = await registers.ToListAsync();
            }
            return Registers.Count;
        }

        public void lookForOldRegisters()
        {
            foreach (Product product in Products)
            {
                var productStores = product.Stores;

                foreach (Store store in productStores)
                {
                    // Get the registers that belong to the product and store
                    var productRegisters = Registers.Where(r => r.ProductName == product.Name
                        && r.StoreName == store.Name).ToList();

                    // Order registers by date
                    productRegisters = productRegisters.OrderByDescending(r => r.SubmitionDate.Date).ToList();

                    // Calculate 80% of registers
                    int index80Percent = calculate80PercentIndex(productRegisters.Count);

                    // Most recent date from all associated registers
                    DateTime F1 = productRegisters.First().SubmitionDate.Date;
                    // Date of the register in the 80% mark
                    DateTime F2 = productRegisters[index80Percent - 1].SubmitionDate.Date;

                    TimeSpan differenceInDays = F2 - F1;
                    int totalDays = (int)differenceInDays.TotalDays * 2;

                    // Third date, any register beyond this date is considered old and needs to be moderated
                    DateTime F3 = F1.AddDays(-totalDays).Date;

                    var oldRegisters = productRegisters.Where(r => r.SubmitionDate.Date <= F3);

                    if (oldRegisters.Any())
                    {
                        ProductsWithOldRegisters.Add(product);
                        StoresWithOldRegisters.Add(store);
                        NumberOfOldRegisters.Add(oldRegisters.Count());
                        dateLimits.Add(F3);
                    }
                }
            }
        }

        public int calculate80PercentIndex (int registerTotal)
        {
            int index80Percent = (int)Math.Round(registerTotal * 0.8);

            // Assures the index won't be negative
            index80Percent = Math.Max(index80Percent, 1);

            return index80Percent;
        }
    }
}
