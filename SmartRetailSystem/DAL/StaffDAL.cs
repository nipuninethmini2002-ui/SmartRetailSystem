using SmartRetailSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SmartRetailSystem.DAL
{
    public class StaffDAL
    {
        string connStr = ConfigurationManager.ConnectionStrings["SmartRetailSystem"].ConnectionString;

        public StaffSummary GetStaffSummary()
        {
            StaffSummary summary = new StaffSummary();

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;

                cmd.CommandText = "SELECT COUNT(*) FROM Products";
                summary.TotalProducts = Convert.ToInt32(cmd.ExecuteScalar());

                cmd.CommandText = "SELECT COUNT(*) FROM Orders";
                summary.TotalOrders = Convert.ToInt32(cmd.ExecuteScalar());

                cmd.CommandText = "SELECT COUNT(*) FROM Orders WHERE DeliveryStatus='Pending'";
                summary.PendingOrders = Convert.ToInt32(cmd.ExecuteScalar());

                cmd.CommandText = "SELECT COUNT(*) FROM Customers";
                summary.TotalCustomers = Convert.ToInt32(cmd.ExecuteScalar());

                cmd.CommandText = "SELECT ISNULL(SUM(OutstandingBalance),0) FROM Customers WHERE OutstandingBalance>0";
                summary.OutstandingCredit = Convert.ToDecimal(cmd.ExecuteScalar());

                cmd.CommandText = "SELECT COUNT(*) FROM Products WHERE StockQuantity < 5";
                summary.LowStock = Convert.ToInt32(cmd.ExecuteScalar());
            }

            return summary;
        }
    }
}