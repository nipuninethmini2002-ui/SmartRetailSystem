using SmartRetailSystem.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRetailSystem.Services
{
    public class DashboardService
    {
        DashboardBLL bll = new DashboardBLL();

        public int GetTotalProducts() => bll.TotalProducts();
        public int GetTotalOrders() => bll.TotalOrders();
        public int GetPendingOrders() => bll.PendingOrders();
        public int GetTotalCustomers() => bll.TotalCustomers();
        public decimal GetTotalOutstandingCredit() => bll.TotalOutstandingCredit();
        public List<int> GetLowStockProductIDs() => bll.LowStockProductIDs();
    }
}