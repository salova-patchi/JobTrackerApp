using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace JobTrackerApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            UpdateStatistics();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCompanyName.Text))
            {
                MessageBox.Show("Company name is required!", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var company = new Company
            {
                Id = Guid.NewGuid(),
                CompanyName = txtCompanyName.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Location = txtLocation.Text.Trim(),
                TechStack = txtTechStack.Text.Trim(),
                Status = (cmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Not Called",
                Remarks = txtRemarks.Text.Trim(),
                DateAdded = DateTime.Now
            };

            DataManager.AddCompany(company);
            UpdateStatistics();
            ClearForm();

            AnimateButton(btnAdd);
            MessageBox.Show($"✅ {company.CompanyName} added successfully!", "Success",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void BtnViewAll_Click(object sender, RoutedEventArgs e)
        {
            var viewWindow = new ViewCompaniesWindow();
            viewWindow.ShowDialog();
            UpdateStatistics();
        }

        private void ClearForm()
        {
            txtCompanyName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            txtLocation.Clear();
            txtTechStack.Clear();
            txtRemarks.Clear();
            cmbStatus.SelectedIndex = 0;
        }

        private void UpdateStatistics()
        {
            var companies = DataManager.GetAllCompanies();

            if (txtTotalCompanies != null)
                txtTotalCompanies.Text = companies.Count.ToString();

            if (txtNotCalled != null)
                txtNotCalled.Text = companies.Count(c => c.Status == "Not Called").ToString();

            if (txtCalled != null)
                txtCalled.Text = companies.Count(c =>
                    c.Status == "Called" || c.Status == "Interview Scheduled").ToString();

            if (txtOffers != null)
                txtOffers.Text = companies.Count(c => c.Status == "Offer Received").ToString();
        }

        private void AnimateButton(Button button)
        {
            var scaleTransform = new System.Windows.Media.ScaleTransform(1, 1);
            button.RenderTransform = scaleTransform;
            button.RenderTransformOrigin = new Point(0.5, 0.5);

            var animation = new DoubleAnimation
            {
                From = 1,
                To = 0.95,
                Duration = TimeSpan.FromMilliseconds(100),
                AutoReverse = true
            };

            scaleTransform.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleXProperty, animation);
            scaleTransform.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleYProperty, animation);
        }
    }
}