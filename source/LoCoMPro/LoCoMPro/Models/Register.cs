﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoCoMPro.Models
{
    [PrimaryKey(nameof(ContributorName), nameof(ProductName), nameof(StoreName), nameof(SubmitionDate))]
    public class Register
    {
        // Primary Key atributes
        [ForeignKey(nameof(ContributorName))]
        public required User Contributor { get; set; }
        [ForeignKey(nameof(ProductName))]
        public required Product Product { get; set; }
        [ForeignKey(nameof(StoreName))]
        public required Store Store { get; set; }
        public required DateTime SubmitionDate { get; set; }

        // Atributes
        public required float Price { get; set; }
        public uint? NumConfirmations { get; set; }
        public uint? NumCorrections { get; set; }
        public string? Comment { get; set; }
        
        // Atributes for navegation properties
        public required string ContributorName { get; set; }
        public required string ProductName { get; set; }
        public required string StoreName { get; set; }
    }
}
