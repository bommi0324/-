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
    public class PersonalCenterController : Controller
    {
        ShopDBEntities1 db = new ShopDBEntities1();
        /// <summary>
        /// 去到修改密码的界面
        /// </summary>
        /// <returns></returns>
        // GET: XiaoNiu/PersonalCenter
        [HttpGet]
        public ActionResult ModifyPWD()
        {
            int customerID= int.Parse(Request.Cookies["CustomerID"].Value);
            Customer cus = db.Customer.Where(x => x.CustomerID == customerID).First();
            ViewBag.pwd = cus.CustomerPWD;//查询到密码 传递到前台修改密码的视图
            return View();
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        // GET: XiaoNiu/PersonalCenter
        [HttpPost]
        public ActionResult ModifyPWD(string newPwd)
        {
            int customerID = int.Parse(Request.Cookies["CustomerID"].Value);
            Customer cus = db.Customer.Where(x => x.CustomerID == customerID).First();
            cus.CustomerPWD=newPwd;//使用新密码替换掉旧密码
            db.SaveChanges();
            return Content("/XiaoNiu/HomePage/Index");
        }
        /// <summary>
        /// 显示我的订单
        /// </summary>
        /// <returns></returns>
        public ActionResult MyOrder()
        {
            //查询待付款的订单
            var or = db.Order.Where(x => x.OrderState == "待付款");
            foreach (Order item in or)
            {
                //获取订单时间之后，获取订单未支付的时间
                string tm = (DateTime.Now - item.CreateTime.Value).TotalHours.ToString();
                //判断是否超过两小时  如果超过 改变订单状态为 超时关闭
                decimal totalTime = decimal.Parse(tm);
                if (totalTime > 2)
                {
                    item.OrderState = "超时关闭";
                }
            }
            db.SaveChanges();
            int customerID = int.Parse(Request.Cookies["CustomerID"].Value);
            ViewBag.count = db.Order.Where(x => x.CustomerID == customerID).Count();
            return View();
        }
        /// <summary>
        /// 查看不同状态的订单：待付款，超时关闭，已付款，退换货 
        /// </summary>
        /// <param name="orderState"></param>
        /// <returns></returns>
        public ActionResult CheckState(string orderState)
        {
            var result = db.Order.AsEnumerable();
            int customerID = int.Parse(Request.Cookies["CustomerID"].Value);
            //一开始 默认显示全部订单
            if (orderState == null || orderState == "全部订单")
            {
                result = db.Order.Where(x => x.CustomerID == customerID).OrderByDescending(x => x.CreateTime);
            }
            else //查询我们所选择的订单行状态
            {
                result = db.Order.Where(x => x.CustomerID == customerID && x.OrderState == orderState).OrderByDescending(x => x.CreateTime);

            }
            List<OrderDeatil> myList = new List<OrderDeatil>();
            foreach (Order item in result)
            {
                var odl = db.OrderDeatil.Where(x => x.OrderID == item.OrderID);
                foreach (OrderDeatil od in odl)
                {
                    myList.Add(od);
                }
            }
            //显示所有的订单
            ViewBag.order = result;
            //显示订单详情商品
            ViewBag.orderDetail = myList.AsEnumerable();
            return PartialView();
        }
        /// <summary>
        /// 订单详情页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult OrderDetail(int? id)
        {
            if (id.HasValue)
            {
                //通过ID查询到订单
                Order or = db.Order.Where(x => x.OrderID == id).First();
                var result = db.OrderDeatil.Where(x => x.OrderID == or.OrderID).AsEnumerable();
                ViewData.Model = or;
                ViewBag.orderdetail = result;
            }
            return View();
        }
        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        public ActionResult LoginOut()
        {
            //让缓存的时间过期
            Response.Cookies["CustomerID"].Expires = DateTime.Now.AddDays(-1);
            return RedirectToAction("Login","HomePage");
        }
    }
}