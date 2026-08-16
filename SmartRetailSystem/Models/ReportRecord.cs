using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRetailSystem.Models
{
    public class ReportRecord
    {
        public int OrderID { get; set; }
        public string Customer { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal GrandTotal { get; set; }
        public string PaymentStatus { get; set; }
        public string DeliveryStatus { get; set; }
    }
}