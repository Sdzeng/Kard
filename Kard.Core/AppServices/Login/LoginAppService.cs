using Kard.Core.Dtos;
using Kard.Core.Entities;
using Kard.Core.IRepositories;
using Kard.Extensions;
using Kard.Json;
using Kard.Runtime.Security;
using Kard.Runtime.Security.Authentication.WeChat;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;


namespace Kard.Core.AppServices.Default
{
    public class LoginAppService: ILoginAppService
    {
        private readonly IPasswordHasher<KuserEntity> _passwordHasher;
        private readonly IDefaultRepository _defaultRepository;

        public LoginAppService(IPasswordHasher<KuserEntity> passwordHasher,IDefaultRepository defaultRepository)
        {
            _passwordHasher = passwordHasher;
            _defaultRepository = defaultRepository;
        }
 

  

        //public ResultDto Signup(KuserEntity user)
        //{
        //    var resultDto = new ResultDto();
        //    bool isExist=  _defaultRepository.IsExistUser(user.Name,user.Phone, user.Email);
        //    if (isExist)
        //    {
        //        resultDto.Message = $"已存在登陆名{user.Name}";
        //        return resultDto;
        //    }

        //    return resultDto;
        //}



        public ResultDto<ClaimsIdentity> WebLogin(string name, string password)
        {
            var result = new ResultDto<ClaimsIdentity>();
            var userList = _defaultRepository.Query<KuserEntity>("select * from kuser where `Name`=@Name",new { Name = name });
            if (userList?.Count() != 1)
            {
                result.Result = false;
                result.Message = "不存在该用户";
                return result;
            }

         
            var user = userList.First();
            //user.Password = password;
            //var pa = _passwordHasher.HashPassword(user, password);
            var loginResult = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            if (loginResult == PasswordVerificationResult.Success || loginResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                result.Result = true;
                result.Data = AddSessionData(user);
            }
            else
            {
                result.Result = false;
                result.Message = "密码错误";
            }


            return result;
        }

        public ResultDto<ClaimsIdentity> WxAlive(string code)
        {
            var result = new ResultDto<ClaimsIdentity>();
            var appid = "******";
            var secret = "******";
            var url = "https://api.weixin.qq.com/sns/jscode2session?appid={0}&secret={1}&js_code={2}&grant_type=authorization_code";
            url = string.Format(url, appid, secret, code);
            var client = new HttpClient();
            var json = client.GetStringAsync(url).Result;
            var wxAuthDto = Serialize.FromJson<WxAuthDto>(json);
            if (wxAuthDto.errcode.HasValue && !string.IsNullOrEmpty(wxAuthDto.errmsg))
            {
                result.Result = false;
                result.Message = $"{wxAuthDto.errcode}:{wxAuthDto.errmsg}";
                return result;
            }
            else
            {
                var user = new KuserEntity();
                user.WxOpenId = wxAuthDto.openid;
                result.Result = true;
                result.Message = "保持alive成功";
                result.Data = AddSessionData(user, WeChatAppDefaults.AuthenticationScheme);
            }




            return result;
        }



        public ResultDto<ClaimsIdentity> WxLogin(KuserEntity user)
        {
            var result = new ResultDto<ClaimsIdentity>();

            var userCount = _defaultRepository.Count("select count(1) from kuser where WxOpenId=@WxOpenId", new { WxOpenId = user.WxOpenId });
            if (userCount <= 0)
            {
                user.KroleId = 1;
                var createResult=_defaultRepository.CreateAndGetId<KuserEntity,long>(user);
                if (!createResult.Result)
                {
                    result.Result = false;
                    result.Message = $"创建用户{user.NikeName}失败";
                    return result;
                }
            }

            var userEntity=  _defaultRepository.FirstOrDefault<KuserEntity>(new { WxOpenId = user.WxOpenId });

            result.Result = true;
            result.Data = AddSessionData(userEntity, WeChatAppDefaults.AuthenticationScheme);
            return result;
        }
        private ClaimsIdentity AddSessionData(KuserEntity user, string scheme= CookieAuthenticationDefaults.AuthenticationScheme)
        {
            user.AuthenticationType = scheme;
            var caimsIdentity = new ClaimsIdentity(user);

            caimsIdentity.AddClaim(new Claim(KardClaimTypes.IsLogin, (user.Id>0).ToString()));

            if (user.Id>0)
            {
                caimsIdentity.AddClaim(new Claim(KardClaimTypes.UserId, user.Id.ToString()));
            }

            if (!user.WxOpenId.IsNullOrEmpty())
            {
                caimsIdentity.AddClaim(new Claim(KardClaimTypes.WxOpenId, user.WxOpenId));
            }


            if (!user.Name.IsNullOrEmpty())
            {
                caimsIdentity.AddClaim(new Claim(KardClaimTypes.Name, user.Name));
            }

            if (!user.NikeName.IsNullOrEmpty())
            {
                caimsIdentity.AddClaim(new Claim(KardClaimTypes.NikeName, user.NikeName));
            }


            if (!user.Phone.IsNullOrEmpty())
            {
                caimsIdentity.AddClaim(new Claim(KardClaimTypes.Phone, user.Phone));
            }


            if (!user.Email.IsNullOrEmpty())
            {
                caimsIdentity.AddClaim(new Claim(KardClaimTypes.Email, user.Email));
            }

            return caimsIdentity;
        }

    }
}
