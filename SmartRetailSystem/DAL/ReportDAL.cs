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
    public class ReportDAL
    {
        private string connStr = ConfigurationManager.ConnectionStrings["SmartRetailSystem"].ConnectionString;

        public List<ReportRecord> GetOrders(ReportFilter filter)
        {
            List<ReportRecord> list = new List<ReportRecord>();

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                string query = @"
                    SELECT o.OrderID, c.Name AS Customer, o.OrderDate, o.SubTotal, o.GrandTotal, o.PaymentStatus, o.DeliveryStatus
                    FROM Orders o
                    INNER JOIN Customers c ON o.CustomerID = c.CustomerID
                    WHERE 1=1";

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;

                if (!string.IsNullOrEmpty(filter.FromDate))
                {
                    query += " AND o.OrderDate >= @FromDate";
                    cmd.Parameters.AddWithValue("@FromDate", filter.FromDate);
                }

                if (!string.IsNullOrEmpty(filter.ToDate))
                {
                    query += " AND o.OrderDate <= @ToDate";
                    cmd.Parameters.AddWithValue("@ToDate", filter.ToDate);
                }

                if (filter.CustomerID > 0)
                {
                    query += " AND o.CustomerID = @CustomerID";
                    cmd.Parameters.AddWithValue("@CustomerID", filter.CustomerID);
                }

                if (filter.ProductID > 0)
                {
                    query += " AND EXISTS (SELECT 1 FROM OrderItems oi WHERE oi.OrderID=o.OrderID AND oi.ProductID=@ProductID)";
                    cmd.Parameters.AddWithValue("@ProductID", filter.ProductID);
                }

                cmd.CommandText = query;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new ReportRecord
                    {
                        OrderID = (int)dr["OrderID"],
                        Customer = dr["Customer"].ToString(),
                        OrderDate = Convert.ToDateTime(dr["OrderDate"]),
                        SubTotal = Convert.ToDecimal(dr["SubTotal"]),
                        GrandTotal = Convert.ToDecimal(dr["GrandTotal"]),
                        PaymentStatus = dr["PaymentStatus"].ToString(),
                        DeliveryStatus = dr["DeliveryStatus"].ToString()
                    });
                }
            }

            return list;
        }
    }
}