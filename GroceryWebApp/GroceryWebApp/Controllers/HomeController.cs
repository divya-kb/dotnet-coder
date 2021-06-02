using GroceryWebApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GroceryWebApp.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {

            List<Product> products = _ctx.Products.ToList<Product>();
            ViewBag.Products = products;

            return View();
        }
        public ActionResult VerifyDetail()
        {

            return View();
        }

        public ActionResult ShopDetail(int PID)
        {

            Product product = _ctx.Products.FirstOrDefault(p => p.PID == PID);
            List<Product> similarProducts = _ctx.Products.Where(p => p.Category == product.Category && p.PID != product.PID).ToList<Product>();
            ViewBag.Product = product;
            ViewBag.SimProducts = similarProducts;

            return View();
        }

        public ActionResult WelcomePage()
        {

            return View();
        }

        public ActionResult PieChart(int monthno)
        {

            if (monthno == 9)
            {
                ViewBag.monthly = "September";
            }
            else if (monthno == 10)
            {
                ViewBag.monthly = "October";
            }
            else if (monthno == 8)
            {
                ViewBag.monthly = "August";
            }
            else if (monthno == 7)
            {
                ViewBag.monthly = "July";
            }
            else if (monthno == 6)
            {
                ViewBag.monthly = "June";
            }
            else if (monthno == 5)
            {
                ViewBag.monthly = "May";
            }
            else if (monthno == 4)
            {
                ViewBag.monthly = "April";
            }
            else
            {
                ViewBag.monthly = "All";
            }

            return View();
        }

        public JsonResult GetPiechartJSON()
        {
            List<BlogPieChart> list = new List<BlogPieChart>();

            List<OrderHistory> orderhistory = TempData["orderhistory"] as List<OrderHistory>;

            var test = orderhistory.GroupBy(userInfo => userInfo.Category)
          .OrderBy(group => group.Key)
          .Select(group => Tuple.Create(group.Key, group.Sum(x => Math.Round(Convert.ToDecimal(x.totalSale), 2))));

            list = test.Select(a => new BlogPieChart { CategoryName = a.Item1, PostCount = a.Item2 }).ToList();


            return Json(new { JSONList = list }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPiechartPlannerJSON()
        {
            List<BlogPieChart> list = new List<BlogPieChart>();

            
            var cart = _ctx.Database.SqlQuery<ShoppingCartDetails>("SP_SHOPPINGCART").ToList();
            //List<OrderHistory> orderhistory = TempData["orderhistory"] as List<OrderHistory>;
            var test = cart.GroupBy(userInfo => userInfo.Category)
          .OrderBy(group => group.Key)
          .Select(group => Tuple.Create(group.Key, group.Sum(x => Math.Round(Convert.ToDecimal(x.UnitPrice), 2))));

            list = test.Select(a => new BlogPieChart { CategoryName = a.Item1, PostCount = a.Item2 }).ToList();


            return Json(new { JSONList = list }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Category(string catName)
        {
            List<Product> products;
            if (catName == "")
            {
                products = _ctx.Products.ToList<Product>();
            }
            else
            {
                products = _ctx.Products.Where(p => p.Category == catName).ToList<Product>();
            }
            ViewBag.Products = products;
            ViewBag.Category = catName;

            return View("Index");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Online Grocery Store.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult ProfileDetails()
        {
            ViewBag.Message = "Your Profile";
           

            return View();
        }
        public ActionResult MyProfile()
        {
            ViewBag.Message = "Your Profile";


            return View();
        }
        public ActionResult OrderHistory()
        {

            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@CID",48)

            };
            var data = _ctx.Database.SqlQuery<OrderHistory>("SP_MONTH_HISTORY @CID", param).ToList();
            data = data.OrderByDescending(p => p.OrderDate).ToList();

            ViewBag.OrderHistory = data;
            List<OrderHistory> TempDataTest = new List<OrderHistory>();
            TempDataTest = data;
            TempData["orderhistory"] = TempDataTest;

            return View();

        }

        public ActionResult Planner()
        {
            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@CID",48)

            };
            var data = _ctx.Database.SqlQuery<OrderHistory>("SP_MONTH_PLANNER_PRODUCT @CID", param).ToList();
            data = data.OrderBy(p => p.Category).ToList();

            ViewBag.Edibleoil = data.Where(p => p.Category == "Edible Oil").ToList();
            ViewBag.Detergents = data.Where(p => p.Category == "Detergents").ToList();
            ViewBag.Beverages = data.Where(p => p.Category == "Beverages").ToList();
            ViewBag.DalPulses = data.Where(p => p.Category == "Dal Pulses").ToList();
            ViewBag.Planner = data;
            List<Product> products = _ctx.Products.ToList<Product>();
            ViewBag.Products = products.Where(p => p.Brand == "Grocery Shop" && p.Category != "Sprouts").ToList();
            ViewBag.Sprouts = products.Where(p => p.Category == "Sprouts").ToList();
            //ViewBag.Recommendations = products.Where(p => p.PID == 20 && p.PID == 26 && p.PID == 27 && p.PID == 33 && p.PID == 3032 && p.PID == 3033).ToList();
            ViewBag.Recommendations = products.Where(p => new[] { 20,26,27,33,3032,3033}.Contains(p.PID)).ToList();
            return View();
        }

        public ActionResult AddAllItems()
        {
            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@CID",48)

            };
            var data = _ctx.Database.SqlQuery<OrderHistory>("SP_MONTH_PLANNER_PRODUCT @CID", param).ToList();

            var products = data.Select(p => p.PID).ToList();
            foreach (int pid in products)
            {
                addToCart(pid);
            }

            return RedirectToAction("Planner");
        }

        public ActionResult OrdersMonthwise(int orderName)
        {
            List<OrderHistory> orderhistory;

            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@CID",48)

            };
            var data = _ctx.Database.SqlQuery<OrderHistory>("SP_MONTH_HISTORY @CID", param).ToList();
            data = data.OrderByDescending(p => p.OrderDate).ToList();
            if (orderName == 0)
            {
                orderhistory = data.ToList<OrderHistory>();
            }
            else
            {
                orderhistory = data.Where(p => p.Order_month == orderName).ToList<OrderHistory>();
            }
            ViewBag.OrderHistory = orderhistory;
            string ss = orderName.ToString();
            TempData["OrderMonth"] = ss;

            List<OrderHistory> TempDataTest = new List<OrderHistory>();
            TempDataTest = orderhistory;
            TempData["orderhistory"] = TempDataTest;

            return View("OrderHistory");

        }
        public ActionResult AddToCart(int id)
        {
            addToCart(id);
            return RedirectToAction("Index");
        }
        public ActionResult AddToCartOrders(int id)
        {
            addToCart(id);
            return RedirectToAction("OrderHistory");
        }

        public ActionResult AddToCartPlanner(int id)
        {
            addToCart(id);
            return RedirectToAction("Planner");
        }
        private void addToCart(int pId)
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
                        UnitPrice = product.UnitPrice - product.UnitPrice * product.Discount / 100,
                        Quantity = 1
                    };

                    _ctx.ShoppingCartDatas.Add(cart);
                }
                product.UnitsInStock--;
                _ctx.SaveChanges();
            }
        }
        public ActionResult QuantityChangePlanner(int id)
        {
            removeCart(id);
            return RedirectToAction("Planner");
        }
        private void removeCart(int pId)
        {
            // check if product is valid
            Product product = _ctx.Products.FirstOrDefault(p => p.PID == pId);
            if (product != null && product.UnitsInStock > 0)
            {
                // check if product already existed
                ShoppingCartData cart = _ctx.ShoppingCartDatas.FirstOrDefault(c => c.PID == pId);
                if (cart != null)
                {
                    cart.Quantity--;
                    product.UnitsInStock--;
                    _ctx.ShoppingCartDatas.Remove(cart);
                    _ctx.SaveChanges();
                }
                
                
            }
        }

    }
}