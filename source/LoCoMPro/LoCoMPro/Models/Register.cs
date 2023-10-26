using LoCoMPro.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoCoMPro.Models
{
    /// <summary>
    /// Model for the Register entity, has a user, store, and a product asociated with it.
    /// It is intended to keep track of a product's price over time.
    /// </summary>
    [PrimaryKey(nameof(ContributorId), nameof(ProductName), nameof(StoreName), nameof(SubmitionDate))]
    public class Register
    {
        /// <summary>
        /// Navigation atribute for user that submitted the register.
        /// </summary>
        [ForeignKey(nameof(ContributorId))]
        public required User Contributor { get; set; }

        /// <summary>
        /// Navigation atribute for product that the register refers to.
        /// </summary>
        [ForeignKey(nameof(ProductName))]
        public required Product Product { get; set; }

        /// <summary>
        /// Navigation atribute for the store that the register refers to.
        /// </summary>
        public required Store Store { get; set; }

        /// <summary>
        /// Date when the register was submitted.
        /// </summary>
        public required DateTime SubmitionDate { get; set; }

        /// <summary>
        /// Price of product as of register's submitted datetime.
        /// </summary>
        public required float Price { get; set; }
        
        /// <summary>
        /// Number of positive votes that the register has.
        /// Currently not implemented.
        /// </summary>
        [NotMapped]
        public uint? NumConfirmations { get; set; }

        /// <summary>
        /// Number of negative votes that the register has.
        /// Currently not implemented.
        /// </summary>

        public uint NumCorrections { get; set; }

        /// <summary>
        /// Comment that the user made about the current register.
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// Id of the user that submitted the register.
        /// </summary>
        public string? ContributorId { get; set; }

        /// <summary>
        /// Name of the product that the register refers to.
        /// </summary>
        public string? ProductName { get; set; }

        /// <summary>
        /// Name of the store that the store refers to.
        /// </summary>
        public string? StoreName { get; set; }

        /// <summary>
        /// Name of the canton where the store is.
        /// </summary>
        public string? CantonName { get; set; } 

        /// <summary>
        /// Name of the province where the store is.
        /// </summary>
        public string? ProvinciaName { get; set; }

        public ICollection<Image> Images { get; set; }
    }
}
