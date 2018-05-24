using Kard.Core.AppServices.Default;
using Kard.Core.Entities;
using Kard.Core.IRepositories;
using Kard.Extensions;
using Kard.Runtime.Session;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Kard.Web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class WebController : BaseController
    {
        private readonly IHostingEnvironment _env;
        private readonly ILoginAppService _loginAppService;
        private readonly IDefaultRepository _defaultRepository;
        public WebController(IHostingEnvironment env,
            ILogger<WebController> logger,
            IMemoryCache memoryCache,
            ILoginAppService loginAppService,
            IDefaultRepository defaultRepository,
            IKardSession kardSession)
            : base(logger, memoryCache, kardSession)
        {
            //HttpContext.Session.SetString("UserId", user.Id.ToString());
            _env = env;
            _loginAppService = loginAppService;
            _defaultRepository = defaultRepository;
        }

        //[Authorize(Roles = "member", AuthenticationSchemes= "members")]
        [HttpGet("test")]
        public IActionResult Test(long? userId)
        {
            return Content("已登陆");
        }


        #region user


        ///// <summary>
        ///// 登陆接口
        ///// </summary>
        ///// <returns></returns>
        //[AllowAnonymous]
        //[HttpGet("user/login")]
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
        [HttpPost("user/login")]
        public async Task<IActionResult> Login(string username, [DataType(DataType.Password)] string password, string returnUrl)
        {
            var result = _loginAppService.WebLogin(username, password);
            if (result.Result)
            {
                var identity = result.Data;
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                //if (returnUrl.IsNullOrEmpty())
                //{
                //    return RedirectToAction("Index");
                //}

                //return Redirect(returnUrl);
                return Json(new { result = true, message = "登录成功" });
            }
            return Json(new { result=false,message= "登录失败，用户名密码不正确"} );
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        // [HttpGet("user")]
        // public IActionResult Index()
        // {
        //     return Content(GetPageFile("user.htm"), "text/html;charset=utf-8");
        // }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("user/{userId}")]
        public IActionResult Index(long? userId)
        {
            return Json(GetUser(userId));
        }

        /// <summary>
        /// 获取用户封面
        /// </summary>
        /// <returns></returns>
        [HttpGet("user/cover")]
        public IActionResult GetCover()
        {
            return Json(GetUser(_kardSession.UserId));
        }


        private KuserEntity GetUser(long? userId)
        {
            string cacheKey = $"user[{userId}]";
            KuserEntity kuserEntity = _memoryCache.GetOrCreate(cacheKey, (cacheEntry) =>
            {
                cacheEntry.SetAbsoluteExpiration(DateTime.Now.Date.AddDays(60));
                return _defaultRepository.GetUser(_kardSession.UserId.Value);
            });
            return kuserEntity;
        }



        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        [HttpGet("user/logout")]
        public IActionResult Logout()
        {

            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Json(new { result = true, message = "退出成功" });
        }

        /// <summary>
        /// 未登录返回结果
        /// </summary>
        /// <returns></returns>
  
        [HttpGet("user/notlogin")]
        [AllowAnonymous]
        public IActionResult NotLogin()
        {
            var json = new
            {
                code = "000",
                message = $"您未登录不能使用该接口",
            };
            var rs = new JsonResult(json);

            rs.StatusCode = 401;
            return rs;
        }
        #endregion
    }
}