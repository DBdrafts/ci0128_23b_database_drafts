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
using NuGet.Packaging;
using System.Diagnostics;
using System.Globalization;

namespace LoCoMPro.Pages
{
    [Authorize(Roles = "Moderator")]
    public class ModerateOldRegistersModel : LoCoMProPageModel
    {
        private readonly UserManager<User> _userManager;

        public IList<Product> Products { get; set; } = new List<Product>(); 

        public IList<Register> Registers { get; set; } = new List<Register>();

        public IList<Product> ProductsWithOldRegisters { get; set; } = new List<Product>();

        public IList<Store> StoresWithOldRegisters { get; set; } = new List<Store>();

        public IList<int> NumberOfOldRegisters { get; set; } = new List<int>();

        public IList<Register> OldRegisters { get; set; } = new List<Register>();

        public IList<DateTime> dateLimits {  get; set; } = new List<DateTime>();

        /// <summary>
        /// User of metauristica
        /// </summary>
        public User userMeta;

        public ModerateOldRegistersModel(LoCoMProContext context, IConfiguration configuration,
            UserManager<User> userManager) : base(context, configuration) {
            _userManager = userManager;
        }

        public async Task OnGetAsync()
        {
            userMeta = _context.Users.FirstOrDefault(user => user.Id == "7d5b4e6b-28eb-4a70-8ee6-e7378e024aa4");

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
            lookForOldRegisters(1);
            generateReports();
        }

        public async Task<int> getProducts()
        {
            var products = from p in _context.Products
                .Include(p => p.Registers)
                .Include(p => p.Stores)
                           select p;
            if (products != null)
            {
                Products = await products.ToListAsync();
            }
            return Products.Count;
        }
        public async Task<int> getRegisters()
        {
            var registers = from r in _context.Registers.Include(r => r.Reports)
                         where r.Reports.All(report => report.ReportState != 2) 
                                && (r.MetahuristicState == 0 || r.MetahuristicState == 2)
                         select r;
            if (registers != null)
            {
                Registers = await registers.ToListAsync();
            }
            return Registers.Count;
        }

        public void lookForOldRegisters(double coefficient)
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
                    int totalDays = (int)Math.Round(differenceInDays.TotalDays * coefficient);

                    // Third date, any register beyond this date is considered old and needs to be moderated
                    DateTime F3 = F1.AddDays(totalDays).Date;

                    var oldRegisters = productRegisters.Where(r => r.SubmitionDate.Date <= F3);

                    if (oldRegisters.Any())
                    {
                        ProductsWithOldRegisters.Add(product);
                        StoresWithOldRegisters.Add(store);
                        NumberOfOldRegisters.Add(oldRegisters.Count());
                        dateLimits.Add(F3);
                        OldRegisters.AddRange(oldRegisters);
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

        public void generateReports()
            {
            foreach (var register in OldRegisters)
            {   
                if(register.MetahuristicState == 0)
                {
                    _context.Reports.Add(new Report
                    {
                        ReportedRegister = register,
                        Reporter = userMeta,
                        ReportDate = DateTime.Now,
                        CantonName = register.CantonName!,
                        ProvinceName = register.ProvinciaName!,
                        ReportState = 6
                    });
                    register.MetahuristicState = 2;

                    _context.SaveChanges();
                }
            }
            
        }

        /// <summary>
        /// POST HTTP request. Makes the report valid, sets its status to 2 and hides it from everyone.
        /// </summary>
        public IActionResult OnPostHideRegisters(string registerData)
        {
            // Create a CultureInfo object with the InvariantCulture.
            CultureInfo culture = CultureInfo.InvariantCulture;

            // Split the reportsData string using the custom delimiter '\x1F' and store the values in an array.
            string[] values = SplitString(registerData, '\x1F');
            string prodName = values[0], storeName = values[1], dateLimit = values[2];
                

            // Parse date strings into DateTime objects.
            DateTime registerDateLimit = DateTime.Parse(dateLimit);

            // Call the getReportToUpdate method to retrieve the relevant report entity from the database.
            var reports = getReportsToUpdate(prodName, storeName);

            foreach (var report in reports)
            {
                report.ReportState = 2;
            }

            // Call the getRegistersToUpdate method to retrieve the relevant register entity from the database.
            var registers = getRegistersToUpdate(prodName, storeName, registerDateLimit);
            foreach (var register in registers)
            {
                register.MetahuristicState = 4;
            }

            // Save changes to the database.
            _context.SaveChanges();

            // Return a JsonResult with the string "OK".
            return new JsonResult("OK");
        }

        /// <summary>
        /// POST HTTP request. Makes the report invalid, sets its status to 0 and returns the register to its original state.
        /// </summary>
        public IActionResult OnPostKeepRegisters(string registerData)
        {
            // Create a CultureInfo object with the InvariantCulture.
            CultureInfo culture = CultureInfo.InvariantCulture;

            // Split the reportData string using the custom delimiter '\x1F' and store the values in an array.
            string[] values = SplitString(registerData, '\x1F');
            string prodName = values[0], storeName = values[1], dateLimit = values[2];


            // Parse date strings into DateTime objects.
            DateTime registerDateLimit = DateTime.Parse(dateLimit);

            // Call the getReportsToUpdate method to retrieve the relevant report entity from the database.
            var reports = getReportsToUpdate(prodName, storeName);

            foreach (var report in reports)
            {
                report.ReportState = 7;
            }

            // Call the getRegistersToUpdate method to retrieve the relevant register entity from the database.
            var registers = getRegistersToUpdate(prodName, storeName, registerDateLimit);
            foreach (var register in registers)
            {
                register.MetahuristicState = 4;
            }

            // Save changes to the database.
            _context.SaveChanges();

            // Return a JsonResult with the string "OK" indicating successful rejection.
            return new JsonResult("OK");
        }

        public List<Report> getReportsToUpdate(string prodName, string storeName)
        {
            var reports = from report in _context.Reports
                          where report.ProductName == prodName && 
                            report.StoreName == storeName && 
                            report.ReportState == 6
                          select report;
            return reports.ToList();
        }

        public List<Register> getRegistersToUpdate(string prodName, string storeName, DateTime dateLimit)
        {
            var registers = from register in _context.Registers
                            where register.ProductName == prodName &&
                                register.StoreName == storeName &&
                                register.SubmitionDate.Date == dateLimit.Date
                            select register;
            return registers.ToList();    
        }
        /// <summary>
        /// Splits a string into an array of substrings based on the specified delimiter character.
        /// </summary>
        /// <param name="input">The input string to split.</param>
        /// <param name="delimiter">The character used as the delimiter.</param>
        /// <returns>An array of substrings created by splitting the input string.</returns>
        internal static string[] SplitString(string input, char delimiter)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("Input string cannot be empty or null.");
            }

            return input.Split(delimiter);
        }

    }
}
