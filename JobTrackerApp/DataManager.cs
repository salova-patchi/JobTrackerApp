using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;

namespace JobTrackerApp
{
    /// <summary>
    /// DataManager using Entity Framework Core with SQL Server
    /// Handles all CRUD operations for the Companies table
    /// </summary>
    public static class DataManager
    {
        /// <summary>
        /// Ensures the database exists and creates it if necessary
        /// Call this when your app starts
        /// </summary>
        public static void InitializeDatabase()
        {
            try
            {
                using (var context = new JobTrackerContext())
                {
                    // Create database if it doesn't exist
                    context.Database.EnsureCreated();

                    // Optional: Seed initial data
                    // context.SeedData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing database: {ex.Message}\n\nInner: {ex.InnerException?.Message}",
                    "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// GET ALL - Retrieves all companies from database
        /// EF Core translates this to: SELECT * FROM Companies
        /// </summary>
        public static List<Company> GetAllCompanies()
        {
            try
            {
                using (var context = new JobTrackerContext())
                {
                    // AsNoTracking() improves performance for read-only queries
                    return context.Companies
                        .AsNoTracking()
                        .OrderByDescending(c => c.DateAdded)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading companies: {ex.Message}", "Database Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<Company>();
            }
        }

        /// <summary>
        /// GET BY ID - Retrieves a single company by ID
        /// SQL: SELECT * FROM Companies WHERE Id = @id
        /// </summary>
        public static Company GetCompanyById(Guid id)
        {
            try
            {
                using (var context = new JobTrackerContext())
                {
                    return context.Companies
                        .AsNoTracking()
                        .FirstOrDefault(c => c.Id == id);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading company: {ex.Message}", "Database Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        /// <summary>
        /// CREATE - Adds a new company to database
        /// SQL: INSERT INTO Companies VALUES (...)
        /// </summary>
        public static void AddCompany(Company company)
        {
            try
            {
                using (var context = new JobTrackerContext())
                {
                    // Add the company to the context
                    context.Companies.Add(company);

                    // SaveChanges() executes the INSERT command
                    int rowsAffected = context.SaveChanges();

                    if (rowsAffected > 0)
                    {
                        // Success
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding company: {ex.Message}\n\nInner: {ex.InnerException?.Message}",
                    "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// UPDATE - Updates an existing company in database
        /// SQL: UPDATE Companies SET ... WHERE Id = @id
        /// </summary>
        public static void UpdateCompany(Company updatedCompany)
        {
            try
            {
                using (var context = new JobTrackerContext())
                {
                    // Find the existing company
                    var existing = context.Companies.Find(updatedCompany.Id);

                    if (existing != null)
                    {
                        // Update properties
                        existing.CompanyName = updatedCompany.CompanyName;
                        existing.Phone = updatedCompany.Phone;
                        existing.Email = updatedCompany.Email;
                        existing.Location = updatedCompany.Location;
                        existing.TechStack = updatedCompany.TechStack;
                        existing.Status = updatedCompany.Status;
                        existing.Remarks = updatedCompany.Remarks;
                        // DateAdded is NOT updated (keeps original value)

                        // SaveChanges() executes the UPDATE command
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating company: {ex.Message}\n\nInner: {ex.InnerException?.Message}",
                    "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// DELETE - Removes a company from database
        /// SQL: DELETE FROM Companies WHERE Id = @id
        /// </summary>
        public static void DeleteCompany(Guid id)
        {
            try
            {
                using (var context = new JobTrackerContext())
                {
                    // Find the company to delete
                    var company = context.Companies.Find(id);

                    if (company != null)
                    {
                        // Remove from context
                        context.Companies.Remove(company);

                        // SaveChanges() executes the DELETE command
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting company: {ex.Message}\n\nInner: {ex.InnerException?.Message}",
                    "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// SEARCH - Find companies by search term (searches multiple fields)
        /// SQL: SELECT * FROM Companies WHERE CompanyName LIKE '%term%' OR ...
        /// </summary>
        public static List<Company> SearchCompanies(string searchTerm)
        {
            try
            {
                using (var context = new JobTrackerContext())
                {
                    searchTerm = searchTerm.ToLower();

                    return context.Companies
                        .AsNoTracking()
                        .Where(c =>
                            c.CompanyName.ToLower().Contains(searchTerm) ||
                            c.TechStack.ToLower().Contains(searchTerm) ||
                            c.Location.ToLower().Contains(searchTerm) ||
                            c.Email.ToLower().Contains(searchTerm))
                        .OrderByDescending(c => c.DateAdded)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching companies: {ex.Message}", "Database Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<Company>();
            }
        }

        /// <summary>
        /// FILTER BY STATUS - Get companies with specific status
        /// SQL: SELECT * FROM Companies WHERE Status = @status
        /// </summary>
        public static List<Company> GetCompaniesByStatus(string status)
        {
            try
            {
                using (var context = new JobTrackerContext())
                {
                    return context.Companies
                        .AsNoTracking()
                        .Where(c => c.Status == status)
                        .OrderByDescending(c => c.DateAdded)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error filtering companies: {ex.Message}", "Database Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<Company>();
            }
        }

        /// <summary>
        /// GET STATISTICS - Get count of companies by status
        /// Returns dictionary with status as key and count as value
        /// </summary>
        public static Dictionary<string, int> GetStatistics()
        {
            try
            {
                using (var context = new JobTrackerContext())
                {
                    return context.Companies
                        .AsNoTracking()
                        .GroupBy(c => c.Status)
                        .ToDictionary(g => g.Key, g => g.Count());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting statistics: {ex.Message}", "Database Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return new Dictionary<string, int>();
            }
        }

        /// <summary>
        /// CHECK CONNECTION - Tests if database connection works
        /// </summary>
        public static bool TestConnection()
        {
            try
            {
                using (var context = new JobTrackerContext())
                {
                    return context.Database.CanConnect();
                }
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Company Entity - Represents a row in the Companies table
    /// Each property maps to a column in the database
    /// </summary>
    [Table("Companies")]
    public class Company
    {
        /// <summary>
        /// Primary Key - Unique identifier for each company
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // We generate Guid ourselves
        public Guid Id { get; set; }

        /// <summary>
        /// Company Name - Required field, max 200 characters
        /// </summary>
        [Required(ErrorMessage = "Company name is required")]
        [MaxLength(200, ErrorMessage = "Company name cannot exceed 200 characters")]
        public string CompanyName { get; set; }

        /// <summary>
        /// Phone Number - Optional, max 50 characters
        /// </summary>
        [MaxLength(50)]
        public string Phone { get; set; }

        /// <summary>
        /// Email Address - Optional, max 200 characters
        /// </summary>
        [MaxLength(200)]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        /// <summary>
        /// Location/Address - Optional, max 200 characters
        /// </summary>
        [MaxLength(200)]
        public string Location { get; set; }

        /// <summary>
        /// Technology Stack - Optional, max 500 characters
        /// </summary>
        [MaxLength(500)]
        public string TechStack { get; set; }

        /// <summary>
        /// Application Status - Required, max 50 characters
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Status { get; set; }

        /// <summary>
        /// Notes/Remarks - Optional, max 2000 characters
        /// </summary>
        [MaxLength(2000)]
        public string Remarks { get; set; }

        /// <summary>
        /// Date when company was added - Required
        /// </summary>
        [Required]
        public DateTime DateAdded { get; set; }
    }
}