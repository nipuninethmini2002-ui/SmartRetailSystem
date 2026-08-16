using SmartRetailSystem.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SmartRetailSystem.DAL
{
    public class CustomerDAL
    {
        string connStr = ConfigurationManager.ConnectionStrings["SmartRetailSystem"].ConnectionString;

        public List<Customer> GetAllCustomers(string search = "")
        {
            List<Customer> customers = new List<Customer>();
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;

                if (string.IsNullOrEmpty(search))
                    cmd.CommandText = "SELECT * FROM Customers";
                else
                {
                    cmd.CommandText =
                    @"SELECT * FROM Customers
                      WHERE Name LIKE @s OR Email LIKE @s OR Phone LIKE @s";
                    cmd.Parameters.AddWithValue("@s", "%" + search + "%");
                }

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    customers.Add(new Customer
                    {
                        CustomerID = Convert.ToInt32(reader["CustomerID"]),
                        Name = reader["Name"].ToString(),
                        Email = reader["Email"].ToString(),
                        Phone = reader["Phone"].ToString(),
                        Address = reader["Address"].ToString(),
                        CreditLimit = Convert.ToDecimal(reader["CreditLimit"]),
                        OutstandingBalance = Convert.ToDecimal(reader["OutstandingBalance"]),
                        Status = reader["Status"].ToString()
                    });
                }
            }
            return customers;
        }

        public void DeleteCustomer(int customerId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // get email first
                SqlCommand getEmail = new SqlCommand(
                    "SELECT Email FROM Customers WHERE CustomerID=@id", con);
                getEmail.Parameters.AddWithValue("@id", customerId);
                string email = Convert.ToString(getEmail.ExecuteScalar());

                // delete from Customers
                SqlCommand delCustomer = new SqlCommand(
                    "DELETE FROM Customers WHERE CustomerID=@id", con);
                delCustomer.Parameters.AddWithValue("@id", customerId);
                delCustomer.ExecuteNonQuery();

                // delete from Users
                SqlCommand delUser = new SqlCommand(
                    "DELETE FROM Users WHERE Email=@Email", con);
                delUser.Parameters.AddWithValue("@Email", email);
                delUser.ExecuteNonQuery();
            }
        }
        public List<Customer> GetAllCustomers()
        {
            var customers = new List<Customer>();
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT CustomerID, Name, CreditLimit, OutstandingBalance FROM Customers", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    customers.Add(new Customer
                    {
                        CustomerID = (int)dr["CustomerID"],
                        Name = dr["Name"].ToString(),
                        CreditLimit = (decimal)dr["CreditLimit"],
                        OutstandingBalance = (decimal)dr["OutstandingBalance"]
                    });
                }
            }
            return customers;
        }

        public Customer GetCustomerById(int id)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("SELECT CustomerID, Name, CreditLimit, OutstandingBalance FROM Customers WHERE CustomerID=@id", con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    return new Customer
                    {
                        CustomerID = (int)dr["CustomerID"],
                        Name = dr["Name"].ToString(),
                        CreditLimit = (decimal)dr["CreditLimit"],
                        OutstandingBalance = (decimal)dr["OutstandingBalance"]
                    };
                }
            }
            return null;
        }
        public void UpdateBalance(int customerId, decimal amount)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                // පවතින OutstandingBalance එකට අලුත් ඇණවුමේ මුදල එකතු කිරීම
                string query = "UPDATE Customers SET OutstandingBalance = OutstandingBalance + @amount WHERE CustomerID = @customerId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@amount", amount);
                cmd.Parameters.AddWithValue("@customerId", customerId);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}