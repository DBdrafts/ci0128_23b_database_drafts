using LoCoMPro.Data;
using LoCoMPro.Models;
using LoCoMPro.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LoCoMPro.Pages
{
    /// <summary>
    /// Page model for Moderate Page, accesess database to show reported registers.
    /// </summary>
    public class ModeratePageModel : LoCoMProPageModel
    {
        /// <summary>
        /// Creates a new ModeratePageModel.
        /// </summary>
        /// <param name="context">DB Context to pull data from.</param>
        /// <param name="configuration">Configuration for page.</param>
        // Moderate Page constructor 
        public ModeratePageModel(LoCoMProContext context, IConfiguration configuration) 
            : base(context, configuration) { }


        /// <summary>
        /// List of the registers that exist in the database.
        /// </summary>
        public IEnumerable<Register>? Registers { get; set; } = new List<Register>();

        /// <summary>
        /// Result of the query.
        /// </summary>
        public IList<Report>? Reports { get; set; } = new List<Report>();

        /// <summary>
        /// List of users that the reported registers belong to.
        /// </summary>
        public IList<User> Users { get; set; } = new List<User>();

        /// <summary>
        /// GET HTTP request, initializes page values.
        /// </summary>
        public async Task OnGetAsync(int? pageIndex)
        {
            var registers = from r in _context.Registers
                            select r;

            registers = registers.Include(r => r.Images);

            var reports = from r in _context.Reports
                          where r.ReportState == 1
                            select r;

            var users = _context.Users
                .Where(user => reports.Any(report => report.ContributorId == user.Id || report.ReporterId == user.Id))
                .ToList();
            Users = users;

            Registers = await registers.ToListAsync();
            // Gets the Data From data base 
            Reports = await reports.ToListAsync();
        }
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
        /// Gets the register on which an interaction was performed.
        /// </summary>
        /// <param name="contributorId">ID of the user associated with the register.</param>
        /// <param name="productName">Product associated with the register.</param>
        /// <param name="storeName">Store associated with the register.</param>
        /// <param name="registSubmitDate">Date and time the registration was made.</param>
        /// <returns>Register to update.</returns>
        public Report getReportToUpdate(string reporterId, string contributorId,
            string productName, string storeName, DateTime submitionDate, string cantonName,
                string provinceName, DateTime reportDate)
        {
            return _context.Reports.Include(r => r.Reporter).First(r => r.ReporterId == reporterId
                && r.ContributorId == contributorId
                && r.ProductName == productName
                && r.StoreName == storeName
                && r.SubmitionDate == submitionDate
                && r.CantonName == cantonName
                && r.ProvinceName == provinceName
                && r.ReportDate == reportDate
                );
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
