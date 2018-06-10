using Dapper;
using Kard.Core.Dtos;
using Kard.Core.Entities;
using Kard.Core.IRepositories;
using Kard.Runtime.Session;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kard.Dapper.Mysql.Repositories
{
    public class DefaultRepository : Repository, IDefaultRepository
    {


        public DefaultRepository(IKardSession session, IConfiguration configuration, ILogger<DefaultRepository> logger) : base(session, configuration, logger)
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
                                left join kuser on essay.creatoruserid=kuser.id 
                                where essay.isdeleted=0 and media.mediatype='picture'  ";
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




        public IEnumerable<TopMediaDto> GetHomeMediaPicture(int count)
        {
            return ConnExecute(conn =>
            {

                string sql = @"select essay.Id,essay.Category,essay.CollectNum,essay.LikeNum,essay.RepostNum,essay.CommentNum,essay.title,essay.CreatorUserId,kuser.NickName CreatorNickName,t2.MediaCount,t2.CdnPath,t2.MediaExtension,tag.*  from 
                (
                select t.EssayId,t.CdnPath,t.MediaExtension,count(media.Id) MediaCount
                from (
                select EssayId,CdnPath,MediaExtension from media  where   Sort=1 and MediaType='picture' order by CreationTime desc limit @Count
                ) t join media on t.EssayId=media.EssayId
                group by t.EssayId,t.CdnPath,t.MediaExtension
                ) t2 
                join essay on t2.EssayId=essay.Id 
                join kuser on essay.CreatorUserId=kuser.Id  
                join tag on essay.Id=tag.EssayId 
                order by essay.LikeNum desc,essay.CreationTime desc";

                var dtoList = new List<TopMediaDto>();
                conn.Query<TopMediaDto, TagEntity,bool>(sql, (dto, tag) =>
                  {
                      var currentDto = dtoList.FirstOrDefault(d => d.Id == dto.Id);
                      if (currentDto == null)
                      {
                          dto.TagList = new List<TagEntity>();
                          dto.TagList.Add(tag);
                          dtoList.Add(dto);
                      }
                      else
                      {
                          currentDto.TagList.Add(tag);
                      }

                      return true;
                  },
                  param: new { Count = count },
                  splitOn: "Id");

                //topMediaDtoList = topMediaDtoList.Where((m, index) => index != 0);

                return dtoList;
            });
        }


        public IEnumerable<TopMediaDto> GetUserMediaPicture(int count)
        {
            return ConnExecute(conn =>
            {
                string sql = @"select t.EssayMediaCount,essay.LikeNum EssayLikeNum,media.EssayId,media.CdnPath,media.MediaExtension,essay.Content EssayContent,essay.CreatorUserId,kuser.NickName CreatorNickName from (
                    select  media.EssayId,min(media.Sort) MinSort,count(media.Id) EssayMediaCount
                    from media join essay on media.EssayId=essay.Id and media.MediaType='picture'  
                    where media.CreatorUserId=@CreatorUserId 
                    group by media.EssayId  order by essay.LikeNum desc  limit @Count 
                    ) t join media on t.EssayId=media.EssayId and t.MinSort=media.Sort 
                   join essay on media.EssayId=essay.Id 
                   join kuser on essay.CreatorUserId=kuser.Id   
                  order by EssayLikeNum desc,essay.CreationTime desc";
                var topMediaDtoList = conn.Query<TopMediaDto>(sql, new { CreatorUserId = KardSession.UserId, Count = count });

                return topMediaDtoList;
            });
        }

        public IEnumerable<EssayEntity> GetEssay(DateTime creationTime)
        {
            return ConnExecute(conn =>
            {

                string sql = @"select *,(select NickName from kuser where Id=essay.CreatorUserId) CreatorUserName 
                from essay 
                left join media on essay.Id=media.EssayId 
                left join tag on essay.Id=tag.EssayId 
                order by essay.LikeNum desc,media.Sort ";
                var essayList = conn.Query<EssayEntity, MediaEntity, TagEntity, EssayEntity>(sql, (essay, media, tag) =>
                {
                    essay.MediaList = essay.MediaList ?? new List<MediaEntity>();
                    essay.TagList = essay.TagList ?? new List<TagEntity>();
                    if (!essay.MediaList.Where(m => m.Id == media.Id).Any())
                    {
                        essay.MediaList.Add(media);
                    }

                    if (!essay.TagList.Where(t => t.Id == tag.Id).Any())
                    {
                        essay.TagList.Add(tag);
                    }

                    return essay;
                },
                  splitOn: "Id");



                return essayList;
            });
        }

        public bool IsExistUser(string name, string phone, string email)
        {
            string sql = "select count(1)  from kuser where `Name`=@Name or Phone=@Phone or Email=@Email";
            var result = ConnExecute(conn => conn.ExecuteScalar<int>(sql, new { Name = name, Phone = phone, Email = email }));
            return result > 0;
        }


        public KuserEntity GetUser(long id)
        {
            return base.FirstOrDefault<KuserEntity>(new { Id = id });
        }

    }
}
