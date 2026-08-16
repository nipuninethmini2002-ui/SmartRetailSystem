using System;
using System.Web;
using System.Web.UI;
using SmartRetailSystem.Models;
using SmartRetailSystem.Services;

namespace SmartRetailSystem
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // අවශ්‍ය නම් මෙහිදී කලින් තිබූ Session Clear කළ හැක
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            lblErrorMessage.Text = "";

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                lblErrorMessage.Text = "Please enter both username and password.";
                return;
            }

            UserService service = new UserService();
            User user = service.Login(username, password);

            if (user == null)
            {
                lblErrorMessage.Text = "Invalid username or password.";
                return;
            }

            if (user.Status != "Active")
            {
                lblErrorMessage.Text = "Your account is blocked. Contact admin.";
                return;
            }

            // Session values setting
            Session["UserID"] = user.UserID;
            Session["Username"] = user.Username;
            Session["Email"] = user.Email;
            Session["UserRole"] = user.Role;
            Session["Status"] = user.Status; // Status එකත් Session එකට දාන්න (CreateOrder එකේ check කරන නිසා)
                                             // Login සාර්ථක වූ පසු:
            Session["Username"] = user.Username;
            Session["Role"] = user.Role; // උදා: "Admin" හෝ "Staff"
            Session["CustomerID"] = user.CustomerID;
            if (user.Role == "Customer")
            {
                CustomerService customerService = new CustomerService();
                var customer = customerService.GetCustomerByEmail(user.Email);
                if (customer != null)
                {
                    Session["CustomerID"] = customer.CustomerID;
                    Session["CustomerName"] = customer.Name;
                    Session["OutstandingBalance"] = customer.OutstandingBalance;

                    // ✅ අත්‍යවශ්‍ය පියවර: CreditLimit එක Session එකට එකතු කිරීම
                    Session["CreditLimit"] = customer.CreditLimit;
                }
                else
                {
                    lblErrorMessage.Text = "Customer record not found in database.";
                    return;
                }
            }

            // Redirect logic
            switch (user.Role)
            {
                case "Admin":
                    Response.Redirect("Dashboard.aspx");
                    break;

                case "Staff":
                    Response.Redirect("StaffDashboard.aspx");
                    break;

                case "Delivery":
                    Response.Redirect("DeliveryDashboard.aspx");
                    break;

                case "Customer":
                    Response.Redirect("CustomerDashboard.aspx");
                    break;

                default:
                    lblErrorMessage.Text = "Role not recognized.";
                    break;
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtUsername.Text = "";
            txtPassword.Text = "";
            lblErrorMessage.Text = "";
        }
    }
}