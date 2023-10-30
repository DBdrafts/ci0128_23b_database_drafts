using LoCoMPro.Data;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoCoMPro.Models
{
    /// <summary>
    /// Model for Canton entity.
    /// </summary>
    [PrimaryKey(nameof(CantonName), nameof(ProvinciaName))]
    public class Canton
    {
        // Primary Key.
        /// <summary>
        /// Name of the canton.
        /// </summary>
        public required string CantonName { get; set; }

        // Navegation atribute
        /// <summary>
        /// Name of the province that the canton asociates with.
        /// </summary>
        public string? ProvinciaName { get; set; }

        /// <summary>
        /// WGS 84 coordinates for geolocation.
        /// </summary>
        public Point? Geolocation { get; set; }

        /// <summary>
        /// Stores that reside inside the canton.
        /// </summary>
        public ICollection<Store>? Stores { get; set; } = new List<Store>();

        /// <summary>
        /// Users asociated with the canton.
        /// </summary>
        public ICollection<User>? Users { get; set; } = new List<User>();

        /// <summary>
        /// Province asociated with the canton.
        /// </summary>
        [ForeignKey(nameof(ProvinciaName))]
        public required Provincia Provincia { get; set; }
    }
}
