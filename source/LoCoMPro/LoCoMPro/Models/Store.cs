using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoCoMPro.Models
{
    [PrimaryKey(nameof(Name), nameof(CantonName), nameof(ProvinciaName))]
    public class Store
    {
        // Primary Key
        public required string Name { get; set; }
        
        // Navegation attribute
        public required string CantonName { get; set; }
        public required string ProvinciaName { get; set; }

        // Foreign Key 
        public required Canton Location { get; set; }
    }
}
