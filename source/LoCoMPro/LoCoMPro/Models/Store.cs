﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoCoMPro.Models
{
    [PrimaryKey(nameof(Name), nameof(CantonName), nameof(ProvinciaName))]
    public class Store
    {
        // Primary Key
        public required string Name { get; set; }
        
        // Navegation attribute
        public string? CantonName { get; set; }
        public string? ProvinciaName { get; set; }
        public List<Register>? Registers { get; set; }

        // Foreign Key 
        public required Canton Location { get; set; }
    }
}
