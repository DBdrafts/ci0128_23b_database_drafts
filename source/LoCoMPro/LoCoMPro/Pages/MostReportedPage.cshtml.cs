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

        /// <summary>
        /// List of registers.
        /// </summary>
        public IList<Register> Registers { get; set; } = new List<Register>();

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
            getUsersMoreReported();
            orderListByNumberOfReportsReceived(UsersWhoMadeReports);
            getUserRatings();
        }

        public int getUsersMoreReported()
        {
            int err = -1; // Error getting users who reported

            // Select all users from the _context.Users table
            var users = from u in _context.Users
                        select u;

            // Select all reports from the _context.Reports table
            var reports = from r in _context.Reports
                          select r;

            // Select all registers from the _context.Registers table
            var registers = from r in _context.Registers
                            select r;

            // Check if there are no users in the context
            if (!users.Any())
            {
                System.Diagnostics.Debug.WriteLine("There are no users in this context");
                return err; // Assuming err is defined somewhere in the code
            }

            // Check if there are no reports in the context
            if (!reports.Any())
            {
                System.Diagnostics.Debug.WriteLine("There are no reports in this context");
                return err; // Assuming err is defined somewhere in the code
            }

            // Check if there are no registers in the context
            if (!registers.Any())
            {
                System.Diagnostics.Debug.WriteLine("There are no registers in this context");
                return err; // Assuming err is defined somewhere in the code
            }

            // Convert the users, reports and registers query result to a list and assign it to the properties
            Users = users.ToList();
            Reports = reports.ToList();
            Registers = registers.ToList();

            var usersWhoMadeReports = new List<User>();

            // for each report
            foreach (var report in reports)
            {
                if (report.ReportState == 1 ||  report.ReportState == 2)
                {
                    var reported = GetReported(report, Users);
                    if (reported != null && !usersWhoMadeReports.Contains(reported))
                    {
                        usersWhoMadeReports.Add(reported);
                    }
                }

            }
            UsersWhoMadeReports = usersWhoMadeReports;

            err = usersWhoMadeReports.Count;

            return err;
        }

        /// <summary>
        /// Method to order as the number of reports received
        /// </summary>
        public void orderListByNumberOfReportsReceived(IList<User> usersWhoMadeReports)
        {
            UsersWhoMadeReports = usersWhoMadeReports
                .OrderByDescending(user => numberOfReceivedReports(user))
                .ThenBy(user => user.UserName)
                .ToList();
        }


        /// <summary>
        /// Method to get the ranking of the users 
        /// </summary>
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


        /// <summary>
        /// Method to calculate the number of registers for a given user  
        /// </summary>
        public int numberOfRegisters(User user)
        {
            // Initialize the variable to store the number of registers
            int numRegisters = 0;

            // Filter registers based on the specified user as a contributor
            var reports = Registers.Where(r => r.Contributor == user);

            // Check if there are any registers for the specified user
            if (reports.Any())
            {
                // If there are, count the number of registers
                numRegisters = reports.Count();
            }

            // Return the calculated number of registers
            return numRegisters;
        }

        /// <summary>
        /// Method to calculate the number of registers for a given user  
        /// </summary>
        public int numberOfReceivedReports(User user)
        {
            int numReceivedReports = 0;

            var receivedReports = _context.Reports
                .Where(r => r.ReportedRegister.ContributorId == user.Id);

            if (receivedReports.Any())
            {
                numReceivedReports = receivedReports.Count();
            }

            return numReceivedReports;
        }

        /// <summary>
        /// Method to get the user reported
        /// </summary>
        public User GetReported(Report report, IList<User> users)
        {
            // Find the user with the matching Id
            return users.FirstOrDefault(u => u.Id == report.ContributorId);
        }

        /// <summary>
        /// Method to calculate the number of registers for a given user  
        /// </summary>
        public int numberOfHiddenContributions(User user)
        {
            int numReceivedReports = 0;

            var receivedReports = _context.Reports
                .Where(r => r.ReportedRegister.ContributorId == user.Id)
                .Where(r => r.ReportState == 2);

            if (receivedReports.Any())
            {
                numReceivedReports = receivedReports.Count();
            }

            return numReceivedReports;
        }
        /// <summary>
        /// Method to calculate the percentage of rejected records for a given user.
        /// </summary>
        public int GetRejectedPercentage(int totalAports, int hiddenAports)
        {
            // Check if the denominator (totalAports) is non-zero to avoid division by zero.
            if (totalAports > 0)
            {
                // Calculate the percentage of rejected records.
                double rejectedPercentage = Math.Round((double)hiddenAports / totalAports * 100);
                return (int)rejectedPercentage;
            }
            else
            {
                // If totalAports is zero, the percentage is also zero.
                return 0;
            }
        }


    }
}
