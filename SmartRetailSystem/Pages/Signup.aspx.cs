using System;
using SmartRetailSystem.Models;
using SmartRetailSystem.Services;

namespace SmartRetailSystem
{
    public partial class Signup : System.Web.UI.Page
    {
        protected void btnSignup_Click(object sender, EventArgs e)
        {
            User user = new User
            {
                Name = txtName.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Username = txtUsername.Text.Trim(),
                Role = ddlRole.SelectedValue,
                Phone = txtPhone.Text.Trim(),
                Address = txtAddress.Text.Trim()
            };

            UserService service = new UserService();

            bool success =
                service.Signup(user, txtPassword.Text.Trim());

            if (success)
            {
                lblMessage.ForeColor =
                    System.Drawing.Color.Green;
                lblMessage.Text =
                    "Account created successfully!";
            }
            else
            {
                lblMessage.Text =
                    "Username or Email already exists.";
            }
        }
        protected void ddlRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            customerFields.Visible =
                ddlRole.SelectedValue == "Customer";
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}