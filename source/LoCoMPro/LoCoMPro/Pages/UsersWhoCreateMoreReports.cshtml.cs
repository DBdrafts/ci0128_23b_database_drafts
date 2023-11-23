using LoCoMPro.Data;
using LoCoMPro.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace LoCoMPro.Pages
{
    public class UsersWhoCreateMoreReportsModel : LoCoMProPageModel
    {
        private readonly UserManager<User> _userManager;

        /// <summary>
        /// List of users.
        /// </summary>
        public IList<User> Users { get; set; } = new List<User>();

        /// <summary>
        /// List of users that have made reports.
        /// </summary>
        public IList<User> UsersWhoMadeReports { get; set; } = new List<User>();

        /// <summary>
        /// List of reports.
        /// </summary>
        public IList<Report>? Reports { get; set; } = new List<Report>();

        public UsersWhoCreateMoreReportsModel(LoCoMProContext context, IConfiguration configuration,
            UserManager<User> userManager)
            : base(context, configuration)
        {
            _userManager = userManager;
        }

        public void OnGet ()
        {
            getUsersWhoReportedRegisters();
            orderListByNumberOfReports(UsersWhoMadeReports);
        }

        public int getUsersWhoReportedRegisters()
        {
            int err = -1; // Error getting users who reported
            var users = from u in _context.Users
                        select u;

            var reports = from r in _context.Reports
                          select r;

            if (!users.Any()) 
            {
                System.Diagnostics.Debug.WriteLine("There are no users in this context");
                return err;
            }
            if (!reports.Any())
            {
                System.Diagnostics.Debug.WriteLine("There are no reports in this context");
                return err;
            }

            var usersWhoMadeReports = new List<User>();
            foreach (var report in reports) { 
                var reporter = getReporter(report);
                if (reporter != null && !usersWhoMadeReports.Contains(reporter))
                {
                    usersWhoMadeReports.Add(reporter);
                }
            }
            UsersWhoMadeReports = usersWhoMadeReports;

            err = usersWhoMadeReports.Count;

            return err;
        }

        public User getReporter(Report report)
        {   
            if (report != null)
            {
                return report.Reporter;
            }
            return null;
        }

        public int numberOfReports(User user)
        {
            int numReports = 0;

            var reports = from r in _context.Reports
                          where r.Reporter == user
                          select r;

            if (reports.Any())
            {
                numReports = reports.Count();
            }

            return numReports;
        }

        public void orderListByNumberOfReports (IList<User> usersWhoMadeReports)
        {
            UsersWhoMadeReports = usersWhoMadeReports.OrderBy(o => numberOfReports(o)).ToList();
        }
    }
}
