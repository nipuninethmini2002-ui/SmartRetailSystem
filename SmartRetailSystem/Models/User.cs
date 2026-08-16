using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRetailSystem.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public string CustomerID { get; set; }
    }
    public class ForgotPasswordRequest
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }
}
