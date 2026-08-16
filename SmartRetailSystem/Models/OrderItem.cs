using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRetailSystem.Models
{
    public class OrderItem
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ProductName { get; set; }
        public decimal Total => Quantity * Price;
        public string Tax { get; set; }
        public string GrandTotal { get; set; }
    }
}