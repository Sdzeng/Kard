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
    public class EssayCommentRepository : IEssayCommentRepository
    {
        private readonly IDefaultRepository _defaultRepository;
        public EssayCommentRepository(IDefaultRepository defaultRepository)
        {
            _defaultRepository = defaultRepository;
        }


        public IEnumerable<EssayCommentDto> GetEssayCommentList(long id)
        {
            string sql = @"select essayComment.*,kuser.AvatarUrl KuserAvatarUrl,kuser.NickName KuserNickName 
                from essayComment 
                left join kuser on essayComment.CreatorUserId=kuser.Id 
                where essayComment.EssayId=@EssayId and essayComment.IsDeleted=0 
               order by essayComment.CreationTime desc";

            return _defaultRepository.ConnExecute(conn => conn.Query<EssayCommentDto>(sql, new { EssayId = id }));
        }
    }
}
