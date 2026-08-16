using SmartRetailSystem.BLL;
using SmartRetailSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRetailSystem.Services
{
    public class ReportService
    {
        private ReportBLL bll = new ReportBLL();

        public List<ReportRecord> GetReports(ReportFilter filter)
        {
            return bll.GetReports(filter);
        }
    }
}