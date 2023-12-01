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
    /// <summary>
    /// Page model for see the Most Reported Users.
    /// </summary>
    [Authorize(Roles = "Moderator")]
    public class MostReportedPageModel : LoCoMProPageModel
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
        /// The rating of the users.
        /// </summary>
        public IList<float> UserRatings { get; set; }

        /// <summary>
        /// List of reports.
        /// </summary>
        public IList<Report>? Reports { get; set; } = new List<Report>();

        public MostReportedPageModel(LoCoMProContext context, IConfiguration configuration,
           UserManager<User> userManager)
           : base(context, configuration)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// On get method for to get the information of each User   
        /// </summary>
        public void OnGet()
        {
            getUsersWhoReportedRegisters();
            orderListByNumberOfReports(UsersWhoMadeReports);
            getUserRatings();
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

            Users = users.ToList();

            Reports = reports.ToList();

            var usersWhoMadeReports = new List<User>();
            foreach (var report in reports)
            {
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

        public void orderListByNumberOfReports(IList<User> usersWhoMadeReports)
        {
            UsersWhoMadeReports = usersWhoMadeReports
                .OrderByDescending(user =>
                {
                    int numReports = numberOfReports(user);
                    return numReports;
                }).ThenByDescending(user => user.UserName).ToList();
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

            var reports = Reports.Where(r => r.Reporter == user);

            if (reports.Any())
            {
                numReports = reports.Count();
            }

            return numReports;
        }

        public int numberOfApprovedReports(User user)
        {
            int numApprovedReports = 0;

            var reports = from r in _context.Reports
                          where r.ReportState == 2 && r.Reporter == user
                          select r;
            if (reports.Any())
            {
                numApprovedReports = reports.Count();
            }
            return numApprovedReports;
        }

        public void getUserRatings()
        {
            // Create a new list to store user ratings
            UserRatings = new List<float>();

            // Iterate through each user who made reports
            foreach (User user in UsersWhoMadeReports)
            {
                // Retrieve the user rating from the context based on the user's ID
                // and add it to the UserRatings list
                float userRating = _context.GetUserRating(user.Id);
                UserRatings.Add(userRating);
            }
            // At this point, UserRatings contains the ratings of all users who made reports
        }



    }
}
