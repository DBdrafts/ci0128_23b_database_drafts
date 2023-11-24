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
            var currentUserId = _userManager.GetUserId(User);

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

    }
}
