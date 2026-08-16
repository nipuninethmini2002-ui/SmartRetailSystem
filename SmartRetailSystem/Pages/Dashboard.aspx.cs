using System;
using System.Collections.Generic;
using SmartRetailSystem.Services;

namespace SmartRetailSystem
{
    public partial class Dashboard : System.Web.UI.Page
    {
        // Low stock භාණ්ඩ ලැයිස්තුව තබා ගැනීමට
        private List<int> LowStockProductIDs = new List<int>();
        DashboardService service = new DashboardService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Username"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
                LoadSummaryCards();
        }

        private void LoadSummaryCards()
        {
            try
            {
                lblTotalProducts.Text = service.GetTotalProducts().ToString();
                lblTotalOrders.Text = service.GetTotalOrders().ToString();
                lblPendingOrders.Text = service.GetPendingOrders().ToString();
                lblTotalCustomers.Text = service.GetTotalCustomers().ToString();
                lblOutstandingCredit.Text = service.GetTotalOutstandingCredit().ToString("F2");

                // Low Stock භාණ්ඩ ප්‍රමාණය ලබා ගැනීම
                LowStockProductIDs = service.GetLowStockProductIDs();
                int count = LowStockProductIDs.Count;
                lblLowStock.Text = count.ToString();

                // භාණ්ඩ අඩු නම් රතු පැහැයෙන් පෙන්වීමට (Smart Alert)
                if (count > 0)
                {
                    pnlLowStockAlert.Visible = true;
                    lnkViewLowStock.Text = $"⚠️ Attention: {count} products are running low on stock!";
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error: " + ex.Message + "');</script>");
            }
        }

        protected void btnManageProducts_Click(object sender, EventArgs e)
        {
            // සාමාන්‍යයෙන් Product List එකට යෑම
            Response.Redirect("ProductList.aspx");
        }

        // Low Stock Card එකක් ලෙස ක්‍රියා කිරීමට හෝ වෙනම බොත්තමක් ලෙස
        protected void btnViewLowStock_Click(object sender, EventArgs e)
        {
            LowStockProductIDs = service.GetLowStockProductIDs();
            if (LowStockProductIDs.Count > 0)
            {
                string ids = string.Join(",", LowStockProductIDs);
                Response.Redirect("ProductList.aspx?lowstock=" + ids);
            }
            else
            {
                Response.Redirect("ProductList.aspx");
            }
        }
    }
}