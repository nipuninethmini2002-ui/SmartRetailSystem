using SmartRetailSystem.BLL;
using SmartRetailSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace SmartRetailSystem.Services
{
    public class ProductService
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["SmartRetailSystem"].ConnectionString;
        public Product GetProductById(int productId)
        {
            Product product = null;
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Products WHERE ProductID=@id", con);
                cmd.Parameters.AddWithValue("@id", productId);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    product = new Product
                    {
                        ProductID = Convert.ToInt32(reader["ProductID"]),
                        ProductName = reader["ProductName"].ToString(),
                        Category = reader["Category"].ToString(),
                        Price = Convert.ToDecimal(reader["Price"]),
                        StockQuantity = Convert.ToInt32(reader["StockQuantity"]),
                        Description = reader["Description"].ToString(),
                        Status = reader["Status"].ToString(),
                        ImagePath = reader["ImagePath"] != DBNull.Value ? reader["ImagePath"].ToString() : null
                    };
                }
                reader.Close();
            }
            return product;
        }

        public void AddProduct(Product product)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                string sql = @"
                    INSERT INTO Products (ProductName, Category, Price, StockQuantity, Description, Status, ImagePath)
                    VALUES (@name,@category,@price,@stock,@desc,@status,@image)";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@name", product.ProductName);
                cmd.Parameters.AddWithValue("@category", product.Category);
                cmd.Parameters.AddWithValue("@price", product.Price);
                cmd.Parameters.AddWithValue("@stock", product.StockQuantity);
                cmd.Parameters.AddWithValue("@desc", product.Description);
                cmd.Parameters.AddWithValue("@status", product.Status);
                cmd.Parameters.AddWithValue("@image", (object)product.ImagePath ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateProduct(Product product)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                string sql = @"
                    UPDATE Products
                    SET ProductName=@name, Category=@category, Price=@price, StockQuantity=@stock,
                        Description=@desc, Status=@status, ImagePath=@image
                    WHERE ProductID=@id";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@name", product.ProductName);
                cmd.Parameters.AddWithValue("@category", product.Category);
                cmd.Parameters.AddWithValue("@price", product.Price);
                cmd.Parameters.AddWithValue("@stock", product.StockQuantity);
                cmd.Parameters.AddWithValue("@desc", product.Description);
                cmd.Parameters.AddWithValue("@status", product.Status);
                cmd.Parameters.AddWithValue("@image", (object)product.ImagePath ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@id", product.ProductID);
                cmd.ExecuteNonQuery();
            }
        }
        public List<Product> GetAllProducts()
        {
            List<Product> products = new List<Product>();

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                // 1. SQL Query එක නිවැරදියි (Category සහ Description මෙහි තිබේ)
                SqlCommand cmd = new SqlCommand("SELECT ProductID, ProductName, StockQuantity, Category, Description, Price, ImagePath FROM Products", con);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        ProductID = Convert.ToInt32(reader["ProductID"]),
                        ProductName = reader["ProductName"].ToString(),
                        StockQuantity = Convert.ToInt32(reader["StockQuantity"]),
                        Price = Convert.ToDecimal(reader["Price"]),
                        ImagePath = reader["ImagePath"] != DBNull.Value ? reader["ImagePath"].ToString() : null,

                        // --- මෙන්න මේ පේළි දෙක ඔබ අතින් මගහැරී තිබුණා ---
                        Category = reader["Category"] != DBNull.Value ? reader["Category"].ToString() : "General",
                        Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : "No description available"
                        // ------------------------------------------------
                    });
                }
            }
            return products;
        }
        private readonly ProductBLL bll = new ProductBLL();

        public string GetLowStockHtml(int threshold = 5)
        {
            var products = bll.GetLowStockProducts(threshold);
            if (products.Count == 0) return "";

            StringBuilder sb = new StringBuilder();
            sb.Append("<div class='lowStockTitle'>⚠ Low Stock Alert</div>");
            foreach (var p in products)
            {
                sb.Append("<div class='lowItem'>📦 " + p.ProductName + " — Only " + p.StockQuantity + " left</div>");
            }

            return sb.ToString();
        }
    }
}