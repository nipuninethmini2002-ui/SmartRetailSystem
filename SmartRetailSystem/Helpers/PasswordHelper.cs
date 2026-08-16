using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SmartRetailSystem.Helpers
{
    public class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "");
            }
        }
    }
}