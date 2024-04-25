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
    public class LoginController : Controller
    {
        private readonly ISession _session;

        public LoginController()
        {
            var bl = new BusinessLogic();
            _session = bl.GetSessionBL();
        }

        public ActionResult Login()
{
    return View("~/Views/Auth/Login.cshtml");
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
            return View("~/Views/Auth/Register.cshtml");
        }
    }

}