using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        // Navegation attributes
        public List<Register>? Registers { get; set; }  // TODO: May have to change to FLUENT API notation
        public List<Category>? Categories { get; set; }
        [NotMapped]
        public List<Store>? Stores { get; set; }
    }
}