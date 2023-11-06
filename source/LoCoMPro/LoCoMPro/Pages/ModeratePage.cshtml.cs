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
                            select r;

            var users = _context.Users
                .Where(user => reports.Any(report => report.ContributorId == user.Id || report.ReporterId == user.Id))
                .ToList();
            Users = users;

            Registers = await registers.ToListAsync();
            // Gets the Data From data base 
            Reports = await reports.ToListAsync();
        }
        
    }
}
