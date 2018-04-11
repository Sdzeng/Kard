using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Kard.Json;
using Kard.Core.Dtos;
using Microsoft.Extensions.Caching.Memory;
using Kard.Runtime.Session;
using Kard.Core.AppServices.Default;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Kard.Core.Entities;

namespace Kard.Web.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    public class WxController : BaseController
    {
        private readonly ILoginAppService _loginAppService;
        public WxController(
          ILogger<ApiController> logger,
          IMemoryCache memoryCache,
             ILoginAppService loginAppService,
          IKardSession kardSession) : base(logger, memoryCache, kardSession)
        {
            _loginAppService = loginAppService;
        }

        [HttpGet("alive")]
        public async Task<ResultDto> Alive(string code)
        {
            var aliveResult = _loginAppService.WxAlive(code);
            if (aliveResult.Result)
            {
                var identity = aliveResult.Data;
                //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            }

            var result = new ResultDto { Result = aliveResult.Result,Message= aliveResult.Message };
            return result;
        }

        [HttpPost("login")]
        public async Task<ResultDto> Login([FromBody]KuserEntity user)
        {
            user.WxOpenId = _kardSession.WxOpenId;
            var aliveResult = _loginAppService.WxLogin(user);
            if (aliveResult.Result)
            {
                var identity = aliveResult.Data;
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            }

            var result = new ResultDto { Result = aliveResult.Result, Message = aliveResult.Message };
            return result;
        }

    }
}