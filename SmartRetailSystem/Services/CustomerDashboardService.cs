using SmartRetailSystem.BLL;
using SmartRetailSystem.Classes;
using SmartRetailSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRetailSystem.Services
{
    public class CustomerDashboardService
    {
        CustomerDashboardBLL bll = new CustomerDashboardBLL();

        public Customer GetCustomer(string email) => bll.GetCustomer(email);
        public int GetOrderCount(int customerId, string status = null) => bll.GetOrderCount(customerId, status);
        public List<Notification> GetNotifications(int customerId) => bll.GetNotifications(customerId);
        public void MarkNotificationRead(int notificationId) => bll.MarkNotificationRead(notificationId);
    }
}
