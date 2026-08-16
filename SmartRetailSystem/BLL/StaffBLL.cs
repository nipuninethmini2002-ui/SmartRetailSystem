using SmartRetailSystem.DAL;
using SmartRetailSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRetailSystem.BLL
{
    public class StaffBLL
    {
        StaffDAL dal = new StaffDAL();

        public StaffSummary GetSummary() => dal.GetStaffSummary();
    }
}
