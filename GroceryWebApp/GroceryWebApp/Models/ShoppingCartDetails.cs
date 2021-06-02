using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GroceryWebApp.Models
{
    public class ShoppingCartDetails
    {
        public int TempOrderID { get; set; }
        public int PID { get; set; }
        public string PName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        [NotMapped]
        public decimal OriginalPrice { get; set; }
        [NotMapped]
        public string Category { get; set; }
    }
}