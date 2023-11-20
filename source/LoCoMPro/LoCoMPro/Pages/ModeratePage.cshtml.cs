using LoCoMPro.Data;
using LoCoMPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Globalization;


namespace LoCoMPro.Pages
{
    /// <summary>
    /// Page model for Moderate Page, accesess database to show reported registers.
    /// </summary>
    [Authorize(Roles = "Moderator")]
    public class ModeratePageModel : LoCoMProPageModel
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
        /// Creates a new ModeratePageModel.
        /// </summary>
        /// <param name="context">DB Context to pull data from.</param>
        /// <param name="configuration">Configuration for page.</param>
        /// <param name="userManager">User manager to handle user permissions.</param>
        // Moderate Page constructor 
        public ModeratePageModel(LoCoMProContext context, IConfiguration configuration, 
            UserManager<User> userManager) 
            : base(context, configuration) {
            _userManager = userManager;
        }

        /// <summary>
        /// GET HTTP request, initializes page values.
        /// </summary>
        public async Task OnGetAsync(int? pageIndex)
        {
            var currentUserId = _userManager.GetUserId(User);

            var registers = from r in _context.Registers
                            select r;

            registers = registers.Include(r => r.Images);

            var reports = from r in _context.Reports
                          where r.ReportState == 1 && 
                          (r.ContributorId != currentUserId && r.ReporterId != currentUserId)
                          select r;

            var users = _context.Users
                .Where(user => reports.Any(report => report.ContributorId == user.Id || report.ReporterId == user.Id))
                .ToList();
            Users = users;

            Registers = await registers.ToListAsync();
            // Gets the Data From data base 
            Reports = await reports.ToListAsync();

            // Gets the average review value of the registers
            ObtainAverageReviewValues();
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
                && r.ReportDate == reportDate
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
    }
}
