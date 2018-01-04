using Kard.Core.Dtos;
using Kard.Core.Entities;
using Kard.Core.IRepositories;
using Kard.Extensions;
using Kard.Runtime.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Kard.Core.AppServices.Default
{
    public class DefaultAppService: IDefaultAppService
    {
        private readonly IPasswordHasher<KuserEntity> _passwordHasher;
        private readonly IDefaultRepository _defaultRepository;

        public DefaultAppService(IPasswordHasher<KuserEntity> passwordHasher,IDefaultRepository defaultRepository)
        {
            _passwordHasher = passwordHasher;
            _defaultRepository = defaultRepository;

        }

        public CoverEntity GetDateCover(DateTime showDate)
        {
            return _defaultRepository.GetDateCover(showDate);
        }

        public IEnumerable<TopMediaDto> GetTopMediaPicture(DateTime creationTime)
        {
            return _defaultRepository.GetTopMediaPicture(creationTime);
        }


        public ResultDto Signup(KuserEntity user)
        {
            var resultDto = new ResultDto();
            bool isExist=  _defaultRepository.IsExistUser(user.Name,user.Phone, user.Email);
            if (isExist)
            {
                resultDto.Message = $"已存在登陆名{user.Name}";
                return resultDto;
            }

            return resultDto;
        }



        public ResultDto<ClaimsIdentity> Login(string name, string password)
        {
            var result = new ResultDto<ClaimsIdentity>();
            var userList = _defaultRepository.GetList<KuserEntity>(new { UserName = name });
            if (userList?.Count() != 1)
            {
                result.Result = false;
                result.Message = "不存在该用户";
                return result;
            }


            var user = userList.First();
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

        private ClaimsIdentity AddSessionData(KuserEntity user)
        {
            user.AuthenticationType = CookieAuthenticationDefaults.AuthenticationScheme;
            var caimsIdentity = new ClaimsIdentity(user);

            caimsIdentity.AddClaim(new Claim(KardClaimTypes.IsLogin, (user.Id>0).ToString()));

            if (user.Id>0)
            {
                caimsIdentity.AddClaim(new Claim(KardClaimTypes.UserId, user.Id.ToString()));
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
