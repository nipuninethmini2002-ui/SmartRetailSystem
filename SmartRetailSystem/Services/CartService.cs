using SmartRetailSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SmartRetailSystem.Services
{
    public class CartService
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["SmartRetailSystem"].ConnectionString;
        public void AddToCart(int customerId, int productId, int quantity)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand check = new SqlCommand(
                    "SELECT Quantity FROM CartItems WHERE CustomerID=@cid AND ProductID=@pid", con);
                check.Parameters.AddWithValue("@cid", customerId);
                check.Parameters.AddWithValue("@pid", productId);
                object exist = check.ExecuteScalar();

                if (exist != null)
                {
                    // update existing row
                    SqlCommand update = new SqlCommand(
                        "UPDATE CartItems SET Quantity=Quantity+@q WHERE CustomerID=@cid AND ProductID=@pid", con);
                    update.Parameters.AddWithValue("@q", quantity);
                    update.Parameters.AddWithValue("@cid", customerId);
                    update.Parameters.AddWithValue("@pid", productId);
                    update.ExecuteNonQuery();
                }
                else
                {
                    // insert new row only if it doesn't exist
                    SqlCommand insert = new SqlCommand(
                        "INSERT INTO CartItems (CustomerID, ProductID, Quantity) VALUES(@cid,@pid,@q)", con);
                    insert.Parameters.AddWithValue("@cid", customerId); // must be valid Customers.CustomerID
                    insert.Parameters.AddWithValue("@pid", productId);
                    insert.Parameters.AddWithValue("@q", quantity);
                    insert.ExecuteNonQuery();
                }
            }
        }
        public void RemoveSelectedItemsFromCart(int customerId, List<int> productIds)
        {
            if (productIds == null || productIds.Count == 0) return;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // Product IDs ලැයිස්තුව (1,2,3) ලෙස සකස් කර ගැනීම
                string ids = string.Join(",", productIds);

                // වැදගත්: මෙතැන Table නම "CartItems" විය යුතුය (ඔබේ DB එකේ නම අනුව)
                string sql = $"DELETE FROM CartItems WHERE CustomerID = @cid AND ProductID IN ({ids})";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@cid", customerId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<CartItem> GetCartItems(int customerId)
        {
            List<CartItem> list = new List<CartItem>();
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                string query = @"SELECT c.ProductID, p.ProductName, p.Price, c.Quantity
                                 FROM CartItems c
                                 INNER JOIN Products p ON c.ProductID=p.ProductID
                                 WHERE c.CustomerID=@cid";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@cid", customerId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new CartItem
                            {
                                ProductID = Convert.ToInt32(reader["ProductID"]),
                                ProductName = reader["ProductName"].ToString(),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                Price = Convert.ToDecimal(reader["Price"])
                            });
                        }
                    }
                }
            }
            return list;
        }

        public void RemoveCartItem(int customerId, int productId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM CartItems WHERE CustomerID=@cid AND ProductID=@pid", con);
                cmd.Parameters.AddWithValue("@cid", customerId);
                cmd.Parameters.AddWithValue("@pid", productId);
                cmd.ExecuteNonQuery();
            }
        }

        public void ClearCart(int customerId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM CartItems WHERE CustomerID=@cid", con);
                cmd.Parameters.AddWithValue("@cid", customerId);
                cmd.ExecuteNonQuery();
            }
        }
        public DataTable GetCart()
        {
            if (System.Web.HttpContext.Current.Session["Cart"] == null)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("ProductID", typeof(int));
                dt.Columns.Add("ProductName", typeof(string));
                dt.Columns.Add("Quantity", typeof(int));
                dt.Columns.Add("Price", typeof(decimal));
                dt.Columns.Add("Total", typeof(decimal));
                System.Web.HttpContext.Current.Session["Cart"] = dt;
            }
            return (DataTable)System.Web.HttpContext.Current.Session["Cart"];
        }

        public void ClearCart() => System.Web.HttpContext.Current.Session["Cart"] = null;
    }
}
