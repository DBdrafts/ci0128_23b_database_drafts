using LoCoMPro.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LoCoMPro.Pages
{
    /// <summary>
    /// General PageModel for LoComPro system.
    /// </summary>
    public class LoCoMProPageModel : PageModel
    {
        /// <summary>
        /// Context to use for page.
        /// </summary>
        protected readonly LoCoMPro.Data.LoCoMProContext _context;

        /// <summary>
        /// Configuration for the page.
        /// </summary>
        protected readonly IConfiguration Configuration;

        /// <summary>
        /// LoComPro Page constructor.
        /// </summary>
        /// <param name="context">DB context to use for page.</param>
        /// <param name="configuration">Configuration for page.</param>
        public LoCoMProPageModel(LoCoMProContext context, IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;
        }

        /// <summary>
        /// Get if the user has location
        /// </summary>
        /// <param name="userToCheck">Register to directly check if register has images</param>
        public bool UserHasLocation(User userToCheck)
        {
            // Initialize a bool var to indicate whether the register has images.
            bool hasLocation = false;

            // Check if the input register is not null
            if (userToCheck.Geolocation != null)
            {
                // Set hasLocation to true 
                hasLocation = true;
            }

            // Return the boolean indicating whether the register has images.
            return hasLocation;
        }
    }
}