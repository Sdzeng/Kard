using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Kard.Web.Filters
{
    /// <summary>
    /// 全局异常过滤器
    /// </summary>
    public class GlobalExceptionFilter : IExceptionFilter
    {
        readonly ILoggerFactory _loggerFactory;
        readonly IHostingEnvironment _env;

        public GlobalExceptionFilter(ILoggerFactory loggerFactory, IHostingEnvironment env)
        {
            _loggerFactory = loggerFactory;
            _env = env;
        }

        public void OnException(ExceptionContext context)
        {
#if !DEBUG
            var logger = _loggerFactory.CreateLogger(context.Exception.TargetSite.ReflectedType);

            logger.LogError(new EventId(context.Exception.HResult),
            context.Exception,
            context.Exception.Message);

            var json = new JsonResultModel<object>()
            {
                code = "500",
                message = "请求发生异常错误",
            };

            //if (_env.IsDevelopment()) json.remark = context.Exception.Message;
            json.remark = context.Exception.Message;

            context.Result = new ApplicationErrorResult(json);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            context.ExceptionHandled = true;

            //string msg = $"Action方法：{context.ActionDescriptor.DisplayName}，\r\n{context.Exception.MsgAndStackTraceString()}";
            //ErrorLogHelper.Log(ErrorLogType.Default.ToByte(), "", "", "未处理异常", msg);
#endif
        }

        public class ApplicationErrorResult : ObjectResult
        {
            public ApplicationErrorResult(object value) : base(value)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }
    }
}
