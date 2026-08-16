using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRetailSystem.Models
{
    public class StaffSummary
    {
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int TotalCustomers { get; set; }
        public decimal OutstandingCredit { get; set; }
        public int LowStock { get; set; }
    }
}