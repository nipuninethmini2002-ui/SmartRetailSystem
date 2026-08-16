using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRetailSystem.Models
{
    public class Delivery
    {
        public int DeliveryID { get; set; }
        public int OrderID { get; set; }
        public int DeliveryPersonID { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public DateTime? AssignedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public string DeliveryPersonName { get; set; }
    }
}