﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LoCoMPro.Models;
using System.Reflection.Metadata;

namespace LoCoMPro.Data
{
    public class LoCoMProContext : DbContext
    {
        public LoCoMProContext(DbContextOptions<LoCoMProContext> options)
            : base(options)
        {
        }

        public DbSet<Canton> Cantones { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Provincia> Provincias { get; set; }
        public DbSet<Register> Registers { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<User> Users { get; set; }


        // TODO: May want to create a builder for each class
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Creating tables
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Product>().ToTable("Product");
            modelBuilder.Entity<Store>().ToTable("Store");
            modelBuilder.Entity<Register>().ToTable("Register");
            modelBuilder.Entity<Canton>().ToTable("Canton");
            modelBuilder.Entity<Provincia>().ToTable("Provincia");

            // Building relationships for Store
            modelBuilder.Entity<Store>()
                .HasOne(p => p.Location)
                .WithMany(e => e.Stores)
                .HasForeignKey(c => new { c.CantonName, c.ProvinciaName });

            // Building relationships for Register
            modelBuilder.Entity<Register>()
                .HasOne(p => p.Store)
                .WithMany(e => e.Registers)
                .HasForeignKey(c => new { c.StoreName, c.CantonName, c.ProvinciaName });

            modelBuilder.Entity<User>()
                .HasOne(p => p.Location)
                .WithMany(e => e.Users)
                .HasForeignKey(c => new { c.CantonName, c.ProvinciaName });

            modelBuilder.Entity<Category>()
                .HasMany(p => p.Products)
                .WithMany(c => c.Categories)
                .UsingEntity(
                    "AsociatedWith"
                    , l => l.HasOne(typeof(Product)).WithMany().HasForeignKey("ProductName")
                    , r => r.HasOne(typeof(Category)).WithMany().HasForeignKey("CategoryName"));

            modelBuilder.Entity<Store>()
                .HasMany(s => s.Products)
                .WithMany(p => p.Stores)
                .UsingEntity(
                    "Sells"
                    , l => l.HasOne(typeof(Product)).WithMany().HasForeignKey("ProductName")
                    , r => r.HasOne(typeof(Store)).WithMany().HasForeignKey("StoreName", "CantonName", "ProvinceName"));

            modelBuilder.Entity<Store>()
                .Navigation(s => s.Products)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            modelBuilder.Entity<Store>()
                .Navigation(s => s.Registers)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            modelBuilder.Entity<Product>()
                .Navigation(p => p.Stores)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            modelBuilder.Entity<Product>()
                .Navigation(p => p.Registers)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            modelBuilder.Entity<Product>()
                .Navigation(p => p.Categories)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            modelBuilder.Entity<User>()
                .Navigation(u => u.Registers)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            modelBuilder.Entity<Category>()
                .Navigation(c => c.Products)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            modelBuilder.Entity<Canton>()
                .Navigation(c => c.Stores)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            modelBuilder.Entity<Provincia>()
                .Navigation(p => p.Cantones)
                .UsePropertyAccessMode(PropertyAccessMode.Property);
        }

    }

}