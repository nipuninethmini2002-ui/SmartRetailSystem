using SmartRetailSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SmartRetailSystem.DAL
{
    public class ProductDAL
    {
        string connStr = ConfigurationManager.ConnectionStrings["SmartRetailSystem"].ConnectionString;

        public List<Product> GetAllProducts()
        {
            var products = new List<Product>();
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT ProductID, ProductName, StockQuantity, Price FROM Products", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    products.Add(new Product
                    {
                        ProductID = (int)dr["ProductID"],
                        ProductName = dr["ProductName"].ToString(),
                        StockQuantity = (int)dr["StockQuantity"],
                        Price = (decimal)dr["Price"]
                    });
                }
            }
            return products;
        }

        public Product GetProductById(int id)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("SELECT ProductID, ProductName, StockQuantity, Price FROM Products WHERE ProductID=@id", con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    return new Product
                    {
                        ProductID = (int)dr["ProductID"],
                        ProductName = dr["ProductName"].ToString(),
                        StockQuantity = (int)dr["StockQuantity"],
                        Price = (decimal)dr["Price"]
                    };
                }
            }
            return null;
        }

        public void UpdateStock(int productId, int qty, SqlConnection con, SqlTransaction tr)
        {
            SqlCommand cmd = new SqlCommand("UPDATE Products SET StockQuantity = StockQuantity - @qty WHERE ProductID=@pid", con, tr);
            cmd.Parameters.AddWithValue("@qty", qty);
            cmd.Parameters.AddWithValue("@pid", productId);
            cmd.ExecuteNonQuery();
        }
        public List<Product> GetProducts(string search = "")
        {
            List<Product> products = new List<Product>();
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;

                if (string.IsNullOrEmpty(search))
                    cmd.CommandText = "SELECT * FROM Products";
                else
                {
                    cmd.CommandText = "SELECT * FROM Products WHERE ProductName LIKE @search";
                    cmd.Parameters.AddWithValue("@search", "%" + search + "%");
                }

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    products.Add(new Product
                    {
                        ProductID = (int)dr["ProductID"],
                        ProductName = dr["ProductName"].ToString(),
                        StockQuantity = (int)dr["StockQuantity"],
                        Price = (decimal)dr["Price"]
                    });
                }
            }
            return products;
        }

        public void DeleteProduct(int productId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Products WHERE ProductID=@id", con);
                cmd.Parameters.AddWithValue("@id", productId);
                cmd.ExecuteNonQuery();
            }
        }

        public List<Product> GetLowStockProducts(int threshold = 5)
        {
            List<Product> lowStock = new List<Product>();
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT ProductName, StockQuantity FROM Products WHERE StockQuantity <= @threshold", con);
                cmd.Parameters.AddWithValue("@threshold", threshold);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lowStock.Add(new Product
                    {
                        ProductName = dr["ProductName"].ToString(),
                        StockQuantity = (int)dr["StockQuantity"]
                    });
                }
            }
            return lowStock;
        }
    }
}
