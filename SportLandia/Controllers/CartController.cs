using eUseControl.BusinessLogic.Interfaces;
using eUseControl.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eUseControl.Domain.Entities.Product;

namespace TW_WebSite.Controllers
{
    public class CartController : Controller
    {
        private ISession _session;

        public CartController()
        {
            var bl = new BusinessLogic();
            _session = bl.GetSessionBL();
        }

        public ActionResult AddToCart(int id)
        {
            var product = _session.GetProductById(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            Cart cart = Session["Cart"] as Cart;
            if (cart == null)
            {
                cart = new Cart();
            }

            cart.AddItem(product, 1); // Add one item to cart
            Session["Cart"] = cart;

            return RedirectToAction("Index", "Cart");
        }

        public ActionResult Index()
        {
            var cart = Session["Cart"] as Cart;
            ViewBag.CartCount = cart?.Lines.Sum(item => item.Quantity) ?? 0;
            return View(cart);
        }


        public ActionResult RemoveFromCart(int id)
        {
            var cart = Session["Cart"] as Cart;
            if (cart != null)
            {
                cart.RemoveItem(id);
                Session["Cart"] = cart;
            }
            return RedirectToAction("Index");
        }

        [ChildActionOnly]
        public ActionResult CartSummary()
        {
            var cart = Session["Cart"] as Cart;
            ViewBag.CartCount = cart?.Lines.Sum(item => item.Quantity) ?? 0;
            return PartialView("_CartSummary");
        }

    }

}