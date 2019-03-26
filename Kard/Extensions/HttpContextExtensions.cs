using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kard.Extensions
{
    public static class HttpContextExtensions
    {
        /// <summary>
        /// 获取请求方的IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetClientIpAddress(this HttpContext context)
        {
            var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Connection.RemoteIpAddress.ToString();
            }
            return ip;
        }
    }
}
