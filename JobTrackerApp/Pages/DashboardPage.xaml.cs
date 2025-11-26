using System.Linq;
using System.Windows.Controls;

namespace JobTrackerApp
{
    public partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            var companies = DataManager.GetAllCompanies();

            // Update statistics
            txtTotalCompanies.Text = companies.Count.ToString();
            txtNotCalled.Text = companies.Count(c => c.Status == "Not Called").ToString();
            txtInProgress.Text = companies.Count(c =>
                c.Status == "Called" || c.Status == "Interview Scheduled").ToString();
            txtOffers.Text = companies.Count(c => c.Status == "Offer Received").ToString();

            // Load recent companies (last 5)
            lstRecentCompanies.ItemsSource = companies
                .OrderByDescending(c => c.DateAdded)
                .Take(5)
                .ToList();
        }
    }
}