using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace LoCoMPro.Models
{
    [PrimaryKey(nameof(UserName))]
    public class User
    {
        public required string UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        
        // Navigation atributes
        //public string? CantonName { get; set; }
        //public string? ProvinciaName { get; set; }

        //public required Canton Location { get; set; }
    }
}
