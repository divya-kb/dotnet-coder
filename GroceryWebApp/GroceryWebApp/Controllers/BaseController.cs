using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GroceryWebApp.Controllers
{
    public class BaseController : Controller
    {
        protected GroceriesWebAppEntities _ctx = new GroceriesWebAppEntities();

        public BaseController()
        {
            ViewBag.CartTotalPrice = CartTotalPrice;
            ViewBag.Cart = Cart;
            ViewBag.CartUnits = Cart.Count;
            
        }
        private List<ShoppingCartData> Cart
        {
            get
            {
                return _ctx.ShoppingCartDatas.ToList();
            }
        }
        private decimal CartTotalPrice
        {
            get
            {
                return Cart.Sum(c => c.Quantity * c.UnitPrice);
            }
        }
        

    }
}