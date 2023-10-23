using LoCoMPro.Data;
using LoCoMPro.Models;
using LoCoMPro.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
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
        public PaginatedList<Register> Register { get; set; } = default!;

        /// <summary>
        /// List of users that the reported registers belong to.
        /// </summary>
        public IList<User> Users { get; set; } = default!;

        /// <summary>
        /// GET HTTP request, initializes page values.
        /// </summary>
        public async Task OnGetAsync(int? pageIndex)
        {
            var registers = from r in _context.Registers select r;

            // Get the registers that are reported
            // registers = registers.Where(x => x.ReportedStatus == 1);

            var users = _context.Users
                .Where(user => registers.Any(register => register.ContributorId == user.Id))
                .ToList();
            Users = users;

            // Get th amount of pages that will be needed for all the registers
            var pageSize = Configuration.GetValue("PageSize", 5);

            // Gets the Data From data base 
            Register = await PaginatedList<Register>.CreateAsync(
                registers.AsNoTracking(), pageIndex ?? 1, pageSize);
        }

        //public async Task OnPostMoreInformationAsync (string prodName, string contributorID, string storeName, DateTime submissionDate)
        //{
        //    System.Diagnostics.Debug.WriteLine(prodName);
        //    System.Diagnostics.Debug.WriteLine(contributorID);
        //    System.Diagnostics.Debug.WriteLine(storeName);
        //    System.Diagnostics.Debug.WriteLine(submissionDate);
        //}
    }
}
