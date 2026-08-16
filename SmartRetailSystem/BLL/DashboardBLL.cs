using SmartRetailSystem.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRetailSystem.BLL
{
    public class DashboardBLL
    {
        DashboardDAL dal = new DashboardDAL();

        public int TotalProducts() => dal.GetTotalProducts();
        public int TotalOrders() => dal.GetTotalOrders();
        public int PendingOrders() => dal.GetPendingOrders();
        public int TotalCustomers() => dal.GetTotalCustomers();
        public decimal TotalOutstandingCredit() => dal.GetTotalOutstandingCredit();
        public List<int> LowStockProductIDs() => dal.GetLowStockProductIDs();
    }
}