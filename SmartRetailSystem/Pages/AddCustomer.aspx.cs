using SmartRetailSystem.Classes;
using SmartRetailSystem.Models;
using SmartRetailSystem.Services;
using System;
using System.Web.UI;

namespace SmartRetailSystem
{
    public partial class AddCustomer : Page
    {
        private CustomerService customerService = new CustomerService();

        protected void btnSave_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";

            // Read form values
            string name = txtName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string address = txtAddress.Text.Trim();
            string email = txtEmail.Text.Trim();
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();
            decimal creditLimit = 50000;

            if (!decimal.TryParse(txtCreditLimit.Text.Trim(), out creditLimit))
                creditLimit = 50000;

            // Validation
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(address)
                || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                lblMessage.Text = "All required fields (*) must be filled.";
                return;
            }

            if (customerService.CheckUsernameExists(username))
            {
                lblMessage.Text = "Username already exists.";
                return;
            }

            // Create Customer object
            Customer customer = new Customer
            {
                Name = name,
                Phone = phone,
                Address = address,
                Email = email,
                Username = username,
                PasswordHash = customerService.HashPassword(password),
                CreditLimit = creditLimit
            };

            // Add to DB
            customerService.AddCustomer(customer);

            Response.Redirect("CustomerList.aspx");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("CustomerList.aspx");
        }
    }
}