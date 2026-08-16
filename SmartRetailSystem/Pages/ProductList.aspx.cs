using SmartRetailSystem.BLL;
using SmartRetailSystem.Services;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartRetailSystem.Pages
{
    public partial class ProductList : System.Web.UI.Page
    {
        private readonly ProductBLL productBLL = new ProductBLL();
        private readonly ProductService productService = new ProductService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadProducts();
                ShowLowStockAlert();
            }
        }

        private void LoadProducts(string search = "")
        {
            gvProducts.DataSource = productBLL.GetProducts(search);
            gvProducts.DataBind();
        }

        private void ShowLowStockAlert()
        {
            string html = productService.GetLowStockHtml();
            pnlLowStock.Visible = !string.IsNullOrEmpty(html);
            lblLowStockAlert.Text = html;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadProducts(txtSearch.Text.Trim());
        }

        protected void btnAddNewProduct_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddEditProduct.aspx");
        }

        protected void gvProducts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int productId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditProduct")
            {
                Response.Redirect("AddEditProduct.aspx?ProductID=" + productId);
            }
            else if (e.CommandName == "DeleteProduct")
            {
                productBLL.DeleteProduct(productId);
                LoadProducts();
                ShowLowStockAlert();
            }
        }
        protected global::System.Web.UI.WebControls.GridView gvProducts;
        protected global::System.Web.UI.WebControls.Panel pnlLowStock;
        protected global::System.Web.UI.WebControls.Label lblLowStockAlert;
        protected global::System.Web.UI.WebControls.TextBox txtSearch;
        protected global::System.Web.UI.WebControls.Button btnSearch;
    }
}