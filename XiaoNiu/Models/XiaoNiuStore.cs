using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shopping.Areas.XiaoNiu.Models
{
    public class XiaoNiuStore
    {
        public int SID { get; set; } //商品属性名称
        public string TypeName { get; set; }  //商品类型名称
        public string Description { get; set; }  //商品描述
        public string IMG { get; set; }//商品图片
        public decimal? Price { get; set; } //商品价格
        public int PID { get; set; }  //商品类型ID
        public DateTime CreateTime { get; set; }  //时间
    }
}