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
    public class DeliveryDAL
    {
        string connStr = ConfigurationManager.ConnectionStrings["SmartRetailSystem"].ConnectionString;

        public List<Delivery> GetDeliveries(int deliveryPersonId)
        {
            List<Delivery> list = new List<Delivery>();
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                string query = @"
                    SELECT d.DeliveryID, o.OrderID, c.Name AS CustomerName, c.Address, 
                           d.Status, d.AssignedAt, d.DeliveredAt
                    FROM Deliveries d
                    INNER JOIN Orders o ON d.OrderID = o.OrderID
                    INNER JOIN Customers c ON o.CustomerID = c.CustomerID
                    WHERE d.DeliveryPersonID = @DeliveryPersonID
                    AND o.PaymentStatus='Paid'
                    ORDER BY d.AssignedAt DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DeliveryPersonID", deliveryPersonId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new Delivery
                    {
                        DeliveryID = (int)row["DeliveryID"],
                        OrderID = (int)row["OrderID"],
                        CustomerName = row["CustomerName"].ToString(),
                        Address = row["Address"].ToString(),
                        Status = row["Status"].ToString(),
                        AssignedAt = row["AssignedAt"] != DBNull.Value ? (DateTime?)row["AssignedAt"] : null,
                        DeliveredAt = row["DeliveredAt"] != DBNull.Value ? (DateTime?)row["DeliveredAt"] : null
                    });
                }
            }
            return list;
        }

        public void UpdateDeliveryStatus(int deliveryId, string status)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // 1. Deliveries table එක Update කිරීම
                // මෙහිදී Status එක 'Delivered' ද කියා බලා DeliveredAt අගය තීරණය කරයි
                string query = @"
            UPDATE Deliveries 
            SET Status=@Status, 
                DeliveredAt = CASE WHEN @Status='Delivered' THEN GETDATE() ELSE NULL END
            WHERE DeliveryID=@DeliveryID";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@DeliveryID", deliveryId);
                cmd.ExecuteNonQuery();

                // 2. Orders table එක Update කිරීම
                string updateOrder = @"
            UPDATE Orders
            SET DeliveryStatus=@Status
            WHERE OrderID=(SELECT OrderID FROM Deliveries WHERE DeliveryID=@DeliveryID)";

                SqlCommand cmd2 = new SqlCommand(updateOrder, con);
                cmd2.Parameters.AddWithValue("@Status", status);
                cmd2.Parameters.AddWithValue("@DeliveryID", deliveryId);
                cmd2.ExecuteNonQuery();
            }
        }
    }
}