using SmartRetailSystem.Classes;
using SmartRetailSystem.DAL;
using SmartRetailSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRetailSystem.BLL
{
    public class CustomerDashboardBLL
    {
        CustomerDashboardDAL dal = new CustomerDashboardDAL();

        public Customer GetCustomer(string email) => dal.GetCustomerByEmail(email);
        public int GetOrderCount(int customerId, string status = null) => dal.GetOrderCount(customerId, status);
        public List<Notification> GetNotifications(int customerId) => dal.GetNotifications(customerId);
        public void MarkNotificationRead(int notificationId) => dal.MarkNotificationRead(notificationId);
    }
}
