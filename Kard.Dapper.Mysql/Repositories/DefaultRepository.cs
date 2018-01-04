using Dapper;
using Kard.Core.Dtos;
using Kard.Core.Entities;
using Kard.Core.IRepositories;
using Kard.Runtime.Session;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kard.Dapper.Mysql.Repositories
{
    public class DefaultRepository : Repository, IDefaultRepository
    {
      

        public DefaultRepository(IKardSession session,IConfiguration configuration) : base(session,configuration)
        {
        }

        public CoverEntity GetDateCover(DateTime showDate)
        {
            return ConnExecute(conn =>
            {
                string sql = @"select 
                                cover.*,
	                            media.*,
                                essay.*,
		                        kuser.*   
		                        from 
		                        (select * from cover where showdate <= @ShowDate order by showdate desc limit 1 ) cover
		                        left join media on cover.mediaid = media.id   
		                        left join essay on media.essayid = essay.id 
                                left join kuser on essay.creator=kuser.id 
                                where essay.isdeleted=0 and media.mediatype='picture' ";
                var entityList = conn.Query<CoverEntity, MediaEntity, EssayEntity, KuserEntity, CoverEntity>(sql, (cover, media, essay, kuser) =>
                  {
                      media.Essay = essay;
                      media.Kuser = kuser;
                      cover.Media = media;
                      return cover;
                  },
                  new { ShowDate = showDate },
                  splitOn: "Id");

            if (entityList != null && entityList.Any())
                {
                    return entityList.First();
                }

                return null;
            });
        }


        public IEnumerable<TopMediaDto> GetTopMediaPicture(DateTime creationTime)
        {
            return ConnExecute(conn =>
            {

                string sql = @"select t.EssayMediaCount,essay.LikeNum EssayLikeNum,media.EssayId,media.CdnPath,media.MediaExtension,essay.Content EssayContent,essay.Creator,kuser.NikeName CreatorNikeName from (
                    select  media.EssayId,min(media.Sort) MinSort,count(media.Id) EssayMediaCount
                    from media join essay on media.EssayId=essay.Id and media.MediaType='picture' and media.CreationTime>@CreationTime 
                    group by media.EssayId  order by essay.LikeNum desc limit 7
                    ) t join media on t.EssayId=media.EssayId and t.MinSort=media.Sort 
                   join essay on media.EssayId=essay.Id 
                   join kuser on essay.Creator=kuser.Id   
                  order by EssayLikeNum desc";
                var topMediaDtoList = conn.Query<TopMediaDto>(sql, new { CreationTime = creationTime });

                topMediaDtoList = topMediaDtoList.Where((m, index) => index != 0);

                return topMediaDtoList;
            });
        }

        public bool IsExistUser(string name, string phone, string email)
        {
            string sql = "select count(1)  from kuser where `Name`=@Name or Phone=@Phone or Email=@Email";
            var result=ConnExecute(conn => conn.ExecuteScalar<int>(sql, new { Name = name, Phone = phone, Email = email }));
            return result > 0;
        }
   

    }
}
