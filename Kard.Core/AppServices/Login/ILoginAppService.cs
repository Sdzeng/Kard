using Kard.Core.Dtos;
using Kard.Core.Entities;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Kard.Core.AppServices.Default
{
    public interface ILoginAppService: IAppService
    {


        //ResultDto Signup(KuserEntity user);

        ResultDto<ClaimsIdentity> Login(string name, string password);


    }
}
