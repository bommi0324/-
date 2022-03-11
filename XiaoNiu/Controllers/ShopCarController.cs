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
    public class ShopCarController : Controller
    {
        ShopDBEntities1 db = new ShopDBEntities1();
        // GET: XiaoNiu/ShopCar
        /// <summary>
        /// 将加入购物车的商品信息 写进数据库
        /// </summary>
        /// <returns></returns>
        [QTFilter]
        public ActionResult AddShop(int? id,int? count)
        {
            if (id.HasValue)
            {
                if (Request.Cookies["CustomerID"] != null)
                {
                    int customerId= int.Parse(Request.Cookies["CustomerID"].Value);
                    bool isExist = db.ShopCar.Any(x => x.CustomerID == customerId && x.ProperID == id);
                    if (isExist) //该商品存在在购物车中
                    {
                        ShopCar sc = db.ShopCar.Where(x => x.ProperID == id).First();
                        sc.Quantity = sc.Quantity + count;
                        db.SaveChanges();
                    }
                    else//此商品不再购物车 将商品信息加入购物车
                    {
                        //先把商品信息获取到 存到数据库中  
                        ShopCar shopCar = new ShopCar();
                        shopCar.CustomerID = int.Parse(Request.Cookies["CustomerID"].Value);
                        shopCar.ProperID = id;  //属性ID
                        shopCar.Quantity = count;//数量
                        shopCar.CreateTime = DateTime.Now;
                        db.ShopCar.Add(shopCar);
                        db.SaveChanges();
                    }                    
                }
            }
            return RedirectToAction("ShopCar");
        }
        /// <summary>
        /// 查询购物车里的信息
        /// </summary>
        /// <returns></returns>
        [QTFilter]
        public ActionResult ShopCar()
        {
            List<ShopC> myList = new List<ShopC>();
            foreach (ShopCar item in db.ShopCar)
            {
                if (Request.Cookies["CustomerID"] != null)
                {
                    if (item.CustomerID==int.Parse(Request.Cookies["CustomerID"].Value))
                    {
                        //根据ID查询商品属性
                        ProductProperty pro =  db.ProductProperty.Where(x => x.ProperID == item.ProperID).First();
                        //通过商品属性中的类型ID查询到商品类型对象
                        ProductType pt = db.ProductType.Where(x => x.TypeID == pro.TypeID).First();
                        ShopC shopC = new ShopC()
                        {
                            ShopID = item.Id.ToString(),//购物车ID
                            TypeName = pt.TypeName,//类型名称
                            ProName = pro.ProperName,//属性名称
                            Price = pro.Price.ToString(),
                            IMG=pro.IMG,
                            Count=item.Quantity.ToString()
                        };
                        myList.Add(shopC);
                    }
                }
            }
            //AsEnumerable 延迟加载
            ViewBag.shop = myList.AsEnumerable();
            return View();
        }
        /// <summary>
        /// 删除购物车里的数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteShopCar(int? id)
        {
            ShopCar data = db.ShopCar.Where(x => x.Id == id).First();
            db.ShopCar.Remove(data);
            db.SaveChanges();
            return Content("删除成功！");
        }
        /// <summary>
        /// 根据购物车ID 次改商品数量
        /// </summary>
        /// <param name="ShopID"></param>
        /// <param name="Count"></param>
        /// <returns></returns>
        public ActionResult UpdateShopCar(int? ShopID, int? Count)
        {
            //找到要修改数量的购物车商品
            var result = db.ShopCar.Where(x => x.Id == ShopID);
            foreach (ShopCar item in result)
            {
                item.Quantity = Count;
            }
            db.SaveChanges();
            return Content("数量修改成功");
        }
    }
}