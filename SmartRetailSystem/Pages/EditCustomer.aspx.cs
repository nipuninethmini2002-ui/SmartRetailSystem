using SmartRetailSystem.Classes;
using SmartRetailSystem.Models;
using SmartRetailSystem.Services;
using System;

namespace SmartRetailSystem
{
    public partial class EditCustomer : System.Web.UI.Page
    {
        private CustomerService customerService = new CustomerService();
        private int CustomerId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["CustomerID"]) &&
                    int.TryParse(Request.QueryString["CustomerID"], out CustomerId))
                {
                    LoadCustomer(CustomerId);
                }
                else
                {
                    Response.Redirect("CustomerList.aspx");
                }
            }
        }

        private void LoadCustomer(int id)
        {
            Customer customer = customerService.GetCustomerById(id);
            if (customer != null)
            {
                txtName.Text = customer.Name;
                txtPhone.Text = customer.Phone;
                txtAddress.Text = customer.Address;
                txtEmail.Text = customer.Email;
                txtCreditLimit.Text = customer.CreditLimit.ToString();
                ddlStatus.SelectedValue = customer.Status;
            }
            else
            {
                Response.Redirect("CustomerList.aspx");
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(Request.QueryString["CustomerID"], out CustomerId))
            {
                Response.Redirect("CustomerList.aspx");
                return;
            }

            Customer customer = new Customer
            {
                CustomerID = CustomerId,
                Name = txtName.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                Address = txtAddress.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                CreditLimit = Convert.ToDecimal(txtCreditLimit.Text.Trim()),
                Status = ddlStatus.SelectedValue
            };

            customerService.UpdateCustomer(customer);
            Response.Redirect("CustomerList.aspx");

            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["SmartRetailSystem"].ConnectionString;
            using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(connStr))
            {
                string updateCreditLimit = "UPDATE CreditAccounts SET CreditLimit = @limit WHERE CustomerID = @cid";
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(updateCreditLimit, con);
                cmd.Parameters.AddWithValue("@limit", customer.CreditLimit);
                cmd.Parameters.AddWithValue("@cid", customer.CustomerID);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            Response.Redirect("CustomerList.aspx");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("CustomerList.aspx");
        }
    }
}