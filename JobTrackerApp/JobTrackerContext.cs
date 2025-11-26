using Microsoft.EntityFrameworkCore;
using System;

namespace JobTrackerApp
{
    /// <summary>
    /// Database Context for Entity Framework Core
    /// This manages the connection to SQL Server and defines your database schema
    /// </summary>
    public class JobTrackerContext : DbContext
    {
        // DbSet represents the "Companies" table in your database
        public DbSet<Company> Companies { get; set; }

        /// <summary>
        /// Configures the database connection
        /// This method is called by EF Core to set up the connection
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Connection string -
                optionsBuilder.UseSqlServer(
                    @"Server=Sma3il-Za3im\SQLEXPRESS;Database=JobTrackerDb;Trusted_Connection=True;TrustServerCertificate=True;"
                );

                // Optional: Enable sensitive data logging (useful for debugging)
                optionsBuilder.EnableSensitiveDataLogging();

                // Optional: Log SQL queries to debug output
                optionsBuilder.LogTo(Console.WriteLine);
            }
        }

        /// <summary>
        /// Configures the database model (table structure, relationships, constraints)
        /// This is called when EF Core creates or updates the database
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the Companies table
            modelBuilder.Entity<Company>(entity =>
            {
                // Table name
                entity.ToTable("Companies");

                // Primary Key
                entity.HasKey(e => e.Id);

                // Column configurations
                entity.Property(e => e.Id)
                    .HasColumnName("Id")
                    .IsRequired();

                entity.Property(e => e.CompanyName)
                    .HasColumnName("CompanyName")
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(e => e.Phone)
                    .HasColumnName("Phone")
                    .HasMaxLength(50);

                entity.Property(e => e.Email)
                    .HasColumnName("Email")
                    .HasMaxLength(200);

                entity.Property(e => e.Location)
                    .HasColumnName("Location")
                    .HasMaxLength(200);

                entity.Property(e => e.TechStack)
                    .HasColumnName("TechStack")
                    .HasMaxLength(500);

                entity.Property(e => e.Status)
                    .HasColumnName("Status")
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.Remarks)
                    .HasColumnName("Remarks")
                    .HasMaxLength(2000);

                entity.Property(e => e.DateAdded)
                    .HasColumnName("DateAdded")
                    .IsRequired();

                // Create index on CompanyName for faster searches
                entity.HasIndex(e => e.CompanyName)
                    .HasDatabaseName("IX_Companies_CompanyName");

                // Create index on Status for faster filtering
                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("IX_Companies_Status");

                // Create index on DateAdded for faster sorting
                entity.HasIndex(e => e.DateAdded)
                    .HasDatabaseName("IX_Companies_DateAdded");
            });

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Seeds initial data (optional - runs when database is created)
        /// </summary>
        public void SeedData()
        {
            // Check if database is empty
            if (!Companies.Any())
            {
                // Add sample companies (optional)
                var sampleCompanies = new[]
                {
                    new Company
                    {
                        Id = Guid.NewGuid(),
                        CompanyName = "Microsoft",
                        Email = "careers@microsoft.com",
                        Location = "Redmond, WA",
                        TechStack = "C#, .NET, Azure, TypeScript",
                        Status = "Not Called",
                        Remarks = "Leading tech company",
                        DateAdded = DateTime.Now
                    },
                    new Company
                    {
                        Id = Guid.NewGuid(),
                        CompanyName = "Google",
                        Email = "jobs@google.com",
                        Location = "Mountain View, CA",
                        TechStack = "Java, Python, Go, Kubernetes",
                        Status = "Not Called",
                        Remarks = "Search engine giant",
                        DateAdded = DateTime.Now
                    }
                };

                // Uncomment to add sample data:
                // Companies.AddRange(sampleCompanies);
                // SaveChanges();
            }
        }
    }
}