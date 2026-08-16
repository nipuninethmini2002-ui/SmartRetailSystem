using SmartRetailSystem.Classes;
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
    public class CustomerDashboardDAL
    {
        string connStr = ConfigurationManager.ConnectionStrings["SmartRetailSystem"].ConnectionString;

        public Customer GetCustomerByEmail(string email)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT CustomerID, Name, OutstandingBalance FROM Customers WHERE Email=@Email", con);
                cmd.Parameters.AddWithValue("@Email", email);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    return new Customer
                    {
                        CustomerID = (int)dr["CustomerID"],
                        Name = dr["Name"].ToString(),
                        OutstandingBalance = dr["OutstandingBalance"] != DBNull.Value ? Convert.ToDecimal(dr["OutstandingBalance"]) : 0
                    };
                }
                return null;
            }
        }

        public int GetOrderCount(int customerId, string status = null)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                string query = "SELECT COUNT(*) FROM Orders WHERE CustomerID=@cid";
                if (!string.IsNullOrEmpty(status))
                    query += " AND DeliveryStatus=@status";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@cid", customerId);
                if (!string.IsNullOrEmpty(status))
                    cmd.Parameters.AddWithValue("@status", status);

                return (int)cmd.ExecuteScalar();
            }
        }

        public List<Notification> GetNotifications(int customerId)
        {
            List<Notification> list = new List<Notification>();
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT NotificationID, Message, CreatedAt, IsRead FROM Notifications WHERE CustomerID=@cid ORDER BY CreatedAt DESC", con);
                cmd.Parameters.AddWithValue("@cid", customerId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new Notification
                    {
                        NotificationID = (int)row["NotificationID"],
                        CustomerID = customerId,
                        Message = row["Message"].ToString(),
                        CreatedAt = (DateTime)row["CreatedAt"],
                        IsRead = (bool)row["IsRead"]
                    });
                }
            }
            return list;
        }

        public void MarkNotificationRead(int notificationId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Notifications SET IsRead=1 WHERE NotificationID=@nid", con);
                cmd.Parameters.AddWithValue("@nid", notificationId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}