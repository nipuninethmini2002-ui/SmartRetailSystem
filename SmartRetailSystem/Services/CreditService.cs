using SmartRetailSystem.Classes;
using SmartRetailSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;

namespace SmartRetailSystem.Services
{
    public class CreditService
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["SmartRetailSystem"].ConnectionString;

        public List<(Customer, CreditAccount)> GetCreditAccounts()
        {
            List<(Customer, CreditAccount)> list = new List<(Customer, CreditAccount)>();

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                string query = @"
                    UPDATE ca
                    SET ca.OverdueDays = CASE WHEN DATEDIFF(DAY, o.OrderDate, GETDATE()) > 0 
                                              THEN DATEDIFF(DAY, o.OrderDate, GETDATE()) 
                                              ELSE 0 END
                    FROM CreditAccounts ca
                    INNER JOIN Orders o ON ca.CustomerID = o.CustomerID
                    WHERE o.PaymentStatus = 'Pending';

                    UPDATE CreditAccounts
                    SET Status = CASE WHEN OutstandingBalance > CreditLimit THEN 'Blocked' ELSE 'Active' END;

                    SELECT c.CustomerID, c.Name, c.Email, ca.CreditLimit, ca.OutstandingBalance, 
                           ca.OverdueDays, ca.Status
                    FROM Customers c
                    INNER JOIN CreditAccounts ca ON c.CustomerID = ca.CustomerID;
                ";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        Customer c = new Customer
                        {
                            CustomerID = Convert.ToInt32(row["CustomerID"]),
                            Name = row["Name"].ToString(),
                            Email = row["Email"].ToString()
                        };
                        CreditAccount ca = new CreditAccount
                        {
                            CustomerID = c.CustomerID,
                            CreditLimit = Convert.ToDecimal(row["CreditLimit"]),
                            OutstandingBalance = Convert.ToDecimal(row["OutstandingBalance"]),
                            OverdueDays = Convert.ToInt32(row["OverdueDays"]),
                            Status = row["Status"].ToString()
                        };
                        list.Add((c, ca));
                    }
                }
            }

            return list;
        }

        public List<Notification> GetNotifications()
        {
            List<Notification> list = new List<Notification>();

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                string query = @"
                    SELECT n.NotificationID, n.CustomerID, n.Message, n.CreatedAt, n.IsRead, c.Name AS CustomerName
                    FROM Notifications n
                    INNER JOIN Customers c ON n.CustomerID = c.CustomerID
                    ORDER BY n.CreatedAt DESC";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        list.Add(new Notification
                        {
                            NotificationID = Convert.ToInt32(row["NotificationID"]),
                            CustomerID = Convert.ToInt32(row["CustomerID"]),
                            Message = row["Message"].ToString(),
                            CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                            IsRead = Convert.ToBoolean(row["IsRead"]),
                            CustomerName = row["CustomerName"].ToString()
                        });
                    }
                }
            }

            return list;
        }

        public void UpdateStatusAndNotify(Customer customer, string status, string message)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                if (!string.IsNullOrEmpty(status))
                {
                    SqlCommand cmdStatus = new SqlCommand("UPDATE Customers SET Status=@status WHERE CustomerID=@cid", con);
                    cmdStatus.Parameters.AddWithValue("@status", status);
                    cmdStatus.Parameters.AddWithValue("@cid", customer.CustomerID);
                    cmdStatus.ExecuteNonQuery();
                }

                SqlCommand cmdNotif = new SqlCommand(
                    "INSERT INTO Notifications (CustomerID, Message, CreatedAt) VALUES (@cid, @msg, @date)", con);
                cmdNotif.Parameters.AddWithValue("@cid", customer.CustomerID);
                cmdNotif.Parameters.AddWithValue("@msg", message);
                cmdNotif.Parameters.AddWithValue("@date", DateTime.Now);
                cmdNotif.ExecuteNonQuery();
            }

            try
            {
                SendEmail(customer.Email, "Smart Retail System Notification", $"Dear {customer.Name},\n\n{message}\n\nBest regards,\nSmart Retail Team");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Email send error: " + ex.Message);
            }
        }

        private void SendEmail(string toEmail, string subject, string body)
        {
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587); // Gmail SMTP
            smtp.Credentials = new System.Net.NetworkCredential("isdnsystem@gmail.com", "pdkh wzxp fokt ffts");
            smtp.EnableSsl = true;

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("isdnsystem@gmail.com", "Smart Retail System");
            msg.To.Add(toEmail);
            msg.Subject = subject;
            msg.Body = body;

            smtp.Send(msg);
        }
    }
}