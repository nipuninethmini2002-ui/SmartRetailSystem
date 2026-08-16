using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SmartRetailSystem.DAL
{
    public class DashboardDAL
    {
        string connStr = ConfigurationManager.ConnectionStrings["SmartRetailSystem"].ConnectionString;

        public int GetTotalProducts()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Products", con);
                return (int)cmd.ExecuteScalar();
            }
        }

        public int GetTotalOrders()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Orders", con);
                return (int)cmd.ExecuteScalar();
            }
        }

        public int GetPendingOrders()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Orders WHERE DeliveryStatus='Pending'", con);
                return (int)cmd.ExecuteScalar();
            }
        }

        public int GetTotalCustomers()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Customers", con);
                return (int)cmd.ExecuteScalar();
            }
        }

        public decimal GetTotalOutstandingCredit()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT ISNULL(SUM(OutstandingBalance),0) FROM Customers WHERE OutstandingBalance>0", con);
                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }

        public List<int> GetLowStockProductIDs()
        {
            List<int> ids = new List<int>();
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT ProductID FROM Products WHERE StockQuantity < 5", con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ids.Add(reader.GetInt32(0));
                }
                reader.Close();
            }
            return ids;
        }
    }
}