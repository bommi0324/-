using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shopping.Areas.XiaoNiu.Models
{
    public class StoreDetail
    {
        public int TID { get; set; } // 商品id
        public string  TypeName { get; set; } //商品类型名称
        public string ProName { get; set; }// 商品属性名称
        public string  Quantity { get; set; } //商品数量
        public string IMG { get; set; } // 商品图片
        public string Price { get; set; } //商品价格
        public string Description { get; set; } //商品描述
    }
}