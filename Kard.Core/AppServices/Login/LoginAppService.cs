using Kard.Core.Dtos;
using Kard.Core.Entities;
using Kard.Core.IRepositories;
using Kard.Extensions;
using Kard.Json;
using Kard.Runtime.Security;
using Kard.Runtime.Security.Authentication.WeChat;
using Kard.Runtime.Session;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;


namespace Kard.Core.AppServices.Default
{
    public class LoginAppService : ILoginAppService
    {
        //private readonly UserManager<KuserEntity> _userManager;
        private readonly IKardSession _kardSession;
        private readonly IPasswordHasher<KuserEntity> _passwordHasher;
        //private readonly IPasswordValidator<KuserEntity> _passwordValidator;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IDefaultRepository _defaultRepository;

 
        public LoginAppService(
             IServiceProvider serviceProvider,
            IKardSession kardSession,
            IPasswordHasher<KuserEntity> passwordHasher,
              IRepositoryFactory repositoryFactory)
        {
          
            _kardSession = kardSession;
            _passwordHasher = passwordHasher;

            _repositoryFactory = repositoryFactory;
            _defaultRepository = repositoryFactory.GetRepository<IDefaultRepository>();


            //测试
            //scope.Dispose()-->childProvider.Dispose()删除对Service实例的引用
            using (var scope = serviceProvider.GetService<IServiceScopeFactory>().CreateScope())
            {
                var childProvider = scope.ServiceProvider;

                var r1 = serviceProvider.GetService<IKuserRepository>();
                var r2 = childProvider.GetService<IKuserRepository>();
                bool result = (r1.GetHashCode().Equals(r2.GetHashCode()));
                string ee = result.ToString();
            }
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



        public ResultDto<ClaimsIdentity> AccountLogin(string name, string password)
        {
            var result = new ResultDto<ClaimsIdentity>();
            var userList = _defaultRepository.Query<KuserEntity>("select * from kuser where `Name`=@Name", new { Name = name });
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
            //if (loginResult == PasswordVerificationResult.Success || loginResult == PasswordVerificationResult.SuccessRehashNeeded
            if (loginResult == PasswordVerificationResult.Success)
            {
                result.Result = true;
                result.Data = AddSessionData(user);
            }
            else if (loginResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                var passwordHash = _passwordHasher.HashPassword(user, password);
                user.Password = passwordHash;
                var updateUserResult = _defaultRepository.Update(user);
                result.Result = updateUserResult.Result;
                if (result.Result)
                {
                    result.Data = AddSessionData(user);
                }
            }
            else
            {
                result.Result = false;
                result.Message = "密码错误";
            }


            return result;
        }




        public ResultDto<ClaimsIdentity> WxLogin(string code, WeChatUserDto userInfo)
        {
            var result = new ResultDto<ClaimsIdentity>();
            var appid = "wx109fc14b4956fc70";
            var secret = "a8e7f19d69cbde0272fd866fe7392874";
            var url = $"https://api.weixin.qq.com/sns/jscode2session?appid={appid}&secret={secret}&js_code={code}&grant_type=authorization_code";
            
            var client = new HttpClient();
            var json = client.GetStringAsync(url).Result;
            var wxAuthDto = Serialize.FromJson<WxAuthDto>(json);
            if (wxAuthDto.errcode.HasValue && !string.IsNullOrEmpty(wxAuthDto.errmsg))
            {
                result.Result = false;
                result.Message = $"{wxAuthDto.errcode}:{wxAuthDto.errmsg}";
                return result;
            }



            var user = _defaultRepository.FirstOrDefaultByPredicate<KuserEntity>(new { WxOpenId = wxAuthDto.openid });
            if (user != null)
            {
                user.WxSessionKey = wxAuthDto.session_key;
                user.AvatarUrl = userInfo.AvatarUrl;
                user.NickName = userInfo.NickName;
                user.City = userInfo.City;
                user.Language = userInfo.Language;
                user.Gender = userInfo.Gender;

                var updateUserResultDto = _defaultRepository.Update(user);
                if (!updateUserResultDto.Result)
                {
                    result.Result = false;
                    result.Message = $"用户{user.NickName}被带晕";
                    return result;
                }
            }
            else
            {
                user = new KuserEntity();
                user.WxOpenId = wxAuthDto.openid;
                user.WxSessionKey = wxAuthDto.session_key;
                user.UserType = "WeChatApp";
                user.KroleId = 1;
                user.AvatarUrl = userInfo.AvatarUrl;
                user.NickName = userInfo.NickName;
                user.City = userInfo.City;
                user.Language = userInfo.Language;
                user.Gender = userInfo.Gender;
                user.CreationTime = DateTime.Now;

                var createResult = _defaultRepository.CreateAndGetId<KuserEntity, long>(user);
                if (!createResult.Result)
                {
                    result.Result = false;
                    result.Message = $"用户{user.NickName}被迷路";
                    return result;
                }
            }



            result.Result = true;
            result.Message = "login成功";
            result.Data = AddSessionData(user, WeChatAppDefaults.AuthenticationScheme);

            return result;
        }


        public ResultDto Register(string registerType, KuserEntity user)
        {
            var resultDto = new ResultDto();
            switch (registerType)
            {
                case "accountRegister": resultDto = AccountRegister(user); break;
                //case "wxRegister": resultDto = WxRegister(user); break;
            }
            return resultDto;
        }

        private ResultDto AccountRegister(KuserEntity user)
        {
            var resultDto = new ResultDto();
            var existUserList = _repositoryFactory.GetRepository<IKuserRepository>().GetExistUser(user.Name, user.Phone, user.NickName);
            var existName = existUserList.Where(u => u.Name == user.Name).Any();
            var existPhone = existUserList.Where(u => u.Phone == user.Phone).Any();
            var existNickName = existUserList.Where(u => u.NickName == user.NickName).Any();
            if (existName || existPhone || existNickName)
            {
                resultDto.Result = false;
                resultDto.Message = $"{(existName ? "邮箱 " : "")}{(existPhone ? "手机 " : "")}{(existNickName ? "昵称 " : "")}已被注册";
                return resultDto;
            }

            //newEntity.AppId = _defaultRepository.FirstOrDefault<RetailerEntity, string>(newEntity.RetailerId)?.AppId;
            //var result = _passwordValidator.ValidateAsync(_userManager, user, user.Password).Result;
            //if (!result.Succeeded)
            //{
            //    resultDto.Result = false;
            //    resultDto.Message = $"密码{ string.Join(";", result.Errors)}";
            //    return resultDto;
            //}

            user.Password = _passwordHasher.HashPassword(user, user.Password);



            //resultDto.Result = _defaultRepository.CreateAccountUser(user);
            //resultDto.Message = resultDto.Result?"注册成功": "注册失败";
            //return resultDto;
            var createResultDto= _defaultRepository.CreateAndGetId<KuserEntity,long>(user);
            resultDto.Result = createResultDto.Result;
            resultDto.Message = createResultDto.Message;
            return resultDto;

        }
        //private ResultDto WxRegister(KuserEntity user)
        //{
        //    var result = new ResultDto();

        //    var userEntity = _defaultRepository.FirstOrDefaultByPredicate<KuserEntity>(new { WxOpenId = user.WxOpenId });
        //    if (userEntity == null)
        //    {
        //        result.Result = false;
        //        result.Message = $"用户{user.NickName}未登陆成功，注册失败";
        //        return result;
        //    }

        //    userEntity.AvatarUrl = user.AvatarUrl;
        //    userEntity.NickName = user.NickName;
        //    userEntity.City = user.City;
        //    userEntity.Language = user.Language;
        //    userEntity.AuditLastModification(_kardSession.UserId.Value);
        //    result = _defaultRepository.Update(userEntity);
        //    if (!result.Result)
        //    {
        //        result.Message = $"用户{user.NickName}注册失败";
        //        return result;
        //    }

        //    result.Result = true;
        //    result.Message = "注册成功";
        //    return result;
        //}

        private ClaimsIdentity AddSessionData(KuserEntity user, string scheme = CookieAuthenticationDefaults.AuthenticationScheme)
        {
            user.AuthenticationType = scheme;
            var caimsIdentity = new ClaimsIdentity(user);

            //caimsIdentity.AddClaim(new Claim(KardClaimTypes.IsLogin, (user.Id > 0).ToString()));

            if (user.Id > 0)
            {
                caimsIdentity.AddClaim(new Claim(KardClaimTypes.UserId, user.Id.ToString()));
            }

            if (!user.WxOpenId.IsNullOrEmpty())
            {
                caimsIdentity.AddClaim(new Claim(KardClaimTypes.WxUnionId, user.WxOpenId));
            }

            //if (!user.WxSessionKey.IsNullOrEmpty())
            //{
            //    caimsIdentity.AddClaim(new Claim(KardClaimTypes.WxSessionKey, user.WxSessionKey));
            //}


            if (!user.Name.IsNullOrEmpty())
            {
                caimsIdentity.AddClaim(new Claim(KardClaimTypes.Name, user.Name));
            }

            //if (!user.NickName.IsNullOrEmpty())
            //{
            //    caimsIdentity.AddClaim(new Claim(KardClaimTypes.NickName, user.NickName));
            //}


            //if (!user.Phone.IsNullOrEmpty())
            //{
            //    caimsIdentity.AddClaim(new Claim(KardClaimTypes.Phone, user.Phone));
            //}


            //if (!user.Email.IsNullOrEmpty())
            //{
            //    caimsIdentity.AddClaim(new Claim(KardClaimTypes.Email, user.Email));
            //}

            return caimsIdentity;
        }

    }
}
