using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartRetailSystem
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 1. මුලින්ම පර්මිෂන් සහ මෙනු සකස් කරන්න
                ApplyRolePermissions();
                HighlightCurrentPage();

                if (Session["Role"] != null)
                {
                    string role = Session["Role"].ToString();
                    lblUserRole.Text = role; // Badge එකේ Role එක පෙන්වීමට

                    // Menu පාලනය
                    phAdminMenu.Visible = (role == "Admin" || role == "Staff");
                    phCustomerMenu.Visible = (role == "Customer");
                    phDeliveryMenu.Visible = (role == "Delivery");
                }
                // 2. ලොග් වී සිටින පරිශීලකයා අනුව Overview ලින්ක් එක සකස් කරන්න
                // සටහන: මෙහිදී Session["UserRole"] ලෙස ඒකාකාරීව පාවිච්චි කර ඇත
                string userRole = Session["UserRole"]?.ToString();

                if (!string.IsNullOrEmpty(userRole))
                {
                    if (userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                    {
                        lnkOverview.HRef = ResolveUrl("~/Pages/Dashboard.aspx");
                    }
                    else if (userRole.Equals("Staff", StringComparison.OrdinalIgnoreCase))
                    {
                        lnkOverview.HRef = ResolveUrl("~/Pages/StaffDashboard.aspx");
                    }
                }
            }
        }
        private void HighlightCurrentPage()
        {
            string activePage = Request.RawUrl.ToLower();

            // 1. Admin/Staff Highlights
            if (activePage.Contains("dashboard.aspx") || activePage.Contains("staffdashboard.aspx"))
                lnkOverview.Attributes.Add("class", "active");
            else if (activePage.Contains("productlist.aspx"))
                lnkInventory.Attributes.Add("class", "active");
            else if (activePage.Contains("customerlist.aspx"))
                lnkCustomers.Attributes.Add("class", "active");
            else if (activePage.Contains("adminpayment.aspx"))
                lnkBilling.Attributes.Add("class", "active");
            else if (activePage.Contains("assigndelivery.aspx"))
                lnkLogistics.Attributes.Add("class", "active");
            else if (activePage.Contains("creditcontrol.aspx"))
                lnkFinance.Attributes.Add("class", "active");
            else if (activePage.Contains("reports.aspx"))
                lnkReports.Attributes.Add("class", "active");

            // 2. Sales (Admin/Customer දෙන්නාටම පොදුයි)
            if (activePage.Contains("createorder.aspx"))
            {
                if (phAdminMenu.Visible) lnkSales.Attributes.Add("class", "active");
                if (phCustomerMenu.Visible) lnkCustOrder.Attributes.Add("class", "active");
            }

            // 3. Customer Highlights
            else if (activePage.Contains("customerdashboard.aspx"))
                lnkCustHome.Attributes.Add("class", "active");
            else if (activePage.Contains("customerorders.aspx"))
                lnkCustTrack.Attributes.Add("class", "active");

            // 4. Delivery Highlights
            else if (activePage.Contains("deliverydashboard.aspx"))
                lnkDelivery.Attributes.Add("class", "active");
        }

        private void ApplyRolePermissions()
        {
            // ආරම්භයේදී සියලුම මෙනු සහ Logout බොත්තම සඟවන්න
            phAdminMenu.Visible = false;
            phCustomerMenu.Visible = false;
            phDeliveryMenu.Visible = false;
            lnkLogout.Visible = false;

            lblUserRole.Text = "Guest Mode";

            // Session එක පරීක්ෂා කිරීම
            if (Session["UserRole"] != null)
            {
                string role = Session["UserRole"].ToString();
                lblUserRole.Text = role + " Mode";
                lnkLogout.Visible = true;

                // Admin සහ Staff දෙදෙනාටම Admin Menu එක පෙන්වන්න
                if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                    role.Equals("Staff", StringComparison.OrdinalIgnoreCase))
                {
                    phAdminMenu.Visible = true;

                    // Admin ද නැද්ද යන්න මත පදනම්ව විශේෂිත ලින්ක් පෙන්වීම/සැඟවීම
                    bool isAdmin = role.Equals("Admin", StringComparison.OrdinalIgnoreCase);

                    if (lnkReports != null) lnkReports.Visible = isAdmin;
                    if (lnkCustomers != null) lnkCustomers.Visible = isAdmin;
                }
                else if (role.Equals("Customer", StringComparison.OrdinalIgnoreCase))
                {
                    phCustomerMenu.Visible = true;
                }
                else if (role.Equals("Delivery", StringComparison.OrdinalIgnoreCase))
                {
                    phDeliveryMenu.Visible = true;
                }
            }
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            // Session එක සම්පූර්ණයෙන් ඉවත් කරන්න
            Session.Clear();
            Session.Abandon();

            // Logout වූ පසු Login පිටුවට යැවීම වඩාත් ආරක්ෂිතයි
            Response.Redirect(ResolveUrl("~/Pages/login.aspx"));
        }
    }
}