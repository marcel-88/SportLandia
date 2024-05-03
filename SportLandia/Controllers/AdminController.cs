using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eUseControl.Domain.Entities.User;
using eUseControl.BusinessLogic.Interfaces;
using eUseControl.BusinessLogic;

namespace TW_WebSite.Controllers
{
    public class AdminController : Controller
    {
        private readonly ISession _session;

        public AdminController()
        {
            var bl = new BusinessLogic();
            _session = bl.GetSessionBL();
        }
        // GET: Admin
        public ActionResult AdminChangeProducts()
        {
            return View();
        }
        public ActionResult ManageUsers()
        {
            var model = _session.GetAllUsers();
            ViewBag.Users = model ?? new List<ULoginData> ();
            return View();
        }

        [HttpPost]
        public ActionResult UpdateUser(ULoginData user)
        {
            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    if (entry.Value.Errors.Any())
                    {
                        foreach (var error in entry.Value.Errors)
                        {
                            System.Diagnostics.Debug.WriteLine($"{entry.Key}: {error.ErrorMessage}");
                        }
                    }
                }
            }

            if (ModelState.IsValid)
            {
                bool updateResult = _session.UpdateUser(user);
                if (updateResult)
                {
                    TempData["Message"] = "User updated successfully.";
                    return RedirectToAction("ManageUsers");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to update user.");
                }
            }

            var errorMessages = new List<string>();
            foreach (var entry in ModelState)
            {
                foreach (var error in entry.Value.Errors)
                {
                    errorMessages.Add($"{entry.Key}: {error.ErrorMessage}");
                }
            }

            TempData["ErrorMessages"] = errorMessages;

            var users = _session.GetAllUsers();
            ViewBag.Users = users ?? new List<ULoginData>();
            return View("ManageUsers");
        }





        [HttpPost]
        public ActionResult DeleteUser(int id)
        {
            _session.DeleteUser(id);
            return RedirectToAction("ManageUsers");
        }
        public AdminController(ISession session)
        {
            _session = session;
        }
    }
}