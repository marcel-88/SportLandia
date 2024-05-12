using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
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

        public ActionResult Logout()
        {

            if (Request.Cookies["X-KEY"] != null)
            {
                var token = Request.Cookies["X-KEY"].Value;
                _session.LogoutUserByCookie(token, HttpContext);
            }

            Session.Clear();
            FormsAuthentication.SignOut();

            return RedirectToAction("Login", "Auth");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(UserLogin login)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("About", "Home");
            }

            // Initialize the login details
            UserLogin data = new UserLogin
            {
                Credential = login.Credential,
                Password = login.Password,
                LoginIp = Request.UserHostAddress,
                LoginDateTime = DateTime.Now,
            };

            // Attempt to log the user in
            var userLogin = _session.UserLogin(data);
            if (!userLogin.Status)
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return RedirectToAction("About", "Home");
            }

            // Generate session token
            var sessionToken = _session.GenCookie(login.Credential);

            // Set session token cookie
            Response.Cookies.Add(sessionToken);

            // Check if the cookie exists after setting it
            var cookie = Request.Cookies["X-KEY"];
            if (cookie == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Log the token from the cookie
            var token = cookie.Value;

            // Fetch user details using the token
            var userMinimal = _session.GetUserByCookie(token);
            if (userMinimal == null)
            {
                System.Diagnostics.Debug.WriteLine("No user found with the given token.");
                return RedirectToAction("Login", "Account");
            }

            // Store username in session
            Session["Username"] = userMinimal.Username;

            // Redirect based on user role
            if (userLogin.Level == URole.Admin)
            {
                return RedirectToAction("AdminChangeProducts", "Admin");
            }

            return RedirectToAction("Index", "Home");
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