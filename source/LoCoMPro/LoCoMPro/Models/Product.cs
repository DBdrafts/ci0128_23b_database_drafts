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
        public ICollection<Register>? Registers { get; set; } = new List<Register>();  // TODO: May have to change to FLUENT API notation
        public ICollection<Category>? Categories { get; set; } = new List<Category>();

        public virtual ICollection<Store>? Stores { get; set; } = new List<Store>();
    }
}