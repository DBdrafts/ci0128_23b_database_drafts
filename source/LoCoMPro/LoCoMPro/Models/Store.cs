using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace LoCoMPro.Models
{
    /// <summary>
    /// Moedl class that represents a store, has a name and a location associated with it.
    /// </summary>
    [PrimaryKey(nameof(Name), nameof(CantonName), nameof(ProvinciaName))]
    public class Store
    {
        /// <summary>
        /// Store name, it's the primary key.
        /// </summary>
        public required string Name { get; set; }
        
        /// <summary>
        /// Name of the canton asociated with the store.
        /// </summary>
        public string? CantonName { get; set; }

        /// <summary>
        /// Name of province asociated withe the store.
        /// </summary>
        public string? ProvinciaName { get; set; }

        /// <summary>
        /// WGS 84 coordinates for geolocation.
        /// </summary>
        [NotMapped]
        public Point? Geolocation { get; set; }

        /// <summary>
        /// Registers asociated with the store.
        /// </summary>
        public ICollection<Register>? Registers { get; set; } = new List<Register>();

        /// <summary>
        /// Navegation atribute for Canton asociated with the sotre.
        /// </summary>
        public required Canton Location { get; set; }

        /// <summary>
        /// Products sold in the store.
        /// </summary>
        public ICollection<Product>? Products { get; set; } = new List<Product>();
    }
}
