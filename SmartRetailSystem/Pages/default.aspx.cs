using SmartRetailSystem.Models;
using SmartRetailSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartRetailSystem
{
    public partial class Default : System.Web.UI.Page
    {
        private ProductService productService = new ProductService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadTrendingProducts();
            }
        }

        private void LoadTrendingProducts()
        {
            try
            {
                List<Product> allProducts = productService.GetAllProducts();

                if (allProducts != null && allProducts.Count > 0)
                {
                    rptTrending.DataSource = allProducts;
                    rptTrending.DataBind();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error Loading Products: " + ex.Message);
            }
        }

        // පින්තූර පෙන්වීමට අදාළ ගැටලුව විසඳන වැදගත්ම කොටස මෙයයි
        public string GetProductImage(object imagePath)
        {
            if (imagePath == null || string.IsNullOrWhiteSpace(imagePath.ToString()) || imagePath == DBNull.Value)
            {
                return "https://via.placeholder.com/300x250?text=No+Image+Found";
            }

            try
            {
                string path = imagePath.ToString().Trim();

                // පථය නිවැරදිව සකස් කිරීම
                string resolvedUrl = ResolveUrl(path);

                // පථයේ හිස්තැන් ඇත්නම් ඒවා URL එකකට ගැළපෙන සේ සැකසීම (Encoding)
                return Page.ResolveUrl(path).Replace(" ", "%20");
            }
            catch
            {
                return "https://via.placeholder.com/300x250?text=Path+Error";
            }
        }

        protected void btnBuy_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                string productId = btn.CommandArgument;

                if (string.IsNullOrEmpty(productId)) return;

                if (Session["UserID"] == null && Session["Username"] == null)
                {
                    Response.Redirect("Login.aspx?RedirectTo=Buy&ProductID=" + productId);
                }
                else
                {
                    Response.Redirect("CreateOrder.aspx?ProductID=" + productId);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Buy Button Error: " + ex.Message);
            }
        }
    }
}