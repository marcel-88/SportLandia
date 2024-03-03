using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TW_WebSite.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult AdminChangeProducts()
        {
            return View();
        }
    }
}