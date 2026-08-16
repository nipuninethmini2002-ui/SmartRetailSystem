using SmartRetailSystem.Models;
using SmartRetailSystem.Services;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace SmartRetailSystem
{
    public partial class DeliveryDashboard : System.Web.UI.Page
    {
        DeliveryService service = new DeliveryService();
        string connString = ConfigurationManager.ConnectionStrings["SmartRetailSystem"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
                Response.Redirect("Login.aspx");

            if (!IsPostBack)
                LoadDeliveries();
        }

        private void LoadDeliveries()
        {
            try
            {
                int deliveryPersonId = Convert.ToInt32(Session["UserID"]);
                var deliveries = service.GetDeliveries(deliveryPersonId);
                gvDeliveries.DataSource = deliveries;
                gvDeliveries.DataBind();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error loading deliveries: " + ex.Message;
                lblMessage.CssClass = "msg-label text-danger d-block text-center";
            }
        }

        protected void gvDeliveries_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlStatus = (DropDownList)e.Row.FindControl("ddlStatus");
                // Model එකේ Property එක 'Status' බව සහතික කරගන්න
                var dataItem = DataBinder.Eval(e.Row.DataItem, "Status");

                if (dataItem != null && ddlStatus != null)
                {
                    string status = dataItem.ToString();
                    if (ddlStatus.Items.FindByText(status) != null)
                    {
                        ddlStatus.ClearSelection();
                        ddlStatus.Items.FindByText(status).Selected = true;
                    }
                }
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;

                int deliveryId = Convert.ToInt32(btn.CommandArgument);
                DropDownList ddlStatus = (DropDownList)row.FindControl("ddlStatus");
                string newStatus = ddlStatus.SelectedValue;

                // කලින් ලියූ SQL Query වෙනුවට මෙය භාවිතා කරන්න
                service.UpdateDeliveryStatus(deliveryId, newStatus);

                lblMessage.Text = "<i class='fas fa-check-circle'></i> Status Updated Successfully!";
                lblMessage.CssClass = "msg-label text-success d-block text-center";

                LoadDeliveries(); // Grid එක refresh කිරීම
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
                lblMessage.CssClass = "msg-label text-danger d-block text-center";
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("Login.aspx");
        }
    }
}