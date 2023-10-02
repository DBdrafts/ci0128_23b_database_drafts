using LoCoMPro.Data;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoCoMPro.Models
{
    [PrimaryKey(nameof(CantonName), nameof(ProvinciaName))]
    public class Canton
    {
        // Primary Key
        public required string CantonName { get; set; }

        // Navegation atribute
        public string? ProvinciaName { get; set; }
        public ICollection<Store>? Stores { get; set; } = new List<Store>();
        public ICollection<User>? Users { get; set; } = new List<User>();

        // Foreing Key
        [ForeignKey(nameof(ProvinciaName))]
        public required Provincia Provincia { get; set; }
    }
}
