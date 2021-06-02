using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GroceryWebApp.Models
{
    public class OrderHistory
    {
        [NotMapped]
        public int Order_month { get; set; }
        [NotMapped]
        public int Order_year { get; set; }
        [NotMapped]
        public string PName { get; set; }
        [NotMapped]
        public int PID { get; set; }
        [NotMapped]
        public int Order_QTY { get; set; }
        [NotMapped]
        public decimal totalSale { get; set; }

        [NotMapped]
        public string Category { get; set; }
        [NotMapped]
        public decimal Current_price { get; set; }
        [NotMapped]
        public System.DateTime OrderDate { get; set; }
        [NotMapped]
        public string Comment { get; set; }

        [NotMapped]
        public int Discount { get; set; }
    }
}