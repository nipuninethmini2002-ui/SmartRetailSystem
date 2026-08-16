using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRetailSystem.Classes
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public decimal CreditLimit { get; set; } = 2000;
        public decimal OutstandingBalance { get; set; } = 0;
        public string Status { get; set; } = "Active";
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
