using Dapper;
using Kard.Core.Dtos;
using Kard.Core.Entities;
using Kard.Core.IRepositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Kard.Dapper.Mysql.Repositories
{
    public class EssayRepository : Repository, IEssayRepository
    {
        public EssayRepository(IConfiguration configuration, ILogger<Repository> logger) : base(configuration, logger)
        {
        }

        public IEnumerable<TopMediaDto> GetHomeMediaPictureList(string category, int pageIndex, int pageSize)
        {
            int pageStart = ((pageIndex - 1) * pageSize);
            pageStart = pageStart > 0 ? (pageStart - 1) : pageStart;

            var sql = $@"select essay.Id,essay.Category,essay.IsOriginal,essay.Score,essay.ShareNum,essay.LikeNum,essay.BrowseNum,essay.CommentNum,essay.title,essay.Location,essay.CreatorUserId,essay.CreationTime,essay.CoverPath,essay.CoverMediaType,essay.CoverExtension,
                               kuser.AvatarUrl,kuser.NickName CreatorNickName,tag.*  
                from 
                essay  
                join kuser on essay.CreatorUserId=kuser.Id  
                left join tag on essay.Id=tag.EssayId and tag.Sort=1 
                where 1=1 {(category != "精选" ? " and essay.Category=@Category " : "")} 
                order by (essay.LikeNum+essay.ShareNum+essay.BrowseNum+essay.CommentNum) desc,essay.Score desc,essay.Id desc limit @PageStart,@PageSize
                ";

            var param = new { Category = category, PageStart = pageStart, PageSize = pageSize };



            //return ConnExecute(conn =>
            //{
            //    var dtoList = new List<TopMediaDto>();
            //    conn.Query<TopMediaDto, TagEntity, bool>(sql, (dto, tag) =>
            //       {
            //           var currentDto = dtoList.FirstOrDefault(d => d.Id == dto.Id);
            //           if (currentDto == null)
            //           {
            //               dto.TagList = new List<TagEntity>();
            //               dto.TagList.Add(tag);
            //               dtoList.Add(dto);
            //           }
            //           else
            //           {
            //               currentDto.TagList.Add(tag);
            //           }

            //           return true;
            //       },
            //      param: param,
            //      splitOn: "Id");

            //    return dtoList;
            //});

            return ConnExecute(conn =>
            {

                var dtoList = conn.Query<TopMediaDto, TagEntity, TopMediaDto>(sql, (dto, tag) =>
                {
                    if (tag != null)
                    {
                        dto.TagList = new List<TagEntity>();
                        dto.TagList.Add(tag);
                    }
                    return dto;
                },
                  param: param,
                  splitOn: "Id");

                return dtoList;
            });


        }


        public IEnumerable<TopMediaDto> GetUserMediaPictureList(long userId, int count)
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
                var topMediaDtoList = conn.Query<TopMediaDto>(sql, new { CreatorUserId = userId, Count = count });

                return topMediaDtoList;
            });
        }



        public IEnumerable<EssayDto> GetEssayList(DateTime creationTime)
        {
            string sql = @"select essay.*,kuser.NickName KuserNickName,tag.* 
                from essay 
                left join kuser on essay.CreatorUserId=kuser.Id 
                left join tag on essay.Id=tag.EssayId 
                order by essay.LikeNum desc,media.Sort ";

            return ConnExecute(conn =>
            {

                var essayList = conn.Query<EssayDto, TagEntity, EssayDto>(sql, (essay, tag) =>
                {

                    essay.TagList = essay.TagList ?? new List<TagEntity>();

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


        public EssayDto GetEssayDto(long id, long? currentUserId)
        {
            string sql = @"select essay.*,kuser.NickName KuserNickName,kuser.Introduction KuserIntroduction,kuser.AvatarUrl KuserAvatarUrl,(essay_like.id!=null)   IsLike
                from essay 
                left join kuser on essay.CreatorUserId=kuser.Id 
                left join essay_like on essay.Id=essay_like.EssayId and essay_like.CreatorUserId=@CurrentUserId 
                where essay.id=@EssayId  ";

            return ConnExecute(conn =>
            {
                var essayDto = conn.QueryFirstOrDefault<EssayDto>(sql, new { EssayId = id, CurrentUserId = currentUserId });
                essayDto.TagList = conn.Query<TagEntity>("select * from tag where EssayId=@EssayId  order by  Sort", new { EssayId = id, })?.ToList();
                return essayDto;
            });
        }

        public bool AddEssay(EssayEntity essayEntity, IEnumerable<TagEntity> tagList)
        {

            return base.TransExecute((conn, trans) =>
            {

                var insertAndGetIdResultDto = conn.CreateAndGetId<EssayEntity, long>(essayEntity, trans);
                if (!insertAndGetIdResultDto.Result)
                {
                    return false;
                }

                if (tagList != null && tagList.Any())
                {
                    tagList = tagList.Select(tag =>
                    {
                        tag.EssayId = insertAndGetIdResultDto.Data;
                        return tag;
                    });

                    var insertListResultDto = conn.CreateList(tagList, trans);
                    if (!insertListResultDto.Result)
                    {
                        return false;
                    }
                }

                return true;
            });
        }


        public bool UpdateBrowseNum(long id)
        {
            //单个改动使用update的排他（update\delete\insert InnoDB会自动给涉及数据集加上）行锁（使用索引）就行
            string sql = "update essay set  BrowseNum=(BrowseNum+1) where Id=@Id";

            var result = ConnExecute(conn => conn.Execute(sql, new { Id = id }));
            return result > 0;
        }

 


        public IEnumerable<EssayEntity> GetEssaySimilarList(long id)
        {
            string sql = @"select distinct b.Id,b.Title,b.Content,b.LikeNum,b.CreationTime from 
            (select essay.Category,tag.TagName from essay join tag on essay.Id=@EssayId and essay.IsDeleted=0 and essay.Id=tag.EssayId ) a
             join (select essay.*,tag.TagName  from essay join tag on  essay.Id<>@EssayId and essay.IsDeleted=0 and essay.Id=tag.EssayId )  b on a.Category=b.Category and a.TagName=b.TagName 
            order by b.LikeNum desc,b.CreationTime desc limit 10";

            return ConnExecute(conn => conn.Query<EssayEntity>(sql, new { EssayId = id }));
        }


        public IEnumerable<EssayEntity> GetEssayOtherList(long id)
        {
            string sql = @"select * from essay where IsDeleted=0 and  Id<>@EssayId and CreatorUserId=(select CreatorUserId from essay where Id=@EssayId) order by LikeNum desc,CreationTime desc limit 10";

            return ConnExecute(conn => conn.Query<EssayEntity>(sql, new { EssayId = id }));
        }



    }
}
