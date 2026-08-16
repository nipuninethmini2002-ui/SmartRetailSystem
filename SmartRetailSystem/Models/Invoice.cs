using System;
using System.Collections.Generic;

namespace SmartRetailSystem.Models
{
    public class Invoice
    {
        public int OrderID { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal GrandTotal { get; set; }
        public string PaymentStatus { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal Balance { get; set; }
        public string ShippingAddress { get; set; }
        public string ContactNumber { get; set; }
        public string PaymentMethod { get; set; }

        // Items list එක initialize කරලා තියෙන්න ඕන
        public List<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    }

    public class InvoiceItem
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}