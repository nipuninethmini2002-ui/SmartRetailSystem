using SmartRetailSystem.Models;
using SmartRetailSystem.Services;
using SmartRetailSystem.DAL; // Outstanding balance update කිරීමට අවශ්‍ය වේ
using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartRetailSystem
{
    public partial class AdminPayment : System.Web.UI.Page
    {
        private PaymentService paymentService = new PaymentService();
        string connStr = ConfigurationManager.ConnectionStrings["SmartRetailSystem"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtDate.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
                LoadPayments();
            }
        }

        private void LoadPayments()
        {
            try
            {
                gvPayments.DataSource = paymentService.GetAllPayments();
                gvPayments.DataBind();
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading payments: " + ex.Message, System.Drawing.Color.Red);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            int orderId;
            if (int.TryParse(txtOrderID.Text.Trim(), out orderId))
            {
                DataTable results = paymentService.GetPaymentsByOrder(orderId);
                gvPayments.DataSource = results;
                gvPayments.DataBind();

                if (results == null || results.Rows.Count == 0)
                {
                    ShowMessage("No payments found for Order ID: " + orderId, System.Drawing.Color.OrangeRed);
                }
                else
                {
                    ShowMessage("Search results loaded.", System.Drawing.Color.Blue);
                }
            }
            else
            {
                ShowMessage("Please enter a valid Order ID.", System.Drawing.Color.Red);
            }
        }

        protected void gvPayments_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int paymentId = Convert.ToInt32(gvPayments.SelectedDataKey.Value);
                PaymentModel pay = paymentService.GetPaymentById(paymentId);

                if (pay != null)
                {
                    txtOrderID.Text = pay.OrderID.ToString();
                    txtAmount.Text = pay.PaidAmount.ToString("F2");
                    ddlMethod.SelectedValue = pay.PaymentMethod;

                    if (ddlStatus.Items.FindByValue(pay.PaymentStatus) != null)
                        ddlStatus.SelectedValue = pay.PaymentStatus;

                    txtDate.Text = pay.PaymentDate.ToString("yyyy-MM-ddTHH:mm");
                    ShowMessage("Record loaded successfully.", System.Drawing.Color.Blue);
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error selecting record: " + ex.Message, System.Drawing.Color.Red);
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (gvPayments.SelectedIndex < 0)
            {
                ShowMessage("Please select a payment from the list first.", System.Drawing.Color.Red);
                return;
            }

            try
            {
                int paymentId = Convert.ToInt32(gvPayments.SelectedDataKey.Value);
                var existingPay = paymentService.GetPaymentById(paymentId);

                if (existingPay != null)
                {
                    PaymentModel pay = new PaymentModel
                    {
                        PaymentID = paymentId,
                        OrderID = Convert.ToInt32(txtOrderID.Text),
                        CustomerID = existingPay.CustomerID,
                        PaidAmount = Convert.ToDecimal(txtAmount.Text),
                        PaymentMethod = ddlMethod.SelectedValue,
                        PaymentStatus = ddlStatus.SelectedValue,
                        PaymentDate = DateTime.Parse(txtDate.Text)
                    };

                    // 1. Payment සහ Order Status Update කිරීම
                    bool success = paymentService.UpdatePayment(pay);

                    if (success)
                    {
                        // 2. Outstanding Balance එක Synchronize කිරීම (වැදගත්ම කොටස)
                        // Paid -> Pending වූ විට එකතු වීම සහ Pending -> Paid වූ විට අඩු වීම මෙයින් සිදුවේ.
                        SyncCustomerBalances(existingPay.CustomerID);

                        ShowMessage("Payment and Status updated. Outstanding balance synchronized!", System.Drawing.Color.Green);
                        LoadPayments();
                        ClearForm();
                    }
                    else
                    {
                        ShowMessage("Update failed. Please check your data.", System.Drawing.Color.Red);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Update failed: " + ex.Message, System.Drawing.Color.Red);
            }
        }

        // වගු දෙකෙහිම අගයන් සමාන කිරීමට භාවිතා කරන Helper Method එක
        private void SyncCustomerBalances(int customerId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                string syncSql = @"
                    DECLARE @ActualTotal DECIMAL(18,2);
                    
                    -- Orders table එකෙන් වත්මන් Pending orders වල එකතුව ගැනීම
                    SELECT @ActualTotal = ISNULL(SUM(GrandTotal), 0) 
                    FROM Orders 
                    WHERE CustomerID = @cid AND PaymentStatus = 'Pending';

                    -- CreditAccounts table එක Update කිරීම
                    UPDATE CreditAccounts 
                    SET OutstandingBalance = @ActualTotal 
                    WHERE CustomerID = @cid;

                    -- Customers table එක Update කිරීම
                    UPDATE Customers 
                    SET OutstandingBalance = @ActualTotal 
                    WHERE CustomerID = @cid;";

                using (SqlCommand cmd = new SqlCommand(syncSql, con))
                {
                    cmd.Parameters.AddWithValue("@cid", customerId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (gvPayments.SelectedIndex < 0)
            {
                ShowMessage("Please select a record to delete.", System.Drawing.Color.Red);
                return;
            }

            try
            {
                int paymentId = Convert.ToInt32(gvPayments.SelectedDataKey.Value);
                var existingPay = paymentService.GetPaymentById(paymentId);
                int customerId = existingPay.CustomerID;

                paymentService.DeletePaymentAndOrder(paymentId);

                // Delete කළ පසුත් balance එක නැවත සකස් කළ යුතුය
                SyncCustomerBalances(customerId);

                ShowMessage("Record deleted and balance recalculated.", System.Drawing.Color.DarkRed);
                ClearForm();
                LoadPayments();
            }
            catch (Exception ex)
            {
                ShowMessage("Delete failed: " + ex.Message, System.Drawing.Color.Red);
            }
        }

        private void ShowMessage(string msg, System.Drawing.Color color)
        {
            lblMessage.Text = msg;
            lblMessage.ForeColor = color;
        }

        private void ClearForm()
        {
            txtOrderID.Text = "";
            txtAmount.Text = "";
            gvPayments.SelectedIndex = -1;
            txtDate.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
        }
    }
}