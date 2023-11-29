using LoCoMPro.Data;
using LoCoMPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using NetTopologySuite.Algorithm;
using System.Diagnostics;
using System.Globalization;


namespace LoCoMPro.Pages
{
    /// <summary>
    /// Page model for Moderate weird prices Page, accesess database to show registers.
    /// </summary>
    [Authorize(Roles = "Moderator")]
    public class ModerateAnomaliesPageModel : LoCoMProPageModel
    {
        private readonly UserManager<User> _userManager;

        /// <summary>
        /// List of the registers that exist in the database.
        /// </summary>
        public IEnumerable<Register>? Registers { get; set; } = new List<Register>();

        /// <summary>
        /// List of reports.
        /// </summary>
        public IList<Report>? Reports { get; set; } = new List<Report>();

        /// <summary>
        /// List of users that the reported registers belong to.
        /// </summary>
        public IList<User> Users { get; set; } = new List<User>();

        /// <summary>
        /// Average review value of the registers
        /// </summary>
        public IList<float> registerAverageReview { get; set; }

        /// <summary>
        /// Average price price of the registers
        /// </summary>
        public IList<float> registerAveragePrice { get; set; }

        /// <summary>
        /// Min prices of the registers
        /// </summary>
        public IList<float> registerMinPrice { get; set; }

        /// <summary>
        /// Max prices of the registers
        /// </summary>
        public IList<float> registerMaxPrice { get; set; }

        /// <summary>
        /// Std Desv prices of the registers
        /// </summary>
        public IList<float> registerStdDevPrice { get; set; }

        /// <summary>
        /// User of metauristica
        /// </summary>
        public User userMeta;

        /// <summary>
        /// liste of anormal registers
        /// </summary>
        public IList<Register> anormalRegisters { get; set; } = new List<Register>();

        /// <summary>
        /// Creates a new ModerateAnomaliesPageModel.
        /// </summary>
        /// <param name="context">DB Context to pull data from.</param>
        /// <param name="configuration">Configuration for page.</param>
        /// <param name="userManager">User manager to handle user permissions.</param>
        // Moderate Page constructor 
        public ModerateAnomaliesPageModel(LoCoMProContext context, IConfiguration configuration,
            UserManager<User> userManager)
            : base(context, configuration)
        {
            _userManager = userManager;
        }


        /// <summary>
        /// GET HTTP request, initializes page values.
        /// </summary>
        public async Task OnGetAsync(int? pageIndex)
        {
            // Get the users
            var users = from r in _context.Users
                        select r;

            // Convert the user to list
            Users = users.ToList();

            userMeta = Users.FirstOrDefault(user => user.Id == "7d5b4e6b-28eb-4a70-8ee6-e7378e024aa4");

            // Get the registers no checked or check and anormal
            var registers = from r in _context.Registers
                            where r.MetahuristicState == 0 || r.MetahuristicState == 1
                            select r;

            // Include his images
            registers = registers.Include(r => r.Images);

            // Get the reports to anormal price
            var reports = from r in _context.Reports
                          where r.ReportState == 4 
                          select r;

            // Gets the Data From data base 
            Registers = await registers.ToListAsync();
            
            // If have reports
            if (!reports.IsNullOrEmpty())
            {
                Reports = await reports.ToListAsync();
            }

            var MetahuristicDone = false;
            // Apply Metahuristic to get weird prices
            if (checkUserAutomatic(userMeta, "7d5b4e6b-28eb-4a70-8ee6-e7378e024aa4") == true)
            {
                MetahuristicDone = priceMetahuristics();

            }

            // Gets the average Price value of the registers
            ObtainAveragePriceValues();

            // Gets the average review value of the registers
            ObtainAverageReviewValues();

            // Gets the minimal prices of the regiters
            ObtainMinPriceValues();

            // Gets the max prices of the regiters
            ObtainMaxPriceValues();

            // Gets the Std Dev prices of the regiters
            ObtainStdDevPriceValues();

            // Generate the reports for anormal prices
            if (MetahuristicDone == true)
            {
                generateReports();
            }

        }

        /// <summary>
        /// POST HTTP request. Makes the report valid, sets its status to 2 and hides it from everyone.
        /// </summary>
        public IActionResult OnPostAcceptReport(string reportData)
        {
            // Create a CultureInfo object with the InvariantCulture.
            CultureInfo culture = CultureInfo.InvariantCulture;

            // Split the reportData string using the custom delimiter '\x1F' and store the values in an array.
            string[] values = SplitString(reportData, '\x1F');
            string reporterId = values[0], contributorId = values[1], productName = values[2],
                storeName = values[3], submitionDate = values[4], cantonName = values[5],
                provinceName = values[6], reportDate = values[7];
            int reportState = int.Parse(values[8]);

            // Parse date strings into DateTime objects.
            DateTime reportSubmitDate = DateTime.Parse(reportDate);
            DateTime contributionDate = DateTime.Parse(submitionDate);

            // Call the getReportToUpdate method to retrieve the relevant report entity from the database.
            var report = getReportToUpdate(reporterId, contributorId, productName, storeName, contributionDate,
                cantonName, provinceName, reportSubmitDate);
            report.ReportState = 2;

            // Call the getRegisterToUpdate method to retrieve the relevant register entity from the database.
            var register = getRegisterToUpdate(productName, storeName, cantonName, provinceName, contributorId);
            register.MetahuristicState = 4;

            // Save changes to the database.
            _context.SaveChanges();

            // Return a JsonResult with the string "OK".
            return new JsonResult("OK");
        }

        /// <summary>
        /// POST HTTP request. Makes the report invalid, sets its status to 0 and returns the register to its original state.
        /// </summary>
        public IActionResult OnPostRejectReport(string reportData)
        {
            // Create a CultureInfo object with the InvariantCulture.
            CultureInfo culture = CultureInfo.InvariantCulture;

            // Split the reportData string using the custom delimiter '\x1F' and store the values in an array.
            string[] values = SplitString(reportData, '\x1F');
            string reporterId = values[0], contributorId = values[1], productName = values[2],
                storeName = values[3], submitionDate = values[4], cantonName = values[5],
                provinceName = values[6], reportDate = values[7];
            int reportState = int.Parse(values[8]);

            // Parse date strings into DateTime objects.
            DateTime reportSubmitDate = DateTime.Parse(reportDate);
            DateTime contributionDate = DateTime.Parse(submitionDate);

            // Call the getReportToUpdate method to retrieve the relevant report entity from the database.
            var report = getReportToUpdate(reporterId, contributorId, productName, storeName, contributionDate,
                cantonName, provinceName, reportSubmitDate);
            report.ReportState = 0;

            // Call the getRegisterToUpdate method to retrieve the relevant register entity from the database.
            var register = getRegisterToUpdate(productName, storeName, cantonName, provinceName, contributorId);
            register.MetahuristicState = 4;

            // Save changes to the database.
            _context.SaveChanges();

            // Return a JsonResult with the string "OK" indicating successful rejection.
            return new JsonResult("OK");
        }

        /// <summary>
        /// Check the user of the metahusritic
        /// </summary>
        /// <param name="user">User to check.</param>
        /// <param name="SpecificId">ID of the user automatic that compare .</param>
        public bool checkUserAutomatic(User user, String SpecificId)
        {
            // Check if the user has the specified ID
            return user != null && user.Id == SpecificId;
        }

        /// <summary>
        /// Gets the report on which an interaction was performed.
        /// </summary>
        /// <param name="reporterId">ID of the user that reported the register.</param>
        /// <param name="contributorId">ID of the user that made the contribution.</param>
        /// <param name="productName">Name of the product.</param>
        /// <param name="storeName">Name of the store.</param>
        /// <param name="submitionDate">Date and time the register was made.</param>
        /// <param name="cantonName">Name of the canton.</param>
        /// <param name="provinceName">Province name.</param>
        /// <param name="reportDate">Date and time the report was made.</param>
        /// <returns>Report to update.</returns>
        public Report getReportToUpdate(string reporterId, string contributorId,
            string productName, string storeName, DateTime submitionDate, string cantonName,
                string provinceName, DateTime reportDate)
        {
            var reports = from r in _context.Reports select r;
            return reports.First(r => r.ReporterId == reporterId
                && r.ContributorId == contributorId
                && r.ProductName == productName
                && r.StoreName == storeName
                && r.CantonName == cantonName
                && r.ProvinceName == provinceName
                );
        }

        /// <summary>
        /// Gets the register on which an interaction was performed.
        /// </summary>
        /// <param name="productName">Name of the product.</param>
        /// <param name="storeName">Name of the store.</param>
        /// <param name="cantonName">Name of the canton.</param>
        /// <param name="provinceName">Province name.</param>
        /// <param name="contributorId">Contributor ID.</param>
        /// <returns>Register to update.</returns>
        public Register getRegisterToUpdate(string productName, string storeName,
            string cantonName, string provinceName, string contributorId)
        {
            var registers = from r in _context.Registers select r;
            return registers.First(r => r.ContributorId == contributorId
                && r.ProductName == productName
                && r.StoreName == storeName
                && r.CantonName == cantonName
                && r.ProvinciaName == provinceName
                );
        }

        /// <summary>
        /// Obtains the averages review values of the registers
        /// </summary>
        public void ObtainAverageReviewValues() 
        {
            registerAverageReview = new List<float>();
            // Gets the average value for each register
            foreach (Register register in Registers)
            {
                registerAverageReview.Add(_context.GetAverageReviewValue(register.ContributorId
                    , register.ProductName, register.StoreName, register.SubmitionDate));
            }
        }

        /// <summary>
        /// Obtains the averages price values of the registers
        /// </summary>
        public void ObtainAveragePriceValues()
        {
            registerAveragePrice = new List<float>();
    
            // Gets the average value for each register
            foreach (Register register in anormalRegisters)
            {
                registerAveragePrice.Add(_context.GetProductValue(register.ProductName
                    , register.StoreName, register.CantonName, register.ProvinciaName));
            }
        }

        /// <summary>
        /// Obtains the min price values of the registers
        /// </summary>
        public void ObtainMinPriceValues()
        {
            registerMinPrice = new List<float>();

            // Gets the average value for each register
            foreach (Register register in anormalRegisters)
            {
                registerMinPrice.Add(_context.GetMinProductValue(register.ProductName
                    , register.StoreName, register.CantonName, register.ProvinciaName));
            }
        }


        /// <summary>
        /// Obtains the max price values of the registers
        /// </summary>
        public void ObtainMaxPriceValues()
        {
            registerMaxPrice = new List<float>();

            // Gets the average value for each register
            foreach (Register register in anormalRegisters)
            {
                registerMaxPrice.Add(_context.GetMaxProductValue(register.ProductName
                    , register.StoreName, register.CantonName, register.ProvinciaName));
            }
        }

        /// <summary>
        /// Obtains the max price values of the registers
        /// </summary>
        public void ObtainStdDevPriceValues()
        {
            registerStdDevPrice = new List<float>();

            // Gets the average value for each register
            foreach (Register register in anormalRegisters)
            {
                float stdDevValue = _context.GetStdDevProductValue(register.ProductName, register.StoreName, register.CantonName, register.ProvinciaName);

                // Round the value to 2 decimal places
                float roundedValue = (float)Math.Round(stdDevValue, 2);

                registerStdDevPrice.Add(roundedValue);
            }
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

        /// <summary>
        /// Apply the metahuristic in the registers
        /// </summary>
        public bool priceMetahuristics()
        {
            //  Define a amosunt of registers var
            int amountRegisters;

            //  Define a amosunt of registers var
            bool metahuristicDone = false;

            // register to apply meta
            foreach (var r in Registers)
            {
                // Metahuristic Price
                if (r.MetahuristicState != 4)
                {
                    // Get the average price for the product
                    double averagePrice = _context.GetProductValue(r.ProductName, r.StoreName, r.CantonName, r.ProvinciaName);

                    // Set a threshold for abnormal prices
                    double threshold = 1.25; // prices that are 1.25 times higher or lower than the average as abnormal

                    // Check if the price is abnormally low or high
                    if (r.Price < averagePrice / threshold || r.Price > averagePrice * threshold)
                    {
                        // Add the register to the list of abnormal registers
                        anormalRegisters.Add(r);
                    }
                }
            }

            // get the amount of register
            amountRegisters = anormalRegisters.Count();

            // var to return if the metahuritic is Apply
            if (amountRegisters > 0)
            {
                metahuristicDone = true;
            }

            // Return the count of registers
            return metahuristicDone;
        }

        /// <summary>
        /// Apply the metahuristic in the registers
        /// </summary>
        public void generateReports()
        {
            // Iterate through each normal register in the collection
            foreach (var r in anormalRegisters)
            {
                // Check if the MetahuristicState of the register is 0
                if (r.MetahuristicState == 0) {

                    // Create a new report using information from the register
                    var report = new Report
                    {
                        ReportedRegister = r,
                        Reporter = userMeta,
                        ReportDate = DateTime.Now,
                        CantonName = r.CantonName!,
                        ProvinceName = r.ProvinciaName!,
                        ReportState = 4
                    };
                    // Update the MetahuristicState of the register to 1
                    r.MetahuristicState = 1;

                    // Add the report to the context and save changes
                    _context.Reports.Add(report);
                    Reports.Add(report);

                    // Add the report to the Reports collection
                    _context.SaveChanges();
                }
            }
        }

    }
}
