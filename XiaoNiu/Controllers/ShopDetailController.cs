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
    public class ShopDetailController : Controller
    {
        ShopDBEntities1 db = new ShopDBEntities1();
        // GET: XiaoNiu/ShopDetail
        /// <summary>
        /// 根据商品属性ID查询商品详情信息
        /// </summary>
        /// <param name="id">前台界面传递过来的ID</param>
        /// <returns></returns>
        public ActionResult Detail(int? id)
        {
            //根据商品属性ID查询商品属性
            ProductProperty pro = db.ProductProperty.Where(p => p.ProperID == id).First();
            //通过属性ID来获取该属性上一级的产品型号
            ProductType pt = db.ProductType.Where(x => x.TypeID == pro.TypeID).First();
            StoreDetail sd = new StoreDetail()
            {
                TypeName = pt.TypeName, //产品类型名称
                ProName = pro.ProperName,
                Quantity = pro.Quantity.ToString(),
                IMG = pro.IMG,
                Price = pro.Price.ToString(),
                Description = pro.Description
            };
            //传到视图页面
            ViewData.Model = sd;
            var result = db.ProductProperty.Where(x => x.TypeID == pt.TypeID);
            ViewBag.pro = result.AsEnumerable();
            return View();
        }
        /// <summary>
        /// 需要得到图片，价格，库存
        /// </summary>
        /// <param name="pid">前台界面传递的属性ID</param>
        /// <returns></returns>
        public ActionResult DataPro(int? id)
        {
            ProductProperty pp = db.ProductProperty.Where(x => x.ProperID == id).First();
            //把需要的字段  用Json格式返回到前台界面
            return Json(new { price = pp.Price.ToString(), IMG = pp.IMG, Quantity = pp.Quantity.ToString() }, JsonRequestBehavior.AllowGet);

        }
    }
}