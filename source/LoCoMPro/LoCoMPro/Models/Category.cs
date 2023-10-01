﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LoCoMPro.Models
{
    [PrimaryKey(nameof(CategoryName))]
    public class Category
    {
        public required string CategoryName { get; set; }

        public ICollection<Product>? Products { get; set; } = new List<Product>();
    }
}