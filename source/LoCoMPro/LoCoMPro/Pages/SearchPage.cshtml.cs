using LoCoMPro.Data;
using LoCoMPro.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NetTopologySuite.Geometries;

namespace LoCoMPro.Pages
{
    /// <summary>
    /// Page model for SearchPage, handles requests, database access, and preparing data for the page.
    /// </summary>
    public class SearchPageModel : LoCoMProPageModel
    {
        private readonly UserManager<User> _userManager;
        /// <summary>
        /// Creates a new SearchPageModel, requires a context and configuration.
        /// </summary>
        /// <param name="context">DB context to use for page.</param>
        /// <param name="configuration">Configuration for page.</param>
        public SearchPageModel(LoCoMProContext context, IConfiguration configuration,
            UserManager<User> userManager)
            : base(context, configuration) 
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Categories that the user wants to filter by.
        /// <p>Its string with category names separated by a comma.</p>
        /// </summary>
        public string SelectedCategories { get; set; } = "";

        /// <summary>
        /// Provinces that the user wants to filter by.
        /// <p>Its string with province names separated by a comma.</p>
        /// </summary>
        internal string SelectedProvinces { get; set; } = "";

        /// <summary>
        /// Cantons that the user wants to filter by.
        /// <p>Its string with canton names separated by a comma.</p>
        /// </summary>
        public string SelectedCantons { get; set; } = "";

        /// <summary>
        /// List of the categories that the user can filter by.
        /// </summary>
        public IList<Category> Category { get; set; } = new List<Category>();

        /// <summary>
        /// List of the provinces that the user can filter by.
        /// </summary>
        public IList<Provincia> Provinces { get; set; } = new List<Provincia>();

        /// <summary>
        /// List of the cantons that the user can filter by.
        /// </summary>
        public IList<Canton> Cantons { get; set; } = new List<Canton>();

        /// <summary>
        /// Search String introduced by the user.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        /// <summary>
        /// Type of search the user is performing.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? SearchType { get; set; }

        /// <summary>
        /// Province the user wants to base the search on.
        /// </summary>
        public string? Province { get; set; }

        /// <summary>
        /// Canton the user wants to base the search on.
        /// </summary>
        public string? Canton { get; set; }

        /// <summary>
        /// Maps product name to list of categories.
        /// </summary>
        public Dictionary<string, string> CategoryMap { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Search results with extended Data Types.
        /// </summary>
        public IEnumerable<SearchResult> SearchResults { get; set; } = new List<SearchResult>();

        /// <summary>
        /// Flag of whether or not non zero distances are calculated for registers.
        /// </summary>
        public bool AreDistancesCalculated { get; set; } = false;

        /// <summary>
        /// User in the page
        /// </summary>
        public User UserInPage;

        /// <summary>
        /// OnGet method that handles the GET request.
        /// </summary>
        /// <param name="latitude">Latitude of location to use as base of search.</param>
        /// <param name="longitude">Longitude of location to use as base of search.</param>
        /// <returns></returns>
        public async Task OnGetAsync(double latitude = 0.0, double longitude = 0.0)
        {
            // get the user in the page
            UserInPage = await _userManager.GetUserAsync(User);

            if (SearchString == "" || SearchString is null || SearchString.Contains(';')) return;

            // Prepare the query to retrieve data from the database
            var registers = from r in _context.Registers
                            select r;

            // Get the coordenades dots to search
            var coordinates = new Coordinate(0.0, 0.0);
            var geolocation = new Point(coordinates.X, coordinates.Y) { SRID = 4326 };

            if (latitude != 0.0 && longitude != 0.0)
            { // check first if the user select a location
                coordinates = new Coordinate(longitude, latitude);
                geolocation = new Point(coordinates.X, coordinates.Y) { SRID = 4326 };
                AreDistancesCalculated = true;

            }else if (UserInPage != null)
            { // else if the user has a location in their profile
                if (UserHasLocation(UserInPage)){
                    coordinates = new Coordinate(UserInPage.Geolocation.X, UserInPage.Geolocation.Y);
                    geolocation = new Point(coordinates.X, coordinates.Y) { SRID = 4326 };
                    AreDistancesCalculated = true;
                }
               
            }
            
            try
            {
                SearchResults = _context.GetSearchResults(SearchType ?? "Nombre", SearchString!, geolocation);

                SearchResults = SearchResults.GroupBy(r => new { r.ProductName, r.StoreName })
                            .Select(grouped => grouped.OrderByDescending(r => r.SubmitionDate).First());
            } catch (Exception ex)
            {
                // Handle the exception here
                Console.Error.WriteLine("An exception occurred: " + ex.Message);
                return;
            } 
            

            var match = GetRegistersByType(registers);

            if (match == null) return;


            /* Retrieve data from the database */
            // Query to get all categories associated with at least one product in the register list
            Category = GetCategories(match);

            // Query to get all provinces associated with at least one register in the register list
            Provinces = _context.Provincias
                            .Where(province => match.Any(register => register.ProvinciaName == province.Name))
                            .ToList();

            // Query to get all cantons associated with at least one register in the register list
            Cantons = _context.Cantones
                            .Where(canton => match.Any(register => register.CantonName == canton.CantonName))
                            .ToList();

            // Fetch data from the database
            var productsInRegisters = _context.Products
                .Where(product => match.Any(register => register.ProductName == product.Name))
                .Include(product => product.Categories)
                .ToList();

            //  Gets the registers that match with the categories
            if (productsInRegisters != null)
            {
                var groupedProductsInRegisters = productsInRegisters
                    .GroupBy(product => product.Name)
                    .Where(group => group.Any(item => item.Categories != null)) // Filter out groups with null Categories
                    .ToDictionary(
                        group => group.Key,  // ProductName as the key
                        group => string.Join(";", group.SelectMany(item => item.Categories!.Select(category => category.CategoryName)))
                    );
                CategoryMap = groupedProductsInRegisters;
                }   
        }

        /// <summary>
        /// Gets the categories of the products that match with the search
        /// </summary>
        /// <param name="match">Registers that match with the search.</param>
        /// <returns></returns>
        public List<Category> GetCategories(IQueryable<Register>? match)
        {
            var categories = from c in _context.Categories
                             select c;

            return categories.Where(category => category.Products!.Any(product =>
                              match!.Any(register => register.ProductName == product.Name)))
                              .ToList();
        }

        /// <summary>
        /// Gets the registers by using the type of search choose.
        /// </summary>
        /// <param name="registersQuery">Registers to base the search on.</param>
        /// <returns></returns>
        public IQueryable<Register> GetRegistersByType(IQueryable<Register>? registersQuery)
        {
            IQueryable<Register> resultQuery;

            // Filter the register by the type of search choose 
            switch (SearchType)
            {
                case "Nombre":
                default:
                    resultQuery = registersQuery!
                        .Where(r => r.ProductName!.Contains(SearchString!));
                    break;
                case "Marca":
                    resultQuery = registersQuery!
                        .Where(r => _context.Products.Any(p => p.Name == r.ProductName && p.Brand!.Contains(SearchString!)));
                    break;
                case "Modelo":
                    resultQuery = registersQuery!
                        .Where(r => _context.Products.Any(p => p.Name == r.ProductName && p.Model!.Contains(SearchString!)));
                    break;
            }

            resultQuery = resultQuery.GroupBy(r => new { r.ProductName, r.StoreName })
                        .Select(grouped => grouped.OrderByDescending(r => r.SubmitionDate).First());

            return resultQuery;
        }
    }
}
