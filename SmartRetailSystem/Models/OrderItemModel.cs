using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRetailSystem.Models
{
    public class OrderItemModel
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total => Quantity * Price;
        public decimal SubTotal => Quantity * Price;
    }
}
