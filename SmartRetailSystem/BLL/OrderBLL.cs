using SmartRetailSystem.DAL;
using SmartRetailSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartRetailSystem.BLL
{
    public class OrderBLL
    {
        OrderDAL dal = new OrderDAL();
        CustomerDAL customerDal = new CustomerDAL();

        // සටහන: මෙහි return type එක string ලෙස තබා ගැනීමෙන් UI එකට message එකක් දිය හැක
        public string SaveOrder(Order order)
        {
            try
            {
                // 1. පාරිභෝගිකයාගේ විස්තර ලබා ගැනීම
                var customer = customerDal.GetCustomerById(order.CustomerID);
                if (customer == null) return "Error: Customer not found.";

                // 2. Rule-based Credit Check
                // ඔබේ Order model එකේ Total වෙනුවට ඇත්තේ TotalAmount විය හැක. 
                // Error එක අනුව මම මෙය order.TotalAmount ලෙස වෙනස් කළෙමි.
                decimal orderTotal = order.GrandTotal; 
                decimal totalDebt = customer.OutstandingBalance + orderTotal;

                if (totalDebt > customer.CreditLimit)
                {
                    return "Order Denied: Credit limit exceeded!";
                }

                // 3. ඇණවුම සුරැකීම
                // මෙහි dal.SaveOrder(order) මගින් int අගයක් (OrderId) ලැබෙන බව උපකල්පනය කෙරේ
                int orderId = dal.SaveOrder(order);

                if (orderId > 0)
                {
                    // 4. සාර්ථක නම් ණය ප්‍රමාණය Update කිරීම
                    customerDal.UpdateBalance(order.CustomerID, orderTotal);
                    return "Success: Order placed successfully.";
                }
                
                return "Error: Could not save order.";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        // 'GetOrdersByCustomer' වෙනුවට ඔබේ DAL එකේ ඇති නම භාවිතා කරන්න
        // ඔබ එවූ ලේඛන අනුව එය බොහෝවිට GetOrders විය හැක
        public List<Order> GetUserOrders(int customerId)
        {
            // ඔබේ OrderDAL එකේ ඇණවුම් ලබා ගන්නා method එකේ නම මෙතැනට යොදන්න
            return dal.GetOrders(customerId); 
        }
    }
}