using System.Windows;
using System.Windows.Controls;

namespace JobTrackerApp
{
    public partial class EditCompanyWindow : Window
    {
        private Company company;

        public EditCompanyWindow(Company companyToEdit)
        {
            InitializeComponent();
            company = companyToEdit;
            LoadCompanyData();
        }

        private void LoadCompanyData()
        {
            txtCompanyName.Text = company.CompanyName;
            txtPhone.Text = company.Phone;
            txtEmail.Text = company.Email;
            txtLocation.Text = company.Location;
            txtTechStack.Text = company.TechStack;
            txtRemarks.Text = company.Remarks;

            // Set status combobox
            foreach (ComboBoxItem item in cmbStatus.Items)
            {
                if (item.Content.ToString() == company.Status)
                {
                    cmbStatus.SelectedItem = item;
                    break;
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCompanyName.Text))
            {
                MessageBox.Show("Company name is required!", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Update company object
            company.CompanyName = txtCompanyName.Text.Trim();
            company.Phone = txtPhone.Text.Trim();
            company.Email = txtEmail.Text.Trim();
            company.Location = txtLocation.Text.Trim();
            company.TechStack = txtTechStack.Text.Trim();
            company.Status = (cmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Not Called";
            company.Remarks = txtRemarks.Text.Trim();

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}