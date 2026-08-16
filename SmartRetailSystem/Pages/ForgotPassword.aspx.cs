using System;
using SmartRetailSystem.Services;

namespace SmartRetailSystem
{
    public partial class ForgotPassword : System.Web.UI.Page
    {
        UserService service = new UserService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                pnlReset.Visible = false;
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            if (string.IsNullOrEmpty(email))
            {
                lblMessage.Text = "Please enter your email.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            if (service.IsEmailRegistered(email))
            {
                ViewState["Email"] = email;
                pnlReset.Visible = true;
                lblMessage.Text = "Email found. Enter new password below.";
                lblMessage.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                lblMessage.Text = "Email not found in system!";
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                lblMessage.Text = "Passwords do not match!";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            string email = ViewState["Email"].ToString();
            bool success = service.ResetPassword(email, txtNewPassword.Text);

            if (success)
            {
                lblMessage.Text = "Password reset successful!";
                lblMessage.ForeColor = System.Drawing.Color.Green;
                pnlReset.Visible = false;
            }
            else
            {
                lblMessage.Text = "Error: Could not reset password.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("Login.aspx");
        }
    }
}