using System;
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
        public LoCoMProContext (DbContextOptions<LoCoMProContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Register> Registers { get; set; }
        public DbSet<Provincia> Provincias { get; set; }
        public DbSet<Canton> Cantones { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
            // Create tables fro User
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

            modelBuilder.Entity<Register>()
                .HasOne(p => p.Store)
                .WithMany(e => e.Registers)
                .HasForeignKey(c => new { c.StoreName, c.CantonName, c.ProvinciaName });


        }

    }

}


