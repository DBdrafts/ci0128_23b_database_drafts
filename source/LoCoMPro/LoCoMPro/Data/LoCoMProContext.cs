using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using LoCoMPro.Models;
using System.Reflection.Metadata;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NetTopologySuite.Geometries;
using Microsoft.Data.SqlClient;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using System.Collections;
using System.Data.Entity.Spatial;
using System.Xml;

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
        /// Search results view for search page
        /// </summary>
        private DbSet<SearchResult> SearchResults { get; set; }    

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

                entity.Navigation(u => u.Reviews)
                   .UsePropertyAccessMode(PropertyAccessMode.Property);

                entity.Navigation(u => u.Reports)
                    .UsePropertyAccessMode(PropertyAccessMode.Property);

                entity.Navigation(u => u.Images)
                    .UsePropertyAccessMode(PropertyAccessMode.Property);
            });


            // Building relationships for User
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.HasOne(p => p.Location)
                    .WithMany(e => e.Users)
                    .HasForeignKey(c => new { c.CantonName, c.ProvinciaName })
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasMany(u => u.Registers)
                .WithOne(r => r.Contributor)
                .HasForeignKey(r => r.ContributorId);

                // Ignoring columns from default IdentityUser
                entity.Ignore(u => u.PhoneNumber);
                entity.Ignore(u => u.PhoneNumberConfirmed);
                entity.Ignore(u => u.TwoFactorEnabled);
                entity.Ignore(u => u.LockoutEnabled);
                entity.Ignore(u => u.LockoutEnd);
                entity.Ignore(u => u.AccessFailedCount);


                // Navigation
                entity.Navigation(u => u.Registers)
                    .UsePropertyAccessMode(PropertyAccessMode.Property);

                entity.Navigation(u => u.Reviews)
                    .UsePropertyAccessMode(PropertyAccessMode.Property);

                entity.Navigation(u => u.Reports)
                    .UsePropertyAccessMode(PropertyAccessMode.Property);
            });

            // Building image Table
            modelBuilder.Entity<Image>(entity =>
            {
                entity.ToTable("Image");

                entity.HasKey(i => new { i.ImageId, i.ContributorId, i.ProductName, i.StoreName, i.CantonName, i.ProvinceName, i.SubmitionDate });

                entity.HasOne(i => i.Register)
                    .WithMany(r => r.Images)
                    .HasForeignKey(i => new { i.ContributorId, i.ProductName, i.StoreName, i.CantonName, i.ProvinceName, i.SubmitionDate })
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Building relationships for Review
            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable("Review");

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
                entity.ToTable("Report");

                entity.HasOne(l => l.Reporter)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(e => e.ReporterId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(l => l.ReportedRegister)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(e => new { e.ContributorId, e.ProductName, e.StoreName, e.CantonName, e.ProvinceName, e.SubmitionDate });
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

                entity.Property(e => e.Geolocation)
                    .HasColumnType("geography");
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

            // Building 'entity' SearchResult.
            modelBuilder.Entity<SearchResult>(entity => {
                entity.Property(e => e.Geolocation)
                    .HasColumnType("geography");

                entity.ToView("SearchResult");
            });
            
        }
           
        /// <summary>
        /// Excecutes provided SQL file in the database connected to context.
        /// </summary>
        /// <param name="scriptFilePath">Absolute path for file to excecute. Must be a valid one.</param>
        public void ExecuteSqlScriptFile(string scriptFilePath)
        {
            string script = File.ReadAllText(scriptFilePath);
            this.Database.ExecuteSqlRaw(script);
        }

        /// <summary>
        /// Gets search results as consequence of excecuting stored procedure: 'GetSearchResults'.
        /// </summary>
        /// <param name="searchType">Type of search to realize.</param>
        /// <param name="searchString">String to search for.</param>
        /// <param name="basePoint">Base location to calculate distance with. Default is coords(0, 0)</param>
        /// <returns>Search results for given query</returns>
        public IEnumerable<SearchResult> GetSearchResults(string searchType, string searchString, Point basePoint)
        {
            double latitude = basePoint.Y;
            double longitude = basePoint.X;
            var results = SearchResults.FromSqlRaw("EXEC GetSearchResults @searchType, @searchString, @latitude, @longitude",
                new SqlParameter("@searchType", searchType),
                new SqlParameter("@searchString", searchString),
                new SqlParameter("@latitude", latitude),
                new SqlParameter("@longitude", longitude));
            // Use FromSqlRaw to call the stored procedure
            return results;
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

            // Round to the closer multiple of 5
            float roundResult = RoundToCloserReal(result);

            // Return the value or 0 if the value was 0
            return roundResult;
        }

        /// <summary>
        /// Round a real number to the closer real number multiple of 0.5
        /// </summary>
        /// <param name="value">Real number to round.</param>
        /// <returns>Closer real number multiple of 0.5</returns>
        public float RoundToCloserReal(float? value)
        {
            float roundResult = 0;
            
            // If exist and is higher than 0
            if (value.HasValue && value > 0)
            {
                roundResult = (float)(Math.Round(value.Value * 2) / 2);
            }
            return roundResult;
        }
    }
}