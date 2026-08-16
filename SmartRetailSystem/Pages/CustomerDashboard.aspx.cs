using SmartRetailSystem.Classes;
using SmartRetailSystem.Models;
using SmartRetailSystem.Services;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartRetailSystem
{
    public partial class CustomerDashboard : System.Web.UI.Page
    {
        // Service class එක initialize කිරීම
        CustomerDashboardService service = new CustomerDashboardService();

        protected void Page_Load(object sender, EventArgs e)
        {
            // 1. Session පරීක්ෂාව (User ලොග් වී ඇත්දැයි බැලීම)
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Customer")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // පළමු වරට පිටුව load වන විට දත්ත පූරණය කිරීම
                LoadCustomerDetails();
                RefreshDashboard();
            }
        }

        private void LoadCustomerDetails()
        {
            if (Session["Email"] != null)
            {
                string email = Session["Email"].ToString();
                Customer customer = service.GetCustomer(email);

                if (customer != null)
                {
                    // පාරිභෝගිකයාගේ විස්තර Session එකේ තබා ගැනීම (පසුව භාවිතයට)
                    Session["CustomerID"] = customer.CustomerID;
                    lblCustomerName.Text = customer.Name;
                    lblOutstandingBalance.Text = customer.OutstandingBalance.ToString("N2");
                }
            }
        }

        private void RefreshDashboard()
        {
            if (Session["CustomerID"] != null)
            {
                int customerId = Convert.ToInt32(Session["CustomerID"]);

                // Summary Cards සඳහා දත්ත ලබා ගැනීම
                lblTotalOrders.Text = service.GetOrderCount(customerId).ToString();
                lblPendingOrders.Text = service.GetOrderCount(customerId, "Pending").ToString();
                lblDeliveredOrders.Text = service.GetOrderCount(customerId, "Delivered").ToString();

                // Notifications ලබා ගැනීම
                LoadNotifications(customerId);
            }
        }

        private void LoadNotifications(int customerId)
        {
            List<Notification> notifications = service.GetNotifications(customerId);
            gvNotifications.DataSource = notifications;
            gvNotifications.DataBind();
        }

        protected void gvNotifications_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "MarkRead")
            {
                int notificationId = Convert.ToInt32(e.CommandArgument);
                service.MarkNotificationRead(notificationId);

                // නැවත දත්ත පූරණය කර GridView එක update කිරීම
                if (Session["CustomerID"] != null)
                {
                    LoadNotifications(Convert.ToInt32(Session["CustomerID"]));
                }
            }
        }
    }   
}