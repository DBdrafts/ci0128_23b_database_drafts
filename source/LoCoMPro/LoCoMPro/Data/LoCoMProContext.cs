using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using LoCoMPro.Models;
using NetTopologySuite.Geometries;
using Microsoft.Data.SqlClient;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using System.Collections;

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

        public DbSet<SearchResult> SearchResults { get; set; }

        // TODO: May want to create a builder for each class
        /// <summary>
        /// Model builder for LoCoMProContext, ensures the initialization and configuration of each table.
        /// </summary>
        /// <param name="modelBuilder">Builder that the context will use.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Building relationships for Store
            modelBuilder.Entity<Store>(entity =>
            {
                entity.ToTable("Store");

                entity.HasOne(p => p.Location)
                    .WithMany(e => e.Stores)
                    .HasForeignKey(c => new { c.CantonName, c.ProvinciaName });

                entity.HasMany(s => s.Products)
                    .WithMany(p => p.Stores)
                    .UsingEntity(
                        "Sells"
                        , l => l.HasOne(typeof(Product)).WithMany().HasForeignKey("ProductName")
                        , r => r.HasOne(typeof(Store)).WithMany().HasForeignKey("StoreName", "CantonName", "ProvinceName"));

                entity.Navigation(s => s.Products)
                    .UsePropertyAccessMode(PropertyAccessMode.Property);

                entity.Navigation(s => s.Registers)
                    .UsePropertyAccessMode(PropertyAccessMode.Property);
            });


            // Building relationships for Register
            modelBuilder.Entity<Register>(entity =>
            {
                entity.ToTable("Register");

                entity.HasOne(p => p.Store)
                    .WithMany(e => e.Registers)
                    .HasForeignKey(c => new { c.StoreName, c.CantonName, c.ProvinciaName });

                // Sets 0 to NumCorrecions by default for Register
                entity.Property(r => r.NumCorrections)
                    .HasDefaultValue(0);


            });
                

            // Building relationships for user
            modelBuilder.Entity<User>(entity => {
                entity.ToTable("User");

                entity.HasOne(p => p.Location)
                    .WithMany(e => e.Users)
                    .HasForeignKey(c => new { c.CantonName, c.ProvinciaName });

                // Ignoring columns from default IdentityUser
                entity.Ignore(u => u.PhoneNumber);
                entity.Ignore(u => u.PhoneNumberConfirmed);
                entity.Ignore(u => u.EmailConfirmed);
                entity.Ignore(u => u.SecurityStamp);
                entity.Ignore(u => u.ConcurrencyStamp);
                entity.Ignore(u => u.TwoFactorEnabled);
                entity.Ignore(u => u.LockoutEnabled);
                entity.Ignore(u => u.LockoutEnd);
                entity.Ignore(u => u.AccessFailedCount);

                // Navigation
                entity.Navigation(u => u.Registers)
                    .UsePropertyAccessMode(PropertyAccessMode.Property);
            });

            // Building relationship for Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Navigation(p => p.Stores)
                    .UsePropertyAccessMode(PropertyAccessMode.Property);

                entity.Navigation(p => p.Registers)
                    .UsePropertyAccessMode(PropertyAccessMode.Property);

                entity.Navigation(p => p.Categories)
                    .UsePropertyAccessMode(PropertyAccessMode.Property);
            });

            // Building relationships for Canton
            modelBuilder.Entity<Canton>(entity =>
            {
                entity.ToTable("Canton");

                entity.Navigation(c => c.Stores)
                .UsePropertyAccessMode(PropertyAccessMode.Property);
            });

            // Building relationships for Province
            modelBuilder.Entity<Provincia>(entity =>
            {
                entity.ToTable("Provincia");

                entity.Navigation(p => p.Cantones)
                    .UsePropertyAccessMode(PropertyAccessMode.Property);
            });

            // Building relationships for Categoty

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.HasMany(p => p.Products)
                    .WithMany(c => c.Categories)
                    .UsingEntity(
                        "AsociatedWith"
                        , l => l.HasOne(typeof(Product)).WithMany().HasForeignKey("ProductName")
                        , r => r.HasOne(typeof(Category)).WithMany().HasForeignKey("CategoryName"));

                entity.Navigation(c => c.Products)
                    .UsePropertyAccessMode(PropertyAccessMode.Property);
            });

            modelBuilder.Entity<SearchResult>().ToView("SearchResult");
        }

        public void ExecuteSqlScriptFile(string scriptFilePath)
        {
            string script = File.ReadAllText(scriptFilePath);
            this.Database.ExecuteSqlRaw(script);
        }

        public IEnumerable<SearchResult> GetSearchResults(string searchType, string searchString, Point basePoint)
        {
            // Use FromSqlRaw to call the stored procedure
            return SearchResults.FromSqlRaw("EXEC GetSearchResults @searchType, @searchString",//, @basePoint",
                    new SqlParameter("@searchType", searchType),
                    new SqlParameter("@searchString", searchString)/*,
                    new SqlParameter("@basePoint", basePoint)*/);
        }

    }
}