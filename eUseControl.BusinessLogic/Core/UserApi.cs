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

    internal HttpCookie Cookie(string loginCredential)
    {
      using (var db = new SessionContext())
      {
        var validate = new EmailAddressAttribute();
        if (validate.IsValid(loginCredential))
        {
          var curent = db.Sessions.FirstOrDefault(e => e.Username == loginCredential && e.ExpireTime > DateTime.Now);
          if (curent == null)
          {
            var apiCookie = new HttpCookie("X-KEY")
            {
              Value = CookieGenerator.Create(loginCredential)
            };
            curent = new Session
            {
              Username = loginCredential,
              CookieString = apiCookie.Value,
              ExpireTime = DateTime.Now.AddMinutes(60)
            };
            db.Sessions.Add(curent);
            db.SaveChanges();
            return apiCookie;
          }
          else
          {
            return new HttpCookie("X-KEY", curent.CookieString);
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


    internal UserMinimal UserCookie(string cookie)
    {
      Session session;
      ULoginData curentUser;

      using (var db = new SessionContext())
      {
        session = db.Sessions.FirstOrDefault(s => s.CookieString == cookie && s.ExpireTime > DateTime.Now);
      }

      // System.Diagnostics.Debug.WriteLine("1"+session.ToString());

      if (session == null) return null;
      using (var db = new UserContext())
      {
        var validate = new EmailAddressAttribute();
        if (validate.IsValid(session.Username))
        {
          curentUser = db.Users.FirstOrDefault(u => u.Email == session.Username);
        }
        else
        {
          curentUser = db.Users.FirstOrDefault(u => u.Username == session.Username);
        }
      }

      // System.Diagnostics.Debug.WriteLine("2"+curentUser.ToString());

      if (curentUser == null) return null;
      // Assuming you've configured AutoMapper somewhere during application startup
            var mapperConfig = AutoMapperConfig.ConfigureMappings();
            var mapper = mapperConfig.CreateMapper();
        // Use the mapper instance to perform mappings
      var userminimal = mapper.Map<UserMinimal>(curentUser);

      // Old Code
      // Mapper.Initialize(cfg => cfg.CreateMap<ULoginData, UserMinimal>());
      // var userminimal = Mapper.Map<UserMinimal>(curentUser);

      return userminimal;
    }
  }
}
