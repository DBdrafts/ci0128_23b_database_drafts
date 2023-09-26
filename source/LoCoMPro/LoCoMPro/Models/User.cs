using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace LoCoMPro.Models
{
    [PrimaryKey(nameof(UserName))]
    public class User
    {
        // Primary Key
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }

        // Navigation atributes
        public string? CantonName { get; set; }
        public string? ProvinciaName { get; set; }

        //  Foreing Key
        public required Canton Location { get; set; }

    }
}
