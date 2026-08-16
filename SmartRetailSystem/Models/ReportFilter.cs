using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRetailSystem.Models
{
    public class ReportFilter
    {
        public int CustomerID { get; set; } = 0; // 0 = all
        public int ProductID { get; set; } = 0;  // 0 = all
        public string FromDate { get; set; } = null;
        public string ToDate { get; set; } = null;
    }
}