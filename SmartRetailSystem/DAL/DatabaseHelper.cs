using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SmartRetailSystem.Models
{
    public class DatabaseHelper
    {
        string connStr = ConfigurationManager.ConnectionStrings["SmartRetailSystem"].ConnectionString;

        public DataTable ExecuteSelect(string query)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public int ExecuteNonQuery(string query)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    return cmd.ExecuteNonQuery();
                }
            }
        }
    }
}