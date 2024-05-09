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
            if (Request.Cookies["X-KEY"] != null)
            {
                var token = Request.Cookies["X-KEY"].Value;
                var UserSession = _session.GetSessionByCookie(token);
                var User = _session.GetUserByCookie(token);

                if (UserSession != null && UserSession.ExpireTime > DateTime.Now && User.Level == URole.Admin)
                {
                    var products = _session.GetAllProductsIncludingCategories();
                    return View(products);
                }
                else return RedirectToAction("Login", "Auth");
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }

        public ActionResult CreateProduct()
        {
            if (Request.Cookies["X-KEY"] != null)
            {
                var token = Request.Cookies["X-KEY"].Value;
                var UserSession = _session.GetSessionByCookie(token);
                var User = _session.GetUserByCookie(token);

                if (UserSession != null && UserSession.ExpireTime > DateTime.Now && User.Level == URole.Admin)
                {
                    ViewBag.Categories = _session.GetAllCategories()
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToList();
                    return View();
                }
                else return RedirectToAction("Login", "Auth");
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }

        [HttpPost]
        public ActionResult CreateProduct(Product product)
        {
            if (Request.Cookies["X-KEY"] != null)
            {
                var token = Request.Cookies["X-KEY"].Value;
                var UserSession = _session.GetSessionByCookie(token);
                var User = _session.GetUserByCookie(token);

                if (UserSession != null && UserSession.ExpireTime > DateTime.Now && User.Level == URole.Admin)
                {
                    if (ModelState.IsValid)
                    {
                        _session.CreateProduct(product);
                        return RedirectToAction("Products");
                    }
                    ViewBag.Categories = new SelectList(_session.GetAllCategories(), "Id", "Name");
                    return View(product);
                }
                else return RedirectToAction("Login", "Auth");
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }

        public ActionResult EditProduct(int id)
        {
            if (Request.Cookies["X-KEY"] != null)
            {
                var token = Request.Cookies["X-KEY"].Value;
                var UserSession = _session.GetSessionByCookie(token);
                var User = _session.GetUserByCookie(token);

                if (UserSession != null && UserSession.ExpireTime > DateTime.Now && User.Level == URole.Admin)
                {
                    var product = _session.GetProductById(id);
                    if (product == null)
                    {
                        return HttpNotFound();
                    }
                    ViewBag.Categories = _session.GetAllCategories()
                        .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                        .ToList();
                    return View(product);
                }
                else return RedirectToAction("Login", "Auth");
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }

        [HttpPost]
        public ActionResult EditProduct(Product product)
        {
            if (Request.Cookies["X-KEY"] != null)
            {
                var token = Request.Cookies["X-KEY"].Value;
                var UserSession = _session.GetSessionByCookie(token);
                var User = _session.GetUserByCookie(token);

                if (UserSession != null && UserSession.ExpireTime > DateTime.Now && User.Level == URole.Admin)
                {
                    if (ModelState.IsValid)
                    {
                        _session.UpdateProduct(product);
                        return RedirectToAction("Products");
                    }
                    ViewBag.Categories = new SelectList(_session.GetAllCategories(), "Id", "Name", product.CategoryId);
                    return View(product);
                }
                else return RedirectToAction("Login", "Auth");
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }

        [HttpPost]
        public ActionResult DeleteProduct(int id)
        {
            if (Request.Cookies["X-KEY"] != null)
            {
                var token = Request.Cookies["X-KEY"].Value;
                var UserSession = _session.GetSessionByCookie(token);
                var User = _session.GetUserByCookie(token);

                if (UserSession != null && UserSession.ExpireTime > DateTime.Now && User.Level == URole.Admin)
                {
                    _session.DeleteProduct(id);
                    return RedirectToAction("Products");
                }
                else return RedirectToAction("Login", "Auth");
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }


        public ActionResult Categories()
        {
            if (Request.Cookies["X-KEY"] != null)
            {
                var token = Request.Cookies["X-KEY"].Value;
                var UserSession = _session.GetSessionByCookie(token);
                var User = _session.GetUserByCookie(token);

                if (UserSession != null && UserSession.ExpireTime > DateTime.Now && User.Level == URole.Admin)
                {
                    var categories = _session.GetAllCategories();
                    return View(categories);
                }
                else return RedirectToAction("Login", "Auth");
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }

        // Create Category - GET
        public ActionResult CreateCategory()
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
                else return RedirectToAction("Login", "Auth");
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }

        // Create Category - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCategory(Category category)
        {
            if (Request.Cookies["X-KEY"] != null)
            {
                var token = Request.Cookies["X-KEY"].Value;
                var UserSession = _session.GetSessionByCookie(token);
                var User = _session.GetUserByCookie(token);

                if (UserSession != null && UserSession.ExpireTime > DateTime.Now && User.Level == URole.Admin)
                {
                    if (ModelState.IsValid)
                    {
                        _session.CreateCategory(category);
                        return RedirectToAction("Categories");
                    }
                    return View(category);
                }
                else return RedirectToAction("Login", "Auth");
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }

        // Edit Category - GET
        public ActionResult EditCategory(int id)
        {
            if (Request.Cookies["X-KEY"] != null)
            {
                var token = Request.Cookies["X-KEY"].Value;
                var UserSession = _session.GetSessionByCookie(token);
                var User = _session.GetUserByCookie(token);

                if (UserSession != null && UserSession.ExpireTime > DateTime.Now && User.Level == URole.Admin)
                {
                    var category = _session.GetCategoryById(id);
                    if (category == null)
                    {
                        return HttpNotFound();
                    }
                    return View(category);
                }
                else return RedirectToAction("Login", "Auth");
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }

        // Edit Category - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCategory(Category category)
        {
            if (Request.Cookies["X-KEY"] != null)
            {
                var token = Request.Cookies["X-KEY"].Value;
                var UserSession = _session.GetSessionByCookie(token);
                var User = _session.GetUserByCookie(token);

                if (UserSession != null && UserSession.ExpireTime > DateTime.Now && User.Level == URole.Admin)
                {
                    if (ModelState.IsValid)
                    {
                        _session.UpdateCategory(category);
                        return RedirectToAction("Categories");
                    }
                    return View(category);
                }
                else return RedirectToAction("Login", "Auth");
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }

        // Delete Category
        [HttpPost]
        public ActionResult DeleteCategory(int id)
        {
            if (Request.Cookies["X-KEY"] != null)
            {
                var token = Request.Cookies["X-KEY"].Value;
                var UserSession = _session.GetSessionByCookie(token);
                var User = _session.GetUserByCookie(token);

                if (UserSession != null && UserSession.ExpireTime > DateTime.Now && User.Level == URole.Admin)
                {
                    _session.DeleteCategory(id);
                    return RedirectToAction("Categories");
                }
                else return RedirectToAction("Login", "Auth");
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }
    }
}