using SmartRetailSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SmartRetailSystem.DAL
{
    public class OrderDAL
    {
        string connStr = ConfigurationManager.ConnectionStrings["SmartRetailSystem"].ConnectionString;

        public int SaveOrder(Order order)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlTransaction tr = con.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand(@"INSERT INTO Orders (CustomerID, SubTotal, Tax, GrandTotal, PaymentStatus, DeliveryStatus)
                                                      OUTPUT INSERTED.OrderID
                                                      VALUES (@cust,@sub,@tax,@grand,@payment,@delivery)", con, tr);
                    cmd.Parameters.AddWithValue("@cust", order.CustomerID);
                    cmd.Parameters.AddWithValue("@sub", order.SubTotal);
                    cmd.Parameters.AddWithValue("@tax", order.Tax);
                    cmd.Parameters.AddWithValue("@grand", order.GrandTotal);
                    cmd.Parameters.AddWithValue("@payment", order.PaymentStatus);
                    cmd.Parameters.AddWithValue("@delivery", order.DeliveryStatus);

                    int orderId = (int)cmd.ExecuteScalar();

                    foreach (var item in order.Items)
                    {
                        SqlCommand itemCmd = new SqlCommand(@"INSERT INTO OrderItems (OrderID, ProductID, Quantity, Price, Total)
                                                             VALUES (@oid,@pid,@qty,@price,@total)", con, tr);
                        itemCmd.Parameters.AddWithValue("@oid", orderId);
                        itemCmd.Parameters.AddWithValue("@pid", item.ProductID);
                        itemCmd.Parameters.AddWithValue("@qty", item.Quantity);
                        itemCmd.Parameters.AddWithValue("@price", item.Price);
                        itemCmd.Parameters.AddWithValue("@total", item.Total);
                        itemCmd.ExecuteNonQuery();

                        // Reduce stock
                        SqlCommand stockCmd = new SqlCommand("UPDATE Products SET StockQuantity = StockQuantity - @qty WHERE ProductID=@pid", con, tr);
                        stockCmd.Parameters.AddWithValue("@qty", item.Quantity);
                        stockCmd.Parameters.AddWithValue("@pid", item.ProductID);
                        stockCmd.ExecuteNonQuery();
                    }

                    tr.Commit();
                    return orderId;
                }
                catch
                {
                    tr.Rollback();
                    throw;
                }
            }
        }
        public List<Order> GetOrders(int customerId)
        {
            List<Order> orders = new List<Order>();
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT * FROM Orders WHERE CustomerID = @customerId ORDER BY OrderDate DESC";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@customerId", customerId);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    orders.Add(new Order
                    {
                        OrderID = (int)dr["OrderID"],
                        CustomerID = (int)dr["CustomerID"],
                        GrandTotal = (decimal)dr["GrandTotal"],
                        DeliveryStatus = dr["DeliveryStatus"].ToString(),
                        OrderDate = (DateTime)dr["OrderDate"]
                    });
                }
            }
            return orders;
        }
    }
}