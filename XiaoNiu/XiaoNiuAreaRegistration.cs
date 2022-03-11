using System.Web.Mvc;

namespace Shopping.Areas.XiaoNiu
{
    public class XiaoNiuAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "XiaoNiu";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "XiaoNiu_default",
                "XiaoNiu/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}