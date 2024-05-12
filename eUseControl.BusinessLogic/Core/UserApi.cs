using eUseControl.BusinessLogic.AutoMapper;
using eUseControl.BusinessLogic.DBModel;
using eUseControl.Domain.Entities.User;
using eUseControl.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using eUseControl.Domain;
using System.Net;
using eUseControl.Domain.Entities.Product;
using System.Net.Http;

namespace eUseControl.BusinessLogic.Core
{
  public class UserApi
  {


        internal ULoginResp UserLoginAction(UserLogin data)
        {
            ULoginData result;
            var validate = new EmailAddressAttribute();
            if (validate.IsValid(data.Credential))
            {
                var pass = LoginHelper.HashGen(data.Password);
                using (var db = new UserContext())
                {
                    result = db.Users.FirstOrDefault(u => u.Email == data.Credential && u.Password == pass);
                }
            }
            else
            {
                var pass = LoginHelper.HashGen(data.Password);
                using (var db = new UserContext())
                {
                    result = db.Users.FirstOrDefault(u => u.Username == data.Credential && u.Password == pass);
                }
            }

            if (result == null)
            {
                return new ULoginResp { Status = false, StatusMsg = "The Username or Password is Incorrect" };
            }

            using (var todo = new UserContext())
            {
                result.LasIp = data.LoginIp;
                result.LastLogin = data.LoginDateTime;
                todo.Entry(result).State = EntityState.Modified;
                todo.SaveChanges();
            }
            URole userRole = (URole)result.Level;
            return new ULoginResp { Status = true, Level = userRole };
        }

        internal ULoginResp UserRegisterAction(UserRegister data)
        {
            var validate = new EmailAddressAttribute();
            if (!validate.IsValid(data.Email))
            {
                return new ULoginResp { Status = false, StatusMsg = "Invalid email address." };
            }

            var pass = LoginHelper.HashGen(data.Password);
            using (var db = new UserContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // Check if the email is already in use
                        var existingUser = db.Users.FirstOrDefault(u => u.Email == data.Email);
                        if (existingUser != null)
                        {
                            return new ULoginResp { Status = false, StatusMsg = "Email is already in use." };
                        }

                        // Create a new user entry
                        var newUser = new ULoginData
                        {
                            Username = data.Email,
                            Email = data.Email,
                            Password = pass,
                            LastLogin = DateTime.Now,
                            LasIp = data.LoginIp,
                            Level = URole.User // Setting default user level here
                        };

                        db.Users.Add(newUser);
                        db.SaveChanges();
                        transaction.Commit();

                        return new ULoginResp { Status = true, Level = URole.User, StatusMsg = "Registration successful." };
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        // Log exception details here using your preferred logging framework
                        return new ULoginResp { Status = false, StatusMsg = "Registration failed due to a server error." };
                    }
                }
            }
        }

        public bool UpdateUserPassword(string username, string oldPassword, string newPassword)
        {
            using (var db = new UserContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Username == username);
                if (user != null && user.Password == LoginHelper.HashGen(oldPassword))
                {
                    user.Password = LoginHelper.HashGen(newPassword);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

    internal HttpCookie Cookie(string loginCredential)
    {
      using (var db = new SessionContext())
      {
        var validate = new EmailAddressAttribute();
        if (validate.IsValid(loginCredential))
        {
          DateTime cc = DateTime.Now;
          Session curent = db.Sessions.FirstOrDefault(e => (e.ExpireTime > cc && e.Username == loginCredential));
          // System.Diagnostics.Debug.WriteLine("0 "+curent.CookieString+" "+curent.ExpireTime+" "+curent.Username);
          if (curent != null)
          {
            // Session exists and is not expired
            return new HttpCookie("X-KEY", curent.CookieString);
          }
          else
          {
            // Session doesn't exist or is expired
            var apiCookie = new HttpCookie("X-KEY")
            {
              Value = CookieGenerator.Create(loginCredential)
            };

            // Update or create a new session
            curent = db.Sessions.FirstOrDefault(e => e.Username == loginCredential);
            if (curent == null)
            {
              // Create a new session
              curent = new Session
              {
                Username = loginCredential,
                CookieString = apiCookie.Value,
                ExpireTime = DateTime.Now.AddMinutes(60)
              };
              System.Diagnostics.Debug.WriteLine("1 "+curent.CookieString+" "+curent.ExpireTime+" "+curent.Username);
              db.Sessions.Add(curent);
              db.SaveChanges();
            }
            else
            {
              // Update existing session with new cookie and expiration time
              curent.CookieString = apiCookie.Value;
              curent.ExpireTime = DateTime.Now.AddMinutes(60);
              System.Diagnostics.Debug.WriteLine("2 "+curent.CookieString+" "+curent.ExpireTime+" "+curent.Username);
              // db.Entry(curent).State = EntityState.Modified;
              db.SaveChanges();
            }

            // db.SaveChanges();
            return apiCookie;
          }
        }
        else
        {
          // Handle invalid email address
          // For example, throw an exception or return a default cookie
          throw new ArgumentException("Invalid email address.");
        }
      }
    }

        internal Session GetSession(string token)
        {
            using (var sessionDb = new SessionContext())
            {
                Session session = sessionDb.Sessions.FirstOrDefault(s => (s.CookieString == token));

                return session;
            }
        }

        internal void LogoutUser(string token, HttpContextBase httpContext)
        {
            using (var sessionDb = new SessionContext())
            {
                Session session = sessionDb.Sessions.FirstOrDefault(s => (s.CookieString == token));
                if(session != null)
                {
                    session.ExpireTime = DateTime.Now.AddDays(-1);
                    sessionDb.SaveChanges();
                }
            }
            if (httpContext.Request.Cookies["X-KEY"] != null)
            {
                var cookie = new HttpCookie("X-KEY")
                {
                    Expires = DateTime.Now.AddDays(-1),
                    HttpOnly = true
                };
                httpContext.Response.Cookies.Add(cookie);
            }

        }

        internal UserMinimal UserCookie(string cookie)
        {
            using (var sessionDb = new SessionContext())
            {
                DateTime cc = DateTime.Now;
                System.Diagnostics.Debug.WriteLine("string Cookie: " + cookie);
                Session session = sessionDb.Sessions.FirstOrDefault(s => (s.CookieString == cookie) && (s.ExpireTime > cc));
                /*System.Diagnostics.Debug.WriteLine("UserMinimal: " + cc + " " + session.CookieString + " " + cookie + " " + session.ExpireTime);*/
                if (session == null)
                {
                    System.Diagnostics.Debug.WriteLine("Session not found or expired");
                    return null;
                }

                using (var userDb = new UserContext())
                {
                    var validate = new EmailAddressAttribute();
                    ULoginData curentUser = null;
                    if (validate.IsValid(session.Username))
                    {
                        curentUser = userDb.Users.FirstOrDefault(u => u.Email == session.Username);
                    }
                    else
                    {
                        curentUser = userDb.Users.FirstOrDefault(u => u.Username == session.Username);
                    }

                    if (curentUser == null)
                    {
                        System.Diagnostics.Debug.WriteLine("User not found");
                        return null;
                    }
                    // Assuming you've configured AutoMapper somewhere during application startup
                    var mapperConfig = AutoMapperConfig.ConfigureMappings();
                    var mapper = mapperConfig.CreateMapper();
                    // Use the mapper instance to perform mappings
                    var userMinimal = mapper.Map<UserMinimal>(curentUser);
                    return userMinimal;
                }
            }
        }

        internal bool UserSubmitReviewAction(Review data)
        {
            using (var context = new ReviewContext())
            {
                try
                {
                    context.Reviews.Add(data);
                    context.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        internal List<Review> GetReviewsByProductIdAction(int productId)
        {
            using (var context = new ReviewContext())
            {
                return context.Reviews.Where(r => r.ProductId == productId).ToList();
            }
        }

    }
}
