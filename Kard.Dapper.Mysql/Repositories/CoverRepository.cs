using Dapper;
using Kard.Core.Dtos;
using Kard.Core.IRepositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Dapper.Mysql.Repositories
{
    public class CoverRepository : Repository, ICoverRepository
    {
        public CoverRepository(IConfiguration configuration, ILogger<Repository> logger) : base(configuration, logger)
        {
        }

        public CoverDto GetDateCover(DateTime showDate)
        {
            string sql = @"select 
                                cover.*,
                                essay.CoverMediaType  EssayCoverMediaType,
                                essay.CoverPath EssayCoverPath,
                                essay.CoverExtension EssayCoverExtension,
                                essay.Title  EssayTitle,
                                essay.SubContent  EssayContent,
                                essay.PageUrl  EssayPageUrl,
                                essay.Location  EssayLocation,
                                essay.CreationTime  EssayCreationTime,
                                kuser.NickName KuserNickName,
                                kuser.Introduction KuserIntroduction, 
                                kuser.AvatarUrl KuserAvatarUrl 
		                        from 
		                        (select * from cover where showdate <= @ShowDate order by showdate desc limit 1 ) cover
		                        left join essay on cover.essayid= essay.id 
                                left join kuser on essay.creatoruserid=kuser.id 
                                where essay.isdeleted=0 ";
            return ConnExecute(conn =>
            {
                return conn.QueryFirstOrDefault<CoverDto>(sql, new { ShowDate = showDate });
            });
        }
    }
}
