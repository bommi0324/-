using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shopping.Areas.XiaoNiu.Models
{
    public class QTFilter:ActionFilterAttribute
    {
       
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            HttpContextBase http = filterContext.HttpContext;
            if (http.Request.Cookies["CustomerID"] == null)
            {
                http.Response.Redirect("/XiaoNiu/HomePage/Login");
            }
        }
        
    }
}