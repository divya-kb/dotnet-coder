using GroceryWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GroceryWebApp.Controllers
{
    public class CheckoutController : BaseController
    {
        private List<object> states;
        private List<object> cards;

        public CheckoutController()
        {
            states = new List<object> {
                new { SID = "NSW", SName = "New South Wales" },
                new { SID = "VIC", SName = "Victoria" },
                new { SID = "QLD", SName = "Queensland" },
                new { SID = "TAs", SName = "Tasmania" },
                new { SID = "NT", SName = "Northern Territory" },
                new { SID = "SA", SName = "South Australia" },
                new { SID = "WA", SName = "Western Australia" },
                new { SID = "ACT", SName = "Australian Capital Territory" }
            };

            cards = new List<object> {
                new { Type = "VISA" },
                new { Type = "Master Card" },
                new { Type = "AMEX" }
            };

        }

        // GET: Checkout
        public ActionResult Index()
        {
            //ViewBag.Cart = _ctx.ShoppingCartDatas.ToList<ShoppingCartData>();
            ViewBag.Cart = _ctx.Database.SqlQuery<ShoppingCartDetails>("SP_SHOPPINGCART").ToList();
           
            return View();
        }

        public JsonResult QuanityChange(int type, int pId)
        {
            GroceriesWebAppEntities context = new GroceriesWebAppEntities();

            ShoppingCartData product = context.ShoppingCartDatas.FirstOrDefault(p => p.PID == pId);
            if (product == null)
            {
                return Json(new { d = "0" });
            }

            Product actualProduct = context.Products.FirstOrDefault(p => p.PID == pId);
            int quantity;
            // if type 0, decrease quantity
            // if type 1, increase quanity
            switch (type)
            {
                case 0:
                    product.Quantity--;
                    actualProduct.UnitsInStock++;
                    break;
                case 1:
                    product.Quantity++;
                    actualProduct.UnitsInStock--;
                    break;
                case -1:
                    actualProduct.UnitsInStock += product.Quantity;
                    product.Quantity = 0;
                    break;
                default:
                    return Json(new { d = "0" });
            }

            if (product.Quantity == 0)
            {
                context.ShoppingCartDatas.Remove(product);
                quantity = 0;
            }
            else
            {
                quantity = product.Quantity;
            }

            context.SaveChanges();
            return Json(new { d = quantity });
        }
        
        public JsonResult AddToCartDetail(int quan, int pId)
        {
            // check if product is valid
            Product product = _ctx.Products.FirstOrDefault(p => p.PID == pId);
            if (product != null && product.UnitsInStock > 0)
            {
                // check if product already existed
                ShoppingCartData cart = _ctx.ShoppingCartDatas.FirstOrDefault(c => c.PID == pId);
                if (cart != null)
                {
                    cart.Quantity++;
                }
                else
                {

                    cart = new ShoppingCartData
                    {
                        PName = product.PName,
                        PID = product.PID,
                        UnitPrice = product.UnitPrice,
                        Quantity = quan
                    };

                    _ctx.ShoppingCartDatas.Add(cart);
                }
                product.UnitsInStock--;
                _ctx.SaveChanges();
               
            }
            return Json(new { d = 2 });
        }
        [HttpGet]
        public JsonResult UpdateTotal()
        {
            GroceriesWebAppEntities context = new GroceriesWebAppEntities();
            decimal total;
            try
            {

                total = context.ShoppingCartDatas.Select(p => p.UnitPrice * p.Quantity).Sum();
            }
            catch (Exception) { total = 0; }

            return Json(new { d = String.Format("{0:c}", total) }, JsonRequestBehavior.AllowGet);
        }

        
        public JsonResult UpdateIndividualPrice(int pId)
        {
            GroceriesWebAppEntities context = new GroceriesWebAppEntities();
            decimal total;
            try
            {

                ShoppingCartData product = context.ShoppingCartDatas.FirstOrDefault(p => p.PID == pId);
                total = product.Quantity * product.UnitPrice;
            }
            catch (Exception) { total = 0; }

            return Json(new { d = String.Format("{0:c}", total) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Clear()
        {
            try
            {
                List<ShoppingCartData> carts = _ctx.ShoppingCartDatas.ToList();
                carts.ForEach(a => {
                    Product product = _ctx.Products.FirstOrDefault(p => p.PID == a.PID);
                    product.UnitsInStock += a.Quantity;
                });
                _ctx.ShoppingCartDatas.RemoveRange(carts);
                _ctx.SaveChanges();
            }
            catch (Exception) { }
            return RedirectToAction("Index", "Home", null);
        }

        public ActionResult Purchase()
        {
            ViewBag.States = states;
            ViewBag.Cards = cards;

            //Customer customer;

            var customer = _ctx.Customers.First(p => p.CID == 48);
            //GroceryWebApp.Models.Customer cust = customer;
            //ViewBag.Customer = customer;

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Purchase(GroceryWebApp.Customer customer)
        {
            ViewBag.States = states;
            ViewBag.Cards = cards;

            if (ModelState.IsValid)
            {
                if (customer.ExpDate <= DateTime.Now)
                {
                    ModelState.AddModelError("", "Credit card has already expired");
                }

                if (customer.Ctype == "AMEX")
                {
                    if (customer.CardNo.Length != 15)
                    {
                        ModelState.AddModelError("", "AMEX must be 15 digits");
                    }
                }
                else
                {
                    if (customer.CardNo.Length != 16)
                    {
                        ModelState.AddModelError("", customer.Ctype + "must be 16 digits");
                    }
                }

                if (ModelState.IsValid)
                {
                    //Customer c = new Customer
                    //{
                    //    FName = customer.FName,
                    //    LName = customer.LName,
                    //    Email = customer.Email,
                    //    Phone = customer.Phone,
                    //    Address1 = customer.Address1,
                    //    Address2 = customer.Address2,
                    //    Suburb = customer.Suburb,
                    //    Postcode = customer.Postcode,
                    //    State = customer.State,
                    //    Ctype = customer.Ctype,
                    //    CardNo = customer.CardNo,
                    //    ExpDate = customer.ExpDate
                    //};

                    Order o = new Order
                    {
                        OrderDate = DateTime.Now,
                        DeliveryDate = DateTime.Now.AddDays(5),
                        CID = 48 
                    };

                    //_ctx.Customers.Add(c);
                    _ctx.Orders.Add(o);

                    Customer prodToUpdate = _ctx.Customers.Where(p => p.CID == 48).FirstOrDefault();

                    if (prodToUpdate != null)
                    {
                        // Just updating one property to demonstrate....

                        prodToUpdate.FName = customer.FName;
                        prodToUpdate.LName = customer.LName;
                        prodToUpdate.Email = customer.Email;
                        prodToUpdate.Phone = customer.Phone;
                        prodToUpdate.Address1 = customer.Address1;
                        prodToUpdate.Address2 = customer.Address2;
                        prodToUpdate.Suburb = customer.Suburb;
                        prodToUpdate.Postcode = customer.Postcode;
                        prodToUpdate.State = customer.State;
                        prodToUpdate.Ctype = customer.Ctype;
                        prodToUpdate.CardNo = customer.CardNo;
                        prodToUpdate.ExpDate = customer.ExpDate;
                    }

                    foreach (var i in _ctx.ShoppingCartDatas.ToList<ShoppingCartData>())
                    {
                        _ctx.Order_Products.Add(new Order_Products
                        {
                            OrderID = o.OrderID,
                            PID = i.PID,
                            Qty = i.Quantity,
                            TotalSale = i.Quantity * i.UnitPrice
                        });
                        _ctx.ShoppingCartDatas.Remove(i);
                    }

                    _ctx.SaveChanges();

                    return RedirectToAction("PurchasedSuccess");

                }
            }

            List<ModelError> errors = new List<ModelError>();
            foreach (ModelState modelState in ViewData.ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    errors.Add(error);
                }
            }
            return View(customer);
        }

        public ActionResult PurchasedSuccess()
        {
            return View();
        }
    }
}
