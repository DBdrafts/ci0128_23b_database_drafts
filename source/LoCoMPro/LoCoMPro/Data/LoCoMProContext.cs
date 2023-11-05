using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using LoCoMPro.Models;
using System.Reflection.Metadata;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
        /// <summary>
        /// Images saved in the database.
        /// </summary>
        public DbSet<Image> Images { get; set; }

        /// <summary>
        /// Reports saved in the database.
        /// </summary>
        public DbSet<Report> Reports { get; set; }


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
            modelBuilder.Entity<Review>().ToTable("Review");
            modelBuilder.Entity<Report>().ToTable("Report");
            modelBuilder.Entity<Image>().ToTable("Image");


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
                    .HasForeignKey(e => new { e.ContributorId, e.ProductName, e.StoreName, e.CantonName, e.ProvinceName, e.SubmitionDate });
            });

            // Building relationships for Report
            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasOne(l => l.Reporter)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(e => e.ReporterId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(l => l.ReportedRegister)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(e => new { e.ContributorId, e.ProductName, e.StoreName, e.CantonName, e.ProvinceName, e.SubmitionDate });
            });


            // Ignoring columns from default IdentityUser
            modelBuilder.Entity<User>().Ignore(u => u.PhoneNumber);
            modelBuilder.Entity<User>().Ignore(u => u.PhoneNumberConfirmed);
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
            
            
            modelBuilder.Entity<User>()
                .Navigation(u => u.Reports)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            modelBuilder.Entity<Register>()
                .Navigation(u => u.Reports)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            modelBuilder.Entity<Register>()
                .Navigation(u => u.Images)
                .UsePropertyAccessMode(PropertyAccessMode.Property);
        }

        /// <summary>
        /// Gets the average review value of a register
        /// </summary>
        /// <param name="contributorId">Id of the user that make the register.</param>
        /// <param name="productName">Name of the product.</param>
        /// <param name="storeName">Name of the store.</param>
        /// <param name="submitionDate">Date the register was made.</param>
        /// <returns>Average review value of the register.</returns>
        public float GetAverageReviewValue(string contributorId
            , string productName, string storeName, DateTime submitionDate)
        {
            // Creates the SQL Query use to get the average review values
            string sqlQuery = "SELECT dbo.GetAverageReviewValue(@ContributorId, @ProductName, @StoreName, @SubmitionDate)";

            // Link the parameters with the register value
            var contributorIdParam = new SqlParameter("@ContributorId", contributorId);
            var productNameParam = new SqlParameter("@ProductName", productName);
            var storeNameParam = new SqlParameter("@StoreName", storeName);
            var submitionDateParam = new SqlParameter("@SubmitionDate", submitionDate);

            // Gets the average review value of the register
            float? result = Database.SqlQueryRaw<float?>(sqlQuery, contributorIdParam, productNameParam, storeNameParam, submitionDateParam)
                .AsEnumerable().SingleOrDefault();

            // Return the value or 0 if the value was 0
            return result ?? 0;
        }
    }
}