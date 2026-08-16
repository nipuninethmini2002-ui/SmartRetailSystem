using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRetailSystem.Models
{
    public class Payment
    {
        public int PaymentID { get; set; }
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public decimal PaidAmount { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; }
        public string PaymentStatus { get; set; }
    }
}