using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kard.Web.Filters
{

    /// <summary>
    /// 全局授权
    /// </summary>
    public class GlobalAuthorizationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {

            var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor == null)
            {
                context.Result = new StatusCodeResult(404);
                return;
            }

 

            if (controllerActionDescriptor.ControllerTypeInfo.IsDefined(typeof(AllowAnonymousAttribute), true)||controllerActionDescriptor.MethodInfo.IsDefined(typeof(AllowAnonymousAttribute), true))
            {
                return;
            }

            //var user = context.HttpContext.User;
            //if (!user.Identity.IsAuthenticated) {
            //    context.Result = new StatusCodeResult(401);
            //    return;
            //}

            if (!controllerActionDescriptor.ControllerTypeInfo.IsDefined(typeof(AuthorizeAttribute), true)&&!controllerActionDescriptor.MethodInfo.IsDefined(typeof(AuthorizeAttribute), true))
            {
                //context.HttpContext.Response.StatusCode = 403;
                //context.Result = new ForbidResult(user.Identity.AuthenticationType);
                context.Result = new StatusCodeResult(403);
                return;
            }
        }


    }
}
