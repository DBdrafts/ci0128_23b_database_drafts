using LoCoMPro.Data;
using LoCoMPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Win32;

namespace LoCoMPro.Pages
{
    [Authorize(Roles = "Moderator")]
    public class UsersWhoCreateMoreReportsModel : LoCoMProPageModel
    {
        /// <summary>
        /// List of users.
        /// </summary>
        public IList<User> Users { get; set; } = new List<User>();

        /// <summary>
        /// List of users that have made reports.
        /// </summary>
        public IList<User> UsersWhoMadeReports { get; set; } = new List<User>();

        /// <summary>
        /// The rating of the users.
        /// </summary>
        public IList<float> UserRatings { get; set; }

        /// <summary>
        /// List of reports.
        /// </summary>
        public IList<Report>? Reports { get; set; } = new List<Report>();

        /// <summary>
        /// Page constructor
        /// </summary>
        public UsersWhoCreateMoreReportsModel(LoCoMProContext context, IConfiguration configuration)
            : base(context, configuration)
        {
        }
        /// <summary>
        /// OnGet Method, fills the class lists with Users who made reports, orders that list based on the number of reports
        /// and gets the user ratings for display.
        /// </summary>
        public void OnGet ()
        {
            getUsersWhoReportedRegisters();
            orderListByNumberOfReports(UsersWhoMadeReports);
            getUserRatings();
        }
        /// <summary>
        /// Initializes the UsersWhoReportedRegister list with users who reported a register. 
        /// Returns the amount of users in the list.
        /// </summary>
        public int getUsersWhoReportedRegisters()
        {
            // ID of the automatic user, used to avoid getting it as a user.
            string autoID = "7d5b4e6b-28eb-4a70-8ee6-e7378e024aa4";
            int err = -1; // Error getting users who reported
            var users = from u in _context.Users
                        where u.Id != autoID
                        select u;

            var reports = from r in _context.Reports
                          where r.ReporterId != autoID
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

            Users = users.ToList();

            Reports = reports.ToList();

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
        /// <summary>
        /// Orders the list of users who made reports by their number of reports decending.
        /// </summary>
        public void orderListByNumberOfReports(IList<User> usersWhoMadeReports)
        {
            UsersWhoMadeReports = usersWhoMadeReports
                .OrderByDescending(user =>
                {
                    int numReports = numberOfReports(user);
                    return numReports;
                }).ThenByDescending(user => user.UserName).ToList();
        }
        /// <summary>
        /// Gets the user who made the report.
        /// </summary>
        public User getReporter(Report report)
        {   
            if (report != null)
            {
                return report.Reporter;
            }
            return null;
        }
        /// <summary>
        /// Gets the number of reports made by the user.
        /// </summary>
        public int numberOfReports(User user)
        {
            int numReports = 0;

            var reports = Reports.Where(r => r.Reporter == user);

            if (reports.Any())
            {
                numReports = reports.Count();
            }

            return numReports;
        }
        /// <summary>
        /// Gets the number of reports made by the user that are approved.
        /// </summary>
        public int numberOfApprovedReports(User user)
        {
            int numApprovedReports = 0;

            var reports = Reports.Where(r => r.Reporter == user && r.ReportState == 2);
            if (reports.Any())
            {
                numApprovedReports = reports.Count();
            }
            return numApprovedReports;
        }
        /// <summary>
        /// Gets the number of reports made by the user that are rejected.
        /// </summary>
        public int numberOfRejectedReports(User user)
        {
            int numRejectedReports = 0;

            var reports = Reports.Where(r => r.Reporter == user && r.ReportState == 0);
            if (reports.Any())
            {
                numRejectedReports = reports.Count();
            }
            return numRejectedReports;
        }
        /// <summary>
        /// Calculates the percentage of register accordingly.
        /// </summary>
        public int calculatePercentage(int totalReports, int moderatedReports)
        {
            int percentage = (int) Math.Round((double)(moderatedReports * 100) / totalReports);

            return percentage;
        }
        /// <summary>
        /// Initializes the list of ratings with the user ratings.
        /// </summary>
        public void getUserRatings()
        {
            UserRatings = new List<float>();

            foreach (User user in UsersWhoMadeReports)
            {
                UserRatings.Add(_context.GetUserRating(user.Id));
            }
        }
        
    }
}
