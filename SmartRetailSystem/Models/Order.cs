using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRetailSystem.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public string DeliveryStatus { get; set; }
        public int CustomerID { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal GrandTotal { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        public Delivery DeliveryInfo { get; set; }
        public string CustomerName { get; set; }
        public decimal PaidAmount => PaymentStatus == "Paid" ? GrandTotal : 0m;
        public decimal Balance => GrandTotal - PaidAmount;

        public List<InvoiceItem> Item { get; set; } = new List<InvoiceItem>();

    }
}