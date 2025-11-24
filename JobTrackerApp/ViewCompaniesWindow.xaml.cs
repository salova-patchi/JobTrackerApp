using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace JobTrackerApp
{
    public partial class ViewCompaniesWindow : Window
    {
        private ObservableCollection<Company> filteredCompanies = new ObservableCollection<Company>();

        public ViewCompaniesWindow()
        {
            InitializeComponent();
            dgCompanies.ItemsSource = filteredCompanies;
            LoadData();
        }

        private void LoadData()
        {
            ApplyFilters();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void CmbFilterStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void CmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void DgCompanies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Selection handling if needed
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var company = button?.DataContext as Company;

            if (company != null)
            {
                var editWindow = new EditCompanyWindow(company);
                if (editWindow.ShowDialog() == true)
                {
                    DataManager.UpdateCompany(company);
                    ApplyFilters();
                    MessageBox.Show("✅ Company updated successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var company = button?.DataContext as Company;

            if (company != null)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete {company.CompanyName}?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    DataManager.DeleteCompany(company.Id);
                    ApplyFilters();
                    MessageBox.Show("🗑️ Company deleted successfully!", "Deleted",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void BtnAddNew_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.ShowDialog();
            ApplyFilters();
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                FileName = $"JobApplications_{DateTime.Now:yyyyMMdd}.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    ExportToCsv(saveFileDialog.FileName);
                    MessageBox.Show("📤 Data exported successfully!", "Export Complete",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting data: {ex.Message}", "Export Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ApplyFilters()
        {
            filteredCompanies.Clear();
            var companies = DataManager.GetAllCompanies();
            var filtered = companies.AsEnumerable();

            // Search filter
            if (txtSearch != null && !string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                string searchText = txtSearch.Text.ToLower();
                filtered = filtered.Where(c =>
                    c.CompanyName.ToLower().Contains(searchText) ||
                    c.TechStack.ToLower().Contains(searchText) ||
                    c.Location.ToLower().Contains(searchText) ||
                    c.Email.ToLower().Contains(searchText));
            }

            // Status filter
            if (cmbFilterStatus != null && cmbFilterStatus.SelectedItem != null)
            {
                var statusFilter = (cmbFilterStatus.SelectedItem as ComboBoxItem)?.Content.ToString();
                if (statusFilter != null && statusFilter != "All")
                {
                    filtered = filtered.Where(c => c.Status == statusFilter);
                }
            }

            // Sorting
            if (cmbSort != null && cmbSort.SelectedItem != null)
            {
                var sortOption = (cmbSort.SelectedItem as ComboBoxItem)?.Content.ToString();
                filtered = sortOption switch
                {
                    "Date Added (Newest)" => filtered.OrderByDescending(c => c.DateAdded),
                    "Date Added (Oldest)" => filtered.OrderBy(c => c.DateAdded),
                    "Company Name (A-Z)" => filtered.OrderBy(c => c.CompanyName),
                    "Company Name (Z-A)" => filtered.OrderByDescending(c => c.CompanyName),
                    _ => filtered.OrderByDescending(c => c.DateAdded)
                };
            }
            else
            {
                filtered = filtered.OrderByDescending(c => c.DateAdded);
            }

            foreach (var company in filtered)
            {
                filteredCompanies.Add(company);
            }
        }

        private void ExportToCsv(string filePath)
        {
            var companies = DataManager.GetAllCompanies();
            var csv = new StringBuilder();
            csv.AppendLine("Company Name,Phone,Email,Location,Tech Stack,Status,Remarks,Date Added");

            foreach (var company in companies)
            {
                csv.AppendLine($"\"{company.CompanyName}\",\"{company.Phone}\",\"{company.Email}\"," +
                              $"\"{company.Location}\",\"{company.TechStack}\",\"{company.Status}\"," +
                              $"\"{company.Remarks}\",\"{company.DateAdded:dd/MM/yyyy}\"");
            }

            System.IO.File.WriteAllText(filePath, csv.ToString());
        }
    }
}