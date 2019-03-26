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
                from essay_comment 
                left join kuser on essay_comment.CreatorUserId=kuser.Id 
                where essay_comment.EssayId=@EssayId and essay_comment.IsDeleted=0 
               order by essay_comment.CreationTime desc";

            return ConnExecute(conn => conn.Query<EssayCommentDto, KuserEntity, EssayCommentDto>(sql, (essayComment, kuser) => { essayComment.Kuser = kuser.ToSecurity(); return essayComment; },
                                                                                                                                                                          new { EssayId = id },
                                                                                                                                                                          splitOn: "Id"));
        }
    }
}
