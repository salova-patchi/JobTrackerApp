using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace JobTrackerApp
{
    public static class DataManager
    {
        private static List<Company> companies = new List<Company>();
        private const string DATA_FILE = "companies.dat";

        static DataManager()
        {
            LoadData();
        }

        public static List<Company> GetAllCompanies()
        {
            return new List<Company>(companies);
        }

        public static void AddCompany(Company company)
        {
            companies.Add(company);
            SaveData();
        }

        public static void UpdateCompany(Company updatedCompany)
        {
            var existing = companies.FirstOrDefault(c => c.Id == updatedCompany.Id);
            if (existing != null)
            {
                int index = companies.IndexOf(existing);
                companies[index] = updatedCompany;
                SaveData();
            }
        }

        public static void DeleteCompany(Guid id)
        {
            var company = companies.FirstOrDefault(c => c.Id == id);
            if (company != null)
            {
                companies.Remove(company);
                SaveData();
            }
        }

        private static void SaveData()
        {
            try
            {
                var lines = new List<string>();
                foreach (var company in companies)
                {
                    lines.Add($"{company.Id}|{company.CompanyName}|{company.Phone}|{company.Email}|" +
                             $"{company.Location}|{company.TechStack}|{company.Status}|" +
                             $"{company.Remarks}|{company.DateAdded:O}");
                }
                File.WriteAllLines(DATA_FILE, lines);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void LoadData()
        {
            try
            {
                if (File.Exists(DATA_FILE))
                {
                    var lines = File.ReadAllLines(DATA_FILE);
                    companies.Clear();

                    foreach (var line in lines)
                    {
                        var parts = line.Split('|');
                        if (parts.Length == 9)
                        {
                            companies.Add(new Company
                            {
                                Id = Guid.Parse(parts[0]),
                                CompanyName = parts[1],
                                Phone = parts[2],
                                Email = parts[3],
                                Location = parts[4],
                                TechStack = parts[5],
                                Status = parts[6],
                                Remarks = parts[7],
                                DateAdded = DateTime.Parse(parts[8])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class Company
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Location { get; set; }
        public string TechStack { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public DateTime DateAdded { get; set; }
    }
}