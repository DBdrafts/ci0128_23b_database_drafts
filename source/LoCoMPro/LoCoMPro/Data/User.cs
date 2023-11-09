using LoCoMPro.Models;
using Microsoft.AspNetCore.Identity;
using NetTopologySuite.Geometries;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace LoCoMPro.Data
{
    // Todo(Any): Assure that this is modeled correctly.
    /// <summary>
    /// Model for user entity.
    /// </summary>
    public class User : IdentityUser
    {

        /// <summary>
        /// The name of the role the user belongs to.
        /// </summary>
        public string? Role { get; set; }

        /// <summary>
        /// Name of the canton that the user has saved as default location.
        /// </summary>
        public string? CantonName { get; set; }

        /// <summary>
        /// Name of the province that the user has saved as default location.
        /// </summary>
        public string? ProvinciaName { get; set; }

        /// <summary>
        /// Geolocation of the user
        /// </summary>
        public Point? Geolocation { get; set; }

        /// <summary>
        /// Registers that the user has submitted.
        /// </summary>
        public ICollection<Register>? Registers { get; set; } = new List<Register>();

        /// <summary>
        /// Default location that the user has saved.
        /// </summary>
        public required Canton Location { get; set; }

        /// <summary>
        /// Reviews made by this user
        /// </summary>
        public ICollection<Review>? Reviews { get; set; } = new List<Review>();

        /// <summary>
        /// Reports made by this user
        /// </summary>
        public ICollection<Report>? Reports { get; set; } = new List<Report>();
    };
}
