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
            var products = _session.GetAllProducts();
            return View(products);
        }


        public ActionResult ShopSingle(int id)
        {
            var product = _session.GetProductById(id);
            if (product == null)
                return HttpNotFound();

            return View(product);
        }


        public ActionResult UProfile()
        {
            var token = Request.Cookies["X-KEY"].Value;

            if (Request.Cookies["X-KEY"] != null)
            {
                var cookie = Request.Cookies["X-KEY"].Value;
                var UserSession = _session.GetSessionByCookie(token);
                var session = _session.GetUserByCookie(cookie);

                if (UserSession != null && UserSession.ExpireTime > DateTime.Now)
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
                System.Diagnostics.Debug.WriteLine("Everything is not ok, invalid model !!!");
                return View("UProfile", model);
            }

            var user = "";

            if (Request.Cookies["X-KEY"] != null)
            {
                var token = Request.Cookies["X-KEY"].Value;
                var userMinimal = _session.GetUserByCookie(token);
                user = userMinimal.Username;
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            System.Diagnostics.Debug.WriteLine("User", user);
            if (user == null)
            {
                System.Diagnostics.Debug.WriteLine("User not recognized");
                ModelState.AddModelError("", "User not recognized.");
                return View("UProfile", model);
            }

            bool result = _session.ChangeUserPassword(user, model.OldPassword, model.NewPassword);
            if (result)
            {
                System.Diagnostics.Debug.WriteLine("Password updated");
                TempData["Message"] = "Password successfully updated.";
                return RedirectToAction("UProfile");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Old password is incorrext or update failed");
                ModelState.AddModelError("", "The old password is incorrect or update failed.");
                return View("UProfile", model);
            }
        }

    }
}