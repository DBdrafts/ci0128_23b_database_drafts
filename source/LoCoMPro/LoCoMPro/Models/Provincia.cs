using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LoCoMPro.Models
{
    /// <summary>
    /// Model for a Province entity.
    /// </summary>
    [PrimaryKey(nameof(Name))]
    public class Provincia
    {
        /// <summary>
        /// Name of the Province.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Cantons that reside inside the province.
        /// </summary>
        public ICollection<Canton>? Cantones { get; set; } = new List<Canton>();
    }
}
