using SmartRetailSystem.BLL;
using SmartRetailSystem.Models;
using SmartRetailSystem.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace SmartRetailSystem.Services
{
    public class PaymentService
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["SmartRetailSystem"].ConnectionString;
        private PaymentBLL bll = new PaymentBLL();
        private PaymentDAL dal = new PaymentDAL();

        // 1. සියලුම ගෙවීම් විස්තර ලබා ගැනීම (Admin Grid එක සඳහා)
        public DataTable GetAllPayments()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                // Sub-query එකක් මගින් එක් එක් Order එකට අදාළ Balance එක ගණනය කිරීම
                string sql = @"
                    SELECT p.PaymentID, p.OrderID, p.CustomerID, p.PaidAmount, p.PaymentMethod, p.PaymentDate,
                           (o.GrandTotal - ISNULL((SELECT SUM(PaidAmount) FROM Payments WHERE OrderID = p.OrderID), 0)) AS Balance,
                           o.PaymentStatus
                    FROM Payments p
                    INNER JOIN Orders o ON p.OrderID = o.OrderID
                    ORDER BY p.PaymentDate DESC";
                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                da.Fill(dt);
            }
            return dt;
        }

        // 2. Order ID එක අනුව සෙවීම
        public DataTable GetPaymentsByOrder(int orderId)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                string sql = @"
                    SELECT p.PaymentID, p.OrderID, p.CustomerID, p.PaidAmount, p.PaymentMethod, p.PaymentDate,
                           (o.GrandTotal - ISNULL((SELECT SUM(PaidAmount) FROM Payments WHERE OrderID = p.OrderID), 0)) AS Balance,
                           o.PaymentStatus
                    FROM Payments p
                    INNER JOIN Orders o ON p.OrderID = o.OrderID
                    WHERE p.OrderID=@oid";
                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                da.SelectCommand.Parameters.AddWithValue("@oid", orderId);
                da.Fill(dt);
            }
            return dt;
        }

        // 3. ID එක අනුව එක් ගෙවීමක විස්තර ලබා ගැනීම
        public PaymentModel GetPaymentById(int paymentId)
        {
            PaymentModel pay = null;
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                string query = @"SELECT p.*, o.PaymentStatus, o.GrandTotal 
                                 FROM Payments p 
                                 INNER JOIN Orders o ON p.OrderID = o.OrderID 
                                 WHERE p.PaymentID=@pid";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@pid", paymentId);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            decimal actualPaid = Convert.ToDecimal(dr["PaidAmount"]);
                            decimal orderTotal = Convert.ToDecimal(dr["GrandTotal"]);

                            pay = new PaymentModel
                            {
                                PaymentID = Convert.ToInt32(dr["PaymentID"]),
                                OrderID = Convert.ToInt32(dr["OrderID"]),
                                CustomerID = Convert.ToInt32(dr["CustomerID"]),
                                // මුදල් ගෙවා නොමැති නම් (Pending), ගෙවිය යුතු මුළු මුදල පෙන්වයි
                                PaidAmount = (actualPaid == 0) ? orderTotal : actualPaid,
                                PaymentMethod = dr["PaymentMethod"].ToString(),
                                PaymentDate = Convert.ToDateTime(dr["PaymentDate"]),
                                PaymentStatus = dr["PaymentStatus"].ToString()
                            };
                        }
                    }
                }
            }
            return pay;
        }

        // 4. Admin විසින් සිදු කරන Update එක
        public bool UpdatePayment(PaymentModel pay)
        {
            bool isUpdated = dal.UpdatePayment(pay);
            if (isUpdated)
            {
                UpdateCustomerOutstandingBalance(pay.CustomerID);
            }
            return isUpdated;
        }
        // PaymentService.cs තුළට...
        public int GetNextOrderID()
        {
            return dal.GetNextOrderID();
        }
        // 5. Payment සහ Order එක මකා දැමීම (Transaction එකක් සහිතව)
        public void DeletePaymentAndOrder(int paymentId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                int orderId = 0;
                int customerId = 0;

                // මකා දැමීමට පෙර අදාළ OrderID සහ CustomerID ලබා ගැනීම
                using (SqlCommand cmdInfo = new SqlCommand("SELECT OrderID, CustomerID FROM Payments WHERE PaymentID=@pid", con))
                {
                    cmdInfo.Parameters.AddWithValue("@pid", paymentId);
                    using (SqlDataReader dr = cmdInfo.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            orderId = Convert.ToInt32(dr["OrderID"]);
                            customerId = Convert.ToInt32(dr["CustomerID"]);
                        }
                    }
                }

                if (orderId > 0)
                {
                    using (SqlTransaction trans = con.BeginTransaction())
                    {
                        try
                        {
                            // FK constraints නිසා අනුපිළිවෙලට මකා දැමිය යුතුය
                            string delItems = "DELETE FROM OrderItems WHERE OrderID=@oid";
                            string delPayments = "DELETE FROM Payments WHERE OrderID=@oid";
                            string delOrders = "DELETE FROM Orders WHERE OrderID=@oid";

                            using (SqlCommand cmdItems = new SqlCommand(delItems, con, trans))
                            {
                                cmdItems.Parameters.AddWithValue("@oid", orderId);
                                cmdItems.ExecuteNonQuery();
                            }

                            using (SqlCommand cmdPay = new SqlCommand(delPayments, con, trans))
                            {
                                cmdPay.Parameters.AddWithValue("@oid", orderId);
                                cmdPay.ExecuteNonQuery();
                            }

                            using (SqlCommand cmdOrd = new SqlCommand(delOrders, con, trans))
                            {
                                cmdOrd.Parameters.AddWithValue("@oid", orderId);
                                cmdOrd.ExecuteNonQuery();
                            }

                            trans.Commit();
                            // මකා දැමීමෙන් පසු Customer Balance එක නැවත ගණනය කිරීම
                            UpdateCustomerOutstandingBalance(customerId);
                        }
                        catch (Exception)
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
        }

        // 6. පාරිභෝගිකයා Checkout කරන විට නව Payment එක ඇතුළත් කිරීම
        public int AddPayment(OrderModel order, PaymentModel payment)
        {
            // BLL එක හරහා දත්ත ඇතුළත් කර අලුතින් හැදුණු OrderID එක ලබා ගනී
            int orderId = bll.ProcessPayment(order, payment);
            if (orderId > 0)
            {
                UpdateCustomerOutstandingBalance(order.CustomerID);
            }
            return orderId;
        }

        // 7. පාරිභෝගිකයාගේ මුළු ණය ශේෂය (Outstanding Balance) යාවත්කාලීන කිරීම
        public void UpdateCustomerOutstandingBalance(int customerId)
        {
            dal.UpdateCustomerOutstandingBalance(customerId);
        }
    }
}