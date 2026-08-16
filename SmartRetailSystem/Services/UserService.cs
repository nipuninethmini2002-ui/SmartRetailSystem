using SmartRetailSystem.BLL;
using SmartRetailSystem.Helpers;
using SmartRetailSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SmartRetailSystem.Services
{
    public class UserService
    {
        UserBLL bll = new UserBLL();

        public bool Signup(User user, string password)
        {
            user.PasswordHash =
                PasswordHelper.HashPassword(password);

            return bll.RegisterUser(user);
        }
        public User Login(string username, string password)
        {
            string hash = PasswordHelper.HashPassword(password);
            return bll.Login(username, hash);
        }
        public bool IsEmailRegistered(string email) => bll.IsEmailRegistered(email);

        public bool ResetPassword(string email, string newPassword)
        {
            string hashed = HashPassword(newPassword);
            return bll.ResetPassword(email, hashed);
        }

        private string HashPassword(string password)
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
        public int GetCustomerIdByEmail(string email)
        {
            CustomerService customerService = new CustomerService();
            var customer = customerService.GetCustomerByEmail(email); // ✅ Pass the string email
            return customer != null ? customer.CustomerID : 0;
        }
    }
}