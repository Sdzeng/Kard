using Kard.Core.Dtos;
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
        readonly ILogger _logger;
        readonly IHostingEnvironment _env;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger, IHostingEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public void OnException(ExceptionContext context)
        {
            //#if !DEBUG
            _logger.LogError(new EventId(context.Exception.HResult),
            context.Exception,
            $"全局捕获异常：{context.Exception.Message}");

            var resultDto = new ResultDto()
            {
                Result =false,
                Message = $"请求发生异常错误{context.Exception.Message}",
            };

            //if (_env.IsDevelopment()) json.remark = context.Exception.Message;
        

            context.Result = new ApplicationErrorResult(resultDto);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            context.ExceptionHandled = true;

            //string msg = $"Action方法：{context.ActionDescriptor.DisplayName}，\r\n{context.Exception.MsgAndStackTraceString()}";
            //ErrorLogHelper.Log(ErrorLogType.Default.ToByte(), "", "", "未处理异常", msg);
//#endif
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
