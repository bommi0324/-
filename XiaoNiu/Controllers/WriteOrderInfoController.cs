using Shopping.Areas.Admin.Models;
using Shopping.Areas.XiaoNiu.Models;
using Shopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shopping.Areas.XiaoNiu
{
    [MyFilter(IsCheck = false)]
    [QTFilter]
    public class WriteOrderInfoController : Controller
    {
        ShopDBEntities1 db = new ShopDBEntities1();
        // GET: XiaoNiu/WriteOrderInfo
        /// <summary>
        /// 跳转到填写订单信息的Action
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult Write(string ids)
        {
            Response.Cookies["ids"].Value = ids;
            return RedirectToAction("WriteInfo");
        }

        public ActionResult WriteInfo()
        {
            //获得要购买的ID
            string id = Request.Cookies["ids"].Value;
            if (id!=null)
            {
                //以|分割字符串 id
                string[] ids = id.Split('|'); //一维数组中存的就是购物车商品ID
                //获得商品件数 
                //ViewBag.count = ids.Length-1;
                List<OrderSC> myList = new List<OrderSC>();
                foreach (string item in ids)
                {
                    if (item != "")
                    {
                        //获取购物车ID
                        int carid = int.Parse(item);
                        ShopCar car = db.ShopCar.Where(x => x.Id == carid).First();
                        ProductProperty ppt = db.ProductProperty.Where(x => x.ProperID == car.ProperID).First();
                        ProductType pt = db.ProductType.Where(x => x.TypeID == ppt.TypeID).First();
                        Product p = db.Product.Where(x => x.ProductID == pt.ProductID).First();
                        //把需要实现的字段存起来
                        OrderSC orderSC = new OrderSC()
                        {
                            IMG = ppt.IMG,
                            TypeName = pt.TypeName,
                            ProName = ppt.ProperName,
                            Price = ppt.Price.ToString(),
                            Count = car.Quantity.ToString(),
                            Postage = p.Postage.ToString(),
                        };
                        myList.Add(orderSC);
                    }
                }
                ViewBag.orderShop = myList.AsEnumerable();
            }
            return View();
        }
        /// <summary>
        /// 去到地址列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewAddress()
        {
            //获取当前登录用户的ID
            int customerID = int.Parse(Request.Cookies["CustomerID"].Value);
            //通过登录用户的ID，查询该用户的地址
            ViewBag.address = db.Address.Where(x => x.CustomerID == customerID);
            //查询用户地址的数量
            ViewBag.count = db.Address.Where(x => x.CustomerID == customerID).Count();
            return PartialView();
        }
        /// <summary>
        /// 跳转页面
        /// </summary>
        /// <param name="addressID"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UpdateAddress(int? addressID)
        {
            //addressID 有值 修改地址请求   查出需要修改的地址信息
            if (addressID.HasValue)
            {
                int customerID = int.Parse(Request.Cookies["CustomerID"].Value);
                ViewData.Model = db.Address.Where(x => x.AddressID == addressID && x.CustomerID == customerID).First();
            }
            //addressID 没值 添加地址
            return PartialView();
        }
        /// <summary>
        /// 添加地址
        /// </summary>
        /// <param name="address">页面传递过来的地址信息</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddAdress(Address address)
        {
            string isDEF = address.IsDefault.ToString();
            int customerID = int.Parse(Request.Cookies["CustomerID"].Value);
            //判断 如果说添加的地址是默认地址 需要将数据库中的默认地址改为false
            if (isDEF == "True")
            {
                //需要判断当前用户是否有默认地址  将数据库中的默认地址改为false
                bool isHas = db.Address.Any(x => x.IsDefault == address.IsDefault && x.CustomerID == customerID);
                if (isHas)
                {
                    Address ad = db.Address.Where(x => x.IsDefault == address.IsDefault && x.CustomerID == customerID).First();
                    ad.IsDefault = false;
                }
            }
            else
            {
                //不是默认地址  判断是否有收货地址
                int count= db.Address.Where(x => x.CustomerID == customerID).Count();
                if (count == 0)
                {
                    //该用户没有收货地址
                    address.IsDefault = true;
                }
            }
            address.CreateTime = DateTime.Now;
            address.CustomerID = customerID;
            db.Address.Add(address);
            db.SaveChanges();
            return RedirectToAction("ViewAddress");
        }
        /// <summary>
        /// 修改地址
        /// </summary>
        /// <param name="address">修改视图页面传递的地址信息</param>
        /// <returns></returns>
        public ActionResult UpdateAddress(Address address)
        {
            string isDEF = address.IsDefault.ToString();
            int customerID = int.Parse(Request.Cookies["CustomerID"].Value);
            //判断 如果说添加的地址是默认地址 需要将数据库中的默认地址改为false
            if (isDEF == "True")
            {
                //需要判断当前用户是否有默认地址  将数据库中的默认地址改为false
                bool isHas = db.Address.Any(x => x.IsDefault == address.IsDefault && x.CustomerID == customerID);
                if (isHas)
                {
                    Address ad = db.Address.Where(x => x.IsDefault == address.IsDefault && x.CustomerID == customerID).First();
                    ad.IsDefault = false;
                }
            }
            var result = db.Address.Where(x => x.AddressID == address.AddressID && x.CustomerID == customerID);
            foreach (Address item in result)
            {
                item.AddressName = address.AddressName;
                item.AddressInfo = address.AddressInfo;
                item.AddressType = address.AddressType;
                item.Areas = address.Areas;
                item.IsDefault = address.IsDefault;
                item.Tel = address.Tel;
                item.Phone = address.Phone;
                item.Postcode = address.Postcode;
            }
            db.SaveChanges();
            return RedirectToAction("ViewAddress");
        }
        /// <summary>
        /// 删除地址
        /// </summary>
        /// <param name="AddressID">需要删除的ID</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteAddress(int? addressid)
        {
            //找到需要删除的地址
            Address address = db.Address.Where(x => x.AddressID == addressid).First();
            int customerID = int.Parse(Request.Cookies["CustomerID"].Value);
            //判断删除的地址是否是默认地址
            if (address.IsDefault == true)
            {
                //如果是删默认地址   吧列表第一个地址改为默认地址
                //获取当前用户的收货地址数量
                int count = db.Address.Where(x => x.CustomerID == customerID).Count();
                if (count != 1)
                {
                    Address ad = db.Address.Where(x => x.IsDefault == false && x.CustomerID == customerID).First();
                    ad.IsDefault = true;
                }
            }
            db.Address.Remove(address);
            db.SaveChanges();
            return RedirectToAction("ViewAddress");
        }
        /// <summary>
        /// 点击立即支付，获取订单信息 将订单信息保存到表 跳转到支付界面
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GoOrder(Order order)
        {
            if (Request.Cookies["CustomerID"].Value!=null)
            {
                order.CustomerID = int.Parse(Request.Cookies["CustomerID"].Value);
                order.CreateTime = DateTime.Now;
                //不同电脑的日期格式
                string aa = DateTime.Now.ToShortDateString();
                string dd = aa.Split('/')[0] + aa.Split('/')[1] + aa.Split('/')[2];//20200616
                order.ExpressNumber = dd + new Random().Next(11111, 99999);
                db.Order.Add(order);
                db.SaveChanges();
            }
            return Content("/XiaoNiu/Pay/TranPay?Number="+order.ExpressNumber);
        }
    }
}