using LoCoMPro.Models;
using Microsoft.AspNetCore.Identity;
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
        /// Name of the canton that the user has saved as default location.
        /// </summary>
        public string? CantonName { get; set; }

        /// <summary>
        /// Name of the province that the user has saved as default location.
        /// </summary>
        public string? ProvinciaName { get; set; }

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
    };
}
