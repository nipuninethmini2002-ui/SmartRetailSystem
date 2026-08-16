using SmartRetailSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SmartRetailSystem.DAL
{
    public class UserDAL
    {
        string connStr =
        ConfigurationManager.ConnectionStrings["SmartRetailSystem"].ConnectionString;

        public bool UserExists(string username, string email)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                SqlCommand cmd =
                new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username=@u OR Email=@e", con);

                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@e", email);

                return (int)cmd.ExecuteScalar() > 0;
            }
        }
        public User GetUserByUsernameAndPassword(string username, string passwordHash)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                string query = "SELECT UserID, Role, Status, Email FROM Users WHERE Username=@Username AND PasswordHash=@PasswordHash";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    User user = new User
                    {
                        UserID = Convert.ToInt32(reader["UserID"]),
                        Username = username,
                        Email = reader["Email"].ToString(),
                        Role = reader["Role"].ToString(),
                        Status = reader["Status"].ToString()
                    };
                    reader.Close();
                    return user;
                }
                reader.Close();
                return null;
            }
        }

        public int InsertUser(User user)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(
                @"INSERT INTO Users
                (Username,PasswordHash,Role,Status,Email)
                VALUES(@Username,@PasswordHash,@Role,'Active',@Email);
                SELECT SCOPE_IDENTITY();", con);

                cmd.Parameters.AddWithValue("@Username", user.Username);
                cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                cmd.Parameters.AddWithValue("@Role", user.Role);
                cmd.Parameters.AddWithValue("@Email", user.Email);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public void InsertCustomer(User user)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(
                @"INSERT INTO Customers
                (Name,Email,Phone,Address,CreditLimit,OutstandingBalance,Status)
                VALUES(@Name,@Email,@Phone,@Address,50000,0,'Active')", con);

                cmd.Parameters.AddWithValue("@Name", user.Name);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Phone", user.Phone);
                cmd.Parameters.AddWithValue("@Address", user.Address);

                cmd.ExecuteNonQuery();
            }
        }
        public bool EmailExists(string email)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT COUNT(*) FROM Users WHERE Email=@Email";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Email", email);

                con.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        public bool UpdatePassword(string email, string passwordHash)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "UPDATE Users SET PasswordHash=@PasswordHash WHERE Email=@Email";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                cmd.Parameters.AddWithValue("@Email", email);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        public bool DeleteUserAndCustomerByEmail(string email)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                // Transaction එකක් භාවිතා කිරීමෙන් වගු දෙකෙන්ම මැකීම සහතික කරයි
                SqlTransaction transaction = con.BeginTransaction();

                try
                {
                    // 1. මුලින්ම Customers වගුවෙන් දත්ත මකා දමන්න
                    string deleteCustomerQuery = "DELETE FROM Customers WHERE Email = @Email";
                    using (SqlCommand cmdCust = new SqlCommand(deleteCustomerQuery, con, transaction))
                    {
                        cmdCust.Parameters.AddWithValue("@Email", email);
                        cmdCust.ExecuteNonQuery();
                    }

                    // 2. පසුව Users වගුවෙන් (Login දත්ත) මකා දමන්න
                    string deleteUserQuery = "DELETE FROM Users WHERE Email = @Email";
                    using (SqlCommand cmdUser = new SqlCommand(deleteUserQuery, con, transaction))
                    {
                        cmdUser.Parameters.AddWithValue("@Email", email);
                        cmdUser.ExecuteNonQuery();
                    }

                    // සියල්ල සාර්ථක නම් දත්ත මැකීම ස්ථිර කරන්න
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    // යම් දෝෂයක් වුවහොත් දත්ත මැකීම අවලංගු කරන්න
                    transaction.Rollback();
                    System.Diagnostics.Debug.WriteLine("Delete Error: " + ex.Message);
                    return false;
                }
            }
        }
    }
}