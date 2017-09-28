using Dapper;
using Kard.Core.Dtos;
using Kard.Core.Entities;
using Kard.Core.IRepositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;

namespace Kard.Dapper.Mysql.Repositories
{
    public class MediaRepository : Repository, IMediaRepository
    {
        public MediaRepository(IConfiguration configuration) : base(configuration)
        {
        }


        public IEnumerable<TopMediaDto> GetTopMediaPicture(DateTime creationTime)
        {
            using (IDbConnection connecton = GetConnection())
            {

                string sql = @"select t.EssayMediaCount,essay.LikeNum EssayLikeNum,media.EssayId,media.CdnPath,media.MediaExtension,essay.Content EssayContent,essay.Creator,kuser.NikeName CreatorNikeName from (
                    select  media.EssayId,min(media.Sort) MinSort,count(media.Id) EssayMediaCount
                    from media join essay on media.EssayId=essay.Id and media.MediaType='picture' and media.CreationTime>@CreationTime 
                    group by media.EssayId  order by essay.LikeNum desc limit 4
                    ) t join media on t.EssayId=media.EssayId and t.MinSort=media.Sort 
                   join essay on media.EssayId=essay.Id 
                   join kuser on essay.Creator=kuser.Id   
                  order by EssayLikeNum desc";
                var topMediaDtoList = connecton.Query<TopMediaDto>(sql, new { CreationTime = creationTime });

                return topMediaDtoList;
            }
        }

    }
}
