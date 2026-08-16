using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRetailSystem.Models
{
    public class OrderModel
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal GrandTotal { get; set; }
        public string PaymentStatus { get; set; }
        public string DeliveryStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItemModel> Items { get; set; } = new List<OrderItemModel>();
    }
}
