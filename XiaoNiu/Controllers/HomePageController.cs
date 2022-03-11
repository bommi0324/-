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
    public class HomePageController : Controller
    {
        ShopDBEntities1 db = new ShopDBEntities1();
        // GET: XiaoNiu/HomePage
        /// <summary>
        /// 去到商城首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 去到登录界面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="customer">接收前台数据</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(Customer customer)
        {
            bool isHasName= db.Customer.Any(x => x.CustomerName == customer.CustomerName);
            if (isHasName)  //该用户存在
            {
                bool isHasPWD = db.Customer.Any(x => x.CustomerName == customer.CustomerName&&x.CustomerPWD==customer.CustomerPWD);
                if (isHasPWD)   //密码正确
                {
                    Customer cus= db.Customer.Where(x => x.CustomerName == customer.CustomerName && x.CustomerPWD == customer.CustomerPWD).First();
                    if (cus.Status == 1)//禁止登录
                    {
                        return Content("1");
                    }
                    else
                    {
                        //登录成功页面跳转前
                        Response.Cookies["CustomerID"].Value = cus.CustomerID.ToString();
                        return Content("/XiaoNiu/HomePage/Index");
                    }
                }
                else
                {
                    return Content("ErroPWD");
                }
            }
            else
            {
                return Content("NoName");
            }
        }
        /// <summary>
        /// 去到注册界面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        /// <summary>
        /// 将注册信息写入数据库
        /// </summary>
        /// <param name="customer">前台传递的信息</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Register(Customer customer)
        {
            bool isHasName = db.Customer.Any(x => x.CustomerName == customer.CustomerName);
            if (isHasName)  //即将要注册的用户名已经存在
            {
                return Content("No");
            }
            else   //可以注册
            {
                customer.CreateTime = DateTime.Now;
                db.Customer.Add(customer);
                db.SaveChanges();
                return Content("/XiaoNiu/HomePage/Login");
            }
        }
        public ActionResult CustomerInfo()
        {
            if (Request.Cookies["CustomerID"] != null)
            {
                //已经登陆的状态
                ViewBag.customerID = Request.Cookies["CustomerID"].Value;
            }
            else
            {
                //没有登陆
                ViewBag.customerID = null;
            }
            //PartialView当前Action返回的是一个分布视图，返回一个网页的局部
            return PartialView();
        }
        public ActionResult ShopCar()
        {
            if (Request.Cookies["CustomerID"] != null)
            {
                //已经登陆的状态
                ViewBag.customerID = Request.Cookies["CustomerID"].Value;
                int customerID = int.Parse(Request.Cookies["CustomerID"].Value);
                //获得此用户在购物车中商品的数量
                ViewBag.count = db.ShopCar.Where(x => x.CustomerID == customerID).Count();
                List<ShopC> myList = new List<ShopC>();
                foreach (ShopCar item in db.ShopCar)
                {
                    if (item.CustomerID == int.Parse(Request.Cookies["CustomerID"].Value))
                    {
                        //根据ID查询商品属性
                        ProductProperty pro = db.ProductProperty.Where(x => x.ProperID == item.ProperID).First();
                        //通过商品属性中的类型ID查询到商品类型对象
                        ProductType pt = db.ProductType.Where(x => x.TypeID == pro.TypeID).First();
                        ShopC shopC = new ShopC()
                        {
                            ShopID = item.Id.ToString(),//购物车ID
                            TypeName = pt.TypeName,//类型名称
                            ProName = pro.ProperName,//属性名称
                            Price = pro.Price.ToString(),
                            IMG = pro.IMG,
                            Count = item.Quantity.ToString()
                        };
                        myList.Add(shopC);
                    }
                }
                ViewBag.shop = myList.AsEnumerable();
            }
            else
            {
                //没有登陆
                ViewBag.customerID = null;
            }
            //PartialView当前Action返回的是一个分布视图，返回一个网页的局部
            return PartialView();
        }
    }
}