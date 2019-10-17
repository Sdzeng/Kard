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

            if (controllerActionDescriptor != null
               || controllerActionDescriptor.MethodInfo.IsDefined(typeof(AuthorizeAttribute), true)
               || controllerActionDescriptor.MethodInfo.IsDefined(typeof(AllowAnonymousAttribute), true))
            {
                return;
            }


            //ForbidResult：未授权403 ChallengeResult：未认证401
            context.Result = new ForbidResult();
        }


    }
}
