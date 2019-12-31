using Dapper;
using Kard.Core.Entities;
using Kard.Core.IRepositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Dapper.Mysql.Repositories
{
    public class KuserRepository : IKuserRepository
    {
        private readonly IDefaultRepository _defaultRepository;
        public KuserRepository(IDefaultRepository defaultRepository)
        {
            _defaultRepository = defaultRepository;
        }

        public IEnumerable<KuserEntity> GetExistUser(string name, string phone, string nickName)
        {
            string sql = "select *  from kuser where `Name`=@Name or Phone=@Phone or NickName=@NickName";
            return _defaultRepository.ConnExecute(conn => conn.Query<KuserEntity>(sql, new { Name = name, Phone = phone, NickName = nickName }));
        }


 

        public bool AddUser(OAuthUserInfo oAuthUserInfo)
        {
            var kuserEntity = new KuserEntity();
            kuserEntity.UserType = "WeChatApp";
            kuserEntity.KroleId = 1;
            kuserEntity.AvatarUrl = oAuthUserInfo.headimgurl;
            kuserEntity.NickName = oAuthUserInfo.nickname;
            kuserEntity.City = oAuthUserInfo.city;
            kuserEntity.Country = oAuthUserInfo.country;
            kuserEntity.Gender = oAuthUserInfo.sex;
            kuserEntity.CreationTime = DateTime.Now;


            var kuserWeChatEntity = new KuserWeChatEntity();
            kuserWeChatEntity.OpenId = oAuthUserInfo.openid;
            kuserWeChatEntity.SessionKey =string.Empty;
            kuserWeChatEntity.UnionId = oAuthUserInfo.unionid;
            return _defaultRepository.TransExecute((conn, trans) =>
            {

                var createResult = conn.CreateAndGetId<KuserEntity, long>(kuserEntity, trans);
                if (!createResult.Result)
                {
                    return false;
                }
                kuserWeChatEntity.KuserId = createResult.Data;
                kuserWeChatEntity.AuditCreation(kuserWeChatEntity.KuserId);
                conn.CreateAndGetId<KuserWeChatEntity, int>(kuserWeChatEntity, trans);
                return true;
            });
       }
    }
}
