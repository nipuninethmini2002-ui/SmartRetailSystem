using SmartRetailSystem.DAL;
using SmartRetailSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRetailSystem.BLL
{
    public class ProductBLL
    {
        ProductDAL dal = new ProductDAL();

        public List<Product> GetAllProducts() => dal.GetAllProducts();

        public Product GetProductById(int id) => dal.GetProductById(id);

        public void UpdateStock(int productId, int qty, System.Data.SqlClient.SqlConnection con, System.Data.SqlClient.SqlTransaction tr)
            => dal.UpdateStock(productId, qty, con, tr);

        public List<Product> GetProducts(string search = "")
        {
            return dal.GetProducts(search);
        }

        public void DeleteProduct(int productId)
        {
            dal.DeleteProduct(productId);
        }

        public List<Product> GetLowStockProducts(int threshold = 5)
        {
            return dal.GetLowStockProducts(threshold);
        }
    }

}