using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using LoCoMPro.Models;
using System.Reflection.Metadata;

namespace LoCoMPro.Data
{
    /// <summary>
    /// DB context for application's database. Abstracts database usage to implement business logic.
    /// </summary>
    public class LoCoMProContext : IdentityDbContext<User>
    {
        /// <summary>
        /// Creates a new LoCoMProContext.
        /// </summary>
        /// <param name="options">Options to initialize context with.</param>
        public LoCoMProContext(DbContextOptions<LoCoMProContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Cantons that reside in the Database.
        /// </summary>
        public DbSet<Canton> Cantones { get; set; }

        /// <summary>
        /// Categories that reside in the Database.
        /// </summary>
        public DbSet<Category> Categories { get; set; }

        /// <summary>
        /// Products that reside in the Database.
        /// </summary>
        public DbSet<Product> Products { get; set; }

        /// <summary>
        /// Provinces that reside in the Database.
        /// </summary>
        public DbSet<Provincia> Provincias { get; set; }

        /// <summary>
        /// Registers saved in the database.
        /// </summary>
        public DbSet<Register> Registers { get; set; }

        /// <summary>
        /// Stores saved in the database.
        /// </summary>
        public DbSet<Store> Stores { get; set; }

        /// <summary>
        /// Reviews saved in the database.
        /// </summary>
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Image> Images { get; set; }

        // TODO: May want to create a builder for each class
        /// <summary>
        /// Model builder for LoCoMProContext, ensures the initialization and configuration of each table.
        /// </summary>
        /// <param name="modelBuilder">Builder that the context will use.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Creating tables
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Product>().ToTable("Product");
            modelBuilder.Entity<Store>().ToTable("Store");
            modelBuilder.Entity<Register>().ToTable("Register");
            modelBuilder.Entity<Canton>().ToTable("Canton");
            modelBuilder.Entity<Provincia>().ToTable("Provincia");
            modelBuilder.Entity<Image>().ToTable("Image");
            modelBuilder.Entity<Review>().ToTable("Review");

            // Building relationships for Store
            modelBuilder.Entity<Store>()
                .HasOne(p => p.Location)
                .WithMany(e => e.Stores)
                .HasForeignKey(c => new { c.CantonName, c.ProvinciaName });

            // Building relationships for Register
            modelBuilder.Entity<Register>()
                .HasOne(p => p.Store)
                .WithMany(e => e.Registers)
                .HasForeignKey(c => new { c.StoreName, c.CantonName, c.ProvinciaName});

            // Sets 0 to NumCorrecions by default for Register
            modelBuilder.Entity<Register>()
                .Property(r => r.NumCorrections)
                .HasDefaultValue(0);


            // Building relationships for Register
            modelBuilder.Entity<User>()
                .HasOne(p => p.Location)
                .WithMany(e => e.Users)
                .HasForeignKey(c => new { c.CantonName, c.ProvinciaName })
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<Image>()
                .HasKey(i => new { i.ImageId, i.ContributorId, i.ProductName, i.StoreName, i.CantonName, i.ProvinceName, i.SubmitionDate });

            modelBuilder.Entity<Image>()
                .HasOne(i => i.Register)
                .WithMany(r => r.Images)
                .HasForeignKey(i => new { i.ContributorId, i.ProductName, i.StoreName, i.CantonName, i.ProvinceName, i.SubmitionDate })
                .OnDelete(DeleteBehavior.NoAction);

            // Building relationships for Register
            modelBuilder.Entity<User>()
                .HasMany(u => u.Registers)
                .WithOne(r => r.Contributor)
                .HasForeignKey(r => r.ContributorId);

            // Building relationships for Review
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasOne(l => l.Reviewer)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(e => e.ReviewerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(l => l.ReviewedRegister)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(e => new {e.ContributorId, e.ProductName, e.StoreName, e.CantonName, e.ProvinceName, e.SubmitionDate});
            });

            // Ignoring columns from default IdentityUser
            modelBuilder.Entity<User>().Ignore(u => u.PhoneNumber);
            modelBuilder.Entity<User>().Ignore(u => u.PhoneNumberConfirmed);
            modelBuilder.Entity<User>().Ignore(u => u.EmailConfirmed);
            modelBuilder.Entity<User>().Ignore(u => u.SecurityStamp);
            modelBuilder.Entity<User>().Ignore(u => u.ConcurrencyStamp);
            modelBuilder.Entity<User>().Ignore(u => u.TwoFactorEnabled);
            modelBuilder.Entity<User>().Ignore(u => u.LockoutEnabled);
            modelBuilder.Entity<User>().Ignore(u => u.LockoutEnd);
            modelBuilder.Entity<User>().Ignore(u => u.AccessFailedCount);

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

            modelBuilder.Entity<User>()
                .Navigation(u => u.Reviews)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            modelBuilder.Entity<Register>()
                .Navigation(u => u.Reviews)
                .UsePropertyAccessMode(PropertyAccessMode.Property);
        }
    }
}