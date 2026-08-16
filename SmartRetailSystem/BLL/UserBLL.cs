using SmartRetailSystem.DAL;
using SmartRetailSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRetailSystem.BLL
{
    public class UserBLL
    {
        UserDAL dal = new UserDAL();

        public bool IsEmailRegistered(string email) => dal.EmailExists(email);

        public bool ResetPassword(string email, string passwordHash) => dal.UpdatePassword(email, passwordHash);
        public User Login(string username, string passwordHash)
        {
            return dal.GetUserByUsernameAndPassword(username, passwordHash);
        }

        public bool RegisterUser(User user)
        {
            if (dal.UserExists(user.Username, user.Email))
                return false;

            dal.InsertUser(user);

            if (user.Role == "Customer")
                dal.InsertCustomer(user);

            return true;
        }
    }
}