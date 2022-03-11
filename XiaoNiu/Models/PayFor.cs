using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shopping.Areas.XiaoNiu.Models
{
    public class PayFor
    {
        public int OrderID { get; set; }
        public string ExpressNumber { get; set; }
        public string AddressInfo { get; set; }
        public string Quantity { get; set; }
        public string InvoiceName { get; set; }
    }
}