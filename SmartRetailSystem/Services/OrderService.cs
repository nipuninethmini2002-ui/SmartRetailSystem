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
    public class OrderService
    {
        private string connStr = ConfigurationManager.ConnectionStrings["SmartRetailSystem"].ConnectionString;

        public List<Order> GetCustomerOrders(int customerId)
        {
            var orders = new List<Order>();
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT OrderID, OrderDate, GrandTotal, PaymentStatus, DeliveryStatus
                    FROM Orders
                    WHERE CustomerID=@CustomerID
                    ORDER BY OrderDate DESC";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@CustomerID", customerId);

                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    orders.Add(new Order
                    {
                        OrderID = Convert.ToInt32(row["OrderID"]),
                        OrderDate = Convert.ToDateTime(row["OrderDate"]),
                        GrandTotal = Convert.ToDecimal(row["GrandTotal"]),
                        PaymentStatus = row["PaymentStatus"].ToString(),
                        DeliveryStatus = row["DeliveryStatus"].ToString()
                    });
                }
            }
            return orders;
        }

        public Order GetOrderDetails(int orderId)
        {
            var order = new Order();
            // වැදගත්: Items List එක initialize වී ඇති බව සහතික කර ගන්න
            order.Items = new List<OrderItem>();

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // Order + Delivery info
                string orderQuery = @"
                    SELECT o.OrderID, o.OrderDate, o.GrandTotal, o.PaymentStatus, o.DeliveryStatus, o.SubTotal, o.Tax,
                           d.DeliveryPersonID, d.AssignedAt, d.DeliveredAt, u.UserName AS DeliveryPersonName
                    FROM Orders o
                    LEFT JOIN Deliveries d ON o.OrderID = d.OrderID
                    LEFT JOIN Users u ON d.DeliveryPersonID = u.UserID
                    WHERE o.OrderID=@OrderID";

                SqlCommand cmdOrder = new SqlCommand(orderQuery, con);
                cmdOrder.Parameters.AddWithValue("@OrderID", orderId);

                SqlDataReader reader = cmdOrder.ExecuteReader();
                if (reader.Read())
                {
                    order.OrderID = Convert.ToInt32(reader["OrderID"]);
                    order.OrderDate = Convert.ToDateTime(reader["OrderDate"]);

                    // මුළු Order එකටම අදාළ මුදල් අගයන්
                    order.SubTotal = reader["SubTotal"] != DBNull.Value ? Convert.ToDecimal(reader["SubTotal"]) : 0;
                    order.Tax = reader["Tax"] != DBNull.Value ? Convert.ToDecimal(reader["Tax"]) : 0;
                    order.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);
                    order.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);

                    order.PaymentStatus = reader["PaymentStatus"].ToString();
                    order.DeliveryStatus = reader["DeliveryStatus"].ToString();

                    order.DeliveryInfo = new Delivery
                    {
                        DeliveryPersonName = reader["DeliveryPersonName"] != DBNull.Value ? reader["DeliveryPersonName"].ToString() : "-",
                        AssignedAt = reader["AssignedAt"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["AssignedAt"]) : null,
                        DeliveredAt = reader["DeliveredAt"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["DeliveredAt"]) : null
                    };
                }
                reader.Close();

                // Order items
                string itemsQuery = @"
                    SELECT oi.ProductID, p.ProductName, oi.Quantity, oi.Price, oi.Total
                    FROM OrderItems oi
                    INNER JOIN Products p ON oi.ProductID = p.ProductID
                    WHERE oi.OrderID=@OrderID";

                SqlDataAdapter da = new SqlDataAdapter(itemsQuery, con);
                da.SelectCommand.Parameters.AddWithValue("@OrderID", orderId);

                DataTable dtItems = new DataTable();
                da.Fill(dtItems);

                foreach (DataRow row in dtItems.Rows)
                {
                    order.Items.Add(new OrderItem
                    {
                        ProductID = Convert.ToInt32(row["ProductID"]),
                        ProductName = row["ProductName"].ToString(),
                        Quantity = Convert.ToInt32(row["Quantity"]),
                        Price = Convert.ToDecimal(row["Price"]),
                       
                    });
                }
            }
            return order;
        }
    }
}