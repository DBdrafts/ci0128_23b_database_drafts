﻿using LoCoMPro.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace LoCoMPro.Data
{
    [PrimaryKey(nameof(UserName))]
    public class User : IdentityUser
    {
        // Primary Key
        //public required string UserName { get; set; }
        //public required string Email { get; set; }
        //public required string Password { get; set; }

        // Navigation atributes
        public string? CantonName { get; set; }
        public string? ProvinciaName { get; set; }
        public ICollection<Register>? Registers { get; set; } = new List<Register>();  // TODO: May have to change to FLUENT API notation

        //  Foreing Key
        public required Canton Location { get; set; }

    };
}
