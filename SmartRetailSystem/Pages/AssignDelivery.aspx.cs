using SmartRetailSystem.Models;
using SmartRetailSystem.Services;
using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartRetailSystem
{
    public partial class AssignDelivery : Page
    {
        private DeliveryService deliveryService = new DeliveryService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserRole"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

                if (!IsPostBack)
            {
                LoadDeliveryPersons();
                LoadPendingOrders();
            }
        }

        private void LoadDeliveryPersons()
        {
            var persons = deliveryService.GetDeliveryPersons();
            ddlDeliveryPerson.DataSource = persons;
            ddlDeliveryPerson.DataTextField = "Username";
            ddlDeliveryPerson.DataValueField = "UserID";
            ddlDeliveryPerson.DataBind();
        }

        private void LoadPendingOrders()
        {
            gvOrders.DataSource = deliveryService.GetPendingOrders();
            gvOrders.DataBind();
        }

        protected void btnAssign_Click(object sender, EventArgs e)
        {
            int orderId;
            int deliveryPersonId;

            if (!int.TryParse(txtOrderID.Text.Trim(), out orderId))
            {
                lblMsg.Text = "❌ Invalid Order ID";
                return;
            }

            if (!int.TryParse(ddlDeliveryPerson.SelectedValue, out deliveryPersonId))
            {
                lblMsg.Text = "❌ Invalid Delivery Person";
                return;
            }

            bool success = deliveryService.AssignDelivery(orderId, deliveryPersonId);
            lblMsg.Text = success ? "✅ Delivery Assigned Successfully" : "❌ Order not found or cannot be assigned";

            LoadPendingOrders();
        }
    }
}