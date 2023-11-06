using LoCoMPro.Data;
using LoCoMPro.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using System.Net;

namespace LoCoMPro.Pages
{
    public class UserInfoPageModel : LoCoMProPageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Creates a new UserInfoPageModel.
        /// </summary>
        /// <param name="context">DB Context to pull data from.</param>
        /// <param name="configuration">Configuration for page.</param>
        /// <param name="userManager">User manager to handle user permissions.</param>
        /// <param name="httpContextAccessor">Allow access to the http context
        // User Page constructor 
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

        /// <summary>
        /// double of latitud
        /// </summary>
        public double latitud { get; set; }

        /// <summary>
        /// Double of longitud
        /// </summary>
        public double longitud { get; set; }

        /// <summary>
        /// Selected geolocation
        /// </summary>
        public Point? geolocationSelected { get; set; }

        /// <summary>
        /// Selected geolocation
        /// </summary>
        public string? geolocationInPerfil { get; set; }

        /// <summary>
        /// GET HTTP request, initializes page values.
        /// </summary>
        public async Task OnGetAsync()
        {
            // Stores the user in the page
            UserInPage = await _userManager.GetUserAsync(User);

            // Set the Province string name to display
            Provincia = UserInPage.ProvinciaName != null ? UserInPage.ProvinciaName : "No agregada";

            // Set the Canton string name to display 
            Canton = UserInPage.CantonName != null ? UserInPage.CantonName : "No agregada";

            // Set the Canton string name to display 
            geolocationSelected = UserInPage.Geolocation;

            // Set the geolocation to display
            geolocationInPerfil = UserInPage.Geolocation != null ? "Agregada" : "No agregada";
        }

        public void OnPostUpdateProvince(double longitude, double latitude)
        {
            // set a new points to add to the user
            var coordinates = new Coordinate(longitude, latitude);
            // stores the geolocaion in user var
            geolocationSelected = new Point(coordinates.X, coordinates.Y) { SRID = 4326 };

            // Set lat and lng
            latitud = latitude;
            longitud = longitude;

            // Wait to save the geolocation in the user profile
            SaveGeolocationAsync().Wait();
        }

        private async Task SaveGeolocationAsync()
        {
            // Get the actual user
            var user = await _userManager.GetUserAsync(User);

            // Set the value of the geolocation selected
            user.Geolocation = geolocationSelected;

            // Save the state to retun in javascript
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                var response = new { message = "Provincia actualizada exitosamente" };
                Response.StatusCode = (int)HttpStatusCode.OK;
                await Response.WriteAsync(JsonConvert.SerializeObject(response));
            }
            else
            {
                var response = new { message = "Error al actualizar la geolocalización" };
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await Response.WriteAsync(JsonConvert.SerializeObject(response));
            }
        }



    }
}
