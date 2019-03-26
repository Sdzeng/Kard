﻿using Dapper;
using Kard.Core.Dtos;
using Kard.Core.Entities;
using Kard.Core.IRepositories;
using Kard.Runtime.Session;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using DapperExtensionsCore;
using MySql.Data.MySqlClient;

namespace Kard.Dapper.Mysql.Repositories
{
    public class DefaultRepository : Repository, IDefaultRepository
    {


        public DefaultRepository(IConfiguration configuration, ILogger<DefaultRepository> logger) : base(configuration, logger)
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
                                essay.Content  EssayContent,
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
                return conn.QueryFirstOrDefault<CoverDto>(sql,   new { ShowDate = showDate } );
            });
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



        public IEnumerable<KuserEntity> GetExistUser(string name, string phone, string nickName)
        {
            string sql = "select *  from kuser where `Name`=@Name or Phone=@Phone or NickName=@NickName";
            return base.QueryList<KuserEntity>(sql, new { Name = name, Phone = phone, NickName = nickName });
        }

        //public bool CreateAccountUser(KuserEntity user)
        //{
        //    return TransExecute<bool>((conn, trans) => {
        //        var insertResultDto = conn.CreateAndGetId<KuserEntity, long>(user, trans);
        //        if (!insertResultDto.Result) {
        //            return false;
        //        }
        //        user = conn.FirstOrDefault<KuserEntity,long>(insertResultDto.Data, trans);
        //        user.AvatarUrl = user.AvatarUrl.Replace("{id}", user.Id.ToString());
        //        return conn.Update(user, trans);
        //    });
        //}

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
                where essay.id=@EssayId 
               order by  tag.Sort";

            return ConnExecute(conn =>
            {
                var essayDto=conn.QueryFirstOrDefault<EssayDto>(sql,  new { EssayId = id, CurrentUserId = currentUserId });
                essayDto.TagList = conn.Query<TagEntity>("select * from tag where EssayId=@EssayId", new { EssayId = id, })?.ToList();
                return essayDto;
            });
        }


        //public EssayEntity GetEssay2(long id)
        //{

        //    return ConnExecute(conn =>
        //    {
        //        var essayEntity = conn.Get<EssayEntity>(id);
        //        essayEntity.Kuser = conn.Get<KuserEntity>(essayEntity.CreatorUserId.Value).ToSecurity();
        //        //essayEntity.MediaList = conn.GetList<EssayCoverEntity>(new { EssayId = essayEntity.Id }).ToList();
        //        essayEntity.TagList = conn.GetList<TagEntity>(new { EssayId = essayEntity.Id }).ToList();
        //        return essayEntity;
        //    });
        //}


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


        //private ReaderWriterLockSlim _essayLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        //public bool UpdateBrowseNum(int id)
        //{
        //多处改动使用事务级别隔离read committed
        //string sql = @"set session transaction isolation level read committed;
        //                           start transaction;
        //                           update essay set  BrowseNum=(BrowseNum+1) where Id=@Id;
        //                           commit;";
        //单个改动使用update的排他（update\delete自动加上）行锁（使用索引）就行
        //string sql = "update essay set  BrowseNum=(BrowseNum+1) where Id=@Id";

        //_essayLock.EnterWriteLock();
        //try
        //{
        //return TransExecute((conn, trans) =>
        // {
        //     var essayEntity = base.FirstOrDefault<EssayEntity, long>(id, conn, trans);
        //     essayEntity.BrowseNum += essayEntity.BrowseNum;
        //     return base.Update(essayEntity, conn, trans);
        // });
        //}
        //finally {
        //    _essayLock.ExitWriteLock();
        //}
        //}


        //~DefaultRepository()
        //{
        //    if (_essayLock != null) _essayLock.Dispose();
        //}


        public bool UpdateBrowseNum(long id)
        {
            //单个改动使用update的排他（update\delete\insert InnoDB会自动给涉及数据集加上）行锁（使用索引）就行
            string sql = "update essay set  BrowseNum=(BrowseNum+1) where Id=@Id";

            var result = ConnExecute(conn => conn.Execute(sql, new { Id = id }));
            return result > 0;
        }

        public ResultDto ChangeEssayLike(long userId, long essayId)
        {
            var resultDto = new ResultDto();

            //多处改动使用事务时则用事务级别隔离read committed加行锁
            DynamicParameters pars = new DynamicParameters();
            pars.Add("@iuserId", userId);
            pars.Add("@iessayId", essayId);
            pars.Add("@icreationTime", DateTime.Now);
            pars.Add("@olikeNum", 0, DbType.Int32, ParameterDirection.Output);
            pars.Add("@oisLike", null, DbType.Byte, ParameterDirection.Output);


            try
            {
                var ee = ConnExecute(conn => conn.Execute("changeEssayLike", pars, commandType: CommandType.StoredProcedure));//res2.Count = 80
                var likeNum = pars.Get<int>("@olikeNum");
                var isLike = pars.Get<byte>("@oisLike") == 1;

                resultDto.Result = true;
                resultDto.Data = new { LikeNum = likeNum, IsLike = isLike };
            }
            catch (Exception ex)
            {
                resultDto.Result = false;
                resultDto.Message = ex.Message;
            }
            return resultDto;
        }


        public IEnumerable<EssayLikeEntity> GetEssayLikeList(long id)
        {
            string sql = @"select * 
                from essay_like 
                left join kuser on essay_like.CreatorUserId=kuser.Id 
                where essay_like.EssayId=@EssayId 
               order by essay_like.CreationTime desc";

            return ConnExecute(conn => conn.Query<EssayLikeEntity, KuserEntity, EssayLikeEntity>(sql, (essayLike, kuser) => { essayLike.Kuser = kuser.ToSecurity(); return essayLike; },
                                                                                                                                                                          new { EssayId = id },
                                                                                                                                                                          splitOn: "Id"));
        }


        public IEnumerable<EssayEntity> GetEssaySimilarList(long id)
        {
            string sql = @"select distinct b.Id,b.Title,b.Content,b.LikeNum,b.CreationTime from 
            (select essay.Category,tag.TagName from essay join tag on essay.Id=@EssayId and essay.IsDeleted=0 and essay.Id=tag.EssayId ) a
             join (select essay.*,tag.TagName  from essay join tag on  essay.Id<>@EssayId and essay.IsDeleted=0 and essay.Id=tag.EssayId )  b on a.Category=b.Category and a.TagName=b.TagName 
            order by b.LikeNum desc,b.CreationTime desc limit 10";

            return ConnExecute(conn => conn.QueryList<EssayEntity>(sql, new { EssayId = id }));
        }


        public IEnumerable<EssayEntity> GetEssayOtherList(long id)
        {
            string sql = @"select * from essay where IsDeleted=0 and  Id<>@EssayId and CreatorUserId=(select CreatorUserId from essay where Id=@EssayId) order by LikeNum desc,CreationTime desc limit 10";

            return ConnExecute(conn => conn.QueryList<EssayEntity>(sql, new { EssayId = id }));
        }



        //public IEnumerable<EssayCommentDto> GetRootEssayCommentList(long id)
        //{
        //    string sql = @"select * 
        //        from essay_comment 
        //        left join kuser on essay_comment.CreatorUserId=kuser.Id 
        //        where essay_comment.EssayId=@EssayId and essay_comment.IsDeleted=0  
        //       order by essay_comment.CreationTime desc limit 10";

        //    return ConnExecute(conn => conn.Query<EssayCommentDto, KuserEntity, EssayCommentDto>(sql, (essayComment, kuser) => { essayComment.Kuser = kuser.ToSecurity(); return essayComment; },
        //                                                                                                                                                                  new { EssayId = id },
        //                                                                                                                                                                  splitOn: "Id"));
        //}

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

        public ResultDto AddTask(LongTaskEntity entity)
        {
            var taskDate = entity.StartDate;
            var taskWeekDay = (int)taskDate.DayOfWeek;
            var taskWeekList = entity.Week.Split(',').Select(w => Convert.ToInt32(w));

            return TransExecute((conn, trans) =>
            {
                var createResult = new ResultDto();
                var createLongResult = conn.CreateAndGetId<LongTaskEntity, long>(entity, trans);
                if (!createLongResult.Result)
                {
                    _logger.LogError("添加长期目标失败，已撤销：" + createLongResult.Message);
                    createResult.Result = false;
                    createResult.Message = "添加长期目标失败";
                    return createResult;
                }

                var taskEntityList = new List<TaskEntity>();
                while (taskDate <= entity.EndDate)
                {
                    if (taskWeekList.Contains(taskWeekDay))
                    {
                        var taskEntity = new TaskEntity()
                        {
                            LongTaskId = createLongResult.Data,
                            TaskDate = taskDate,
                            StartTime = entity.StartTime,
                            EndTime = entity.EndTime,
                            Content = entity.Content,
                            IsRemind = entity.IsRemind,
                            IsDone = false
                        };
                        taskEntity.AuditCreation(entity.CreatorUserId.Value);
                        taskEntityList.Add(taskEntity);
                    }

                    taskDate = taskDate.AddDays(1);
                    taskWeekDay = (taskWeekDay + 1) % 7;
                }


                if (!conn.CreateList(taskEntityList, trans).Result)
                {
                    _logger.LogError("添加小目标失败，已撤销");
                    createResult.Result = false;
                    createResult.Message = "添加小目标失败";
                    return createResult;
                }

                createResult.Result = true;
                return createResult;
            });
        }

    }
}
