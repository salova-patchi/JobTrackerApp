using System;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace JobTrackerApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Navigate to Dashboard by default
            NavigateTo(new DashboardPage());
            UpdateSidebarStats();
        }

        private void BtnNavDashboard_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(btnNavDashboard);
            NavigateTo(new DashboardPage());
        }

        private void BtnNavAddCompany_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(btnNavAddCompany);
            NavigateTo(new AddCompanyPage());
        }

        private void BtnNavViewAll_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(btnNavViewAll);
            NavigateTo(new ViewCompaniesPage());
        }

        private void BtnNavSettings_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(btnNavSettings);
            NavigateTo(new SettingsPage());
        }

        private void NavigateTo(Page page)
        {
            MainFrame.Navigate(page);
            UpdateSidebarStats();
        }

        private void SetActiveButton(Button activeButton)
        {
            // Reset all buttons to inactive style
            btnNavDashboard.Style = (Style)FindResource("NavButtonStyle");
            btnNavAddCompany.Style = (Style)FindResource("NavButtonStyle");
            btnNavViewAll.Style = (Style)FindResource("NavButtonStyle");
            btnNavSettings.Style = (Style)FindResource("NavButtonStyle");

            // Set active button style
            activeButton.Style = (Style)FindResource("ActiveNavButtonStyle");
        }

        public void UpdateSidebarStats()
        {
            var companies = DataManager.GetAllCompanies();
            txtSidebarTotal.Text = companies.Count.ToString();
            txtSidebarPending.Text = companies.Count(c => c.Status == "Not Called").ToString();
        }
    }
}