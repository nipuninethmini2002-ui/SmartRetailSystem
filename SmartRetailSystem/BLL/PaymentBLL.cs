using SmartRetailSystem.DAL;
using SmartRetailSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRetailSystem.BLL
{
    public class PaymentBLL
    {
        private PaymentDAL dal = new PaymentDAL();

        public int ProcessPayment(OrderModel order, PaymentModel payment)
        {
            int orderId = dal.InsertOrder(order); // 1
            payment.OrderID = orderId;
            dal.InsertPayment(payment); // 2
            dal.InsertOrderItems(orderId, order.Items); // 3

            // අනිවාර්යයෙන්ම මෙම පේළිය මෙතැන තිබිය යුතුයි!
            dal.UpdateCustomerOutstandingBalance(order.CustomerID);

            return orderId;
        }
    }
}