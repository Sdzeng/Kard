using Kard.Core.AppServices.Default;
using Kard.Core.Dtos;
using Kard.Core.Entities;
using Kard.Core.IRepositories;
using Kard.Extensions;
using Kard.Runtime.Session;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Kard.Web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("webuser")]
    public class WebUserController : BaseController
    {

        private readonly ILoginAppService _loginAppService;
        public WebUserController(
            ILogger<WebUserController> logger,
            IMemoryCache memoryCache,
            ILoginAppService loginAppService,
            IKardSession kardSession)
            : base(logger, memoryCache, kardSession)
        {
            //HttpContext.Session.SetString("UserId", user.Id.ToString());
            _loginAppService = loginAppService;
        }

     

        #region user


        ///// <summary>
        ///// 登陆接口
        ///// </summary>
        ///// <returns></returns>
        //[AllowAnonymous]
        //[HttpGet("login")]
        //public IActionResult Login()
        //{
        //    return Content(GetPageFile("login.htm"), "text/html;charset=utf-8");
        //}

        /// <summary>
        /// 登陆接口
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ResultDto> Login(string username, [DataType(DataType.Password)] string password, string returnUrl)
        {
            var result = _loginAppService.WebLogin(username, password);
            if (result.Result)
            {
                var identity = result.Data;
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), new AuthenticationProperties { IsPersistent = true });

                //if (returnUrl.IsNullOrEmpty())
                //{
                //    return RedirectToAction("Index");
                //}

                //return Redirect(returnUrl);
                return new ResultDto() { Result = true, Message = "登录成功" }; 
            }
            return new ResultDto() { Result = false, Message = "登录失败，用户名密码不正确" };
        }

 


        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        [HttpGet("logout")]
        public ResultDto Logout()
        {

            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return new ResultDto() { Result = true, Message = "退出成功" }; 
        }

 
        #endregion
    }
}