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
using System.ComponentModel.DataAnnotations;
using Senparc.Weixin.MP.AdvancedAPIs;
using Microsoft.Extensions.Configuration;
using Senparc.Weixin;
using Kard.Extensions;

namespace Kard.Web.Controllers
{
    [Produces("application/json")]
    [Route("login/[action]")]
    public class LoginController : BaseController
    {
        private readonly ILoginAppService _loginAppService;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IDefaultRepository _defaultRepository;
        private readonly IConfiguration _configuration;

        public LoginController(
          ILogger<LoginController> logger,
          IMemoryCache memoryCache,
          ILoginAppService loginAppService,
          IKardSession kardSession,
          IConfiguration configuration,
          IRepositoryFactory repositoryFactory) : base(logger, memoryCache, kardSession)
        {
            _loginAppService = loginAppService;
            _configuration = configuration;
            _repositoryFactory = repositoryFactory;
            _defaultRepository = repositoryFactory.GetRepository<IDefaultRepository>();
        }

        #region Account Login
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
        /// <param name="remember"></param> 
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<ResultDto> AccountLogin(string username, [DataType(DataType.Password)] string password, string remember, string returnUrl)
        {
            var result = _loginAppService.AccountLogin(username, password);
            if (result.Result)
            {
                var identity = result.Data;

                await HttpContext.SignInAsync(identity.AuthenticationType, new ClaimsPrincipal(identity), new AuthenticationProperties { IsPersistent = (remember == "on") });

                //if (returnUrl.IsNullOrEmpty())
                //{
                //    return RedirectToAction("Index");
                //}

                //return Redirect(returnUrl);
                return new ResultDto() { Result = true, Message = "登录成功" };
            }
            return await Task.FromResult(new ResultDto() { Result = false, Message = "登录失败，用户名密码不正确" });
        }


        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<ResultDto> AccountRegister(KuserEntity user)
        {
            user.UserType = "WebAccount";
            user.KroleId = 1;
            user.City = "广州";
            user.AvatarUrl = @"user\id\avatar.jpg";
            user.CoverPath = "";
            //user.AuditCreation(1);
            return await Task.FromResult(_loginAppService.Register("accountRegister", user));
        }


        #endregion

        #region WeChat Login

        [AllowAnonymous]
        [HttpGet]
        public ResultDto GetLoginQr(string code)
        {
            var resultDto = new ResultDto();
            var appId = _configuration.GetValue<string>("AppSetting:WeChat:Open:AppId");
            var appSecret = _configuration.GetValue<string>("AppSetting:WeChat:Open:AppSecret");


            resultDto.Result = true;
            resultDto.Data = $"https://open.weixin.qq.com/connect/oauth2/authorize?appid={appId}&redirect_uri=http%3A%2F%2Fapi.coretn.cn%2Fgetuserinfo&response_type=code&scope=snsapi_userinfo&state=STATE";
            return resultDto;
        }

        [AllowAnonymous]
        [HttpGet]
        public ResultDto WeChatLoginCallback(string code, string state)
        {
            var resultDto = new ResultDto();
            var appId = _configuration.GetValue<string>("AppSetting:WeChat:Official:AppId");
            var appSecret = _configuration.GetValue<string>("AppSetting:WeChat:Official:AppSecret");
            var accessTokenResult = OAuthApi.GetAccessToken(appId, appSecret, code);
            if (accessTokenResult.errcode != ReturnCode.请求成功)
            {
                resultDto.Result = false;
                resultDto.Message = "未知的code";
                return resultDto;
            }

            var authWeixin = OAuthApi.GetUserInfo(accessTokenResult.access_token, accessTokenResult.openid);
            if (authWeixin == null)
            {
                resultDto.Result = false;
                resultDto.Message = "用户授权失败";
                return resultDto;
            }

            var cacheKey = Guid.NewGuid().ToString("N");
            _memoryCache.GetCache(cacheKey, () => authWeixin.unionid, 300);

            resultDto.Result = true;
            resultDto.Data = new { LoginToken = cacheKey };
            return resultDto;
        }

        //public static OAuthUserInfo GetAuthUserInfo(string oauthAccessToken, string openId)
        //{
        //    return OAuthApi.GetUserInfo(oauthAccessToken, openId);
        //}

        //public static UserInfoJson GetUserInfo(string accessTokenOrAppId, string openId)
        //{
        //    return UserApi.Info(accessTokenOrAppId, openId);
        //}

        /// <summary>
        /// 微信登陆
        /// </summary>
        /// <param name="loginToken"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public Task<ResultDto> WeChatLoginState(string loginToken)
        {
            return Task.Run(() =>
            {
                var unionId = _memoryCache.Get<string>(loginToken);
                if (unionId.IsNullOrEmpty())
                {

                }
                var result = new ResultDto { Result = true, Message ="待实现" };
                return result;
            });

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
        [HttpPost("wechatregister")]
        public ResultDto WeChatRegister([FromBody]KuserEntity user)
        {
            //var ssss=HttpContext.Session.GetString("ssss");
            if (string.IsNullOrEmpty(_kardSession.WxUnionId))
            {
                return new ResultDto { Result = false, Message = "未找到open" };
            }
            user.WxOpenId = _kardSession.WxUnionId;

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
                var taskEntity = new TaskEntity
                {
                    TaskDate = entity.TaskDate,
                    StartTime = entity.StartTime,
                    EndTime = entity.EndTime,
                    Content = entity.Content,
                    IsRemind = entity.IsRemind,
                    IsDone = false
                };
                taskEntity.AuditCreation(_kardSession.UserId.Value);
                var createResult = _defaultRepository.CreateAndGetId<TaskEntity, long>(taskEntity);
                result.Result = createResult.Result;
                result.Message = createResult.Message;

                return result;
            }


            entity.AuditCreation(_kardSession.UserId.Value);
            result = _repositoryFactory.GetRepository<ILongTaskRepository>().AddTask(entity);

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
            result.Result = true;
            result.Data = _defaultRepository.Query<TaskEntity>("select * from task where CreatorUserId=@CreatorUserId and TaskDate=@TaskDate and IsDeleted=0 order by IsDone,StartTime", new { CreatorUserId = _kardSession.UserId, TaskDate = DateTime.Now.Date });
            return result;
        }

        #endregion

        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResultDto> Logout()
        {
            await HttpContext.SignOutAsync(_kardSession.AuthenticationType);
            return await Task.FromResult(new ResultDto() { Result = true, Message = "退出成功" });
        }

        /// <summary>
        /// 未登录返回结果
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> NotLogin()
        {
            var rs = new JsonResult(new
            {
                message = "您未登录不能查看该内容"

            });

            rs.StatusCode = 401;
            return await Task.FromResult(rs);
        }

    }
}