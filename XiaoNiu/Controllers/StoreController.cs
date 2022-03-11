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
    public class StoreController : Controller
    {
        ShopDBEntities1 db = new ShopDBEntities1();
        // GET: XiaoNiu/Store
        /// <summary>
        /// 两表联查 查询新品上架 小牛周边
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewProduct()
        {
            //需要把要显示的数据合并 存在Store.cs里
            //连接查询join on 
            var result = from p in db.ProductType
                         join c in db.ProductProperty on p.TypeID equals c.TypeID
                         group new { p.TypeID, p.TypeName, p.Description, c.IMG, c.Price, c.ProperID, c.CreateTime } by p.TypeName into g
                         select new XiaoNiuStore
                         {
                             SID = g.FirstOrDefault().ProperID,
                             TypeName = g.FirstOrDefault().TypeName,
                             Description = g.FirstOrDefault().Description,
                             IMG = g.FirstOrDefault().IMG,
                             Price = g.FirstOrDefault().Price,
                             PID = g.FirstOrDefault().TypeID,
                             CreateTime = g.FirstOrDefault().CreateTime.Value
                         };
            //Take()  从序列的开头返回指定数量的元素
            //显示前17条数据
            ViewBag.newProduct = result.OrderByDescending(p => p.CreateTime).Take(17);
            //Skip()  跳过指定数量的元素 返回剩下的元素
            //跳过17调数据 并且不包含电动车
            ViewBag.zhouBian = result.OrderByDescending(x => x.CreateTime).Skip(17).Where(x => x.PID > 2);
            return View();
        }
    }
}