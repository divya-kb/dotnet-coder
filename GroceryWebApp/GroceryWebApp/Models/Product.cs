using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroceryWebApp
{
    public partial class Product
    {
        private GroceriesWebAppEntities _ctx = new GroceriesWebAppEntities();
        public List<Product> All
        {
            get
            {
                return _ctx.Products.ToList<Product>();

            }
        }
    }
}