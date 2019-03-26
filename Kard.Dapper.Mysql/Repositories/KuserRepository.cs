using Dapper;
using Kard.Core.Entities;
using Kard.Core.IRepositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Dapper.Mysql.Repositories
{
    public class KuserRepository : Repository, IKuserRepository
    {
        public KuserRepository(IConfiguration configuration, ILogger<Repository> logger) : base(configuration, logger)
        {
        }

        public IEnumerable<KuserEntity> GetExistUser(string name, string phone, string nickName)
        {
            string sql = "select *  from kuser where `Name`=@Name or Phone=@Phone or NickName=@NickName";
            return ConnExecute(conn => conn.Query<KuserEntity>(sql, new { Name = name, Phone = phone, NickName = nickName }));
        }
    }
}
