using SmartRetailSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SmartRetailSystem.DAL
{
    public class PaymentDAL
    {
        private string connStr = ConfigurationManager.ConnectionStrings["SmartRetailSystem"].ConnectionString;

        // 1. අලුත් ඇණවුමක් ඇතුළත් කර අලුත්ම OrderID එක ලබා ගැනීම
        // PaymentDAL.cs තුළ තිබිය යුතු නිවැරදිම INSERT එක
        public int InsertOrder(OrderModel order)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                // OUTPUT INSERTED.OrderID යන්න අනිවාර්යයෙන්ම තිබිය යුතුයි
                string sql = @"INSERT INTO Orders (CustomerID, SubTotal, Tax, GrandTotal, PaymentStatus, DeliveryStatus, OrderDate)
                       OUTPUT INSERTED.OrderID
                       VALUES (@cust,@sub,@tax,@grand,@payStatus,@delStatus,@date)";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@cust", order.CustomerID);
                    cmd.Parameters.AddWithValue("@sub", order.SubTotal);
                    cmd.Parameters.AddWithValue("@tax", order.Tax);
                    cmd.Parameters.AddWithValue("@grand", order.GrandTotal);
                    cmd.Parameters.AddWithValue("@payStatus", order.PaymentStatus);
                    cmd.Parameters.AddWithValue("@delStatus", order.DeliveryStatus);
                    cmd.Parameters.AddWithValue("@date", DateTime.Now);

                    // මෙන්න මෙතනින් තමයි අලුත්ම අංකය (උදා: 71) return වෙන්නේ
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        // PaymentDAL.cs තුළට...
        public int GetNextOrderID()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                // දැනට තිබෙන උපරිම ID එක ලබා ගනී (හිස් නම් 0 ලෙස ගෙන 1ක් එකතු කරයි)
                string sql = "SELECT ISNULL(MAX(OrderID), 0) + 1 FROM Orders";
                SqlCommand cmd = new SqlCommand(sql, con);
                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        // 5. Admin විසින් සිදු කරන Update ක්‍රියාවලිය (Status සහ Paid Amount යාවත්කාලීන කිරීම)
        public bool UpdatePayment(PaymentModel pay)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();

                try
                {
                    // 1. Payments Table එක Update කිරීම (Paid Amount එක සහ දිනය)
                    string sqlPay = @"UPDATE Payments 
                             SET PaidAmount = @amt, 
                                 PaymentMethod = @meth, 
                                 PaymentDate = @date 
                             WHERE PaymentID = @pid";

                    using (SqlCommand cmdPay = new SqlCommand(sqlPay, con, trans))
                    {
                        cmdPay.Parameters.AddWithValue("@amt", pay.PaidAmount);
                        cmdPay.Parameters.AddWithValue("@meth", pay.PaymentMethod);
                        cmdPay.Parameters.AddWithValue("@date", pay.PaymentDate);
                        cmdPay.Parameters.AddWithValue("@pid", pay.PaymentID);
                        cmdPay.ExecuteNonQuery();
                    }

                    // 2. Orders Table එකේ PaymentStatus එක (Paid/Pending) Update කිරීම
                    string sqlOrder = "UPDATE Orders SET PaymentStatus = @stat WHERE OrderID = @oid";
                    using (SqlCommand cmdOrder = new SqlCommand(sqlOrder, con, trans))
                    {
                        cmdOrder.Parameters.AddWithValue("@stat", pay.PaymentStatus);
                        cmdOrder.Parameters.AddWithValue("@oid", pay.OrderID);
                        cmdOrder.ExecuteNonQuery();
                    }

                    // සියල්ල සාර්ථක නම් Commit කරන්න
                    trans.Commit();

                    // 3. පාරිභෝගිකයාගේ ශේෂය (Outstanding Balance) නැවත ගණනය කර Update කිරීම
                    UpdateCustomerOutstandingBalance(pay.CustomerID);

                    return true;
                }
                catch (Exception ex)
                {
                    // දෝෂයක් ආවොත් Rollback කරන්න
                    trans.Rollback();
                    return false;
                }
            }
        }

        // 2. ගෙවීම් විස්තර ඇතුළත් කිරීම
        public void InsertPayment(PaymentModel payment)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                string sql = @"
                    INSERT INTO Payments (OrderID, CustomerID, PaidAmount, PaymentMethod, PaymentDate)
                    VALUES (@oid,@cid,@amt,@method,@date)";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@oid", payment.OrderID);
                    cmd.Parameters.AddWithValue("@cid", payment.CustomerID);
                    cmd.Parameters.AddWithValue("@amt", payment.PaidAmount);
                    cmd.Parameters.AddWithValue("@method", payment.PaymentMethod);
                    cmd.Parameters.AddWithValue("@date", DateTime.Now);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // 3. ඇණවුමේ භාණ්ඩ ඇතුළත් කිරීම සහ තොග (Stock) අඩු කිරීම
        public void InsertOrderItems(int orderId, List<OrderItemModel> items)
        {
            if (items == null) return;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                foreach (var item in items)
                {
                    // Order Item එක ඇතුළත් කිරීම
                    string sqlItem = @"
                        INSERT INTO OrderItems (OrderID, ProductID, Quantity, Price, Total, SubTotal) 
                        VALUES (@oid, @pid, @qty, @price, @total, @subtotal)";

                    using (SqlCommand cmd = new SqlCommand(sqlItem, con))
                    {
                        cmd.Parameters.AddWithValue("@oid", orderId);
                        cmd.Parameters.AddWithValue("@pid", item.ProductID);
                        cmd.Parameters.AddWithValue("@qty", item.Quantity);
                        cmd.Parameters.AddWithValue("@price", item.Price);
                        cmd.Parameters.AddWithValue("@total", item.Total);
                        cmd.Parameters.AddWithValue("@subtotal", item.SubTotal);
                        cmd.ExecuteNonQuery();
                    }

                    // Stock එක අඩු කිරීම
                    string sqlStock = "UPDATE Products SET StockQuantity = StockQuantity - @qty WHERE ProductID=@pid";
                    using (SqlCommand cmdStock = new SqlCommand(sqlStock, con))
                    {
                        cmdStock.Parameters.AddWithValue("@qty", item.Quantity);
                        cmdStock.Parameters.AddWithValue("@pid", item.ProductID);
                        cmdStock.ExecuteNonQuery();
                    }
                }
            }
        }

        // 6. පාරිභෝගිකයාගේ ශේෂය යාවත්කාලීන කිරීම (Calculated Update)
        public void UpdateCustomerOutstandingBalance(int customerId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                string sql = @"
                    UPDATE Customers 
                    SET OutstandingBalance = 
                        (SELECT ISNULL(SUM(GrandTotal), 0) FROM Orders WHERE CustomerID = @cid) - 
                        (SELECT ISNULL(SUM(PaidAmount), 0) FROM Payments WHERE CustomerID = @cid)
                    WHERE CustomerID = @cid";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@cid", customerId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // සටහන: GetAllPayments සහ GetPaymentById යන methods ද ඔබගේ මුල් කේතයේ පරිදිම නිවැරදියි.
    }
}