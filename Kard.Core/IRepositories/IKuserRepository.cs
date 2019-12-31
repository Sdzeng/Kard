using Kard.Core.Entities;
using Kard.DI;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Core.IRepositories
{
    public interface IKuserRepository : IRepository
    {
        IEnumerable<KuserEntity> GetExistUser(string name, string phone, string nickName);

        bool AddUser(OAuthUserInfo oAuthUserInfo);
    }
}
