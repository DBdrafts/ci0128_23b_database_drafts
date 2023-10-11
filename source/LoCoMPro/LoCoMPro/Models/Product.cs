using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoCoMPro.Models
{
    /// <summary>
    /// Model for product entity. 
    /// </summary>
    [PrimaryKey(nameof(Name))]
    public class Product
    {
        /// <summary>
        /// Name of the product.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Brand of the product.
        /// </summary>
        public string? Brand { get; set; }

        /// <summary>
        /// Model of the product.
        /// </summary>
        public string? Model { get; set; }

        // Navegation attributes
        /// <summary>
        /// Registers asociated with the product.
        /// </summary>
        public ICollection<Register>? Registers { get; set; } = new List<Register>();

        /// <summary>
        /// Categories asociated with the product.
        /// </summary>
        public ICollection<Category>? Categories { get; set; } = new List<Category>();

        /// <summary>
        /// Stores asociated with the product.
        /// </summary>d
        public virtual ICollection<Store>? Stores { get; set; } = new List<Store>();
    }
}