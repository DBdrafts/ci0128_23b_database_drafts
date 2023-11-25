using LoCoMPro.Data;
using LoCoMPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
            var users = from r in _context.Users
                        select r;

            Users = users.ToList();


            userMeta = Users.FirstOrDefault(user => user.Id == "7d5b4e6b-28eb-4a70-8ee6-e7378e024aa4");

      
            var registers = from r in _context.Registers
                            where r.MetahuristicState == 0 || r.MetahuristicState == 1
                            select r;

            registers = registers.Include(r => r.Images);

            var reports = from r in _context.Reports
                          where r.ReportState == 4 
                          select r;


            // Gets the Data From data base 
            Registers = await registers.ToListAsync();
            
            if (!reports.IsNullOrEmpty())
            {
                Reports = await reports.ToListAsync();

            }

            // Gets the average review value of the registers
            ObtainAverageReviewValues();

            meta();

            generateReports();

        }

        /// <summary>
        /// POST HTTP request. Makes the report valid, sets its status to 2 and hides it from everyone.
        /// </summary>
        public IActionResult OnPostAcceptReport(string reportData)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;
            string[] values = SplitString(reportData, '\x1F');
            string reporterId = values[0], contributorId = values[1], productName = values[2],
                storeName = values[3], submitionDate = values[4], cantonName = values[5],
                provinceName = values[6], reportDate = values[7];
            int reportState = int.Parse(values[8]);

            DateTime reportSubmitDate = DateTime.Parse(reportDate);
            DateTime contributionDate = DateTime.Parse(submitionDate);

            var report = getReportToUpdate(reporterId, contributorId, productName, storeName, contributionDate,
                cantonName, provinceName, reportSubmitDate);

            report.ReportState = 2;

            _context.SaveChanges();

            return new JsonResult("OK");
        }

        /// <summary>
        /// POST HTTP request. Makes the report invalid, sets its status to 0 and returns the register to its original state.
        /// </summary>
        public IActionResult OnPostRejectReport(string reportData)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;
            string[] values = SplitString(reportData, '\x1F');
            string reporterId = values[0], contributorId = values[1], productName = values[2],
                storeName = values[3], submitionDate = values[4], cantonName = values[5],
                provinceName = values[6], reportDate = values[7];
            int reportState = int.Parse(values[8]);

            DateTime reportSubmitDate = DateTime.Parse(reportDate);
            DateTime contributionDate = DateTime.Parse(submitionDate);

            var report = getReportToUpdate(reporterId, contributorId, productName, storeName, contributionDate,
                cantonName, provinceName, reportSubmitDate);

            report.ReportState = 0;

            _context.SaveChanges();

            return new JsonResult("OK");
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

        
        public int meta()
        {
            int amountRegisters = 0;

            // register to apply meta
            foreach (var r in Registers)
            {
               if (r.Price > 1500000)
               {
                    anormalRegisters.Add(r);
               }
            }

            amountRegisters = anormalRegisters.Count();

            return amountRegisters;
            
        }

        public int generateReports()
        {
            int amountRoports = 0;

            foreach(var r in anormalRegisters)
            {
                if (r.MetahuristicState == 0) {

                    var report = new Report
                    {
                        ReportedRegister = r,
                        Reporter = userMeta,
                        ReportDate = DateTime.Now,
                        CantonName = r.CantonName!,
                        ProvinceName = r.ProvinciaName!,
                        ReportState = 4
                    };

                    r.MetahuristicState = 1;
                    _context.Reports.Add(report);
                    Reports.Add(report);
                    _context.SaveChanges();
                }
               
                
            }
            return amountRoports;
        }
        

    }
}
