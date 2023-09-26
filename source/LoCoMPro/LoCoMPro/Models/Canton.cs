using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoCoMPro.Models
{
    [PrimaryKey(nameof(CantonName), nameof(ProvinciaName))]
    public class Canton
    {
        // Primary Key atributte
        public required string CantonName { get; set; }
        // Navegation atribute
        public required string ProvinciaName { get; set; }
        public List<Store>? Stores { get; set; }
        //public List<User>? Users { get; set; }
        // Relationship name
        [ForeignKey(nameof(ProvinciaName))]
        public required Provincia Provincia { get; set; }
    }
}
