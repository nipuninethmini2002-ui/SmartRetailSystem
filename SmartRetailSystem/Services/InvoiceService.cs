using SmartRetailSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SmartRetailSystem.Services
{
    public class InvoiceService
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["SmartRetailSystem"].ConnectionString;

        public SmartRetailSystem.Models.Invoice GetInvoice(int orderId)
        {
            SmartRetailSystem.Models.Invoice inv = null;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // Load invoice info
                string orderQuery = @"
                    SELECT o.OrderID, o.GrandTotal, o.SubTotal, o.Tax, o.PaymentStatus, 
                           c.Name AS CustomerName, c.Address AS ShippingAddress, c.Phone AS ContactNumber, 
                           o.OrderDate
                    FROM Orders o
                    INNER JOIN Customers c ON o.CustomerID = c.CustomerID
                    WHERE o.OrderID = @oid";

                SqlCommand cmdOrder = new SqlCommand(orderQuery, con);
                cmdOrder.Parameters.AddWithValue("@oid", orderId);

                SqlDataReader reader = cmdOrder.ExecuteReader();
                if (reader.Read())
                {
                    inv = new SmartRetailSystem.Models.Invoice
                    {
                        OrderID = Convert.ToInt32(reader["OrderID"]),
                        CustomerName = reader["CustomerName"].ToString(),
                        OrderDate = Convert.ToDateTime(reader["OrderDate"]),
                        SubTotal = Convert.ToDecimal(reader["SubTotal"]),
                        Tax = Convert.ToDecimal(reader["Tax"]),
                        GrandTotal = Convert.ToDecimal(reader["GrandTotal"]),
                        PaymentStatus = reader["PaymentStatus"].ToString(),

                        // අතුරුදහන් වී තිබූ කොටස - මෙන්න මේ පේළි දෙක එකතු කරන්න
                        ShippingAddress = reader["ShippingAddress"].ToString(),
                        ContactNumber = reader["ContactNumber"].ToString(),

                        PaidAmount = 0,
                        Balance = 0
                    };
                }
                reader.Close();

                if (inv != null)
                {
                    string itemsQuery = @"
                        SELECT p.ProductName, oi.Quantity, oi.Price
                        FROM OrderItems oi
                        INNER JOIN Products p ON oi.ProductID = p.ProductID
                        WHERE oi.OrderID=@oid";

                    SqlCommand cmdItems = new SqlCommand(itemsQuery, con);
                    cmdItems.Parameters.AddWithValue("@oid", orderId);

                    DataTable dt = new DataTable();
                    dt.Load(cmdItems.ExecuteReader());

                    foreach (DataRow row in dt.Rows)
                    {
                        inv.Items.Add(new SmartRetailSystem.Models.InvoiceItem
                        {
                            ProductName = row["ProductName"].ToString(),
                            Quantity = Convert.ToInt32(row["Quantity"]),
                            Price = Convert.ToDecimal(row["Price"])
                        });
                    }
                }
            }

            return inv;
        }
    }
}