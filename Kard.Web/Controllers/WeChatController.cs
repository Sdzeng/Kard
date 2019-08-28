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
using Kard.Runtime.Security.Authentication.WeChat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Kard.Core.IRepositories;

namespace Kard.Web.Controllers
{
    [Authorize(AuthenticationSchemes = WeChatAppDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("wx")]
    public class WeChatController : BaseController
    {
        private readonly ILoginAppService _loginAppService;
        private readonly IDefaultRepository _defaultRepository;

        public WeChatController(
          ILogger<WeChatController> logger,
          IMemoryCache memoryCache,
          ILoginAppService loginAppService,
          IKardSession kardSession,
          IDefaultRepository defaultRepository) : base(logger, memoryCache, kardSession)
        {
            _loginAppService = loginAppService;
            _defaultRepository = defaultRepository;
        }

        /// <summary>
        /// 微信登陆
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("login")]
        public async Task<ResultDto> Login(string code,string userInfo)
        {

            var aliveResult = _loginAppService.WxLogin(code, Serialize.FromJson<WeChatUserDto>(userInfo));
            if (aliveResult.Result)
            {
                var identity = aliveResult.Data;
                //HttpContext.Session.SetString("ssss", "hello world!");
                await HttpContext.SignOutAsync(WeChatAppDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(WeChatAppDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), new AuthenticationProperties { IsPersistent = true });
            }

            var result = new ResultDto { Result = aliveResult.Result, Message = aliveResult.Message };
            return result;
        }

    

        /// <summary>
        /// 微信登录
        /// </summary>
        /// <param name="code">授权成功返回的状态码</param>
        /// <param name="type">登录类型 "web" 或者 "app"</param>
        /// <remarks>
        /// {"code":"201","data":"cacheKey","message":"未绑定手机号码，请绑定"}
        /// 出现此状态是未绑定手机号，需把cacheKey传入接口 api/user/bindphone 绑定手机号
        /// </remarks>
        /// <returns></returns>
        //[HttpPost]
        //[Route("api/user/wechatlogin")]
        //[AllowAnonymous]
        //public async Task<IActionResult> WeChatLogin(string code, string type)
        //{
        //    var json = new ResultDto<object>()
        //    {
        //        code = "200",
        //        message = "登录成功"
        //    };

        //    var tokenResult = new QRConnectAccessTokenResult();
        //    if (type == "web")
        //    {
        //        var wxWebConfig = _appConfig.ApplicationConfig.WxWebConfig;
        //        tokenResult = await QRConnectAPI.GetAccessTokenAsync(wxWebConfig.AppId, wxWebConfig.AppSecret, code);
        //    }
        //    else if (type == "app")
        //    {
        //        var wxAppConfig = _appConfig.ApplicationConfig.WxAppConfig;
        //        tokenResult = await QRConnectAPI.GetAccessTokenAsync(wxAppConfig.AppId, wxAppConfig.AppSecret, code);

        //        //var wxAppConfig = _appConfig.ApplicationConfig.WxWebConfig;
        //        //tokenResult = await QRConnectAPI.GetAccessTokenAsync(wxAppConfig.AppId, wxAppConfig.AppSecret, code);
        //    }
        //    else
        //    {
        //        json.code = "400";
        //        json.message = "登录失败，无效的登录类型";
        //        return json.ToJsonResult();
        //    }

        //    if (string.IsNullOrWhiteSpace(tokenResult.access_token) || string.IsNullOrWhiteSpace(tokenResult.openid))
        //    {
        //        json.code = "400";
        //        json.message = "登录失败，无效的登录凭证";
        //        return json.ToJsonResult();
        //    }

        //    var user = await new dt_user() { Union_id = tokenResult.unionid }.QueryForUnionIdAsync();
        //    if (user == null)
        //    {
        //        var wxUserInfo = await QRConnectAPI.GetUserInfoAsync(tokenResult.access_token, tokenResult.openid);
        //        var cacheKey = $"WeChat{Utils.GetLetterOrNumberRandom(6)}";
        //        if (_cache.Add(cacheKey, wxUserInfo))
        //        {

        //            json.code = "201";
        //            json.data = cacheKey;
        //            json.message = "未绑定手机号码，请绑定";
        //        }
        //        else
        //        {
        //            json.code = "400";
        //            json.message = "登记用户信息失败";
        //        }
        //        return json.ToJsonResult();
        //    }

        //    await _signInManager.SignInAsync(new ApplicationUser() { Id = user.Id, UserName = user.Username }, true);
        //    json.data = await GetUserInfo(user);

        //    return json.ToJsonResult();
        //}



        /// <summary>
        /// 微信注册
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public ResultDto Register([FromBody]KuserEntity user)
        {
            //var ssss=HttpContext.Session.GetString("ssss");
            if (string.IsNullOrEmpty(_kardSession.WxOpenId))
            {
                return new ResultDto { Result = false, Message = "未找到open" };
            }
            user.WxOpenId = _kardSession.WxOpenId;

            var result = _loginAppService.Register("wxRegister", user);
            return result;
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("addtask")]
        public ResultDto AddTask([FromBody]LongTaskEntity entity)
        {

            var result = new ResultDto();
            if (!entity.IsLongTerm)
            {
                var taskEntity= new TaskEntity
                {
                    TaskDate = entity.TaskDate,
                    StartTime = entity.StartTime,
                    EndTime = entity.EndTime,
                    Content = entity.Content,
                    IsRemind = entity.IsRemind,
                    IsDone=false
                };
                taskEntity.AuditCreation(_kardSession.UserId.Value);
                var createResult = _defaultRepository.CreateAndGetId<TaskEntity, long>(taskEntity);
                result.Result = createResult.Result;
                result.Message = createResult.Message;

                return result;
            }


            entity.AuditCreation(_kardSession.UserId.Value);
            result = _defaultRepository.LongTask.AddTask(entity);

            return result;
        }

        /// <summary>
        /// 获取任务列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("gettask")]
        public ResultDto<IEnumerable<TaskEntity>> GetTask()
        {
            var result = new ResultDto<IEnumerable<TaskEntity>>();
            result.Result =true;
            result.Data = _defaultRepository.Query<TaskEntity>("select * from task where CreatorUserId=@CreatorUserId and TaskDate=@TaskDate and IsDeleted=0 order by IsDone,StartTime", new { CreatorUserId = _kardSession.UserId,TaskDate = DateTime.Now.Date });
            return result;
        }
    }
}