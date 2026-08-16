using SmartRetailSystem.BLL;
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
    public class DeliveryService
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["SmartRetailSystem"].ConnectionString;
        DeliveryBLL bll = new DeliveryBLL();

        // Load all delivery persons
        public List<User> GetDeliveryPersons()
        {
            var list = new List<User>();
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT UserID, Username FROM Users WHERE Role='Delivery'", con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new User
                    {
                        UserID = Convert.ToInt32(reader["UserID"]),
                        Username = reader["Username"].ToString()
                    });
                }
            }
            return list;
        }

        // Load all pending orders
        public DataTable GetPendingOrders()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(@"
                    SELECT o.OrderID, c.Name AS CustomerName, o.GrandTotal, o.PaymentStatus, o.DeliveryStatus, o.OrderDate
                    FROM Orders o
                    INNER JOIN Customers c ON o.CustomerID = c.CustomerID
                    WHERE o.PaymentStatus IN ('Paid', 'Cash on Delivery') 
                          AND o.DeliveryStatus = 'Pending'
                    ORDER BY o.OrderDate ASC", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        // Assign delivery person to order
        public bool AssignDelivery(int orderId, int deliveryPersonId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                SqlCommand checkCmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM Orders 
                    WHERE OrderID=@OrderID AND PaymentStatus IN ('Paid','Cash on Delivery') 
                          AND DeliveryStatus='Pending'", con);
                checkCmd.Parameters.AddWithValue("@OrderID", orderId);
                int exists = (int)checkCmd.ExecuteScalar();
                if (exists == 0) return false;

                // Deliveries table එකේ Column නම 'Status' බව සහතික කරගන්න
                SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO Deliveries(OrderID, DeliveryPersonID, Status, AssignedAt)
                    VALUES(@OrderID, @DeliveryPersonID, 'Pending', GETDATE())", con);
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                cmd.Parameters.AddWithValue("@DeliveryPersonID", deliveryPersonId);
                cmd.ExecuteNonQuery();

                SqlCommand updateOrder = new SqlCommand(
                    "UPDATE Orders SET DeliveryStatus='Assigned' WHERE OrderID=@OrderID", con);
                updateOrder.Parameters.AddWithValue("@OrderID", orderId);
                updateOrder.ExecuteNonQuery();

                return true;
            }
        }

        // මෙම Method එක Dashboard එකේදී භාවිතා කරන්න
        public void UpdateDeliveryStatus(int deliveryId, string status)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                // SQL Transaction එකක් භාවිතා කිරීම වඩාත් ආරක්ෂිතයි
                SqlTransaction trans = con.BeginTransaction();

                try
                {
                    string query = "";
                    // අකුරු වල ලොකු කුඩා බව නොසලකා පරීක්ෂා කිරීම (Case-insensitive check)
                    if (string.Equals(status, "Delivered", StringComparison.OrdinalIgnoreCase))
                    {
                        query = "UPDATE Deliveries SET Status = @status, DeliveredAt = GETDATE() WHERE DeliveryID = @id";
                    }
                    else
                    {
                        query = "UPDATE Deliveries SET Status = @status, DeliveredAt = NULL WHERE DeliveryID = @id";
                    }

                    SqlCommand cmd = new SqlCommand(query, con, trans);
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@id", deliveryId);
                    cmd.ExecuteNonQuery();

                    // Orders Table එක Update කිරීම
                    string getOrderIdQuery = "SELECT OrderID FROM Deliveries WHERE DeliveryID = @id";
                    SqlCommand cmdId = new SqlCommand(getOrderIdQuery, con, trans);
                    cmdId.Parameters.AddWithValue("@id", deliveryId);
                    object orderObj = cmdId.ExecuteScalar();

                    if (orderObj != null)
                    {
                        string ordUpdate = "UPDATE Orders SET DeliveryStatus = @status WHERE OrderID = @oid";
                        SqlCommand cmdOrd = new SqlCommand(ordUpdate, con, trans);
                        cmdOrd.Parameters.AddWithValue("@status", status);
                        cmdOrd.Parameters.AddWithValue("@oid", orderObj);
                        cmdOrd.ExecuteNonQuery();
                    }

                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public List<Delivery> GetDeliveries(int deliveryPersonId) => bll.GetDeliveries(deliveryPersonId);
    }
}