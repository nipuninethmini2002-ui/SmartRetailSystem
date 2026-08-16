using SmartRetailSystem.DAL;
using SmartRetailSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRetailSystem.BLL
{
    public class DeliveryBLL
    {
        DeliveryDAL dal = new DeliveryDAL();

        public List<Delivery> GetDeliveries(int deliveryPersonId) => dal.GetDeliveries(deliveryPersonId);
        public void UpdateDeliveryStatus(int deliveryId, string status) => dal.UpdateDeliveryStatus(deliveryId, status);

    }
}
