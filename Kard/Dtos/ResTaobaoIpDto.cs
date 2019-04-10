using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Dtos
{
    public class ResTaobaoIpDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }

        public detail data { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public class detail
        {
            /// <summary>
            /// 中国
            /// </summary>
            public string country { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string country_id { get; set; }
            /// <summary>
            /// 华南
            /// </summary>
            public string area { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string area_id { get; set; }
            /// <summary>
            /// 广东省
            /// </summary>
            public string region { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string region_id { get; set; }
            /// <summary>
            /// 广州市
            /// </summary>
            public string city { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string city_id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string county { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string county_id { get; set; }
            /// <summary>
            /// 电信
            /// </summary>
            public string isp { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string isp_id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string ip { get; set; }
        }
    }
}
