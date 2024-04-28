using eUseControl.BusinessLogic.DBModel;
using eUseControl.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace eUseControl.BusinessLogic.Core
{
    public class AdminApi
    {
        public bool UpdateUserDetails(ULoginData userData)
        {
            using (var db = new UserContext())
            {
                var existingUser = db.Users.Find(userData.Id);
                if (existingUser != null)
                {
                    existingUser.Username = userData.Username;
                    existingUser.Email = userData.Email;
                    existingUser.Password = userData.Password;
                    existingUser.LastLogin = userData.LastLogin;
                    existingUser.LasIp = userData.LasIp;
                    existingUser.Level = userData.Level;

                    try
                    {
                        db.Entry(existingUser).State = EntityState.Modified;
                        db.SaveChanges();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle it as needed
                        Console.WriteLine($"An error occurred: {ex.Message}");
                        return false;
                    }
                }
                return false;
            }
        }


        public void DeleteUser(int userId)
        {
            using (var db = new UserContext())
            {
                var user = db.Users.Find(userId);
                if (user != null)
                {
                    db.Users.Remove(user);
                    db.SaveChanges();
                }
            }
        }
        public List<ULoginData> FetchAllUsers()
        {
            using (var db = new UserContext())
            {
                return db.Users.ToList();
            }
        }
    }
}
