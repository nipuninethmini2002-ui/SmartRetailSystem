using SmartRetailSystem.BLL;
using SmartRetailSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRetailSystem.Services
{
    public class StaffService
    {
        StaffBLL bll = new StaffBLL();

        public StaffSummary GetSummary() => bll.GetSummary();
    }
}