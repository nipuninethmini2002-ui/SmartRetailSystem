using SmartRetailSystem.DAL;
using SmartRetailSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRetailSystem.BLL
{
    public class ReportBLL
    {
        private ReportDAL dal = new ReportDAL();

        public List<ReportRecord> GetReports(ReportFilter filter)
        {
            // You can add business logic here if needed (e.g., validate dates)
            return dal.GetOrders(filter);
        }
    }
}