using SmartRetailSystem.Models;
using SmartRetailSystem.Services;
using System;
using System.IO;

namespace SmartRetailSystem
{
    public partial class AddEditProduct : System.Web.UI.Page
    {
        private int ProductID;
        private ProductService productService = new ProductService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["ProductID"])
                    && int.TryParse(Request.QueryString["ProductID"], out ProductID))
                {
                    litHeader.Text = "Edit Product";
                    LoadProduct(ProductID);
                }
                else
                {
                    litHeader.Text = "Add Product";
                }
            }
        }

        private void LoadProduct(int id)
        {
            Product product = productService.GetProductById(id);
            if (product != null)
            {
                txtProductName.Text = product.ProductName;
                ddlCategory.SelectedValue = product.Category;
                txtPrice.Text = product.Price.ToString("F2");
                txtStockQuantity.Text = product.StockQuantity.ToString();
                txtDescription.Text = product.Description;
            }
            else
            {
                Response.Redirect("ProductList.aspx");
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // 1. පවතින Product ID එක ලබා ගැනීම (Edit නම්)
            int currentId = 0;
            bool isEdit = !string.IsNullOrEmpty(Request.QueryString["ProductID"]) && int.TryParse(Request.QueryString["ProductID"], out currentId);

            // 2. දැනට තියෙන පින්තූරය කුමක්දැයි පරීක්ෂා කිරීම (Edit අවස්ථාවකදී පමණක්)
            string existingImagePath = "~/Images/no-image.png";
            if (isEdit)
            {
                var currentProduct = productService.GetProductById(currentId);
                if (currentProduct != null)
                {
                    existingImagePath = currentProduct.ImagePath; // පරණ පින්තූරය තබා ගන්න
                }
            }

            Product product = new Product
            {
                ProductName = txtProductName.Text.Trim(),
                Category = ddlCategory.SelectedValue,
                Price = Convert.ToDecimal(txtPrice.Text.Trim()),
                StockQuantity = Convert.ToInt32(txtStockQuantity.Text.Trim()),
                Description = txtDescription.Text.Trim(),
                Status = Convert.ToInt32(txtStockQuantity.Text.Trim()) < 5 ? "Low Stock" : "In Stock",
                ImagePath = existingImagePath // Default එක ලෙස පරණ පින්තූරය ලබා දෙන්න
            };

            // 3. නව පින්තූරයක් Upload කර ඇත්නම් පමණක් එය Update කරන්න
            if (fuImage.HasFile)
            {
                try
                {
                    string folder = Server.MapPath("~/Images/");
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                    // පින්තූරයේ නම අද්විතීය (Unique) කිරීමට GUID එකක් එකතු කිරීම වඩාත් සුදුසුයි
                    string filename = Guid.NewGuid().ToString() + "_" + Path.GetFileName(fuImage.FileName);
                    product.ImagePath = "~/Images/" + filename;
                    fuImage.SaveAs(Path.Combine(folder, filename));
                }
                catch (Exception ex)
                {
                    // Error එකක් ආවොත් පරණ පින්තූරයම තබා ගන්න
                    product.ImagePath = existingImagePath;
                }
            }

            // 4. Database එකට දත්ත යැවීම
            if (isEdit)
            {
                product.ProductID = currentId;
                productService.UpdateProduct(product);
            }
            else
            {
                productService.AddProduct(product);
            }

            Response.Redirect("ProductList.aspx");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("ProductList.aspx");
        }
    }
}