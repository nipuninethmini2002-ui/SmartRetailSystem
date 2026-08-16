using SmartRetailSystem.Classes;
using SmartRetailSystem.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRetailSystem.BLL
{
    public class CustomerBLL
    {
        CustomerDAL dal = new CustomerDAL();

        public List<Customer> GetCustomers(string search = "") => dal.GetAllCustomers(search);

        public void RemoveCustomer(int customerId) => dal.DeleteCustomer(customerId);

        public List<Customer> GetAllCustomers() => dal.GetAllCustomers();

        public Customer GetCustomerById(int id) => dal.GetCustomerById(id);
    }
}