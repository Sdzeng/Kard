using Dapper;
using DapperExtensions;
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
using System.Text.RegularExpressions;

namespace Kard.Dapper.Mysql.Repositories
{
    public class EssayRepository : Repository, IEssayRepository
    {
        public EssayRepository(IConfiguration configuration, ILogger<Repository> logger) : base(configuration, logger)
        {
        }

        public IEnumerable<TopMediaDto> GetHomeMediaPictureList(string keyword, int pageIndex, int pageSize, string orderBy)
        {
            int pageStart = ((pageIndex - 1) * pageSize);
            pageStart = pageStart > 0 ? (pageStart - 1) : pageStart;


            if (string.IsNullOrEmpty(orderBy))
            {
                orderBy = "essay.id desc";
            }
            else
            {
                switch (orderBy)
                {
                    case "choiceness": orderBy = "(essay.LikeNum+essay.ShareNum+essay.BrowseNum+essay.CommentNum) desc,essay.Score desc,essay.Id desc"; break;
                    default: break;
                }
            }

            var sql = $@"select essay.Id,essay.Category,essay.Score,essay.ScoreHeadCount,essay.ShareNum,essay.LikeNum,essay.BrowseNum,essay.CommentNum,essay.Title,essay.SubContent,essay.PageUrl,essay.Location,essay.CreatorUserId,essay.CreationTime,essay.CoverPath,essay.CoverMediaType,essay.CoverExtension,
                               kuser.AvatarUrl,kuser.NickName CreatorNickName,tag.*  
                from 
                essay  
                join kuser on essay.CreatorUserId=kuser.Id  
                left join tag on essay.Id=tag.EssayId and tag.Sort=1 
                where essay.IsPublish=1 {((!string.IsNullOrEmpty(keyword )) ? " and (essay.Category like @Keyword or essay.Title like @Keyword ) " : "")} 
                order by {orderBy} limit @PageStart,@PageSize
                ";

            var param = new { Keyword = $"%{keyword}%", PageStart = pageStart, PageSize = pageSize };



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

        public IEnumerable<object> GetUserNews(long userId, int pageIndex, int pageSize, string orderBy)
        {

            var resultList = new List<object>();
            int pageStart = ((pageIndex - 1) * pageSize);
            pageStart = pageStart > 0 ? (pageStart - 1) : pageStart;
            string followUserSql = "select BeConcernedUserId from kuserFans where CreatorUserId =@CreatorUserId ";
            var followUsers = Query<long>(followUserSql, new { CreatorUserId = userId }).ToList();
            followUsers.Add(userId);
            string sql = $@"select t.* from(
                                    select 'essay' as newsType,a.Id,a.CreationTime,a.Title as EssayTitle,a.PageUrl as EssayPageUrl,b.NickName,b.AvatarUrl from essay a join kuser b on a.CreatorUserId=b.id where a.IsPublish=1 and a.IsDeleted=0 and a.CreatorUserId in @FollowUsers 
                                    union
                                    select 'essayLike' as newsType,a.Id,a.CreationTime ,c.Title as EssayTitle,c.PageUrl as EssayPageUrl,b.NickName,b.AvatarUrl from essayLike a join kuser b on a.CreatorUserId=b.id join essay c on  a.EssayId=c.Id where c.CreatorUserId =@CreatorUserId 
                                    union
                                    select 'essayComment' as newsType,a.Id,a.CreationTime,c.Title as EssayTitle,c.PageUrl as EssayPageUrl,b.NickName,b.AvatarUrl from essayComment a join kuser b on a.CreatorUserId=b.id join essay c on  a.EssayId=c.Id where a.IsDeleted=0 and a.CreatorUserId <> @CreatorUserId and c.CreatorUserId = @CreatorUserId 
                                    union
                                    select 'kuserFans' as newsType,a.Id,a.CreationTime,'' as EssayTitle,'' as EssayPageUrl,b.NickName,b.AvatarUrl from kuserFans  a join kuser b on a.CreatorUserId=b.id where a.BeConcernedUserId =@CreatorUserId 
                                    union
                                    select 'kuserFollow' as newsType,a.Id,a.CreationTime,'' as EssayTitle,'' as EssayPageUrl,b.NickName,b.AvatarUrl from kuserFans a join kuser b on a.BeConcernedUserId=b.id  where a.CreatorUserId =@CreatorUserId 
                                    ) t
                                    order by {orderBy} limit @PageStart,@PageSize";

            var userNewsDtoList = ConnExecute(conn => conn.Query<UserNewsDto>(sql, new { FollowUsers = followUsers, CreatorUserId = userId, PageStart = pageStart, PageSize = pageSize })) ?? new List<UserNewsDto>();
            foreach (var dto in userNewsDtoList)
            {
                object info = null;
                switch (dto.NewsType)
                {
                    case "essay":
                        info = ConnExecute(conn => conn.QueryFirstOrDefault<EssayEntity>("select Id,Title,SubContent,Location, Category, CoverMediaType, CoverPath, CoverExtension,CreationTime from essay where id=@Id", new { Id = dto.Id }));
                        break;
                    case "essayLike": info = FirstOrDefault<EssayLikeEntity>(dto.Id); break;
                    case "essayComment": info = FirstOrDefault<EssayCommentEntity>(dto.Id); break;
                    case "kuserFans": info = FirstOrDefault<KuserFansEntity>(dto.Id); break;
                    case "kuserFollow": info = FirstOrDefault<KuserFansEntity>(dto.Id); break;
                    default: continue;
                }
                resultList.Add(new { Dto = dto, Info = info });
            }

            return resultList;
        }



        public IEnumerable<EssayEntity> GetUserEssay(long userId, int pageIndex, int pageSize, string orderBy, bool? isPublish=null)
        {
          
            int pageStart = ((pageIndex - 1) * pageSize);
            pageStart = pageStart > 0 ? (pageStart - 1) : pageStart;
          
            string sql = $@"select * from essay  where CreatorUserId =@CreatorUserId and IsDeleted=0 {(isPublish.HasValue? "and IsPublish=@IsPublish":"")}  order by {orderBy} limit @PageStart,@PageSize";

            var essayEntityList = ConnExecute(conn =>  conn.Query<EssayEntity>(sql, new { CreatorUserId = userId, IsPublish= isPublish,PageStart = pageStart, PageSize = pageSize }))??new List<EssayEntity>();

            return essayEntityList;
        }


        public IEnumerable<EssayLikeDto> GetUserLike(long userId, int pageIndex, int pageSize, string orderBy)
        {
        
            int pageStart = ((pageIndex - 1) * pageSize);
            pageStart = pageStart > 0 ? (pageStart - 1) : pageStart;

            string sql = $@"select a.*,b.Title as EssayTitle,b.PageUrl EssayPageUrl  from essayLike a join essay b on  a.EssayId=b.Id  where a.CreatorUserId =@CreatorUserId and b.IsPublish=1 and b.IsDeleted=0 order by {orderBy} limit @PageStart,@PageSize";

            var essayEntityList = ConnExecute(conn => conn.Query<EssayLikeDto>(sql, new { CreatorUserId = userId, PageStart = pageStart, PageSize = pageSize })) ?? new List<EssayLikeDto>();

            return essayEntityList;
        }


        public IEnumerable<EssayCommentDto> GetUserComment(long userId, int pageIndex, int pageSize, string orderBy)
        {

            int pageStart = ((pageIndex - 1) * pageSize);
            pageStart = pageStart > 0 ? (pageStart - 1) : pageStart;

            string sql = $@"select a.*,b.Title as EssayTitle,b.PageUrl EssayPageUrl,d.AvatarUrl KuserAvatarUrl,d.NickName KuserNickName  from essayComment a join essay b on  a.EssayId=b.Id left join essayComment c on a.ParentId=c.Id left join kuser d on c.CreatorUserId=d.Id   where a.CreatorUserId =@CreatorUserId and b.IsPublish=1 and b.IsDeleted=0 order by {orderBy} limit @PageStart,@PageSize";

            var essayEntityList = ConnExecute(conn => conn.Query<EssayCommentDto>(sql, new { CreatorUserId = userId, PageStart = pageStart, PageSize = pageSize })) ?? new List<EssayCommentDto>();

            return essayEntityList;
        }

        public IEnumerable<EssayLikeDto> GetUserFans(long userId, int pageIndex, int pageSize, string orderBy)
        {

            int pageStart = ((pageIndex - 1) * pageSize);
            pageStart = pageStart > 0 ? (pageStart - 1) : pageStart;

            string sql = $@"select b.Id KuserId,b.AvatarUrl KuserAvatarUrl,b.NickName KuserNickName  from kuserFans a  join kuser b on a.CreatorUserId=b.Id   where a.BeConcernedUserId =@BeConcernedUserId order by {orderBy} limit @PageStart,@PageSize";

            var essayEntityList = ConnExecute(conn => conn.Query<EssayLikeDto>(sql, new { BeConcernedUserId = userId, PageStart = pageStart, PageSize = pageSize })) ?? new List<EssayLikeDto>();

            return essayEntityList;
        }


        //public IEnumerable<EssayDto> GetEssayList(DateTime creationTime)
        //{
        //    string sql = @"select essay.*,kuser.NickName KuserNickName,tag.* 
        //        from essay 
        //        left join kuser on essay.CreatorUserId=kuser.Id 
        //        left join tag on essay.Id=tag.EssayId 
        //        order by essay.LikeNum desc,media.Sort ";

        //    return ConnExecute(conn =>
        //    {

        //        var essayList = conn.Query<EssayDto, TagEntity, EssayDto>(sql, (essay, tag) =>
        //        {

        //            essay.TagList = essay.TagList ?? new List<TagEntity>();

        //            if (!essay.TagList.Where(t => t.Id == tag.Id).Any())
        //            {
        //                essay.TagList.Add(tag);
        //            }

        //            return essay;
        //        },
        //          splitOn: "Id");

        //        return essayList;
        //    });
        //}


        public EssayDto GetHtmlEssayDto(long id)
        {
            string sql = @"select essay.*,essayContent.Content,kuser.NickName KuserNickName,kuser.Introduction KuserIntroduction,kuser.AvatarUrl KuserAvatarUrl
                from essay join essayContent on essay.Id=essayContent.EssayId 
                left join kuser on essay.CreatorUserId=kuser.Id 
                where essay.id=@EssayId  ";

            return ConnExecute(conn =>
            {
                var essayDto = conn.QueryFirstOrDefault<EssayDto>(sql, new { EssayId = id});
                essayDto.TagList = conn.Query<TagEntity>("select * from tag where EssayId=@EssayId  order by  Sort", new { EssayId = id, })?.ToList();
                return essayDto;
            });
        }


        public EssayDto GetEssayDto(long id, long? currentUserId)
        {
            string sql = @"select essay.Score,essay.ScoreHeadCount,essay.ShareNum,essay.BrowseNum,essay.CommentNum,essay.LikeNum,case when(essayLike.Id>0) then 1 else 0 end IsLike
                from essay 
          
                left join essayLike on essay.Id=essayLike.EssayId and essayLike.CreatorUserId=@CurrentUserId 
                where essay.id=@EssayId  ";

            return ConnExecute(conn => conn.QueryFirstOrDefault<EssayDto>(sql, new { EssayId = id, CurrentUserId = currentUserId }));
        
        }




        public ResultDto<long> AddEssay(EssayEntity essayEntity, EssayContentEntity essayConentEntity, IEnumerable<TagEntity> tagList)
        {
            var resultDto = new ResultDto<long>();
            resultDto.Result= base.TransExecute((conn, trans) =>
            {

                var insertAndGetIdResultDto = conn.CreateAndGetId<EssayEntity, long>(essayEntity, trans);
                if (!insertAndGetIdResultDto.Result)
                {
                    return false;
                }

                essayConentEntity.EssayId = insertAndGetIdResultDto.Data;
                if (conn.Insert<EssayContentEntity>(essayConentEntity, trans)<=0) {
                    return false;
                }

                resultDto.Data = insertAndGetIdResultDto.Data;
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

            return resultDto;
        }

        public bool UpdateEssay(EssayEntity essayEntity,EssayContentEntity essayContentEntity, IEnumerable<TagEntity> tagList)
        {
            return base.TransExecute((conn, trans) =>
            {
                var updateResult = conn.Update(essayEntity, trans);
                if (!updateResult)
                {
                    return false;
                }

                string sql = "delete from tag  where EssayId=@EssayId";

                conn.Execute(sql, new { EssayId = essayEntity.Id }, trans);

                if (conn.Execute("update essayContent set Content=@Content  where EssayId=@EssayId", essayContentEntity, trans) <= 0)
                {
                    return false;
                }

                if (tagList != null && tagList.Any())
                {
                    tagList = tagList.Select(tag =>
                    {
                        tag.EssayId = essayEntity.Id;
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
            string sql = @"select distinct b.Id,b.PageUrl,b.Title,b.LikeNum,b.CreationTime from 
            (select essay.Category,tag.TagName from essay join tag on essay.Id=@EssayId and essay.IsPublish=1 and essay.IsDeleted=0 and essay.Id=tag.EssayId ) a
             join (select essay.*,tag.TagName  from essay join tag on  essay.Id<>@EssayId and essay.IsPublish=1 and essay.IsDeleted=0 and essay.Id=tag.EssayId )  b on a.Category=b.Category and a.TagName=b.TagName 
            order by b.LikeNum desc,b.CreationTime desc limit 10";

            return ConnExecute(conn => conn.Query<EssayEntity>(sql, new { EssayId = id }));
        }


        public IEnumerable<EssayEntity> GetEssayOtherList(long id)
        {
            string sql = @"select * from essay where IsPublish=1 and IsDeleted=0 and  Id<>@EssayId and CreatorUserId=(select CreatorUserId from essay where Id=@EssayId) order by LikeNum desc,CreationTime desc limit 10";

            return ConnExecute(conn => conn.Query<EssayEntity>(sql, new { EssayId = id }));
        }



    }
}
