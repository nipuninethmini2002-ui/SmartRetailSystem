using System;
using SmartRetailSystem.Services;

namespace SmartRetailSystem
{
    public partial class StaffDashboard : System.Web.UI.Page
    {
        StaffService service = new StaffService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadSummaryCards();
        }

        private void LoadSummaryCards()
        {
            var summary = service.GetSummary();
            lblTotalProducts.Text = summary.TotalProducts.ToString();
            lblTotalOrders.Text = summary.TotalOrders.ToString();
            lblPendingOrders.Text = summary.PendingOrders.ToString();
            lblTotalCustomers.Text = summary.TotalCustomers.ToString();
            lblOutstandingCredit.Text = summary.OutstandingCredit.ToString("F2");
            lblLowStock.Text = summary.LowStock.ToString();
        }
    }
}