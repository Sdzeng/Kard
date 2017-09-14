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

                string sql = @"select t2.*,essay.Title,kuser.NikeName CreatorNikeName from (
                select count(t.EssayId) EssayMediaCount,t.HeartNum MediaHeartNum,t.EssayId,t.CdnPath,t.MediaExtension,t.Creator 
                                from(select * from media where CreationTime >@CreationTime and MediaType = 'picture' order by HeartNum desc) t
                                    group by t.EssayId limit 4
                )t2 LEFT join  essay on t2.EssayId=essay.Id LEFT JOIN kuser on t2.Creator=kuser.Id
                ";
                var topMediaDtoList = connecton.Query<TopMediaDto>(sql, new { CreationTime = creationTime });

                return topMediaDtoList;
            }
        }

    }
}
