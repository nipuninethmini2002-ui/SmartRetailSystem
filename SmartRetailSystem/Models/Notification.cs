using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRetailSystem.Models
{
    public class Notification
    {
        public int NotificationID { get; set; }
        public int CustomerID { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public string CustomerName { get; set; }
    }
}