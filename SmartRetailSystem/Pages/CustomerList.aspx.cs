using System;
using SmartRetailSystem.Services;

namespace SmartRetailSystem
{
    public partial class CustomerList : System.Web.UI.Page
    {
        CustomerService service = new CustomerService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadCustomers();
        }

        private void LoadCustomers(string search = "")
        {
            gvCustomers.DataSource = service.GetCustomers(search);
            gvCustomers.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadCustomers(txtSearch.Text.Trim());
        }

        protected void btnAddCustomer_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddCustomer.aspx");
        }

        protected void gvCustomers_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditCustomer")
            {
                Response.Redirect("EditCustomer.aspx?CustomerID=" + e.CommandArgument);
            }
            else if (e.CommandName == "DeleteCustomer")
            {
                service.DeleteCustomer(Convert.ToInt32(e.CommandArgument));
                LoadCustomers();
            }
        }
    }
}