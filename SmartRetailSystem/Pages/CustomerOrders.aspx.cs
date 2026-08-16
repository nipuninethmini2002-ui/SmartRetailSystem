using SmartRetailSystem.Classes;
using SmartRetailSystem.Models;
using SmartRetailSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartRetailSystem
{
    public partial class CustomerOrders : Page
    {
        private OrderService orderService = new OrderService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Username"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                if (Session["CustomerID"] == null) LoadCustomerId();
                LoadOrders(); // මුලින්ම සියල්ල පෙන්වයි
            }
        }

        private void LoadCustomerId()
        {
            if (Session["Email"] == null) return;
            CustomerService customerService = new CustomerService();
            Customer customer = customerService.GetCustomerByEmail(Session["Email"].ToString());
            if (customer != null) Session["CustomerID"] = customer.CustomerID;
        }

        // Filter අගය අනුව දත්ත ගෙන ඒම
        private void LoadOrders(string filterStatus = "All")
        {
            if (Session["CustomerID"] != null)
            {
                int currentCustId = Convert.ToInt32(Session["CustomerID"]);
                List<Order> orders = orderService.GetCustomerOrders(currentCustId);

                // C# LINQ භාවිතා කර දත්ත Filter කිරීම
                if (filterStatus != "All")
                {
                    orders = orders.Where(o => o.DeliveryStatus.Equals(filterStatus, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                gvOrders.DataSource = orders;
                gvOrders.DataBind();

                if (orders.Count == 0)
                {
                    lblMessage.Text = "No orders found for the selected status.";
                    lblMessage.CssClass = "alert alert-warning";
                }
                else
                {
                    lblMessage.Text = "";
                }
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            string selectedStatus = ddlStatusFilter.SelectedValue;
            LoadOrders(selectedStatus);
            pnlOrderDetails.Visible = false; // Filter කරන විට විස්තර පැනලය වසා දමයි
            gvOrders.Visible = true;
        }

        protected void gvOrders_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string orderId = e.CommandArgument.ToString();

            if (e.CommandName == "ViewOrder")
            {
                int id = Convert.ToInt32(orderId);
                var order = orderService.GetOrderDetails(id);

                if (order != null)
                {
                    // මූලික තොරතුරු
                    litOrderID.Text = order.OrderID.ToString();
                    lblOrderID.Text = order.OrderID.ToString();
                    lblOrderDate.Text = order.OrderDate.ToString("dd/MM/yyyy");

                    // --- මුදල් අගයන් (Summary Section) ---
                    lblSubTotal.Text = order.SubTotal.ToString("N2");

                    // 1. ඔබේ Model එකේ නම TaxAmount නම් මෙය භාවිතා කරන්න
                    lblTaxAmount.Text = order.Tax.ToString("N2");

                    // 2. ඉහළ ඇති ලේබලය (lblGrandTotal)
                    lblGrandTotal.Text = "Rs. " + order.GrandTotal.ToString("N2");

                    // 3. පහළ Summary Section එකේ ලේබලය (මෙය අනිවාර්යයෙන්ම අවශ්‍යයි)
                    lblGrandTotalSummary.Text = order.GrandTotal.ToString("N2");

                    lblPaymentStatus.Text = order.PaymentStatus;
                    lblDeliveryStatus.Text = order.DeliveryStatus;

                    // බෙදාහැරීමේ තොරතුරු
                    if (order.DeliveryInfo != null)
                    {
                        lblDeliveryPerson.Text = order.DeliveryInfo.DeliveryPersonName ?? "Not Assigned";
                        lblAssignedAt.Text = order.DeliveryInfo.AssignedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-";
                        lblDeliveredAt.Text = order.DeliveryInfo.DeliveredAt?.ToString("dd/MM/yyyy HH:mm") ?? "-";
                    }

                    // GridView එකට දත්ත ලබා දීම
                    gvOrderItems.DataSource = order.Items;
                    gvOrderItems.DataBind();

                    // පැනල් පාලනය
                    pnlOrderDetails.Visible = true;
                    gvOrders.Visible = false;
                }
            }
            else if (e.CommandName == "ViewInvoice")
            {
                Response.Redirect("Invoice.aspx?OrderID=" + orderId);
            }
        }

        protected void btnCloseDetails_Click(object sender, EventArgs e)
        {
            pnlOrderDetails.Visible = false;
            gvOrders.Visible = true;
        }
    }
}