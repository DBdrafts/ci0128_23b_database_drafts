using LoCoMPro.Data;
using LoCoMPro.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace LoCoMPro.Pages
{
    public class UserInfoPageModel : LoCoMProPageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserInfoPageModel(LoCoMProContext context, IConfiguration configuration,
            UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
            : base(context, configuration)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// User in the Page
        /// </summary>
        public User UserInPage { get; set; }

        /// <summary>
        /// Name in the province
        /// </summary>
        public string Provincia { get; set; }

        /// <summary>
        /// Name of the canton
        /// </summary>
        public string Canton { get; set; }

        public async Task OnGetAsync()
        {
            // Stores the user in the page
            UserInPage = await _userManager.GetUserAsync(User);

            // Set the Province string name to display
            if (UserInPage.ProvinciaName != null)
            {
                Provincia = UserInPage.ProvinciaName;
            }
            else
            {
                Provincia = "No agregada";
            }
            

            // Set the Canton string name to display 
            if (UserInPage.CantonName != null)
            {
                Canton = UserInPage.CantonName;
            }
            else
            {
                Canton = "No agregada";
            }


        }
    }
}
