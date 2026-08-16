using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartRetailSystem
{
    public partial class CreditControl : Page
    {
        // Connection String ලබා ගැනීම
        string connStr = ConfigurationManager.ConnectionStrings["SmartRetailSystem"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RefreshDashboard();
            }
        }

        private void RefreshDashboard()
        {
            LoadCreditData();
            LoadNotifications();
        }

        private void LoadCreditData()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // 1. Outstanding Balance එක යාවත්කාලීන කිරීම
                    string updateBalanceQuery = @"
                        UPDATE ca
                        SET ca.OutstandingBalance = ISNULL((
                            SELECT SUM(o.GrandTotal) 
                            FROM Orders o 
                            WHERE o.CustomerID = ca.CustomerID 
                            AND o.PaymentStatus = 'Pending'
                        ), 0)
                        FROM CreditAccounts ca";

                    using (SqlCommand cmdBalance = new SqlCommand(updateBalanceQuery, con))
                    {
                        cmdBalance.ExecuteNonQuery();
                    }

                    // 2. ස්වයංක්‍රීයව Block කිරීම සහ BlockedDate එක සටහන් කිරීම
                    // මෙහිදී අලුතින් Block වන විට පමණක් GETDATE() එකතු වේ.
                    string updateStatusQuery = @"
                        UPDATE CreditAccounts
                        SET Status = CASE 
                                WHEN OutstandingBalance > CreditLimit THEN 'Blocked' 
                                ELSE Status END,
                            BlockedDate = CASE 
                                WHEN OutstandingBalance > CreditLimit AND (Status <> 'Blocked' OR Status IS NULL) THEN GETDATE() 
                                WHEN OutstandingBalance <= CreditLimit AND Status = 'Active' THEN NULL
                                ELSE BlockedDate END";

                    using (SqlCommand cmdStatus = new SqlCommand(updateStatusQuery, con))
                    {
                        cmdStatus.ExecuteNonQuery();
                    }

                    // 3. Overdue Days යාවත්කාලීන කිරීම (BlockedDate එකේ සිට අද දක්වා)
                    string updateOverdueQuery = @"
                        UPDATE ca
                        SET ca.OverdueDays = CASE 
                            WHEN ca.Status = 'Blocked' AND ca.BlockedDate IS NOT NULL 
                            THEN DATEDIFF(DAY, ca.BlockedDate, GETDATE())
                            ELSE 0 
                        END
                        FROM CreditAccounts ca";

                    using (SqlCommand cmdOverdue = new SqlCommand(updateOverdueQuery, con))
                    {
                        cmdOverdue.ExecuteNonQuery();
                    }

                    // 4. GridView එක සඳහා දත්ත ලබා ගැනීම
                    string selectQuery = @"
                        SELECT c.CustomerID, c.Name, c.Email, ca.CreditLimit, ca.OutstandingBalance, 
                               ca.OverdueDays, ISNULL(ca.Status, 'Active') AS Status
                        FROM Customers c
                        INNER JOIN CreditAccounts ca ON c.CustomerID = ca.CustomerID";

                    using (SqlCommand cmd = new SqlCommand(selectQuery, con))
                    {
                        DataTable dt = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);

                        gvCreditControl.DataSource = dt;
                        gvCreditControl.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Load Error: " + ex.Message);
            }
        }

        private void LoadNotifications()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT n.NotificationID, n.CustomerID, n.Message, n.CreatedAt, n.IsRead, c.Name AS CustomerName
                    FROM Notifications n
                    INNER JOIN Customers c ON n.CustomerID = c.CustomerID
                    ORDER BY n.CreatedAt DESC";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvNotifications.DataSource = dt;
                gvNotifications.DataBind();
            }
        }

        protected void gvCreditControl_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandArgument == null) return;

            int customerId = Convert.ToInt32(e.CommandArgument);
            string email = "";
            string customerName = "";

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT Email, Name FROM Customers WHERE CustomerID=@cid", con);
                cmd.Parameters.AddWithValue("@cid", customerId);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        email = dr["Email"].ToString();
                        customerName = dr["Name"].ToString();
                    }
                }
            }

            if (e.CommandName == "Block")
            {
                UpdateStatusAndNotify(customerId, "Blocked", "Your account has been blocked due to credit concerns.", email, customerName);
            }
            else if (e.CommandName == "Unblock")
            {
                UpdateStatusAndNotify(customerId, "Active", "Your account has been unblocked. You can now proceed with orders.", email, customerName);
            }
            else if (e.CommandName == "Reminder")
            {
                UpdateStatusAndNotify(customerId, null, "Reminder: You have an outstanding balance. Please settle it soon.", email, customerName);
            }

            RefreshDashboard();
        }

        private void UpdateStatusAndNotify(int customerId, string status, string message, string email, string customerName)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();

                try
                {
                    if (!string.IsNullOrEmpty(status))
                    {
                        // 1. CreditAccounts Table එක Update කිරීම (BlockedDate එක සමඟ)
                        string updateCreditSQL = @"
                            UPDATE CreditAccounts 
                            SET Status = @status, 
                                BlockedDate = CASE WHEN @status = 'Blocked' THEN GETDATE() ELSE NULL END 
                            WHERE CustomerID = @cid";

                        SqlCommand cmdCredit = new SqlCommand(updateCreditSQL, con, transaction);
                        cmdCredit.Parameters.AddWithValue("@status", status);
                        cmdCredit.Parameters.AddWithValue("@cid", customerId);
                        cmdCredit.ExecuteNonQuery();

                        // 2. Customers Table එක Update කිරීම
                        SqlCommand cmdCust = new SqlCommand("UPDATE Customers SET Status=@status WHERE CustomerID=@cid", con, transaction);
                        cmdCust.Parameters.AddWithValue("@status", status);
                        cmdCust.Parameters.AddWithValue("@cid", customerId);
                        cmdCust.ExecuteNonQuery();
                    }

                    // 3. Notification එක ඇතුළත් කිරීම
                    SqlCommand cmdNotif = new SqlCommand(
                        "INSERT INTO Notifications (CustomerID, Message, CreatedAt, IsRead) VALUES (@cid, @msg, @date, 0)", con, transaction);
                    cmdNotif.Parameters.AddWithValue("@cid", customerId);
                    cmdNotif.Parameters.AddWithValue("@msg", message);
                    cmdNotif.Parameters.AddWithValue("@date", DateTime.Now);
                    cmdNotif.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    System.Diagnostics.Debug.WriteLine("Update Status Error: " + ex.Message);
                }
            }

            if (!string.IsNullOrEmpty(email))
            {
                SendEmailNotification(email, customerName, message);
            }
        }

        private void SendEmailNotification(string toEmail, string name, string msg)
        {
            try
            {
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new System.Net.NetworkCredential("isdnsystem@gmail.com", "towy bmkm bexu lmay");
                    smtp.EnableSsl = true;

                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress("isdnsystem@gmail.com", "Smart Retail System");
                    mail.To.Add(toEmail);
                    mail.Subject = "Account Status Update - Smart Retail";
                    mail.Body = $"Dear {name},\n\n{msg}\n\nThank you,\nManagement";

                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Email Fail: " + ex.Message);
            }
        }
    }
}