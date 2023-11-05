using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;


namespace LoCoMPro.Models
{
    /// <summary>
    /// View that represents a search result for the search page.
    /// </summary>
    [Keyless]
    public class SearchResult
    {
        /// <summary>
        /// Submition Date of register.
        /// </summary>
        public DateTime SubmitionDate { get; set; }

        /// <summary>
        /// Id of contribuitor user.
        /// </summary>
        public string? ContributorId { get; set; }

        /// <summary>
        /// Name of the product referred to by register.
        /// </summary>
        public string? ProductName { get; set; }

        /// <summary>
        /// Brand of Product
        /// </summary>
        public string? Brand { get; set; }
        /// <summary>
        /// Model of Product
        /// </summary>
        public string? Model { get; set; }

        /// <summary>
        /// Name of store
        /// </summary>
        public string? StoreName { get; set; }

        /// <summary>
        /// Price of product
        /// </summary>
        public float Price { get; set; }

        /// <summary>
        /// Name of province where the store is located.
        /// </summary>
        public string? ProvinciaName { get; set; }

        /// <summary>
        /// Name of the canton where the store is located.
        /// </summary>
        public string? CantonName { get; set; }

        /// <summary>
        /// Name of province where the sotre is located.
        /// </summary>
        public Point? Geolocation { get; set; }

        /// <summary>
        /// Distance from reference coordinates to this register's location.
        /// </summary>
        public double? Distance { get; set; }
    }
}
