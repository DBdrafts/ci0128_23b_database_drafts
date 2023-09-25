using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LoCoMPro.Models
{
    [PrimaryKey(nameof(Contributor), nameof(Product), nameof(Store), nameof(SubmitionDate))]
    public class Register
    {
        // Primary Key atributes
        public required User Contributor { get; set; }
        public required Product Product { get; set; }
        public required Store Store { get; set; }
        public required DateTime SubmitionDate { get; set; }

        // Atributes
        public required float Price { get; set; }
        public uint? NumConfirmations { get; set; }
        public uint? NumCorrections { get; set; }
        
    }
}
