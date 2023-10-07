using LoCoMPro.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace LoCoMPro.Data
{
    // Todo(Any): Assure that this is modeled correctly.
    public class User : IdentityUser
    {

        // Navigation atributes
        public string? CantonName { get; set; }
        public string? ProvinciaName { get; set; }
        public ICollection<Register>? Registers { get; set; } = new List<Register>();  // TODO: May have to change to FLUENT API notation

        //  Foreing Key
        public required Canton Location { get; set; }

    };
}
