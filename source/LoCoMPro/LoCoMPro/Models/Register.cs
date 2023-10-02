﻿using LoCoMPro.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoCoMPro.Models
{
    [PrimaryKey(nameof(ContributorName), nameof(ProductName), nameof(StoreName), nameof(SubmitionDate))]
    public class Register
    {
        // Primary Keys
        [ForeignKey(nameof(ContributorName))]
        public required User Contributor { get; set; }
        [ForeignKey(nameof(ProductName))]
        public required Product Product { get; set; }

        public required Store Store { get; set; }
        public required DateTime SubmitionDate { get; set; }

        // Atributes
        public required float Price { get; set; }
        [NotMapped]
        public uint? NumConfirmations { get; set; }
        [NotMapped]
        public uint? NumCorrections { get; set; }
        public string? Comment { get; set; }

        // Navegation atributtes
        public string? ContributorName { get; set; }
        public string? ProductName { get; set; }
        public string? StoreName { get; set; }
        public string? CantonName { get; set; } 
        public string? ProvinciaName { get; set; }
    }
}
