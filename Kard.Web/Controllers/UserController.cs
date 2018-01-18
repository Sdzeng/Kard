using Kard.Core.AppServices.Default;
using Kard.Core.Entities;
using Kard.Extensions;
using Kard.Runtime.Session;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Kard.Web.Controllers
{
    [Route("user")]
    public class UserController : BaseController
    {

        private readonly IDefaultAppService _defaultAppService;
        public UserController(ILogger<UserController> logger,
            IMemoryCache memoryCache, 
            IDefaultAppService defaultAppService,
            IKardSession kardSession) 
            :base(logger, memoryCache, kardSession)
        {
            //HttpContext.Session.SetString("UserId", user.Id.ToString());
            _defaultAppService = defaultAppService;
        }

        [HttpGet("signup")]
        public IActionResult Signup()
        {
            return Content(GetPageFile("login.htm"), "text/html;charset=utf-8");
        }

        [HttpPost("signup")]
        public IActionResult Signup(KuserEntity user)
        {
            return Content(GetPageFile("login.htm"), "text/html;charset=utf-8");
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return Content(GetPageFile("login.htm"), "text/html;charset=utf-8");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string username, [DataType(DataType.Password)] string password, string returnUrl)
        {
            var result = _defaultAppService.Login(username, password);
            if (result.Result)
            {
                var identity = result.Data;
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            
                if (returnUrl.IsNullOrEmpty())
                {
                    return Content(GetPageFile("user.htm"), "text/html;charset=utf-8");
                }

                return Redirect(returnUrl);
            }

            return Json("{result:false,message:'登录失败，用户名密码不正确'}");
        }



        [Authorize]
        [Route("cover")]
        public IActionResult GetUserCover()
        {
            return Json("{session:'"+ _kardSession .NikeName+ "'}");
        }
    }
}