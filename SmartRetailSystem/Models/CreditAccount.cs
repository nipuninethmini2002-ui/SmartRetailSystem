using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRetailSystem.Models
{
    public class CreditAccount
    {
        public int CustomerID { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal OutstandingBalance { get; set; }
        public int OverdueDays { get; set; }
        public string Status { get; set; }
        public string CustomerName { get; set; }
        public decimal Balance { get; set; }
    }
}