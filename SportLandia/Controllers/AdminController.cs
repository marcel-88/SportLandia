using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eUseControl.Domain.Entities.User;
using eUseControl.BusinessLogic.Interfaces;
using eUseControl.BusinessLogic;
using System.Net;
using eUseControl.Domain;
using eUseControl.Domain.Entities.Product;

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
            if (Request.Cookies["X-KEY"] != null)
            {
                var token = Request.Cookies["X-KEY"].Value;
                var UserSession = _session.GetSessionByCookie(token);
                var User = _session.GetUserByCookie(token);

                if (UserSession != null && UserSession.ExpireTime > DateTime.Now && User.Level == URole.Admin)
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Auth");
                }
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }
        public ActionResult ManageUsers()
        {
            if (Request.Cookies["X-KEY"] != null)
            {
                var token = Request.Cookies["X-KEY"].Value;
                var UserSession = _session.GetSessionByCookie(token);
                var User = _session.GetUserByCookie(token);

                if (UserSession != null && UserSession.ExpireTime > DateTime.Now && User.Level == URole.Admin)
                {
                    var model = _session.GetAllUsers();
                    ViewBag.Users = model ?? new List<ULoginData>();
                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Auth");
                }
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }

        [HttpPost]
        public ActionResult UpdateUser(ULoginData user)
        {
            if (Request.Cookies["X-KEY"] != null)
            {
                var token = Request.Cookies["X-KEY"].Value;
                var UserSession = _session.GetSessionByCookie(token);
                var User = _session.GetUserByCookie(token);

                if (UserSession != null && UserSession.ExpireTime > DateTime.Now && User.Level == URole.Admin)
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
                else
                {
                    return RedirectToAction("Login", "Auth");
                }
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }


        [HttpPost]
        public ActionResult DeleteUser(int id)
        {
            if (Request.Cookies["X-KEY"] != null)
            {
                var token = Request.Cookies["X-KEY"].Value;
                var UserSession = _session.GetSessionByCookie(token);
                var User = _session.GetUserByCookie(token);

                if (UserSession != null && UserSession.ExpireTime > DateTime.Now && User.Level == URole.Admin)
                {
                    _session.DeleteUser(id);
                    return RedirectToAction("ManageUsers");

                }
                else return RedirectToAction("Login", "Auth");
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }

        public AdminController(ISession session)
        {
            _session = session;
        }

        public ActionResult Products()
        {
            var products = _session.GetAllProducts();
            return View(products);
        }

        public ActionResult CreateProduct()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                _session.CreateProduct(product);
                return RedirectToAction("Products");
            }
            return View(product);
        }

        public ActionResult EditProduct(int id)
        {
            var product = _session.GetProductById(id);
            return View(product);
        }

        [HttpPost]
        public ActionResult EditProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                _session.UpdateProduct(product);
                return RedirectToAction("Products");
            }
            return View(product);
        }

        [HttpPost]
        public ActionResult DeleteProduct(int id)
        {
            _session.DeleteProduct(id);
            return RedirectToAction("Products");
        }
    }
}