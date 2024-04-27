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
      var apiCookie = new HttpCookie("X-KEY")
      {
        Value = CookieGenerator.Create(loginCredential)
      };

      using (var db = new SessionContext())
      {
        Session curent;
        var validate = new EmailAddressAttribute();
        if (validate.IsValid(loginCredential))
        {
          curent = (from e in db.Sessions where e.Username == loginCredential select e).FirstOrDefault();
        }
        else
        {
          curent = (from e in db.Sessions where e.Username == loginCredential select e).FirstOrDefault();
        }

        if (curent != null)
        {
          curent.CookieString = apiCookie.Value;
          curent.ExpireTime = DateTime.Now.AddMinutes(60);
          using (var todo = new SessionContext())
          {
            todo.Entry(curent).State = EntityState.Modified;
            todo.SaveChanges();
          }
        }
        else
        {
          db.Sessions.Add(new Session
          {
            Username = loginCredential,
            CookieString = apiCookie.Value,
            ExpireTime = DateTime.Now.AddMinutes(60)
          });
          db.SaveChanges();
        }
      }

      return apiCookie;
    }

    internal UserMinimal UserCookie(string cookie)
    {
      Session session;
      ULoginData curentUser;

      using (var db = new SessionContext())
      {
        session = db.Sessions.FirstOrDefault(s => s.CookieString == cookie && s.ExpireTime > DateTime.Now);
      }

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
