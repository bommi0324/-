using Shopping.Areas.Admin.Models;
using Shopping.Areas.XiaoNiu.Models;
using Shopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shopping.Areas.XiaoNiu.Controllers
{
    [MyFilter(IsCheck = false)]
    [QTFilter]
    public class PayController : Controller
    {
        ShopDBEntities1 db = new ShopDBEntities1();
        // GET: XiaoNiu/Pay
        public ActionResult TranPay(string number)
        {
            Response.Cookies["number"].Value = number;
            Order order = db.Order.Where(x => x.ExpressNumber == number).First();
            //获取购物车ID
            string ids = Request.Cookies["ids"].Value;
            if (ids != null)  //要购买的商品ID不为空
            {
                string[] id = ids.Split('|');
                ViewBag.count = id.Length - 1;
                foreach (string item in id)
                {
                    if (item!="")
                    {
                        int carId = int.Parse(item);
                        ShopCar cc = db.ShopCar.Where(x => x.Id == carId).First();
                        int proID = cc.ProperID.Value, quantity = cc.Quantity.Value;
                        ProductProperty pp = db.ProductProperty.Where(x => x.ProperID == proID).First();
                        ProductType pt = db.ProductType.Where(x => x.TypeID == pp.TypeID).First();
                        OrderDeatil od = new OrderDeatil();
                        od.OrderID = order.OrderID;
                        od.ProperID = proID;
                        od.Quantity = quantity;
                        od.TypeName = pt.TypeName;
                        od.ProperName = pp.ProperName;
                        od.IMG = pp.IMG;
                        od.Price = pp.Price;
                        od.TotalMoney = quantity * pp.Price;
                        od.CreateTime = DateTime.Now;
                        db.OrderDeatil.Add(od);

                        db.ShopCar.Remove(cc);
                    }
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Paypage");
        }
        public ActionResult Paypage()
        {
            string number = Request.Cookies["number"].Value;
            Order order = db.Order.Where(x => x.ExpressNumber == number).First();
            var result = db.OrderDeatil.Where(x => x.OrderID == order.OrderID);
            List<OrderSC> myList = new List<OrderSC>();
            int total = 0;
            foreach (OrderDeatil item in result)
            {
                total = total + item.Quantity.Value;
                OrderSC oc = new OrderSC()
                {
                    IMG = item.IMG,
                    TypeName = item.TypeName,
                    ProName = item.ProperName,
                    Price = item.Price.ToString(),
                    Count = item.Quantity.ToString(),
                    Postage = order.Postage.ToString()
                };
                myList.Add(oc);
            }
            ViewBag.orderShop = myList.AsEnumerable();
            string[] adressInfos = order.AddressInfo.Split(':');
            PayFor pf = new PayFor()
            {
                OrderID = order.OrderID,
                ExpressNumber = order.ExpressNumber,
                InvoiceName = order.InvoiceName,
                AddressInfo = adressInfos[0] + "， 联系方式：" + adressInfos[1],
                Quantity = total.ToString()
            };
            ViewData.Model = pf;
            return View();
        }
    }
}