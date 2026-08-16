using SmartRetailSystem.BLL;
using SmartRetailSystem.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SmartRetailSystem.Services
{
    public class CustomerService
    {
        CustomerBLL bll = new CustomerBLL();
        private readonly string connStr = ConfigurationManager.ConnectionStrings["SmartRetailSystem"].ConnectionString;

        public List<Customer> GetAllCustomers()
        {
            List<Customer> customers = new List<Customer>();
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT CustomerID, Name, OutstandingBalance, CreditLimit, Status FROM Customers";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        customers.Add(new Customer
                        {
                            CustomerID = Convert.ToInt32(reader["CustomerID"]),
                            Name = reader["Name"].ToString(),
                            OutstandingBalance = Convert.ToDecimal(reader["OutstandingBalance"]),
                            CreditLimit = Convert.ToDecimal(reader["CreditLimit"]),
                            Status = reader["Status"].ToString()
                        });
                    }
                }
            }
            return customers;
        }

        public List<Customer> GetCustomers(string search = "") => bll.GetCustomers(search);

        public void DeleteCustomer(int customerId) => bll.RemoveCustomer(customerId);
        // --- යාවත්කාලීන කළ යුතු Delete Method එක ---
        public bool DeleteCustomerFully(string email)
        {
            // මෙහිදී අපි කලින් UserDAL එකේ හැදූ method එක භාවිතා කරමු
            SmartRetailSystem.DAL.UserDAL userDal = new SmartRetailSystem.DAL.UserDAL();
            return userDal.DeleteUserAndCustomerByEmail(email);
        }

        public Customer GetCustomerByEmail(string email)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Customers WHERE Email=@email", con);
                cmd.Parameters.AddWithValue("@email", email);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Customer
                        {
                            CustomerID = Convert.ToInt32(reader["CustomerID"]),
                            Name = reader["Name"].ToString(),
                            Email = reader["Email"].ToString(),
                            Status = reader["Status"].ToString(),
                            OutstandingBalance = Convert.ToDecimal(reader["OutstandingBalance"]),
                            CreditLimit = Convert.ToDecimal(reader["CreditLimit"])
                        };
                    }
                }
            }
            return null;
        }

        public bool CheckUsernameExists(string username)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username=@Username", con);
                cmd.Parameters.AddWithValue("@Username", username);
                int exists = (int)cmd.ExecuteScalar();
                return exists > 0;
            }
        }

        public void AddCustomer(Customer customer)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand cmdUser = new SqlCommand(
                    "INSERT INTO Users (Username, PasswordHash, Role, Status, Email) VALUES (@Username,@PasswordHash,'Customer','Active',@Email)", con);
                cmdUser.Parameters.AddWithValue("@Username", customer.Username);
                cmdUser.Parameters.AddWithValue("@PasswordHash", customer.PasswordHash);
                cmdUser.Parameters.AddWithValue("@Email", customer.Email);
                cmdUser.ExecuteNonQuery();

                SqlCommand cmdCustomer = new SqlCommand(
                    @"INSERT INTO Customers (Name, Phone, Address, Email, CreditLimit, OutstandingBalance, Status, CreatedAt)
                      VALUES (@Name,@Phone,@Address,@Email,@CreditLimit,@OutstandingBalance,@Status,@CreatedAt)", con);
                cmdCustomer.Parameters.AddWithValue("@Name", customer.Name);
                cmdCustomer.Parameters.AddWithValue("@Phone", customer.Phone);
                cmdCustomer.Parameters.AddWithValue("@Address", customer.Address);
                cmdCustomer.Parameters.AddWithValue("@Email", customer.Email);
                cmdCustomer.Parameters.AddWithValue("@CreditLimit", customer.CreditLimit);
                cmdCustomer.Parameters.AddWithValue("@OutstandingBalance", customer.OutstandingBalance);
                cmdCustomer.Parameters.AddWithValue("@Status", customer.Status);
                cmdCustomer.Parameters.AddWithValue("@CreatedAt", customer.CreatedAt);
                cmdCustomer.ExecuteNonQuery();
            }
        }

        public string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha.ComputeHash(bytes);
                StringBuilder builder = new StringBuilder();
                foreach (byte b in hash)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }

        public Customer GetCustomerById(int customerId)
        {
            Customer customer = null;
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                string query = "SELECT * FROM Customers WHERE CustomerID=@id";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", customerId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            customer = new Customer
                            {
                                CustomerID = customerId,
                                Name = reader["Name"].ToString(),
                                Phone = reader["Phone"].ToString(),
                                Address = reader["Address"].ToString(),
                                Email = reader["Email"].ToString(),
                                CreditLimit = Convert.ToDecimal(reader["CreditLimit"]),
                                OutstandingBalance = Convert.ToDecimal(reader["OutstandingBalance"]),
                                Status = reader["Status"].ToString()
                            };
                        }
                    }
                }
            }
            return customer;
        }

        // --- යාවත්කාලීන කළ කොටස: Tables දෙකම Update කිරීම ---
        public void UpdateCustomer(Customer customer)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                // SQL Transaction එකක් පාවිච්චි කිරීමෙන් Tables දෙකම එකවර ආරක්ෂිතව update වේ
                SqlTransaction trans = con.BeginTransaction();

                try
                {
                    // 1. Customers Table එක Update කිරීම
                    string queryCustomer = @"
                        UPDATE Customers 
                        SET Name=@name, Phone=@phone, Address=@address, Email=@email, CreditLimit=@credit, Status=@status 
                        WHERE CustomerID=@id";

                    using (SqlCommand cmd1 = new SqlCommand(queryCustomer, con, trans))
                    {
                        cmd1.Parameters.AddWithValue("@name", customer.Name);
                        cmd1.Parameters.AddWithValue("@phone", customer.Phone);
                        cmd1.Parameters.AddWithValue("@address", customer.Address);
                        cmd1.Parameters.AddWithValue("@email", customer.Email);
                        cmd1.Parameters.AddWithValue("@credit", customer.CreditLimit);
                        cmd1.Parameters.AddWithValue("@status", customer.Status);
                        cmd1.Parameters.AddWithValue("@id", customer.CustomerID);
                        cmd1.ExecuteNonQuery();
                    }

                    // 2. CreditAccounts Table එකේ CreditLimit එකත් Update කිරීම
                    string queryCredit = @"
                        UPDATE CreditAccounts 
                        SET CreditLimit=@credit 
                        WHERE CustomerID=@id";

                    using (SqlCommand cmd2 = new SqlCommand(queryCredit, con, trans))
                    {
                        cmd2.Parameters.AddWithValue("@credit", customer.CreditLimit);
                        cmd2.Parameters.AddWithValue("@id", customer.CustomerID);
                        cmd2.ExecuteNonQuery();
                    }

                    trans.Commit(); // සාර්ථක නම් දත්ත save කරන්න
                }
                catch (Exception ex)
                {
                    trans.Rollback(); // ගැටලුවක් ආවොත් වෙනස්කම් අවලංගු කරන්න
                    throw ex;
                }
            }
        }
    }
}