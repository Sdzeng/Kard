using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Kard.Extensions;
using WinCs.Core.IAppServices;
using System.Text;
using WinCs.Common.Utils;

namespace WinCs.Web.Controllers
{
 
    public class LoginController : Controller
    {
        private readonly IUserAppService _userAppService;
        public LoginController(IUserAppService userAppService)
        {
            _userAppService = userAppService;
        }

     
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Index(string name, string password, string ReturnUrl)
        {
            var result = _userAppService.Login(name, password);
            if (result.Result)
            {
                var user = result.Data;

                user.AuthenticationType = CookieAuthenticationDefaults.AuthenticationScheme;
                var identity = new ClaimsIdentity(user);
                identity.AddClaim(new Claim(ClaimTypes.Name, user.Id.ToString()));
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                if (ReturnUrl.IsNullOrEmpty())
                {
                    return RedirectToAction("Index", "Home");
                }
                return Redirect(ReturnUrl);
            }
        
            ModelState.AddModelError("", "登录失败，用户名密码不正确");
            return View("index");
        }



        public IActionResult Denied()
        {
            return View();
        }
    }
}