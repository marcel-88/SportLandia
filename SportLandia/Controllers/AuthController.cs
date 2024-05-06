using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using eUseControl.BusinessLogic;
using eUseControl.BusinessLogic.Interfaces;
using eUseControl.Domain;
using eUseControl.Domain.Entities.User;

namespace TW_WebSite.Controllers
{
    public class AuthController : Controller
    {
        private readonly ISession _session;

        public AuthController()
        {
            var bl = new BusinessLogic();
            _session = bl.GetSessionBL();
        }

        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(UserLogin login)
        {
            if (ModelState.IsValid)
            {
                UserLogin data = new UserLogin
                {
                    Credential = login.Credential,
                    Password = login.Password,
                    LoginIp = Request.UserHostAddress,
                    LoginDateTime = DateTime.Now,
                };

                var userLogin = _session.UserLogin(data);
                if (userLogin.Status)
                {
                    // Generate session token
                    var sessionToken = _session.GenCookie(login.Credential);

                    // Set session token cookie
                    Response.Cookies.Add(sessionToken);

          System.Diagnostics.Debug.WriteLine(sessionToken);

          if (Request.Cookies["X-KEY"] != null)
          {
            var token = Request.Cookies["X-KEY"].Value;
            var userMinimal = _session.GetUserByCookie(token);
            System.Diagnostics.Debug.WriteLine(token.ToString());
          }
          else
          {
            return RedirectToAction("Login", "Account");
          }

                    if (userLogin.Level == URole.Admin)
                    {
                        return RedirectToAction("AdminChangeProducts", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                    return RedirectToAction("About","Home");
                }
            }
            else
            {
                return RedirectToAction("About", "Home");

            }
        }





        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(UserRegister register)
        {
            if (ModelState.IsValid)
            {
                UserRegister data = new UserRegister
                {
                    Email = register.Email,
                    Username = register.Email,
                    Password = register.Password,
                    LoginIp = Request.UserHostAddress,
                    LoginDateTime = DateTime.Now,
                    Role = URole.User,
                };

            var userRegister = _session.UserRegister(data);
                 if (userRegister.Status)
                 {
                    ViewBag.Status = "Succes";
                       return RedirectToAction("Index", "Home");
                 } else
                {
                    ViewBag.Status = "Insucces";
                    return View("Register");
                }

            }

            
            return View("Register");
        }

    }

}