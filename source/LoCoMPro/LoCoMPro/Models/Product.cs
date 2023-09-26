using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LoCoMPro.Models
{
    [PrimaryKey(nameof(Name))]
    public class Product
    {
        //  Primary Key
        public required string Name { get; set; } 

        //  Atributtes
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? Category { get; set; }

        // Navegation attributes
        public List<Register>? Registers { get; set; }  // TODO: May have to change to FLUENT API notation
    }
}
