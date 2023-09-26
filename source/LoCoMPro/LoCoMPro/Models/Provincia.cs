using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LoCoMPro.Models
{
    [PrimaryKey(nameof(Name))]
    public class Provincia
    {
        //  Primary Key atributte
        public required string Name { get; set; }

        // Navegation atribute
        public ICollection<Canton>? Cantones { get; set; }
    }
}
