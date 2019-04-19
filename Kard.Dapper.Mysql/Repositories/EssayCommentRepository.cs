using Dapper;
using Kard.Core.Dtos;
using Kard.Core.Entities;
using Kard.Core.IRepositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Dapper.Mysql.Repositories
{
    public class EssayCommentRepository : Repository, IEssayCommentRepository
    {
        public EssayCommentRepository(IConfiguration configuration, ILogger<Repository> logger) : base(configuration, logger)
        {
        }


        public IEnumerable<EssayCommentDto> GetEssayCommentList(long id)
        {
            string sql = @"select * 
                from essayComment 
                left join kuser on essayComment.CreatorUserId=kuser.Id 
                where essayComment.EssayId=@EssayId and essayComment.IsDeleted=0 
               order by essayComment.CreationTime desc";

            return ConnExecute(conn => conn.Query<EssayCommentDto, KuserEntity, EssayCommentDto>(sql, (essayComment, kuser) => { essayComment.Kuser = kuser.ToSecurity(); return essayComment; },
                                                                                                                                                                          new { EssayId = id },
                                                                                                                                                                          splitOn: "Id"));
        }
    }
}
