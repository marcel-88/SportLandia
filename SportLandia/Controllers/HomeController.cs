using eUseControl.BusinessLogic;
using eUseControl.BusinessLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TW_WebSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISession _session;

        public HomeController()
        {
            var bl = new BusinessLogic();
            _session = bl.GetSessionBL();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Shop()
        {
            return View();
        }

        public ActionResult ShopSingle()
        {
            return View();
        }

        public ActionResult UProfile()
        {
            var cookie = Request.Cookies["X-KEY"].Value;
            if (cookie != null)
            {
                var session = _session.GetUserByCookie(cookie);
                if (session != null)  // && session.ExpireTime > DateTime.Now
                {
                    ViewBag.Username = session.Username;
                    return View(new PasswordChangeModel());
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Session Expired");
                    TempData["Error"] = "Session expired. Please log in again.";
                    return RedirectToAction("Login", "Auth");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("No session found");
                TempData["Error"] = "No active session found. Please log in.";
                return RedirectToAction("Login", "Auth");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(PasswordChangeModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = HttpContext.User.Identity.Name;
            if (user == null)
            {
                ModelState.AddModelError("", "User not recognized.");
                return View(model);
            }

            bool result = _session.ChangeUserPassword(user, model.OldPassword, model.NewPassword);
            if (result)
            {
                TempData["Message"] = "Password successfully updated.";
                return RedirectToAction("UProfile");
            }
            else
            {
                ModelState.AddModelError("", "The old password is incorrect or update failed.");
                return View(model);
            }
        }

    }
}