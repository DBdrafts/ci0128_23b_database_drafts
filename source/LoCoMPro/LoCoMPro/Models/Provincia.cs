using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LoCoMPro.Models
{
    [PrimaryKey(nameof(Name))]
    public class Provincia
    {
        //  Primary Key
        public required string Name { get; set; }

        // Navegation atributte
        public ICollection<Canton>? Cantones { get; set; } = new List<Canton>();
    }
}
